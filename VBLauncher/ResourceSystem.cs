using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VBLauncher.VB3DLib;
using Veldrid.ImageSharp;

namespace VBLauncher;

// Replacement for GrpBrowser's internal resource system.
// Provides access to all game resources via their filename, just as the game does.
// Initialize() builds an index of every resource's name + extension + GRP location;
// the actual file bytes stay on disk and the parsed object is built only when a
// caller pulls the asset via FindResource. Iterating .Keys is therefore free —
// the heavy work happens once per asset on first access.

public static class ResourceSystem
{
    // Dictionary structure: grp -> ext -> name -> LazyResource
    // FindResource transparently unwraps the LazyResource for callers that
    // expect the parsed value. Direct dictionary access (e.g. iterating
    // Resources["Interface"]["tga"].Keys for type-ahead UI) only touches the
    // names — no GRP I/O happens until a key is actually fetched.
    public static Dictionary<string, Dictionary<string, Dictionary<string, dynamic>>> Resources { get; } = new(StringComparer.OrdinalIgnoreCase);

    // The GrpBrowser owns the cached lump tables and the file-name index. We
    // keep it alive for the lifetime of the process because the lazy
    // resources need to call back into it to fetch their bytes on demand.
    private static GrpBrowser? _grpb;

    static bool _isInitialized = false;
    public static void Initialize()
    {
        if (_isInitialized)
            return;
        try {
            // headless: skip the WinForms+WPF UI setup. ResourceSystem only
            // needs the GRP data layer (ReadRHT / readExtensions / lump
            // tables); instantiating the WPF Viewport3D was throwing the
            // "type initializer threw an exception" failure on this path.
            _grpb = new GrpBrowser(null, previewMode: false, headless: true);
            _grpb.ReadRHT();
            _grpb.readExtensions();
            for (var i = 0; i < _grpb.NumberOfEntries; i++)
            {
                var (grp, name, ext) = _grpb.GetFileMetadata(i);

                if (!Resources.TryGetValue(grp, out var grpDict))
                {
                    grpDict = new Dictionary<string, Dictionary<string, dynamic>>(StringComparer.OrdinalIgnoreCase);
                    Resources[grp] = grpDict;
                }
                if (!grpDict.TryGetValue(ext, out var extDict))
                {
                    extDict = new Dictionary<string, dynamic>(StringComparer.OrdinalIgnoreCase);
                    grpDict[ext] = extDict;
                }

                // Capture the per-entry context the loader needs. We cannot
                // capture `i` directly because the loop variable would
                // change semantics if this was a foreach over a non-array;
                // an explicit local also keeps the closure size minimal.
                var capturedIndex = i;
                var capturedName = name;
                var capturedExt = ext;
                extDict[name] = new LazyResource(() => LoadAsset(capturedIndex, capturedName, capturedExt));
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to initialize resource system: {ex.Message}", "Resource System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        _isInitialized = true;
    }

    // Reads the asset bytes from the GRP and turns them into the parsed type
    // the rest of the codebase expects. Called from LazyResource.Get() the
    // first time an asset is touched; subsequent accesses hit the cached
    // value inside the LazyResource. Failures are swallowed (returning null)
    // for the formats whose original initializer also swallowed them, so the
    // overall lazy/eager behavioural contract stays the same.
    private static dynamic? LoadAsset(int globalIndex, string name, string ext)
    {
        var fb = _grpb!.GetFileBytesByIndex(globalIndex);
        switch (ext)
        {
            case "bmp":
            case "tga":
                try { return new ImageSharpTexture(new MemoryStream(fb)); }
                catch { return null; }
            case "rle":
                return _grpb.DecodeRLEToImageSharp(fb);
            case "b3d":
                try
                {
                    var b3d = new B3DModel(fb);
                    Console.WriteLine($"[b3d] {name,-40} {b3d.DescribeTransformSignals()}");
                    return b3d;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load model {name}.{ext}: {ex.Message}");
                    return null;
                }
            case "8":
                return new _8Model(fb);
            case "amo":
                return fb.ReadAMO();
            case "arm":
                return fb.ReadARM();
            case "crt":
                return fb.ReadCRT();
            case "itm":
                return fb.ReadITM();
            case "map":
                try { return fb.ReadMap(); }
                catch { return null; }
            case "con":
            case "dor":
            case "use":
                return fb.ReadUSE();
            case "wea":
                return fb.ReadWEA();
            case "veg":
                return new VEG(fb);
            case "gr2":
            case "skl":
                return GrannyFormats.ReadFileFromMemory(fb);
            case "ini":
            case "tok":
            case "sco":
            case "pce":
            case "g3d":
            case "gsf":
            case "psf":
            case "pco":
            case "gls":
                return System.Text.Encoding.UTF8.GetString(fb);
            case "wav":
            case "int":
            case "amx":
            case "dlg":
                return fb;
            default:
                MessageBox.Show($"Unknown file type: {ext} for file {name}.{ext}", "Resource System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
        }
    }

    // Function to find a resource by its filename.
    // Intended to mimic the game's resource lookup system.
    public static dynamic? FindResource(string filename)
    {
        var parts = filename.Split('.');
        if (parts.Length < 2)
            return null;

        var ext = parts[^1].ToLowerInvariant();
        if (ext == "dds")
            ext = "tga"; // textures are stored as TGA but sometimes referenced as DDS
        var name = string.Join('.', parts, 0, parts.Length - 1);

        return FindResource(name, ext);
    }
    
    public static dynamic? FindResource(string name, string ext)
    {
        foreach (var grpDict in Resources.Values)
            if (grpDict.TryGetValue(ext, out var extDict) &&
                extDict.TryGetValue(name, out var res))
                return res is LazyResource lr ? lr.Get() : res;
        return null;
    }

}

public sealed class LazyResource
{
    private Func<dynamic?>? _factory;
    private dynamic? _value;
    private bool _loaded;
    private readonly object _lock = new();

    public LazyResource(Func<dynamic?> factory) => _factory = factory;

    public dynamic? Get()
    {
        if (_loaded) return _value;
        lock (_lock)
        {
            if (_loaded) return _value;
            _value = _factory!();
            _factory = null;
            _loaded = true;
            return _value;
        }
    }
}