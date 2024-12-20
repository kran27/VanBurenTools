using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using AltUI.Forms;
using VBLauncher.My;
using VBLauncher.Properties;

namespace VBLauncher;

public partial class EditorWindow
{
    private void OpenFile()
    {
        ResetTempValues();
        // Loads all regions of a given file into their respective classes
        var ofd = new OpenFileDialog { Filter = "Van Buren Data File|*.amo;*.arm;*.con;*.crt;*.dor;*.int;*.itm;*.map;*.use;*.wea;*.veg", Multiselect = false, ValidateNames = true };
        if (ofd.ShowDialog() != DialogResult.OK) return;
        _filename = ofd.FileName.Split('\\').Last().Split('.')[0];
        _extension = "." + ofd.FileName.Split('.').Last();
        var fb = File.ReadAllBytes(ofd.FileName);
        LoadFile(fb, _extension);
    }

    private void OpenFromGRP()
    {
        ResetTempValues();
        using var grpb = new GrpBrowser(["amo", "arm", "con", "crt", "dor", "int", "itm", "map", "use", "wea", "veg"]);
        grpb.ShowDialog();
        _filename = grpb.FileName;
        LoadFile(grpb.FileBytes, "." + grpb.Extension);
    }

    private void SaveFile()
    {
        var sfd = new SaveFileDialog { Filter = $"Van Buren Data File|*{_extension}", ValidateNames = true, DefaultExt = _extension, FileName = _filename };
        if (sfd.ShowDialog() != DialogResult.OK) return;
        if (Settings.Default.STFEditEnabled && _stf is not null)
            File.WriteAllBytes(Settings.Default.STFDir, Extensions.TXTToSTF(_stf.ToArray()));
        if (_extension.EndsWith("veg"))
        {
            _currentFile.Text = _vegTextEditor.AllText;
            File.WriteAllBytes(sfd.FileName, _currentFile.Compile());
        }
        else
            File.WriteAllBytes(sfd.FileName, (byte[])_currentFile.ToByte().ToArray());
    }

    private void SetEngStfLocation()
    {
        var ofd = new OpenFileDialog { Multiselect = false, CheckFileExists = true, Filter = "English.stf|*.stf" };
        if (ofd.ShowDialog() != DialogResult.OK) return;
        Settings.Default.STFDir = ofd.FileName;
        Settings.Default.Save();
        _stf = Extensions.STFToTXT(File.ReadAllBytes(ofd.FileName)).ToArray();
    }

    private bool CheckAndLoadStf()
    {
        if (string.IsNullOrEmpty(Settings.Default.STFDir))
        {
            if (DarkMessageBox.ShowInformation("English.STF Location not set, please locate it.",
                    "English.stf Not Selected") != DialogResult.OK) return false;
            SetEngStfLocation();
            return true;

        }

        if (!File.Exists(Settings.Default.STFDir))
        {
            if (DarkMessageBox.ShowInformation("Previous English.STF not found, please select a new one.",
                    "English.stf Not Found") != DialogResult.OK) return false;
            SetEngStfLocation();
            return true;

        }

        _stf = Extensions.STFToTXT(File.ReadAllBytes(Settings.Default.STFDir)).ToArray();
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
                    _currentFile = fb.ReadAMO();
                    break;
                case ".arm":
                    _currentFile = fb.ReadARM();
                    break;
                case ".crt":
                    _currentFile = fb.ReadCRT();
                    break;
                case ".int":
                    DarkMessageBox.ShowMessage("Not yet implemented", "Not Implemented");
                    break;
                case ".itm":
                    _currentFile = fb.ReadITM();
                    break;
                case ".map":
                    _currentFile = fb.ReadMap();
                    break;
                // .con, .dor, .use are all the same format since the only different chunk is static and GOBJ
                // (shared between all) dictates type.
                case ".con":
                case ".dor":
                case ".use":
                    _currentFile = fb.ReadUSE();
                    break;
                case ".wea":
                    _currentFile = fb.ReadWEA();
                    break;
                case ".veg":
                    {
                        InitVEG();
                        _currentFile = new VEG(fb);
                        _vegTextEditor.AllText = _currentFile.Text;
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

