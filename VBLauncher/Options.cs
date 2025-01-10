using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AltUI.Config;
using static VBLauncher.VideoInfo;

namespace VBLauncher;

public partial class Options
{
    private readonly MainMenuDef[] _mainMenus =
    [
        new("mainmenu.map", "0", "5.5", "0", "2", "0.75", "59.2"),
        new("zz_TestMapsaarontemp2.map", "0", "0", "0", "0", "0", "0"),
        new("zz_TestMapsTest_City_Building01.map", "0", "5.5", "0", "45", "-2.5", "57"),
        new("zz_TestMapsTest_City_Building02.map", "0", "5.5", "0", "43", "-2.5", "57"),
        new("zz_TestMapsTest_City_Building03.map", "0", "5.5", "0", "43", "-5", "57"),
        new("zz_TestMapsTest_City_Building04.map", "0", "5.5", "0", "43", "-5.5", "57"),
        new("98_Canyon_Random_01.map", "50", "5", "10", "61", "0", "45"),
        new("98_Canyon_Random_02.map", "55", "5", "10", "36", "-2.5", "50"),
        new("04_0202_Spelunking.map", "70", "5", "45", "15", "5", "50"),
        new("zz_TestMapsTest_City_Fences.map", "0", "40", "0", "42", "35", "50"),
        new("zz_TestMapsScottE_Test1.map", "85", "30", "30", "255", "39", "60"),
        new("zz_TestMapsScottE_Test2.map", "145", "80", "-85", "0.5", "25", "75"),
        new("zz_TestMapsScottE_Test4.map", "0", "7.5", "0", "45", "12.5", "50"),
        new("zz_TestMapsTest_Junktown_Shacks.map", "0", "50", "-10", "42", "39", "50"),
        new("Default_StartMap.map", "60", "7.5", "25", "270", "8", "27"),
        new("00_03_Tutorial_Junktown.map", "80", "7.5", "50", "5", "10", "68"),
        new("00_04_Tutorial_Vault.map", "50", "50.5", "0", "36", "25", "68")
    ];

    private readonly string[] _maps =
    [
        "mainmenu.map", "zz_TestMapsaarontemp2.map", "zz_TestMapsTest_City_Building01.map",
        "zz_TestMapsTest_City_Building02.map", "zz_TestMapsTest_City_Building03.map",
        "zz_TestMapsTest_City_Building04.map", "98_Canyon_Random_01.map", "98_Canyon_Random_02.map",
        "04_0202_Spelunking.map", "zz_TestMapsTest_City_Fences.map", "zz_TestMapsScottE_Test1.map",
        "zz_TestMapsScottE_Test2.map", "zz_TestMapsScottE_Test4.map", "zz_TestMapsTest_Junktown_Shacks.map",
        "Default_StartMap.map", "00_03_Tutorial_Junktown.map", "00_04_Tutorial_Vault.map"
    ];

    private string[] _dgV2Conf;
    private readonly string[] _aaModes = ["off", "2x", "4x", "8x"];
    private readonly string[] _fModes = ["appdriven", "pointsampled", "Linearmip", "2", "4", "8", "16"];
    private readonly string[] _ssModes = ["unforced", "2x", "3x", "4x"];
    private int _row;
    private string _key;
    private string _modifier;
    private bool _wantKey;

    public Options()
    {
        InitializeComponent();
    }

    private static void SetMainMenu(MainMenuDef mmd)
    {
        IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "map name", mmd.MapName);
        IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "target x", mmd.TargetX);
        IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "target y", mmd.TargetY);
        IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "target z", mmd.TargetZ);
        IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "azimuth", mmd.Azimuth);
        IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "elevation", mmd.Elevation);
        IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "fov", mmd.FOV);
    }

    private void CheckOptions(object sender, EventArgs e)
    {
        DarkTabControl1.Invalidate();
        IntrosCB.Checked = IniManager.Ini(ref IniManager.F3Ini, "Graphics", "enable startup movies") == "1";
        MainMenuCB.SelectedIndex = Array.IndexOf(_maps, IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "map name"));
        CameraCB.Checked = IniManager.Ini(ref IniManager.SysIni, "Camera", "FOV Min") == "0.5";
        AltCamCB.Checked = IniManager.Ini(ref IniManager.SysIni, "Camera", "Distance Max") == "70";
        var files = General.SearchForFiles("Override", ["*.map"]);
        foreach (var fi in files.Select(file => new FileInfo(file)).Where(fi => !NewGameCB.Items.Contains(fi.Name)))
        {
            NewGameCB.Items.Add(fi.Name);
        }

        NewGameCB.Text = IniManager.Ini(ref IniManager.SysIni, "Server", "Start map");
        DarkNumericUpDown1.Value = int.Parse(IniManager.Ini(ref IniManager.SysIni, "Server", "Start map entry point"));
        // Graphics
        TextureCB.BringToFront();
        SSFCB.BringToFront();
        if (File.Exists("dgVoodo.conf"))
        {
            _dgV2Conf = File.ReadAllLines("dgVoodoo.conf");
        }
        else
        {
            File.WriteAllText("dgVoodoo.conf", My.Resources.Resources.dgV2conf);
            _dgV2Conf = File.ReadAllLines("dgVoodoo.conf");
        }

        ResolutionCB.Items.Clear();
        ResolutionCB.Items.AddRange(GetResAsStrings());
        var inires = new Resolution
        {
            Width = int.Parse(IniManager.Ini(ref IniManager.F3Ini, "Graphics", "width")),
            Height = int.Parse(IniManager.Ini(ref IniManager.F3Ini, "Graphics", "height")),
            Hz = int.Parse(IniManager.Ini(ref IniManager.F3Ini, "Graphics", "refresh"))
        };
        ResolutionCB.SelectedItem = ResToStr(inires);
        SetupSscb(sender, e);
        FullscreenCB.Checked = IniManager.Ini(ref IniManager.F3Ini, "Graphics", "fullscreen") == "1";
        APICB.SelectedIndex = File.Exists("d3d11.dll") ? 1 : 0;

        AACB.SelectedIndex = _aaModes.ToList().IndexOf(IniManager.Ini(ref _dgV2Conf, "DirectX", "Antialiasing"));
        TextureCB.SelectedIndex = _fModes.ToList().IndexOf(IniManager.Ini(ref _dgV2Conf, "DirectX", "Filtering"));
        SSFCB.SelectedIndex = _ssModes.ToList().IndexOf(IniManager.Ini(ref _dgV2Conf, "DirectX", "Resolution"));
        try
        {
            MipmapCB.Checked = !bool.Parse(IniManager.Ini(ref _dgV2Conf, "DirectX", "DisableMipmapping"));
        }
        catch
        {
            // ignored
        }

        try
        {
            PhongCB.Checked = bool.Parse(IniManager.Ini(ref _dgV2Conf, "DirectX", "PhongShadingWhenPossible"));
        }
        catch
        {
            // ignored
        }

        LoadKeybinds(sender, e);
    }

    private void LoadKeybinds(object sender, EventArgs e)
    {
        DarkLabel1.Hide();
        DarkScrollBar1.BringToFront();
        var bindString = IniManager.F3Ini.GetSection("HotKeys");
        var binds = new List<Keybind>();
        foreach (var s in bindString)
        {
            if (s.StartsWith("+"))
            {
                var keys = s[..(s.IndexOf("=") - 1)].Trim().Split("+");
                var action = s[(s.IndexOf("=") + 1)..].Trim();
                if (keys.Length > 2)
                {
                    binds.Add(new Keybind(keys[2], keys[1], action));
                }
                else
                {
                    binds.Add(new Keybind(keys[1], "", action));
                }
            }
            else if (s.StartsWith("-"))
            {
                var keys = s[..(s.IndexOf("=") - 1)].Trim().Split("-");
                var action = s[(s.IndexOf("=") + 1)..].Trim();
                binds.Add(keys.Length > 2
                    ? new Keybind(keys[2], keys[1], action, false)
                    : new Keybind(keys[1], "", action, false));
            }
        }

        var dt = new DataTable();

        dt.Columns.Add("Modifier");
        dt.Columns.Add("Key");
        dt.Columns.Add("Action");
        dt.Columns.Add("On Key");
        foreach (var bind in binds)
            dt.Rows.Add(bind.Modifier, bind.Key, bind.Action, bind.OnPress ? "Down" : "Up");
        DataGridView1.GridColor = ThemeProvider.Theme.Colors.GreySelection;
        DataGridView1.BackgroundColor = ThemeProvider.Theme.Colors.LightBackground;
        DarkScrollBar1.BackColor = ThemeProvider.Theme.Colors.LightBackground;
        DataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        var cellStyle = new DataGridViewCellStyle
        {
            BackColor = ThemeProvider.Theme.Colors.LightBackground,
            ForeColor = ThemeProvider.Theme.Colors.LightText,
            SelectionBackColor = ThemeProvider.Theme.Colors.BlueSelection,
            SelectionForeColor = ThemeProvider.Theme.Colors.LightText
        };
        DataGridView1.DefaultCellStyle = cellStyle;
        DataGridView1.ColumnHeadersDefaultCellStyle = cellStyle;
        DataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        DataGridView1.EnableHeadersVisualStyles = false;
        DataGridView1.DataSource = dt;
        DataGridView1.Columns[0].ReadOnly = true;
        DataGridView1.Columns[1].ReadOnly = true;
        DataGridView1.Columns[3].ReadOnly = true;
    }

    private void ApplyChanges(object sender, EventArgs e)
    {
        IniManager.Ini(ref IniManager.F3Ini, "Graphics", "enable startup movies",
            (IntrosCB.Checked ? 1 : 0).ToString());
        SetMainMenu(_mainMenus[MainMenuCB.SelectedIndex]);
        IniManager.Ini(ref IniManager.SysIni, "Camera", "Distance Max", (AltCamCB.Checked ? 70 : 350).ToString());
        IniManager.Ini(ref IniManager.SysIni, "Camera", "Distance Min", (AltCamCB.Checked ? 70 : 350).ToString());
        IniManager.Ini(ref IniManager.SysIni, "Camera", "FOV Speed", (CameraCB.Checked ? 10d : 32.5d).ToString());
        IniManager.Ini(ref IniManager.SysIni, "Camera", "Scroll Speed", (CameraCB.Checked ? 250 : 96).ToString());
        IniManager.Ini(ref IniManager.SysIni, "Camera", "FOV Min",
            (CameraCB.Checked ? 0.5d : AltCamCB.Checked ? 30 : 6).ToString());
        IniManager.Ini(ref IniManager.SysIni, "Camera", "FOV Max",
            (CameraCB.Checked ? 100 : AltCamCB.Checked ? 75 : 15).ToString());
        IniManager.Ini(ref IniManager.SysIni, "Server", "Start map", NewGameCB.Text);
        IniManager.Ini(ref IniManager.SysIni, "Server", "Start map entry point",
            DarkNumericUpDown1.Value.ToString());
        var res = StrToRes(ResolutionCB.Text);
        IniManager.Ini(ref IniManager.F3Ini, "Graphics", "fullscreen", (FullscreenCB.Checked ? 1 : 0).ToString());
        IniManager.Ini(ref IniManager.F3Ini, "Graphics", "width", res.Width.ToString());
        IniManager.Ini(ref IniManager.F3Ini, "Graphics", "height", res.Height.ToString());
        switch (APICB.SelectedIndex)
        {
            case 0:
                {
                    File.Delete("d3d9.dll");
                    File.Delete("d3d11.dll");
                    File.Delete("dxgi.dll");
                    File.Delete("wined3d.dll");
                    File.WriteAllBytes("d3d8.dll", My.Resources.Resources.D3D8);
                    break;
                }
            case 1:
                {
                    File.Delete("d3d9.dll");
                    File.Delete("wined3d.dll");
                    File.WriteAllBytes("d3d11.dll", My.Resources.Resources.d3d11);
                    File.WriteAllBytes("dxgi.dll", My.Resources.Resources.dxgi);
                    break;
                }
        }

        IniManager.Ini(ref _dgV2Conf, "DirectX", "Antialiasing", _aaModes[AACB.SelectedIndex]);
        IniManager.Ini(ref _dgV2Conf, "DirectX", "Filtering", _fModes[TextureCB.SelectedIndex]);
        IniManager.Ini(ref _dgV2Conf, "DirectX", "Resolution", _ssModes[SSFCB.SelectedIndex]);
        IniManager.Ini(ref _dgV2Conf, "DirectX", "DisableMipmapping", !MipmapCB.Checked ? "true" : "false");
        IniManager.Ini(ref _dgV2Conf, "DirectX", "PhongShadingWhenPossible", PhongCB.Checked ? "true" : "false");
        IniManager.Ini(ref _dgV2Conf, "GeneralExt", "FPSLimit", res.Hz.ToString());
        IniManager.Ini(ref IniManager.F3Ini, "Graphics", "refresh", res.Hz.ToString());
        var newBinds = (from DataGridViewRow r in DataGridView1.Rows
                        select RowToStr(r)).ToList();

        var sectionStart = Array.FindIndex(IniManager.F3Ini, x => x.StartsWith("[HotKeys]"));
        var sectionEnd = Array.FindIndex(IniManager.F3Ini, sectionStart + 1, x => x.StartsWith("[")) - 1;
        if (sectionEnd < 0)
            sectionEnd = IniManager.F3Ini.Length - 1;

        for (int i = sectionStart + 1, loopTo = sectionEnd - 1; i <= loopTo; i++)
            General.RemoveAt(ref IniManager.F3Ini, sectionStart + 1);

        for (int i = 1, loopTo1 = newBinds.Count() - 1; i <= loopTo1; i++)
            General.InsertAt(ref IniManager.F3Ini, sectionStart + i, newBinds[i - 1]);
        File.WriteAllLines("dgVoodoo.conf", _dgV2Conf);
        File.WriteAllLines(IniManager.SysDir, IniManager.SysIni);
        File.WriteAllLines(IniManager.F3Dir, IniManager.F3Ini);
        Hide();
    }

    private void SetupSscb(object sender, EventArgs e)
    {
        var index = SSFCB.SelectedIndex;
        var res = StrToRes(ResolutionCB.Text);
        SSFCB.Items.Clear();
        object[] resolutions = [ResToStr(res, false), ResToStr(res, false, 2), ResToStr(res, false, 3), ResToStr(res, false, 4)];
        SSFCB.Items.AddRange(resolutions);
        SSFCB.SelectedIndex = index;
    }

    private void NewGameToolTip(object sender, EventArgs e)
    {
        ToolTip1.SetToolTip(NewGameCB, NewGameCB.Text);
    }
    private static string RowToStr(DataGridViewRow row)
    {
        var ch = (string)row.Cells[3].Value == "Up"
            ? "-"
            : "+";
        return (string)row.Cells[0].Value == ""
            ? $"{ch}{row.Cells[1].Value} = {row.Cells[2].Value}"
            : $"{ch}{row.Cells[0].Value}{ch}{row.Cells[1].Value} = {row.Cells[2].Value}";
    }

    private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
            return;
        switch (e.ColumnIndex)
        {
            case 0:
            case 1:
                {
                    _row = e.RowIndex;
                    DarkLabel1.Show();
                    _wantKey = true;
                    break;
                }
            case 2:
                {
                    DarkLabel1.Hide();
                    _wantKey = false;
                    break;
                }
            case 3:
                {
                    if (ReferenceEquals(DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value, DBNull.Value))
                        DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "Up";
                    DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value =
                        (string)DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == "Up"
                            ? "Down"
                            : "Up";
                    DarkLabel1.Hide();
                    _wantKey = false;
                    break;
                }
        }
    }

    private void DataGridView1_ScrollChanged(object sender, EventArgs e)
    {
        DarkScrollBar1.ScrollTo((int)Math.Round(DataGridView1.FirstDisplayedScrollingRowIndex /
            (double)(DataGridView1.Rows.Count - DataGridView1.DisplayedRowCount(false)) * DarkScrollBar1.Maximum));
    }

    private void DarkScrollBar1_Click(object sender, EventArgs e)
    {
        Timer1.Start();
    }

    private void DarkScrollBar1_MouseUp(object sender, EventArgs e)
    {
        Timer1.Stop();
    }

    private void Timer1_Tick(object sender, EventArgs e)
    {
        DataGridView1.FirstDisplayedScrollingRowIndex = (int)Math.Round(DarkScrollBar1.Value /
            (double)DarkScrollBar1.Maximum * (DataGridView1.Rows.Count - DataGridView1.DisplayedRowCount(false)));
    }

    private void DataGridView1_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.ControlKey:
            case Keys.Menu:
            case Keys.ShiftKey:
                {
                    break;
                }

            default:
                {
                    if (_wantKey)
                    {
                        _modifier = e.Modifiers.ToString();
                        _key = e.KeyCode.ToString();
                        DataGridView1.Rows[_row].Cells[0].Value = _properName.TryGetValue(_modifier, out var value) ? value : _modifier;
                        DataGridView1.Rows[_row].Cells[1].Value = _properName.TryGetValue(_key, out var value1) ? value1 : _key;
                    }

                    break;
                }
        }
    }

    private readonly Dictionary<string, string> _properName = new()
    {
        { "D1", "1" },
        { "D2", "2" },
        { "D3", "3" },
        { "D4", "4" },
        { "D5", "5" },
        { "D6", "6" },
        { "D7", "7" },
        { "D8", "8" },
        { "D9", "9" },
        { "D0", "0" },
        { "Control", "Ctrl" },
        { "None", "" },
        { "Oemtilde", "`" },
        { "Oemminus", "-" },
        { "Oemplus", "+" },
        { "OemOpenBrackets", "[" },
        { "Oem6", "]" },
        { "Oem5", @"\" },
        { "Oem1", ";" },
        { "Oem7", "\"" },
        { "Oemcomma", "," },
        { "Oemperiod", "." },
        { "OemQuestion", "/" }
    };
}

public class MainMenuDef(
    string mapName,
    string targetX,
    string targetY,
    string targetZ,
    string azimuth,
    string elevation,
    string fov)
{
    public string MapName { get; set; } = mapName;
    public string TargetX { get; set; } = targetX;
    public string TargetY { get; set; } = targetY;
    public string TargetZ { get; set; } = targetZ;
    public string Azimuth { get; set; } = azimuth;
    public string Elevation { get; set; } = elevation;
    public string FOV { get; set; } = fov;
}

public class Keybind(string key, string modifier, string action, bool onPress = true)
{
    public readonly string Key = key;
    public readonly string Modifier = modifier;
    public readonly string Action = action;
    public readonly bool OnPress = onPress;
}