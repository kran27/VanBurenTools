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
// Assets are parsed upon initialization and stored in a nested dictionary structure ready for access.

public static class ResourceSystem
{
    // Dictionary structure: grp -> ext -> name -> resource
    // Returned resource is it's parsed type, e.g. ImageSharpTexture, B3DModel, etc.
    // You are expected to know what type of resource you are looking for. No mechanism for automatic type detection is provided.
    public static Dictionary<string, Dictionary<string, Dictionary<string, dynamic>>> Resources { get; } = new(StringComparer.OrdinalIgnoreCase);
    
    static bool _isInitialized = false;
    public static void Initialize()
    {
        if (_isInitialized)
            return;
        try {
            using var grpb = new GrpBrowser(null);
            grpb.ReadRHT();
            grpb.readExtensions();
            for (var i = 0; i < grpb.NumberOfEntries; i++)
            {
                var (grp, name, ext, fb) = grpb.ExtractFileToMemory(i);
            
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
            
                switch (ext)
                {
                    case "bmp":
                    case "tga":
                        try {
                            extDict[name] = new ImageSharpTexture(new MemoryStream(fb));
                        } catch (Exception ex) {
                            //MessageBox.Show($"Failed to load texture {name}.{ext}: {ex.Message}");
                        }
                        break;
                    case "rle":
                        extDict[name] = grpb.DecodeRLEToImageSharp(fb);
                        break;
                    case "b3d":
                        try
                        {
                            extDict[name] = new B3DModel(fb);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Failed to load model {name}.{ext}: {ex.Message}");
                        }
                        break;
                    case "8":
                        extDict[name] = new _8Model(fb, true); // TODO: REPLACE HARDCODED VALUE
                        break;
                    case "amo":
                        extDict[name] = fb.ReadAMO();
                        break;
                    case "arm":
                        extDict[name] = fb.ReadARM();
                        break;
                    case "crt":
                        extDict[name] = fb.ReadCRT();
                        break;
                    case "itm":
                        extDict[name] = fb.ReadITM();
                        break;
                    case "map":
                        try {
                            extDict[name] = fb.ReadMap();
                        }
                        catch (Exception ex) {
                            //MessageBox.Show($"Failed to load map {name}.{ext}: {ex.Message}");
                        }
                        break;
                    case "con":
                    case "dor":
                    case "use":
                        extDict[name] = fb.ReadUSE();
                        break;
                    case "wea":
                        extDict[name] = fb.ReadWEA();
                        break;
                    case "veg":
                        extDict[name] = new VEG(fb);
                        break;
                    case "gr2":
                    case "skl":
                        extDict[name] = GrannyFormats.ReadFileFromMemory(fb);
                        break;
                    case "ini":
                    case "tok":
                    case "sco":
                    case "pce":
                    case "g3d":
                    case "gsf":
                    case "psf":
                    case "pco":
                    case "gls":
                        extDict[name] = System.Text.Encoding.UTF8.GetString(fb);
                        break;
                    case "wav":
                    case "int":
                    case "amx":
                    case "dlg":
                        extDict[name] = fb;
                        break;
                    default:
                        MessageBox.Show($"Unknown file type: {ext} for file {name}.{ext}", "Resource System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to initialize resource system: {ex.Message}", "Resource System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        _isInitialized = true;
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
                return res;
        return null;
    }
}