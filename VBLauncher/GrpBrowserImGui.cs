using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using ImGuiNET;
using VBLauncher.VB3DLib;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace VBLauncher;

// Standalone GRP browser. Owns its own top-level OS window, swapchain,
// command list, ImGui context, and ImGui renderer — completely external to
// the editor's main window. Toggling visibility creates/destroys the OS
// window so when it's closed there's zero overhead.
//
// 3D viewport: shares the editor's SceneRenderer so the geometry only has
// to be uploaded once. Each ImGui context maintains its OWN texture-id
// table, so we register a per-renderer binding for the scene's color
// target via SceneRenderer.SceneColorTarget. Camera input still flows
// through the editor's main viewport (the browser window's events go to
// its own SDL message queue and don't reach the global InputTracker), so
// the browser viewport is view-only — drag the camera in the editor.
//
// Lifetime: instantiate once after the editor's SceneRenderer is set up.
// Call Frame(dt) from the host's main loop AFTER the editor swap.
// Toggle() opens/closes the window.
public sealed class GrpBrowserImGui
{
    private readonly GraphicsDevice _gd;
    // Optional "Load in Editor" hook. When set and the current selection
    // is an editor-known type, the right pane shows a button that hands
    // the resource (and its name/ext) back to the host. Lets us replace
    // the legacy modal "Open From GRP" dialog with this same window —
    // the host's File→Open-From-GRP just toggles us open and waits for
    // the user to click the button.
    private readonly Action<string, string, dynamic?>? _loadInEditor;

    // Top-level OS window state. All null when the browser is closed.
    private Sdl2Window? _window;
    private Swapchain? _swapchain;
    private CommandList? _cl;
    private ImGuiRenderer? _imguiRenderer;
    private SceneRenderer? _scene;       // browser's OWN scene — independent
                                          // of the editor's main viewport so
                                          // selecting a file here doesn't
                                          // mutate what the editor is showing
    private IntPtr _imGuiContext;        // browser's own ImGui context
    private IntPtr _editorImGuiContext;  // saved on each Frame() entry

    public bool IsOpen => _window != null && _window.Exists;

    // Selection state. Reset by SelectFile.
    private string? _selGrp;
    private string? _selExt;
    private string? _selName;
    private string? _selText;
    private Texture? _selImageTexture;
    private IntPtr _selImageBinding;
    private Vector2 _selImageSize;
    private string? _selPropertyDump;
    private string? _selHexDump;
    private string? _selErrorInfo;
    private bool _selIsScene;

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
    // Every Entity-derived type lands in the 3D viewport via AddEntity (it
    // resolves the base mesh from EEN2.skl). USE/CON/DOR/AMO are Entities
    // too, so they show up here even though they're "just" property
    // records. .8 / .b3d / .g3d / .map have their own dedicated paths.
    private static readonly HashSet<string> SceneExts = new(StringComparer.OrdinalIgnoreCase)
    {
        "8", "b3d", "g3d", "crt", "map", "arm", "wea", "itm", "amo", "use", "con", "dor",
    };
    private static readonly HashSet<string> EditorExts = new(StringComparer.OrdinalIgnoreCase)
    {
        "amo", "arm", "con", "crt", "dor", "int", "itm", "map", "use", "veg", "wea",
    };

    public GrpBrowserImGui(GraphicsDevice gd, Action<string, string, dynamic?>? loadInEditor = null)
    {
        _gd = gd;
        _loadInEditor = loadInEditor;
    }

    public void Toggle()
    {
        if (IsOpen) Close();
        else Open();
    }

    public void Open()
    {
        if (IsOpen) return;

        _window = VeldridStartup.CreateWindow(new WindowCreateInfo
        {
            X = 200, Y = 200,
            WindowWidth = 1100, WindowHeight = 680,
            WindowTitle = "GRP Browser"
        });
        _window.Resized += OnResized;

        var src = VeldridStartup.GetSwapchainSource(_window);
        _swapchain = _gd.ResourceFactory.CreateSwapchain(new SwapchainDescription(
            src,
            (uint)_window.Width, (uint)_window.Height,
            PixelFormat.D24_UNorm_S8_UInt,
            true /* SyncToVerticalBlank */));

        _cl = _gd.ResourceFactory.CreateCommandList();

        // New ImGui context for this window. Veldrid's ImGuiRenderer
        // creates one inside its constructor — capture it after, save the
        // editor's context so we can restore around our frame.
        _editorImGuiContext = ImGui.GetCurrentContext();
        _imguiRenderer = new ImGuiRenderer(
            _gd,
            _swapchain.Framebuffer.OutputDescription,
            _window.Width, _window.Height);
        _imGuiContext = ImGui.GetCurrentContext();

        // Apply a basic dark style; we don't try to mirror the editor's
        // AltUI palette here so it's obvious at a glance the browser is a
        // separate context. Easy to plumb in if we want later.
        ImGui.StyleColorsDark();

        // Browser's own SceneRenderer. Independent from the editor's main
        // viewport — its model list, framebuffer, and camera are all
        // separate, so selecting a file here doesn't mutate what the
        // editor is currently showing. ImGuiBinding here is registered
        // against the browser's ImGuiRenderer in the constructor.
        _scene = new SceneRenderer(_gd, _imguiRenderer, _window, _cl,
            _window.Width, _window.Height);

        // Switch back to the editor context so we don't leak browser
        // state into the editor's first post-Open frame.
        ImGui.SetCurrentContext(_editorImGuiContext);
    }

    public void Close()
    {
        // Drop the per-selection image binding before nuking the renderer
        // (the renderer-owned id table is about to vanish anyway, but
        // ditching the texture explicitly avoids GPU-leak warnings).
        DisposePreviewImage();

        // SceneRenderer doesn't expose Dispose; its GPU resources outlive
        // the window for now (small one-shot leak per Open/Close cycle).
        // Drop the reference so the next Open builds a fresh one.
        _scene = null;

        if (_imguiRenderer != null)
        {
            var saved = ImGui.GetCurrentContext();
            if (_imGuiContext != IntPtr.Zero) ImGui.SetCurrentContext(_imGuiContext);
            _imguiRenderer.Dispose();
            _imguiRenderer = null;
            ImGui.SetCurrentContext(saved);
        }
        _imGuiContext = IntPtr.Zero;

        _cl?.Dispose();
        _cl = null;
        _swapchain?.Dispose();
        _swapchain = null;

        if (_window != null)
        {
            _window.Resized -= OnResized;
            if (_window.Exists) _window.Close();
            _window = null;
        }
    }

    private void OnResized()
    {
        if (_window == null || _swapchain == null || _imguiRenderer == null) return;
        _swapchain.Resize((uint)_window.Width, (uint)_window.Height);
        _imguiRenderer.WindowResized(_window.Width, _window.Height);
        _scene?.HandleWindowResize((uint)_window.Width, (uint)_window.Height);
    }

    // Called once per host frame. No-op when closed.
    public void Frame(float deltaTime)
    {
        if (_window == null) return;
        if (!_window.Exists) { Close(); return; }
        if (_swapchain == null || _cl == null || _imguiRenderer == null) return;

        var snapshot = _window.PumpEvents();
        if (!_window.Exists) { Close(); return; }

        // Switch into our own ImGui context. Restore on exit so the editor
        // continues rendering against its own context next frame.
        _editorImGuiContext = ImGui.GetCurrentContext();
        ImGui.SetCurrentContext(_imGuiContext);
        try
        {
            _imguiRenderer.Update(deltaTime, snapshot);

            // Draw our independent 3D scene to its own render target. The
            // editor's main scene renders separately in its own loop pass,
            // so the two viewports never interfere.
            if (_scene != null && _scene.Initialized)
                _scene.DrawScene(deltaTime);

            BuildUI();

            _cl.Begin();
            _cl.SetFramebuffer(_swapchain.Framebuffer);
            _cl.ClearColorTarget(0, RgbaFloat.Black);
            _imguiRenderer.Render(_gd, _cl);
            _cl.End();
            _gd.SubmitCommands(_cl);
            _gd.SwapBuffers(_swapchain);
        }
        finally
        {
            ImGui.SetCurrentContext(_editorImGuiContext);
        }
    }

    private void BuildUI()
    {
        // Fill the whole OS window. No title bar/menu/etc. — the OS
        // window itself is the browser's "frame".
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(viewport.WorkPos);
        ImGui.SetNextWindowSize(viewport.WorkSize);
        var flags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
                    ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
        ImGui.Begin("##grpbrowser", flags);

        if (ImGui.InputTextWithHint("##filter", "filter (substring, name or ext)", ref _filterRaw, 128))
            _filterLower = _filterRaw.ToLowerInvariant();

        var avail = ImGui.GetContentRegionAvail();
        var leftWidth = MathF.Min(340f, avail.X * 0.35f);

        ImGui.BeginChild("##tree", new Vector2(leftWidth, 0), true);
        DrawTree();
        ImGui.EndChild();

        ImGui.SameLine();
        ImGui.BeginChild("##right", new Vector2(0, 0), false);
        DrawRightPane();
        ImGui.EndChild();

        ImGui.End();
    }

    private void DrawTree()
    {
        // Up/Down arrows cycle through filenames within the currently
        // selected GRP+ext (ignoring the tree's open/closed state — the
        // user just wants to scrub through "the next CRT" without
        // opening a new TreeNode each time). Skip when a text input has
        // focus so the filter box keeps normal cursor navigation.
        var io = ImGui.GetIO();
        var typing = io.WantTextInput;
        if (!typing && _selGrp != null && _selExt != null && _selName != null
            && ResourceSystem.Resources.TryGetValue(_selGrp, out var grpForNav)
            && grpForNav.TryGetValue(_selExt, out var extForNav))
        {
            var navList = FilterNames(extForNav.Keys, _selExt);
            if (navList.Count > 0)
            {
                var down = ImGui.IsKeyPressed(ImGuiKey.DownArrow, true);
                var up   = ImGui.IsKeyPressed(ImGuiKey.UpArrow,   true);
                if (down || up)
                {
                    var idx = navList.IndexOf(_selName);
                    if (idx < 0) idx = 0;
                    else idx = (idx + (down ? 1 : -1) + navList.Count) % navList.Count;
                    SelectFile(_selGrp, _selExt, navList[idx]);
                }
            }
        }

        foreach (var grp in ResourceSystem.Resources.Keys.OrderBy(s => s, StringComparer.OrdinalIgnoreCase))
        {
            var grpDict = ResourceSystem.Resources[grp];
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
                    // Auto-scroll to the active selection so arrow-key
                    // cycling through a long list doesn't strand the
                    // highlighted row off-screen.
                    if (isSel) ImGui.SetScrollHereY(0.5f);
                }
                ImGui.TreePop();
            }
            ImGui.TreePop();
        }
    }

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

    private void DrawRightPane()
    {
        if (_selName == null)
        {
            ImGui.TextDisabled("Select a file from the tree.");
            return;
        }

        ImGui.Text($"{_selName}.{_selExt}");
        ImGui.SameLine();
        ImGui.TextDisabled($"   in {_selGrp}");

        // "Load in Editor" hook — only enabled when the host wired a
        // callback at construction AND the current selection is a type
        // the editor's drawers know how to handle.
        if (_loadInEditor != null && _selExt != null && EditorExts.Contains(_selExt))
        {
            ImGui.SameLine();
            if (ImGui.Button($"Load in Editor##{_selName}.{_selExt}"))
            {
                var resource = ResourceSystem.FindResource(_selName!, _selExt);
                _loadInEditor(_selName!, _selExt, resource);
            }
        }
        ImGui.Separator();

        if (_selErrorInfo != null)
        {
            ImGui.TextWrapped(_selErrorInfo);
            return;
        }

        if (_selIsScene)
        {
            DrawEmbeddedViewport();
            ImGui.Separator();
        }

        if (_selImageBinding != IntPtr.Zero)
        {
            DrawEmbeddedImage();
            return;
        }

        if (_selPropertyDump != null || _selText != null || _selHexDump != null)
        {
            if (ImGui.BeginTabBar("##previewTabs"))
            {
                if (_selPropertyDump != null && ImGui.BeginTabItem("Properties"))
                {
                    DrawScrollableText(_selPropertyDump);
                    ImGui.EndTabItem();
                }
                if (_selText != null && ImGui.BeginTabItem("Text"))
                {
                    DrawScrollableText(_selText);
                    ImGui.EndTabItem();
                }
                if (_selHexDump != null && ImGui.BeginTabItem("Hex"))
                {
                    DrawScrollableText(_selHexDump);
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }
        else if (!_selIsScene)
        {
            ImGui.TextDisabled($"No inline preview for .{_selExt}.");
        }
    }

    private void DrawEmbeddedViewport()
    {
        var avail = ImGui.GetContentRegionAvail();
        var hasBelow = _selPropertyDump != null || _selText != null || _selHexDump != null;
        var viewH = hasBelow ? MathF.Max(180f, avail.Y * 0.6f) : MathF.Max(180f, avail.Y - 8f);
        if (_scene != null && _scene.Initialized && _scene.ImGuiBinding != IntPtr.Zero)
        {
            // Same UV-crop pattern the editor uses: render to a sub-rect
            // of the fixed-size framebuffer and sample only that region
            // 1:1 so the viewport pane doesn't bilinear-blur the texture.
            uint pixelW = (uint)MathF.Max(1, MathF.Floor(avail.X));
            uint pixelH = (uint)MathF.Max(1, MathF.Floor(viewH));
            pixelW = Math.Min(pixelW, _scene.FbWidth);
            pixelH = Math.Min(pixelH, _scene.FbHeight);
            if (_scene.Width != pixelW || _scene.Height != pixelH)
                _scene.HandleWindowResize(pixelW, pixelH);
            var size = new Vector2(pixelW, pixelH);
            var uv1 = new Vector2(pixelW / (float)_scene.FbWidth,
                                   pixelH / (float)_scene.FbHeight);
            ImGui.Image(_scene.ImGuiBinding, size, Vector2.Zero, uv1);
            _scene.AllowInput = ImGui.IsItemHovered();
        }
        else
        {
            ImGui.Dummy(new Vector2(MathF.Max(64f, avail.X), MathF.Max(64f, viewH)));
        }
    }

    private void DrawEmbeddedImage()
    {
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
    }

    private void DrawScrollableText(string body)
    {
        var avail = ImGui.GetContentRegionAvail();
        var size = new Vector2(avail.X, MathF.Max(40f, avail.Y - 4f));
        var buf = body;
        ImGui.InputTextMultiline("##scrolltext", ref buf, (uint)Math.Max(buf.Length + 1, 1024),
            size, ImGuiInputTextFlags.ReadOnly);
    }

    private void SelectFile(string grp, string ext, string name)
    {
        _selGrp = grp;
        _selExt = ext;
        _selName = name;
        _selText = null;
        _selPropertyDump = null;
        _selHexDump = null;
        _selErrorInfo = null;
        _selIsScene = false;
        DisposePreviewImage();

        try
        {
            dynamic? resource = ResourceSystem.FindResource(name, ext);

            if (SceneExts.Contains(ext))
                LoadIntoScene(ext, name, resource);

            if (ImageExts.Contains(ext))
                LoadImage(name, ext, resource);

            if (TextExts.Contains(ext))
                _selText = resource as string ?? "(resource is not text)";

            if (EditorExts.Contains(ext))
                _selPropertyDump = BuildPropertyDump(ext, resource);
            else
                _selPropertyDump = Build3DPropertyDump(ext, resource);

            if (!SceneExts.Contains(ext) && !ImageExts.Contains(ext) &&
                !TextExts.Contains(ext) && !EditorExts.Contains(ext))
            {
                _selHexDump = BuildHexDump(name, ext);
            }
        }
        catch (Exception ex)
        {
            _selErrorInfo = $"Failed to preview {name}.{ext}:\n{ex.Message}";
        }
    }

    private void LoadIntoScene(string ext, string name, dynamic? resource)
    {
        if (_scene == null) return;
        _scene.Reset();
        if (resource == null)
        {
            _selErrorInfo = $"Resource {name}.{ext} could not be loaded.";
            return;
        }
        _scene.CameraMode = (ext == "8" || ext == "map") ? CameraMode.Free : CameraMode.Orbit;

        switch (ext)
        {
            case "8":   _scene.Add8Model((_8Model)resource); _scene.FocusCamera(); _selIsScene = true; break;
            case "b3d": _scene.AddB3DModel((B3DModel)resource); _scene.FocusCamera(); _selIsScene = true; break;
            // AddMap no longer focuses on its own (so editor edits don't
            // snap the camera each frame). Browser's a one-shot preview
            // — frame the camera on the first load.
            case "map": _scene.AddMap((Map)resource); _scene.FocusCamera(); _selIsScene = true; break;
            case "g3d":
            {
                // ResourceSystem stores .g3d as the raw text; rebuild a
                // G3DModel from those bytes. Only Critters/CR_Monsta.g3d
                // ships in the install and the existing parser is
                // hardcoded for its line offsets — anything else will
                // throw and fall through to the catch block in SelectFile.
                var bytes = Encoding.UTF8.GetBytes((string)resource);
                _scene.AddG3DModel(new G3DModel(bytes));
                _scene.FocusCamera();
                _selIsScene = true;
                break;
            }
            // Every Entity-derived type funnels through AddEntity. AddEntity
            // resolves the visible mesh from EEN2.skl + EEOV texture, so it
            // works uniformly across CRT (creatures), ARM (armor), WEA
            // (weapons), ITM (items), AMO (ammo), USE (usable objects),
            // CON (containers), DOR (doors).
            case "crt":
            case "arm":
            case "wea":
            case "itm":
            case "amo":
            case "use":
            case "con":
            case "dor":
                _scene.AddEntity((Entity)resource);
                _scene.FocusCamera();
                _selIsScene = true;
                break;
        }
    }

    private void LoadImage(string name, string ext, dynamic? resource)
    {
        if (resource is not ImageSharpTexture img)
        {
            _selErrorInfo = $"Resource {name}.{ext} could not be decoded as an image.";
            return;
        }
        if (_imguiRenderer == null) return;
        var factory = _gd.ResourceFactory;
        _selImageTexture = img.CreateDeviceTexture(_gd, factory);
        _selImageBinding = _imguiRenderer.GetOrCreateImGuiBinding(factory, _selImageTexture);
        _selImageSize = new Vector2(_selImageTexture.Width, _selImageTexture.Height);
    }

    private void DisposePreviewImage()
    {
        if (_selImageTexture != null && _imguiRenderer != null)
        {
            try { _imguiRenderer.RemoveImGuiBinding(_selImageTexture); }
            catch { /* ignore — context might be torn down */ }
            _selImageTexture.Dispose();
            _selImageTexture = null;
        }
        _selImageBinding = IntPtr.Zero;
        _selImageSize = Vector2.Zero;
    }

    private static string BuildPropertyDump(string ext, dynamic? resource)
    {
        if (resource == null) return "(resource is null)";
        var sb = new StringBuilder();
        try
        {
            switch (ext.ToLowerInvariant())
            {
                case "crt":
                {
                    var crt = (CRT)resource;
                    sb.AppendLine($"Skeleton:   {crt.EEN2.skl}");
                    var inv = crt.GCRE?.Inventory;
                    sb.AppendLine($"Inventory:  {inv?.Length ?? 0} item(s)");
                    if (inv != null)
                        foreach (var i in inv) sb.AppendLine($"  • {i}");
                    break;
                }
                case "arm":
                {
                    var arm = (ARM)resource;
                    sb.AppendLine($"Body:    {arm.GITM.Bod.Model}  tex={arm.GITM.Bod.Tex}");
                    sb.AppendLine($"Head:    {arm.GITM.Hea.Model}  tex={arm.GITM.Hea.Tex}");
                    sb.AppendLine($"Hide hair={arm.GITM.hHai}  beard={arm.GITM.hBea}  eye={arm.GITM.hEye}");
                    break;
                }
                case "wea":
                {
                    var wea = (WEA)resource;
                    sb.AppendLine($"In-hand model: {wea.GITM.IHS.Model}  tex={wea.GITM.IHS.Tex}");
                    break;
                }
                case "itm":
                case "amo":
                {
                    var itm = (ITM)resource;
                    sb.AppendLine($"Item type: {ext.ToUpperInvariant()}");
                    sb.AppendLine($"GITM model: {itm.GITM.IHS?.Model}");
                    break;
                }
                case "map":
                {
                    var map = (Map)resource;
                    sb.AppendLine($"Octree:    {map.EMAP.s1}");
                    sb.AppendLine($"Entities:  {map.EME2?.Count ?? 0}");
                    sb.AppendLine($"Entry pts: {map.EMEP?.Count ?? 0}");
                    break;
                }
                case "use":
                case "con":
                case "dor":
                {
                    var use = (USE)resource;
                    sb.AppendLine($"Type:      {ext.ToUpperInvariant()}");
                    sb.AppendLine($"Name:      {use.EEN2?.skl}");
                    sb.AppendLine($"GOBJ.Type: {use.GOBJ?.Type}");
                    break;
                }
                case "veg":
                {
                    var veg = (VEG)resource;
                    sb.AppendLine($"VEG file ({veg.Text?.Length ?? 0} chars of script)");
                    break;
                }
                case "int":
                {
                    sb.AppendLine("Interaction record. Open in editor for full details.");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"(property dump failed: {ex.Message})");
        }
        if (sb.Length == 0) sb.AppendLine("(no summary available)");
        return sb.ToString();
    }

    private static string Build3DPropertyDump(string ext, dynamic? resource)
    {
        if (resource == null) return "";
        var sb = new StringBuilder();
        try
        {
            switch (ext.ToLowerInvariant())
            {
                case "8":
                {
                    var m = (_8Model)resource;
                    sb.AppendLine($"OCTREE map geometry");
                    sb.AppendLine($"Vertices: {m.GVP.Count}   Faces: {m.IDX.Count}");
                    sb.AppendLine($"Materials: {m.Materials.Count}   Textures: {m.TextureNames.Count}");
                    sb.AppendLine($"Lights: {m.Lights.Count}   Lightmap: {(m.HasLightmap ? "yes" : "no")}");
                    if (m.LightmapStart > 0)
                        sb.AppendLine($"LightmapStart (TXTD count1): {m.LightmapStart}");
                    break;
                }
                case "b3d":
                {
                    var m = (B3DModel)resource;
                    sb.AppendLine($"B3D model");
                    sb.AppendLine($"Vertices: {m.Model_Vertex_Position.Count}   Faces: {m.Model_Faces_Index.Count}");
                    sb.AppendLine($"Materials: {m.Materials.Count}   Bones: {m.Bones.Count}");
                    sb.AppendLine($"FormatVer: {m.FormatVer}   Coord: {m.CoordinateSystem}");
                    sb.AppendLine($"Token0x16: {(m.Token16Value.HasValue ? m.Token16Value.Value.ToString() : "-")}");
                    sb.AppendLine($"Token0x1C: {(m.HasToken1C ? m.Token1CByte.ToString() : "-")}");
                    sb.AppendLine($"Scene nodes: {m.NodeNames.Count}    NodeXforms: {m.NodeTransforms.Count}");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"(stats failed: {ex.Message})");
        }
        return sb.Length == 0 ? "" : sb.ToString();
    }

    private static string BuildHexDump(string name, string ext)
    {
        try
        {
            var raw = ResourceSystem.FindResource(name, ext) as byte[];
            if (raw == null) return "(no preview — extension not registered for byte-level access)";
            var n = Math.Min(raw.Length, 1024);
            var sb = new StringBuilder(n * 4);
            sb.Append($"file size: {raw.Length} bytes (showing first {n})\n\n");
            for (var off = 0; off < n; off += 16)
            {
                sb.Append($"{off:X8}  ");
                for (var i = 0; i < 16; i++)
                {
                    if (off + i < n) sb.Append($"{raw[off + i]:X2} ");
                    else sb.Append("   ");
                    if (i == 7) sb.Append(' ');
                }
                sb.Append(' ');
                for (var i = 0; i < 16 && off + i < n; i++)
                {
                    var b = raw[off + i];
                    sb.Append(b >= 0x20 && b < 0x7f ? (char)b : '.');
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"(hex preview failed: {ex.Message})";
        }
    }
}
