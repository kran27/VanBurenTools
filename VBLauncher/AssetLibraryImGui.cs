using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace VBLauncher;

// Per-asset 2× cell-size render slot. Built lazily as cells become
// visible and the per-frame budget allows. RenderState tracks whether
// we've already attempted (and failed) to render — failed entries
// keep a Binding=0 sentinel so DrawAssetCell falls back to the
// placeholder fill instead of re-queuing forever.
internal sealed class ThumbCacheEntry
{
    public Texture? Texture;
    public IntPtr Binding;
    public bool Failed;
}

// Owns a small dedicated SceneRenderer (rendering at 2× cell size for
// a crisp downscale via ImGui) plus a per-asset texture cache that
// the asset library samples through ImGui bindings registered against
// its own renderer. RenderOne does the real work: pull the entity,
// build the scene, configure an iso camera + camera-aligned headlight,
// draw, and CopyTexture the resulting framebuffer into the per-asset
// cache slot so the SceneRenderer can be reused for the next request.
internal sealed class ThumbnailRenderer
{
    public const uint ThumbSize = 144;

    private readonly GraphicsDevice _gd;
    private readonly ImGuiRenderer _imguiRenderer;
    private readonly Sdl2Window _window;
    private readonly SceneRenderer _scene;
    private readonly CommandList _copyCl;

    private readonly Dictionary<string, ThumbCacheEntry> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly Queue<(string Key, string Name, string Ext)> _pending = new();
    private readonly HashSet<string> _queued = new(StringComparer.OrdinalIgnoreCase);

    public ThumbnailRenderer(GraphicsDevice gd, ImGuiRenderer imguiRenderer, Sdl2Window window)
    {
        _gd = gd;
        _imguiRenderer = imguiRenderer;
        _window = window;
        var sceneCl = gd.ResourceFactory.CreateCommandList();
        _scene = new SceneRenderer(gd, imguiRenderer, window, sceneCl, (int)ThumbSize, (int)ThumbSize)
        {
            AllowInput = false,
        };
        _copyCl = gd.ResourceFactory.CreateCommandList();
    }

    // Returns the ImGui binding for this asset's thumbnail if rendered,
    // or IntPtr.Zero if pending / failed. First call on a key queues it.
    public IntPtr GetOrRequest(string name, string ext)
    {
        var key = $"{name}.{ext}";
        if (_cache.TryGetValue(key, out var e)) return e.Binding;
        if (_queued.Add(key)) _pending.Enqueue((key, name, ext));
        return IntPtr.Zero;
    }

    // Drain up to `budget` requests this frame. Each one runs a full
    // mini-scene-build + draw + texture-copy, so the budget keeps the
    // per-frame cost bounded while still making steady progress.
    public void ProcessFrame(int budget)
    {
        while (budget > 0 && _pending.Count > 0)
        {
            var req = _pending.Dequeue();
            RenderOne(req.Key, req.Name, req.Ext);
            budget--;
        }
    }

    private void RenderOne(string key, string name, string ext)
    {
        try
        {
            var resource = ResourceSystem.FindResource(name, ext);
            if (resource is not Entity entity)
            {
                _cache[key] = new ThumbCacheEntry { Failed = true };
                return;
            }

            _scene.Reset();
            _scene.AddEntity(entity);
            if (!_scene.TryGetCombinedAabb(out var min, out var max))
            {
                _cache[key] = new ThumbCacheEntry { Failed = true };
                return;
            }
            _scene.ConfigureForThumbnail(min, max);
            _scene.DrawScene(0f);

            // Allocate the per-asset destination texture matching the
            // scene's color-target format/size, copy in, then register
            // with ImGui for binding.
            var factory = _gd.ResourceFactory;
            var dst = factory.CreateTexture(TextureDescription.Texture2D(
                ThumbSize, ThumbSize, 1, 1,
                PixelFormat.R8_G8_B8_A8_UNorm,
                TextureUsage.Sampled));
            _copyCl.Begin();
            _copyCl.CopyTexture(_scene.SceneColorTarget, dst);
            _copyCl.End();
            _gd.SubmitCommands(_copyCl);

            var binding = _imguiRenderer.GetOrCreateImGuiBinding(factory, dst);
            _cache[key] = new ThumbCacheEntry { Texture = dst, Binding = binding };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[thumb] render failed for {key}: {ex.Message}");
            _cache[key] = new ThumbCacheEntry { Failed = true };
        }
    }
}

// Standalone asset-library window. Mirrors GrpBrowserImGui's host pattern
// (own SDL window + swapchain + ImGui context + command list) but the UI
// is purpose-built for placing entities into the editor's currently-loaded
// map: tabs by category, search, grid of clickable entries, click → host
// callback adds an EME2c with that name to the current Map.EME2 list.
//
// Thumbnails: each grid cell currently shows a flat gray placeholder
// where a 64×64 rendered icon will go. Wiring up live render-to-texture
// per asset is non-trivial (offscreen renderer, lazy load, GPU memory
// budget) — slot is in place via DrawAssetCell so we can fill it in
// without restructuring the layout.
public sealed class AssetLibraryImGui
{
    private readonly GraphicsDevice _gd;
    // Host plug-in: invoked when the user clicks an asset cell. Args are
    // the resource name (no extension) and extension. The host appends an
    // EME2c with name = "{name}.{ext}" to the loaded map and re-runs the
    // hash-driven scene rebuild so the entity appears in the viewport.
    private readonly Action<string, string>? _placeInMap;

    private Sdl2Window? _window;
    private Swapchain? _swapchain;
    private CommandList? _cl;
    private ImGuiRenderer? _imguiRenderer;
    private IntPtr _imGuiContext;
    private IntPtr _editorImGuiContext;
    private ThumbnailRenderer? _thumbs;
    // Render-budget per frame. Each thumbnail render involves loading
    // the entity (potentially heavy), AddEntity → AABB → DrawScene →
    // CopyTexture, so we keep this small to avoid hitches. Bumping it
    // shortens the time to fill a freshly-opened tab; lowering it
    // smooths scrolling through a tab full of unrendered cells.
    private const int ThumbsPerFrame = 2;

    public bool IsOpen => _window != null && _window.Exists;

    private string _filterRaw = string.Empty;
    private string _filterLower = string.Empty;

    // Categories surfaced as tabs. Each holds a sorted list of asset
    // names (no extension) culled from ResourceSystem at first paint and
    // cached for the lifetime of the window.
    private sealed record Category(string Label, string Ext);
    private static readonly Category[] Categories =
    [
        new("Critters",   "crt"),
        new("Items",      "itm"),
        new("Armor",      "arm"),
        new("Weapons",    "wea"),
        new("Ammo",       "amo"),
        new("Doors",      "dor"),
        new("Containers", "con"),
        new("Use Objs",   "use"),
    ];

    private readonly Dictionary<string, List<string>> _catalog = new(StringComparer.OrdinalIgnoreCase);
    private bool _catalogBuilt;

    // Cell sizing — 72px square plus a name strip beneath.
    private const float CellSize = 72f;
    private const float CellPadding = 6f;

    public AssetLibraryImGui(GraphicsDevice gd, Action<string, string>? placeInMap = null)
    {
        _gd = gd;
        _placeInMap = placeInMap;
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
            X = 260, Y = 260,
            WindowWidth = 760, WindowHeight = 600,
            WindowTitle = "Asset Library"
        });
        _window.Resized += OnResized;

        var src = VeldridStartup.GetSwapchainSource(_window);
        _swapchain = _gd.ResourceFactory.CreateSwapchain(new SwapchainDescription(
            src,
            (uint)_window.Width, (uint)_window.Height,
            PixelFormat.D24_UNorm_S8_UInt,
            true /* SyncToVerticalBlank */));

        _cl = _gd.ResourceFactory.CreateCommandList();

        _editorImGuiContext = ImGui.GetCurrentContext();
        _imguiRenderer = new ImGuiRenderer(
            _gd,
            _swapchain.Framebuffer.OutputDescription,
            _window.Width, _window.Height);
        _imGuiContext = ImGui.GetCurrentContext();
        ImGui.StyleColorsDark();

        // Thumbnail renderer is bound to this window's ImGuiRenderer
        // (per-asset texture bindings live in the renderer's id table,
        // which is per-context). Constructed after the renderer/context
        // are live so its child SceneRenderer can register textures.
        _thumbs = new ThumbnailRenderer(_gd, _imguiRenderer, _window);

        ImGui.SetCurrentContext(_editorImGuiContext);
    }

    public void Close()
    {
        // Drop the thumbnail renderer first; it holds GPU resources tied
        // to the ImGuiRenderer's id table which we're about to destroy.
        // Per-asset textures leak (small one-shot per Open/Close) — they
        // don't have an explicit owner; revisit if Toggle becomes hot.
        _thumbs = null;

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
    }

    public void Frame(float deltaTime)
    {
        if (_window == null) return;
        if (!_window.Exists) { Close(); return; }
        if (_swapchain == null || _cl == null || _imguiRenderer == null) return;

        var snapshot = _window.PumpEvents();
        if (!_window.Exists) { Close(); return; }

        _editorImGuiContext = ImGui.GetCurrentContext();
        ImGui.SetCurrentContext(_imGuiContext);
        try
        {
            _imguiRenderer.Update(deltaTime, snapshot);

            BuildCatalogOnce();
            // Render queued thumbnails BEFORE the UI is built so cells
            // that just came into view this frame can still pick up the
            // freshly-rendered binding from GetOrRequest. Keeps the
            // worst-case visible delay to a single frame instead of two.
            _thumbs?.ProcessFrame(ThumbsPerFrame);
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

    // Walks the resource index once. Each category's list is sorted
    // case-insensitively so the grid is alphabetised.
    private void BuildCatalogOnce()
    {
        if (_catalogBuilt) return;
        foreach (var cat in Categories)
        {
            var names = new List<string>();
            foreach (var grpDict in ResourceSystem.Resources.Values)
            {
                if (!grpDict.TryGetValue(cat.Ext, out var extDict)) continue;
                names.AddRange(extDict.Keys);
            }
            names.Sort(StringComparer.OrdinalIgnoreCase);
            _catalog[cat.Ext] = names;
        }
        _catalogBuilt = true;
    }

    private void BuildUI()
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(viewport.WorkPos);
        ImGui.SetNextWindowSize(viewport.WorkSize);
        var flags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
                    ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
        ImGui.Begin("##assetlib", flags);

        if (ImGui.InputTextWithHint("##filter", "filter (substring of name)", ref _filterRaw, 128))
            _filterLower = _filterRaw.ToLowerInvariant();
        ImGui.SameLine();
        ImGui.TextDisabled("click an asset to add it to the loaded map");

        if (ImGui.BeginTabBar("##cat"))
        {
            foreach (var cat in Categories)
            {
                _catalog.TryGetValue(cat.Ext, out var names);
                var count = names?.Count ?? 0;
                if (ImGui.BeginTabItem($"{cat.Label} ({count})"))
                {
                    DrawGrid(cat, names);
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndTabBar();
        }

        ImGui.End();
    }

    private void DrawGrid(Category cat, List<string>? names)
    {
        if (names == null || names.Count == 0)
        {
            ImGui.TextDisabled("no entries");
            return;
        }

        ImGui.BeginChild("##grid", new Vector2(0, 0), false);
        var avail = ImGui.GetContentRegionAvail().X;
        var stride = CellSize + CellPadding * 2f;
        var cols = Math.Max(1, (int)(avail / stride));

        var col = 0;
        foreach (var name in names)
        {
            if (_filterLower.Length > 0 && name.IndexOf(_filterLower, StringComparison.OrdinalIgnoreCase) < 0)
                continue;

            DrawAssetCell(cat, name);
            col++;
            if (col % cols != 0) ImGui.SameLine();
        }
        ImGui.EndChild();
    }

    // One grid cell. A square button (placeholder for the rendered icon)
    // followed by the asset's name. Click the button → host callback
    // places the asset in the loaded map.
    private void DrawAssetCell(Category cat, string name)
    {
        ImGui.BeginGroup();
        ImGui.PushID(cat.Ext + "/" + name);

        var cursor = ImGui.GetCursorScreenPos();
        // Invisible button reserves the icon's footprint and gives us
        // the click; the visual fill is drawn manually so we can swap
        // in a real thumbnail texture later without changing layout.
        var clicked = ImGui.InvisibleButton("##icon", new Vector2(CellSize, CellSize));
        var hovered = ImGui.IsItemHovered();

        var dl = ImGui.GetWindowDrawList();
        var min = cursor;
        var max = cursor + new Vector2(CellSize, CellSize);
        var bg = hovered
            ? ImGui.ColorConvertFloat4ToU32(new Vector4(0.30f, 0.30f, 0.36f, 1f))
            : ImGui.ColorConvertFloat4ToU32(new Vector4(0.18f, 0.18f, 0.22f, 1f));
        var border = ImGui.ColorConvertFloat4ToU32(new Vector4(0.45f, 0.45f, 0.52f, 1f));
        dl.AddRectFilled(min, max, bg, 4f);

        // Thumbnail: ThumbnailRenderer queues a render on first request,
        // returns a binding once ready. Until then, fall back to the
        // 3-letter ext placeholder so the cell still reads.
        var binding = _thumbs?.GetOrRequest(name, cat.Ext) ?? IntPtr.Zero;
        if (binding != IntPtr.Zero)
        {
            dl.AddImage(binding, min, max);
        }
        else
        {
            var label = cat.Ext.ToUpperInvariant();
            var sz = ImGui.CalcTextSize(label);
            dl.AddText(min + new Vector2((CellSize - sz.X) * 0.5f, (CellSize - sz.Y) * 0.5f),
                ImGui.ColorConvertFloat4ToU32(new Vector4(0.75f, 0.75f, 0.80f, 1f)), label);
        }
        dl.AddRect(min, max, border, 4f);

        // Name beneath, truncated to fit the cell width.
        var displayName = name;
        var nameWidth = ImGui.CalcTextSize(displayName).X;
        while (displayName.Length > 4 && nameWidth > CellSize)
        {
            displayName = displayName[..^1];
            nameWidth = ImGui.CalcTextSize(displayName + "…").X;
        }
        if (displayName.Length < name.Length) displayName += "…";
        ImGui.TextUnformatted(displayName);

        if (hovered) ImGui.SetTooltip($"{name}.{cat.Ext}\nclick to add to map");

        if (clicked) _placeInMap?.Invoke(name, cat.Ext);

        ImGui.PopID();
        ImGui.EndGroup();
    }
}
