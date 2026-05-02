using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VBLauncher;

public static class IniManager
{
    private class SectionData
    {
        public string[]? Lines;
        public Dictionary<string, string> NormalKeys = new();
        public Dictionary<string, string> MultilineKeys = new();
    }

    private static readonly ConditionalWeakTable<string[], Dictionary<string, SectionData>> _cache = new();

    private static Dictionary<string, SectionData> GetCache(string[] array)
    {
        return _cache.GetValue(array, _ => new Dictionary<string, SectionData>());
    }

    private static SectionData GetSectionData(string[] array, string section)
    {
        var dict = GetCache(array);
        lock (dict)
        {
            if (dict.TryGetValue(section, out var data)) return data;

            data = new SectionData();
            // Find bounds of section
            var sectionStart = Array.FindIndex(array, x => x.StartsWith($"[{section}]"));
            if (sectionStart != -1)
            {
                var sectionEnd = Array.FindIndex(array, sectionStart + 1, x => x.StartsWith("["));
                if (sectionEnd == -1)
                    sectionEnd = array.Length;
                sectionEnd--;

                data.Lines = array.Skip(sectionStart + 1).Take(sectionEnd - sectionStart).ToArray();

                // Pre-parse keys in this section
                for (int i = sectionStart + 1; i <= sectionEnd; i++)
                {
                    var line = array[i].Trim();
                    if (string.IsNullOrEmpty(line) || line.StartsWith(";") || line.StartsWith("[")) continue;

                    if (line.StartsWith("<") && line.Contains(">"))
                    {
                        var keyEndTag = line.IndexOf(">", StringComparison.Ordinal);
                        var key = line[1..keyEndTag];
                        var endTag = $"</{key}>";
                        var keyEndIndex = Array.FindIndex(array, i + 1, x => x.StartsWith(endTag));
                        if (keyEndIndex != -1)
                        {
                            data.MultilineKeys[key] = string.Join(Environment.NewLine, array, i + 1, keyEndIndex - i - 1);
                            i = keyEndIndex;
                        }
                    }
                    else if (line.Contains("="))
                    {
                        var eqIndex = line.IndexOf("=", StringComparison.Ordinal);
                        var key = line[..eqIndex].Trim();
                        var value = line[(eqIndex + 1)..].Trim();

                        var commentIndex = value.LastIndexOf(";", StringComparison.Ordinal);
                        if (commentIndex != -1)
                        {
                            // Original logic for comments is a bit fragile, let's try to replicate it
                            // KeyValue = KeyValue[..KeyValue.LastIndexOf(";", StringComparison.Ordinal)].Trim();
                            value = value[..commentIndex].Trim();
                        }
                        data.NormalKeys[key] = value;
                    }
                }
            }
            dict[section] = data;
            return data;
        }
    }

    public static string F3Dir = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Personal)}\F3\F3.ini";
    public static string SysDir = @"Override\MenuMap\Engine\sys.ini";
    public static string[] F3Ini;
    public static string[] SysIni;

    public enum KeyType
    {
        Normal,
        Multiline
    }

    public static string? Ini(ref string[] IniArray, string IniSection, string IniKey, KeyType KeyType = KeyType.Normal)
    {
        var data = GetSectionData(IniArray, IniSection);
        if (KeyType == KeyType.Normal)
        {
            return data.NormalKeys.TryGetValue(IniKey, out var value) ? value : null;
        }
        else
        {
            return data.MultilineKeys.TryGetValue(IniKey, out var value) ? value : null;
        }
    }

    public static void Ini(ref string[] IniArray, string IniSection, string IniKey, string Value, KeyType KeyType = KeyType.Normal)
    {
        _cache.Remove(IniArray);
        // Find bounds of section
        var SectionStart = Array.FindIndex(IniArray, x => x.StartsWith($"[{IniSection}]"));
        var SectionEnd = Array.FindIndex(IniArray, SectionStart + 1, x => x.StartsWith("[")) - 1;
        if (SectionEnd < 0)
            SectionEnd = IniArray.Length - 1;

        if (KeyType == KeyType.Normal)
        {
            // Find key location and value
            var KeyIndex = Array.FindIndex(IniArray, SectionStart + 1, x => x.StartsWith(IniKey));
            if (KeyIndex > SectionEnd)
                return;
            var KeyLine = IniArray[KeyIndex];
            var KeyValue = KeyLine[(KeyLine.IndexOf("=") + 1)..].Trim();
            // Find comment, if any
            object Comment = null;
            try
            {
                Comment = KeyValue[KeyValue.IndexOf(";")..];
            }
            catch
            {
            }
            // Set to new value
            IniArray[KeyIndex] = $"{IniKey} = {Value} {Comment}".Trim();
        }
        else
        {
            var startIndex = Array.FindIndex(IniArray, SectionStart + 1, x => x.StartsWith($"<{IniKey}>"));
            var endIndex = Array.FindIndex(IniArray, startIndex + 1, x => x.StartsWith($"</{IniKey}>"));

            if (startIndex < SectionEnd & endIndex > SectionEnd)
            {
                var keyValueStart = startIndex + 1;
                var keyValueEnd = endIndex - 1;

                string[] newValues;
                newValues = new string[SectionEnd - SectionStart + 1 + keyValueEnd - keyValueStart + 2 + 1];
                Array.Copy(IniArray, SectionStart, newValues, 0, SectionEnd - SectionStart + 1);
                newValues[SectionEnd - SectionStart + 1] = $"<{IniKey}>";
                Array.Copy(Value.Split(Environment.NewLine), 0, newValues, SectionEnd - SectionStart + 2, keyValueEnd - keyValueStart + 1);
                newValues[newValues.Length - 1] = $"</{IniKey}>";
                IniArray = newValues;
            }
        }
    }


    public static string[] GetSection(this string[] IniArray, string IniSection)
    {
        var data = GetSectionData(IniArray, IniSection);
        return data.Lines ?? Array.Empty<string>();
    }

}

public static class General
{

    public static List<string> SearchForFiles(string RootFolder, string[] FileFilter)
    {
        var ReturnedData = new List<string>();
        var FolderStack = new Stack<string>();
        FolderStack.Push(RootFolder);
        while (FolderStack.Count > 0)
        {
            var ThisFolder = FolderStack.Pop();
            try
            {
                foreach (var SubFolder in Directory.GetDirectories(ThisFolder))
                    FolderStack.Push(SubFolder);
                foreach (var FileExt in FileFilter)
                    ReturnedData.AddRange(Directory.GetFiles(ThisFolder, FileExt));
            }
            catch
            {
            }
        }
        return ReturnedData;
    }

    public static void RemoveAt<T>(ref T[] arr, int index)
    {
        // create an array 1 element less than the input array
        var outArr = new T[arr.Length];
        // copy the first part of the input array
        Array.Copy(arr, 0, outArr, 0, index);
        // then copy the second part of the input array
        Array.Copy(arr, index + 1, outArr, index, arr.Length - 1 - index);

        arr = outArr;
    }

    public static void InsertAt<T>(ref T[] sourceArray, int insertIndex, T newValue)
    {

        int newPosition;
        int counter;

        newPosition = insertIndex;
        if (newPosition < 0)
            newPosition = 0;
        if (newPosition > sourceArray.Length)
            newPosition = sourceArray.Length;

        Array.Resize(ref sourceArray, sourceArray.Length + 1);

        var loopTo = newPosition;
        for (counter = sourceArray.Length - 2; counter >= loopTo; counter -= 1)
            sourceArray[counter + 1] = sourceArray[counter];

        sourceArray[newPosition] = newValue;
    }

    public static string RemoveExtension(this string fileName)
    {
        var lastDotIndex = fileName.LastIndexOf(".");
        return lastDotIndex == -1 ? fileName : fileName[..lastDotIndex];
    }

    public static string GetExtension(this string fileName)
    {
        var lastDotIndex = fileName.LastIndexOf(".");
        return lastDotIndex == -1 ? "" : fileName[(lastDotIndex + 1)..];
    }


}

public class VideoInfo
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DevModeW
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        private readonly string dmDeviceName;
        private readonly ushort dmSpecVersion;
        private readonly ushort dmDriverVersion;
        public ushort dmSize;
        private readonly ushort dmDriverExtra;
        private readonly uint dmFields;
        private readonly Struct1 Union1;
        private readonly short dmColor;
        private readonly short dmDuplex;
        private readonly short dmYResolution;
        private readonly short dmTTOption;
        private readonly short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        private readonly string dmFormName;
        private readonly ushort dmLogPixels;
        private readonly uint dmBitsPerPel;
        public readonly uint dmPelsWidth;
        public readonly uint dmPelsHeight;
        private readonly DFN Union2;
        public readonly uint dmDisplayFrequency;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct Struct1
    {
        [FieldOffset(0)]
        private readonly PDODFO Struct1Field;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct DFN
    {
        [FieldOffset(0)]
        private readonly uint dmDisplayFlags;
        [FieldOffset(0)]
        private readonly uint dmNup;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PDODFO
    {
        private readonly PointL dmPosition;
        private readonly uint dmDisplayOrientation;
        private readonly uint dmDisplayFixedOutput;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PointL
    {
        private readonly int x;
        private readonly int y;
    }

    [DllImport("user32.dll", EntryPoint = "EnumDisplaySettingsExW")]
    private static extern bool EnumDisplaySettingsExW([MarshalAs(UnmanagedType.LPWStr)] string DeviceName, int ModeNum, ref DevModeW DevMode, uint Flags);

    public static object[] GetResAsStrings()
    {
        return (from S in GetResolution()
                select $"{S.Width}x{S.Height}@{S.Hz}").ToArray();
    }

    private static Resolution[] GetResolution()
    {
        var DM = new DevModeW();
        var Index = 0;
        var SizeList = new List<Resolution>();
        DM.dmSize = (ushort)Marshal.SizeOf(typeof(DevModeW));
        while (EnumDisplaySettingsExW(Screen.PrimaryScreen.DeviceName, Index, ref DM, 0U))
        {
            var Resolution = new Resolution { Width = (int)DM.dmPelsWidth, Height = (int)DM.dmPelsHeight, Hz = (int)DM.dmDisplayFrequency };
            if (!SizeList.Contains(Resolution))
            {
                try
                {
                    if (SizeList.Last().Width == Resolution.Width)
                        SizeList.Remove(SizeList.Last());
                }
                catch
                {
                    // ignored
                }

                SizeList.Add(Resolution);
            }
            Index += 1;
        }
        return SizeList.ToArray();
    }

    public static Resolution StrToRes(string str)
    {
        try
        {
            var a = str.Split(['x']);
            var b = a[1].Split(['@']);
            return new Resolution
            {
                Width = int.Parse(a[0]),
                Height = int.Parse(b[0]),
                Hz = int.Parse(b[1])
            };
        }
        catch
        {
            return new Resolution
            {
                Width = 0,
                Height = 0,
                Hz = 0
            };
        }
    }

    public static string ResToStr(Resolution res, bool ShowHz = true, int mult = 1)
    {
        return $"{res.Width * mult}x{res.Height * mult}{(ShowHz ? $"@{res.Hz}" : "")}";
    }

}

public class Resolution
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int Hz { get; set; }
}