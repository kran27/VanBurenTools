using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using VBLauncher.VB3DLib;
using Veldrid;
using Veldrid.ImageSharp;

namespace VBLauncher;

// ImGui replacement for the legacy WinForms GrpBrowser dialog. The data layer
// (GrpBrowser.cs / ResourceSystem.cs) stays — every other call site still
// depends on it for raw byte access, RLE/TGA decoding, and the global
// resource catalog. This class only owns the *user-facing browse + preview*
// UX: a filterable tree of GRPs → extensions → file names plus a preview
// pane that routes 3D-renderable files through SceneRenderer (instead of
// HelixToolkit) and shows text/images inline.
//
// Lifetime: instantiate once after SceneRenderer is initialized; call Draw()
// from the host's DrawImGui() loop and toggle Visible from a menu item.
public sealed class GrpBrowserImGui
{
    private readonly GraphicsDevice _gd;
    private readonly ImGuiRenderer _imguiRenderer;
    private readonly SceneRenderer _scene;

    public bool Visible;

    // Selection state — null when nothing is selected.
    private string? _selGrp;
    private string? _selExt;
    private string? _selName;

    // Preview state — exactly one of these is populated based on the
    // resolved resource type. We hold on to the GPU texture for image
    // previews (and dispose the previous one on each new selection) so the
    // ImGui binding stays valid for as long as the file is shown.
    private string? _selText;
    private Texture? _selImageTexture;
    private IntPtr _selImageBinding;
    private Vector2 _selImageSize;
    private string? _selBinaryInfo;
    private string? _selErrorInfo;

    // Filter (case-insensitive substring) applied across both extensions
    // and file names. Cached lowercase form means we don't re-allocate on
    // every TreeNode iteration.
    private string _filterRaw = string.Empty;
    private string _filterLower = string.Empty;

    private static readonly HashSet<string> TextExts = new(StringComparer.OrdinalIgnoreCase)
    {
        "ini", "tok", "sco", "pce", "g3d", "gsf", "psf", "pco", "gls"
    };

    private static readonly HashSet<string> ImageExts = new(StringComparer.OrdinalIgnoreCase)
    {
        "tga", "rle", "bmp"
    };

    private static readonly HashSet<string> SceneExts = new(StringComparer.OrdinalIgnoreCase)
    {
        "8", "b3d", "crt", "map", "arm", "wea", "itm"
    };

    public GrpBrowserImGui(GraphicsDevice gd, ImGuiRenderer imguiRenderer, SceneRenderer scene)
    {
        _gd = gd;
        _imguiRenderer = imguiRenderer;
        _scene = scene;
    }

    public void Draw()
    {
        if (!Visible) return;

        ImGui.SetNextWindowSize(new Vector2(820, 540), ImGuiCond.FirstUseEver);
        var open = Visible;
        if (!ImGui.Begin("GRP Browser", ref open))
        {
            Visible = open;
            ImGui.End();
            return;
        }
        Visible = open;

        if (ImGui.InputTextWithHint("##filter", "filter (substring, name or ext)", ref _filterRaw, 128))
            _filterLower = _filterRaw.ToLowerInvariant();

        var avail = ImGui.GetContentRegionAvail();
        var leftWidth = MathF.Min(320f, avail.X * 0.4f);

        ImGui.BeginChild("##tree", new Vector2(leftWidth, 0), true);
        DrawTree();
        ImGui.EndChild();

        ImGui.SameLine();
        ImGui.BeginChild("##preview", new Vector2(0, 0), true);
        DrawPreview();
        ImGui.EndChild();

        ImGui.End();
    }

    private void DrawTree()
    {
        // Sort GRPs alphabetically so the layout doesn't shift between
        // sessions (Dictionary enumeration order is implementation-defined).
        foreach (var grp in ResourceSystem.Resources.Keys.OrderBy(s => s, StringComparer.OrdinalIgnoreCase))
        {
            var grpDict = ResourceSystem.Resources[grp];
            // When a filter is active, hide GRPs whose total filtered count
            // is zero — keeps the tree from being mostly empty headers.
            if (_filterLower.Length > 0 && !GrpHasMatch(grpDict)) continue;

            if (!ImGui.TreeNode(grp)) continue;
            foreach (var ext in grpDict.Keys.OrderBy(s => s, StringComparer.OrdinalIgnoreCase))
            {
                var extDict = grpDict[ext];
                var matches = FilterNames(extDict.Keys, ext);
                if (matches.Count == 0) continue;
                if (!ImGui.TreeNode($"{ext} ({matches.Count})")) continue;
                foreach (var name in matches)
                {
                    var isSel = _selGrp == grp && _selExt == ext && _selName == name;
                    if (ImGui.Selectable($"{name}.{ext}##{grp}/{ext}/{name}", isSel))
                        SelectFile(grp, ext, name);
                }
                ImGui.TreePop();
            }
            ImGui.TreePop();
        }
    }

    // Cheap pre-filter: does any (ext, name) under this GRP match the
    // filter? Avoids the per-file Selectable cost when the user has typed
    // something specific.
    private bool GrpHasMatch(Dictionary<string, Dictionary<string, dynamic>> grpDict)
    {
        foreach (var (ext, extDict) in grpDict)
        {
            if (ext.ToLowerInvariant().Contains(_filterLower)) return true;
            foreach (var name in extDict.Keys)
                if (name.ToLowerInvariant().Contains(_filterLower)) return true;
        }
        return false;
    }

    private List<string> FilterNames(IEnumerable<string> names, string ext)
    {
        var result = new List<string>();
        var extMatches = _filterLower.Length == 0 || ext.ToLowerInvariant().Contains(_filterLower);
        foreach (var name in names)
        {
            if (extMatches || name.ToLowerInvariant().Contains(_filterLower))
                result.Add(name);
        }
        result.Sort(StringComparer.OrdinalIgnoreCase);
        return result;
    }

    private void DrawPreview()
    {
        if (_selName == null)
        {
            ImGui.TextDisabled("Select a file from the tree.");
            return;
        }

        ImGui.Text($"{_selName}.{_selExt}");
        ImGui.SameLine();
        ImGui.TextDisabled($"   in {_selGrp}");
        ImGui.Separator();

        if (_selErrorInfo != null)
        {
            ImGui.TextWrapped(_selErrorInfo);
            return;
        }

        if (SceneExts.Contains(_selExt!))
        {
            // 3D scene previews live in the editor's existing Viewport
            // window — the SceneRenderer drives that surface. We just
            // tell the user where to look so the browser stays focused
            // on browse/select.
            ImGui.TextWrapped("Loaded into the Viewport window. Use the camera there to inspect.");
            return;
        }

        if (_selImageBinding != IntPtr.Zero)
        {
            // Fit the image into the available pane while preserving its
            // aspect ratio, capped at native size so small atlases stay
            // crisp instead of getting filtered up to fill the panel.
            var avail = ImGui.GetContentRegionAvail();
            var w = MathF.Min(_selImageSize.X, avail.X);
            var h = w * (_selImageSize.Y / MathF.Max(1f, _selImageSize.X));
            if (h > avail.Y)
            {
                h = avail.Y;
                w = h * (_selImageSize.X / MathF.Max(1f, _selImageSize.Y));
            }
            ImGui.Image(_selImageBinding, new Vector2(w, h));
            ImGui.TextDisabled($"{(int)_selImageSize.X}×{(int)_selImageSize.Y}");
            return;
        }

        if (_selText != null)
        {
            // Multi-line read-only preview. Sized to fill the remaining
            // pane minus a tiny bottom margin; ImGui's input widgets need a
            // size or they collapse to a single line.
            var avail = ImGui.GetContentRegionAvail();
            ImGui.InputTextMultiline("##textpreview", ref _selText, (uint)Math.Max(_selText.Length + 1, 1024),
                new Vector2(avail.X, MathF.Max(40, avail.Y - 4)),
                ImGuiInputTextFlags.ReadOnly);
            return;
        }

        if (_selBinaryInfo != null)
        {
            ImGui.TextWrapped(_selBinaryInfo);
        }
    }

    private void SelectFile(string grp, string ext, string name)
    {
        _selGrp = grp;
        _selExt = ext;
        _selName = name;
        _selText = null;
        _selBinaryInfo = null;
        _selErrorInfo = null;
        DisposePreviewImage();

        try
        {
            if (SceneExts.Contains(ext))
            {
                LoadIntoScene(ext, name);
            }
            else if (ImageExts.Contains(ext))
            {
                LoadImage(name, ext);
            }
            else if (TextExts.Contains(ext))
            {
                var resource = ResourceSystem.FindResource(name, ext);
                _selText = resource as string ?? "(resource is not text)";
            }
            else
            {
                // Don't pull bytes for fully unrecognised types — the
                // legacy ResourceSystem.LoadAsset pops a MessageBox for
                // unknown extensions and we don't want one per click.
                _selBinaryInfo = $"No inline preview for .{ext}. Use the legacy Tools menu to extract.";
            }
        }
        catch (Exception ex)
        {
            _selErrorInfo = $"Failed to preview {name}.{ext}:\n{ex.Message}";
        }
    }

    private void LoadIntoScene(string ext, string name)
    {
        // Reset before adding so the viewport always reflects exactly the
        // selected file (no left-over models from the previous selection).
        _scene.Reset();
        var resource = ResourceSystem.FindResource(name, ext);
        if (resource == null)
        {
            _selErrorInfo = $"Resource {name}.{ext} could not be loaded.";
            return;
        }

        // Orbit feels right for prop-style previews (a single asset centered
        // in the viewport) — the user spins around it and zooms in. Free-fly
        // is what you want for maps and full octree scenes where you need to
        // navigate inside the geometry. Set the mode BEFORE FocusCamera so
        // the orbit target/distance are seeded along with yaw/pitch.
        _scene.CameraMode = (ext == "8" || ext == "map") ? CameraMode.Free : CameraMode.Orbit;

        switch (ext)
        {
            case "8":
                _scene.Add8Model((_8Model)resource);
                _scene.FocusCamera();
                break;
            case "b3d":
                _scene.AddB3DModel((B3DModel)resource);
                _scene.FocusCamera();
                break;
            case "map":
                _scene.AddMap((Map)resource);
                break;
            case "crt":
            case "arm":
            case "wea":
            case "itm":
                _scene.AddEntity((Entity)resource);
                _scene.FocusCamera();
                break;
        }
    }

    private void LoadImage(string name, string ext)
    {
        var resource = ResourceSystem.FindResource(name, ext);
        if (resource is not ImageSharpTexture img)
        {
            _selErrorInfo = $"Resource {name}.{ext} could not be decoded as an image.";
            return;
        }
        var factory = _gd.ResourceFactory;
        _selImageTexture = img.CreateDeviceTexture(_gd, factory);
        _selImageBinding = _imguiRenderer.GetOrCreateImGuiBinding(factory, _selImageTexture);
        _selImageSize = new Vector2(_selImageTexture.Width, _selImageTexture.Height);
    }

    private void DisposePreviewImage()
    {
        if (_selImageTexture != null)
        {
            _imguiRenderer.RemoveImGuiBinding(_selImageTexture);
            _selImageTexture.Dispose();
            _selImageTexture = null;
        }
        _selImageBinding = IntPtr.Zero;
        _selImageSize = Vector2.Zero;
    }
}
