using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VBLauncher;

public static class IniManager
{
    public static string F3Dir = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Personal)}\F3\F3.ini";
    public static string SysDir = @"Override\MenuMap\Engine\sys.ini";
    public static string[] F3Ini;
    public static string[] SysIni;

    public enum KeyType
    {
        Normal,
        Multiline
    }

    public static string Ini(ref string[] IniArray, string IniSection, string IniKey, KeyType KeyType = KeyType.Normal)
    {
        // Find bounds of section
        var SectionStart = Array.FindIndex(IniArray, x => x.StartsWith($"[{IniSection}]"));
        var SectionEnd = Array.FindIndex(IniArray, SectionStart + 1, x => x.StartsWith("[")) - 1;
        if (SectionEnd == -1)
            SectionEnd = IniArray.Length - 1;

        if (KeyType == KeyType.Normal)
        {
            // Find key location and value
            var KeyIndex = Array.FindIndex(IniArray, SectionStart + 1, x => x.StartsWith(IniKey));
            if (KeyIndex > SectionEnd)
                return null;
            var KeyLine = IniArray[KeyIndex];
            var KeyValue = KeyLine[(KeyLine.IndexOf("=", StringComparison.Ordinal) + 1)..].Trim();
            // Find comment, if any
            object Comment = null;
            try
            {
                Comment = KeyValue[KeyValue.IndexOf(";", StringComparison.Ordinal)..];
            }
            catch
            {
            }
            if (Comment is not null)
                KeyValue = KeyValue[..KeyValue.LastIndexOf(";", StringComparison.Ordinal)].Trim();
            // Return read value
            return KeyValue;
        }
        else
        {
            // Multiline key
            var KeyStart = Array.FindIndex(IniArray, SectionStart + 1, x => x.StartsWith($"<{IniKey}>"));
            var KeyEnd = Array.FindIndex(IniArray, KeyStart + 1, x => x.StartsWith($"</{IniKey}>"));
            if (KeyEnd < 0)
                KeyEnd = IniArray.Length - 1;
            // Get key value
            var KeyValue = string.Join(Environment.NewLine, IniArray, KeyStart + 1, KeyEnd - KeyStart - 1);
            // Return read value
            return KeyValue;
        }
    }

    public static void Ini(ref string[] IniArray, string IniSection, string IniKey, string Value, KeyType KeyType = KeyType.Normal)
    {
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
        // Find bounds of section
        var SectionStart = Array.FindIndex(IniArray, x => x.StartsWith($"[{IniSection}]"));
        var SectionEnd = Array.FindIndex(IniArray, SectionStart + 1, x => x.StartsWith("[")) - 1;
        if (SectionEnd < 0)
            SectionEnd = IniArray.Length - 1;
        // Return section
        return IniArray.Skip(SectionStart + 1).Take(SectionEnd - SectionStart).ToArray();
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