using System.IO;
using System.Linq;
using System.Windows.Forms;
using AltUI.Forms;
using VBLauncher.My;

namespace VBLauncher;

public partial class EditorWindow
{
    private void OpenFile()
    {
        // Loads all regions of a given file into their respective classes, and from there into the UI
        var ofd = new OpenFileDialog { Filter = "Van Buren Data File|*.amo;*.arm;*.con;*.crt;*.dor;*.int;*.itm;*.map;*.use;*.wea", Multiselect = false, ValidateNames = true };
        if (ofd.ShowDialog() != DialogResult.OK) return;
        _filename = ofd.FileName;
        _extension = _filename.Substring(_filename.LastIndexOf('.'), 4).ToLower();
        var fb = File.ReadAllBytes(_filename);
        LoadFile(fb, _extension);
    }

    private void OpenFromGRP()
    {
        using var grpb = new GrpBrowser(["amo", "arm", "con", "crt", "dor", "int", "itm", "map", "use", "wea"]);
        grpb.ShowDialog();
        _filename = grpb.FileName;
        LoadFile(grpb.FileBytes, "." + grpb.Extension);
    }

    private void SaveFile()
    {
        var sfd = new SaveFileDialog { Filter = $"Van Buren Data File|*{_extension}", ValidateNames = true, DefaultExt = _extension };
        if (sfd.ShowDialog() == DialogResult.OK)
        {
            if (MySettingsProperty.Settings.STFEditEnabled && _stf is not null)
                File.WriteAllBytes(MySettingsProperty.Settings.STFDir, Extensions.TXTToSTF(_stf.ToArray()));
            File.WriteAllBytes(sfd.FileName, (byte[])_currentFile.ToByte().ToArray());
        }
    }

    private void SetEngStfLocation()
    {
        var ofd = new OpenFileDialog { Multiselect = false, CheckFileExists = true, Filter = "English.stf|*.stf" };
        if (ofd.ShowDialog() != DialogResult.OK) return;
        MySettingsProperty.Settings.STFDir = ofd.FileName;
        _stf = Extensions.STFToTXT(File.ReadAllBytes(ofd.FileName)).ToArray();
    }

    private bool CheckAndLoadStf()
    {
        if (string.IsNullOrEmpty(MySettingsProperty.Settings.STFDir))
        {
            if (DarkMessageBox.ShowInformation("English.STF Location not set, please locate it.",
                    "English.stf Not Selected") != DialogResult.OK) return false;
            SetEngStfLocation();
            return true;

        }

        if (!File.Exists(MySettingsProperty.Settings.STFDir))
        {
            if (DarkMessageBox.ShowInformation("Previous English.STF not found, please select a new one.",
                    "English.stf Not Found") != DialogResult.OK) return false;
            SetEngStfLocation();
            return true;

        }

        _stf = Extensions.STFToTXT(File.ReadAllBytes(MySettingsProperty.Settings.STFDir)).ToArray();
        return true;
    }

    private void FullSTFToText()
    {
        var ofd = new OpenFileDialog { Filter = "String Table File|*.stf", Multiselect = false };
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            var sfd = new SaveFileDialog { Filter = "Text File|*.txt" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(sfd.FileName, Extensions.STFToTXT(File.ReadAllBytes(ofd.FileName)));
            }
        }
    }

    private void FullTextToSTF()
    {
        var ofd = new OpenFileDialog { Filter = "Text File|*.txt", Multiselect = false };
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            var sfd = new SaveFileDialog { Filter = "String Table File|*.stf" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sfd.FileName, Extensions.TXTToSTF(File.ReadAllLines(ofd.FileName)));
            }
        }
    }

    void ExtractAllGRPFiles(bool conv = false)
    {
        using var grpb = new GrpBrowser(null);
        grpb.ReadRHT();
        grpb.readExtensions();
        grpb.extractFile(-1, conv);
        DarkMessageBox.ShowInformation("Done Extracting", "Finished");
    }

    private void LoadFile(byte[] fb, string ext)
    {
        if (CheckAndLoadStf())
        {
            switch (ext ?? "")
            {
                case ".amo":
                    {
                        DarkMessageBox.ShowMessage("Not yet implemented", "Not Implemented");
                        break;
                    }
                case ".arm":
                    {
                        DarkMessageBox.ShowMessage("Not yet implemented", "Not Implemented");
                        break;
                    }
                case ".con":
                    {
                        DarkMessageBox.ShowMessage("Not yet implemented", "Not Implemented");
                        break;
                    }
                case ".crt":
                    {
                        _currentFile = fb.ReadCRT();
                        break;
                    }
                case ".dor":
                    {
                        DarkMessageBox.ShowMessage("Not yet implemented", "Not Implemented");
                        break;
                    }
                case ".int":
                    {
                        DarkMessageBox.ShowMessage("Not yet implemented", "Not Implemented");
                        break;
                    }
                case ".itm":
                    {
                        _currentFile = fb.ReadITM();
                        break;
                    }
                case ".map":
                    {
                        _currentFile = fb.ReadMap();
                        break;
                    }
                case ".use":
                    {
                        DarkMessageBox.ShowMessage("Not yet implemented", "Not Implemented");
                        break;
                    }
                case ".wea":
                    {
                        DarkMessageBox.ShowMessage("Not yet implemented", "Not Implemented");
                        break;
                    }
            }
        }
        else
        {
            DarkMessageBox.ShowError($".STF Not selected, loading of {_filename} aborted", ".STF Not Selected");
        }
    }
}

