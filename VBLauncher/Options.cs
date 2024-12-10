using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AltUI.Config;
using Microsoft.VisualBasic.CompilerServices;
using static VBLauncher.VideoInfo;

namespace VBLauncher
{
    public partial class Options
    {
        private readonly MainMenuDef[] MainMenus = new[]
        {
            MMD("mainmenu.map", 0.ToString(), 5.5d.ToString(), 0.ToString(), 2.ToString(), 0.75d.ToString(),
                59.2d.ToString()),
            MMD("zz_TestMapsaarontemp2.map", 0.ToString(), 0.ToString(), 0.ToString(), 0.ToString(), 0.ToString(),
                0.ToString()),
            MMD("zz_TestMapsTest_City_Building01.map", 0.ToString(), 5.5d.ToString(), 0.ToString(), 45.ToString(),
                (-2.5d).ToString(), 57.ToString()),
            MMD("zz_TestMapsTest_City_Building02.map", 0.ToString(), 5.5d.ToString(), 0.ToString(), 43.ToString(),
                (-2.5d).ToString(), 57.ToString()),
            MMD("zz_TestMapsTest_City_Building03.map", 0.ToString(), 5.5d.ToString(), 0.ToString(), 43.ToString(),
                (-5).ToString(), 57.ToString()),
            MMD("zz_TestMapsTest_City_Building04.map", 0.ToString(), 5.5d.ToString(), 0.ToString(), 43.ToString(),
                (-5.5d).ToString(), 57.ToString()),
            MMD("98_Canyon_Random_01.map", 50.ToString(), 5.ToString(), 10.ToString(), 61.ToString(), 0.ToString(),
                45.ToString()),
            MMD("98_Canyon_Random_02.map", 55.ToString(), 5.ToString(), 10.ToString(), 36.ToString(),
                (-2.5d).ToString(), 50.ToString()),
            MMD("04_0202_Spelunking.map", 70.ToString(), 5.ToString(), 45.ToString(), 15.ToString(), 5.ToString(),
                50.ToString()),
            MMD("zz_TestMapsTest_City_Fences.map", 0.ToString(), 40.ToString(), 0.ToString(), 42.ToString(),
                35.ToString(), 50.ToString()),
            MMD("zz_TestMapsScottE_Test1.map", 85.ToString(), 30.ToString(), 30.ToString(), 255.ToString(),
                39.ToString(), 60.ToString()),
            MMD("zz_TestMapsScottE_Test2.map", 145.ToString(), 80.ToString(), (-85).ToString(), 0.5d.ToString(),
                25.ToString(), 75.ToString()),
            MMD("zz_TestMapsScottE_Test4.map", 0.ToString(), 7.5d.ToString(), 0.ToString(), 45.ToString(),
                12.5d.ToString(), 50.ToString()),
            MMD("zz_TestMapsTest_Junktown_Shacks.map", 0.ToString(), 50.ToString(), (-10).ToString(), 42.ToString(),
                39.ToString(), 50.ToString()),
            MMD("Default_StartMap.map", 60.ToString(), 7.5d.ToString(), 25.ToString(), 270.ToString(), 8.ToString(),
                27.ToString()),
            MMD("00_03_Tutorial_Junktown.map", 80.ToString(), 7.5d.ToString(), 50.ToString(), 5.ToString(),
                10.ToString(), 68.ToString()),
            MMD("00_04_Tutorial_Vault.map", 50.ToString(), 50.5d.ToString(), 0.ToString(), 36.ToString(), 25.ToString(),
                68.ToString())
        };

        private readonly string[] Maps = new[]
        {
            "mainmenu.map", "zz_TestMapsaarontemp2.map", "zz_TestMapsTest_City_Building01.map",
            "zz_TestMapsTest_City_Building02.map", "zz_TestMapsTest_City_Building03.map",
            "zz_TestMapsTest_City_Building04.map", "98_Canyon_Random_01.map", "98_Canyon_Random_02.map",
            "04_0202_Spelunking.map", "zz_TestMapsTest_City_Fences.map", "zz_TestMapsScottE_Test1.map",
            "zz_TestMapsScottE_Test2.map", "zz_TestMapsScottE_Test4.map", "zz_TestMapsTest_Junktown_Shacks.map",
            "Default_StartMap.map", "00_03_Tutorial_Junktown.map", "00_04_Tutorial_Vault.map"
        };

        private string[] dgV2Conf;
        private readonly string[] AAModes = new[] { "off", "2x", "4x", "8x" };
        private readonly string[] FModes = new[] { "appdriven", "pointsampled", "Linearmip", "2", "4", "8", "16" };
        private readonly string[] SSModes = new[] { "unforced", "2x", "3x", "4x" };
        private int Row;
        private string Key;
        private string Modifier;
        private bool WantKey;

        public Options()
        {
            InitializeComponent();
        }

        private static void SetMainMenu(MainMenuDef MMD)
        {
            IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "map name", MMD.MapName);
            IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "target x", MMD.TargetX);
            IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "target y", MMD.TargetY);
            IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "target z", MMD.TargetZ);
            IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "azimuth", MMD.Azimuth);
            IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "elevation", MMD.Elevation);
            IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "fov", MMD.FOV);
        }

        private static MainMenuDef MMD(string MapName, string TargetX, string TargetY, string TargetZ, string Azimuth,
            string Elevation, string FOV)
        {
            return new MainMenuDef
            {
                MapName = MapName,
                TargetX = TargetX,
                TargetY = TargetY,
                TargetZ = TargetZ,
                Azimuth = Azimuth,
                Elevation = Elevation,
                FOV = FOV
            };
        }

        private void CheckOptions(object sender, EventArgs e)
        {
            DarkTabControl1.Invalidate();
            IntrosCB.Checked =
                Conversions.ToDouble(IniManager.Ini(ref IniManager.F3Ini, "Graphics", "enable startup movies")) == 1d;
            MainMenuCB.SelectedIndex =
                Array.IndexOf(Maps, IniManager.Ini(ref IniManager.SysIni, "Mainmenu", "map name"));
            CameraCB.Checked = Conversions.ToDouble(IniManager.Ini(ref IniManager.SysIni, "Camera", "FOV Min")) == 0.5d;
            AltCamCB.Checked = Conversions.ToDouble(IniManager.Ini(ref IniManager.SysIni, "Camera", "Distance Max")) ==
                               70d;
            var Files = General.SearchForFiles("Override", new[] { "*.map" });
            foreach (var File in Files)
            {
                var FI = new FileInfo(File);
                if (!NewGameCB.Items.Contains(FI.Name))
                    NewGameCB.Items.Add(FI.Name);
            }

            NewGameCB.Text = IniManager.Ini(ref IniManager.SysIni, "Server", "Start map");
            DarkNumericUpDown1.Value =
                Conversions.ToDecimal(IniManager.Ini(ref IniManager.SysIni, "Server", "Start map entry point"));
            // Graphics
            TextureCB.BringToFront();
            SSFCB.BringToFront();
            try
            {
                dgV2Conf = File.ReadAllLines("dgVoodoo.conf");
            }
            catch
            {
                File.Exists("dgVoodoo.conf");
                File.WriteAllText("dgVoodoo.conf", My.Resources.Resources.dgV2conf);
                dgV2Conf = File.ReadAllLines("dgVoodoo.conf");
            }

            ResolutionCB.Items.Clear();
            ResolutionCB.Items.AddRange(GetResAsStrings());
            var inires = new Resolution
            {
                Width = Conversions.ToInteger(IniManager.Ini(ref IniManager.F3Ini, "Graphics", "width")),
                Height = Conversions.ToInteger(IniManager.Ini(ref IniManager.F3Ini, "Graphics", "height")),
                Hz = Conversions.ToInteger(IniManager.Ini(ref IniManager.F3Ini, "Graphics", "refresh"))
            };
            ResolutionCB.SelectedItem = ResToStr(inires);
            SetupSSCB(sender, e);
            FullscreenCB.Checked =
                Conversions.ToDouble(IniManager.Ini(ref IniManager.F3Ini, "Graphics", "fullscreen")) == 1d;
            if (File.Exists("d3d11.dll"))
            {
                APICB.SelectedIndex = 1;
            }
            else
            {
                APICB.SelectedIndex = 0;
            }

            AACB.SelectedIndex = AAModes.ToList().IndexOf(IniManager.Ini(ref dgV2Conf, "DirectX", "Antialiasing"));
            TextureCB.SelectedIndex = FModes.ToList().IndexOf(IniManager.Ini(ref dgV2Conf, "DirectX", "Filtering"));
            SSFCB.SelectedIndex = SSModes.ToList().IndexOf(IniManager.Ini(ref dgV2Conf, "DirectX", "Resolution"));
            try
            {
                MipmapCB.Checked = !bool.Parse(IniManager.Ini(ref dgV2Conf, "DirectX", "DisableMipmapping"));
            }
            catch
            {
            }

            try
            {
                PhongCB.Checked = bool.Parse(IniManager.Ini(ref dgV2Conf, "DirectX", "PhongShadingWhenPossible"));
            }
            catch
            {
            }

            LoadKeybinds(sender, e);
        }

        private void LoadKeybinds(object sender, EventArgs e)
        {
            DarkLabel1.Hide();
            DarkScrollBar1.BringToFront();
            var BindString = IniManager.F3Ini.GetSection("HotKeys");
            var Binds = new List<Keybind>();
            foreach (var s in BindString)
            {
                if (s.StartsWith("+"))
                {
                    var keys = s.Substring(0, s.IndexOf("=") - 1).Trim().Split("+");
                    var action = s.Substring(s.IndexOf("=") + 1).Trim();
                    if (keys.Length > 2)
                    {
                        Binds.Add(new Keybind(keys[2], keys[1], action));
                    }
                    else
                    {
                        Binds.Add(new Keybind(keys[1], "", action));
                    }
                }
                else if (s.StartsWith("-"))
                {
                    var keys = s.Substring(0, s.IndexOf("=") - 1).Trim().Split("-");
                    var action = s.Substring(s.IndexOf("=") + 1).Trim();
                    if (keys.Length > 2)
                    {
                        Binds.Add(new Keybind(keys[2], keys[1], action, false));
                    }
                    else
                    {
                        Binds.Add(new Keybind(keys[1], "", action, false));
                    }
                }
            }

            var dt = new DataTable();

            dt.Columns.Add("Modifier");
            dt.Columns.Add("Key");
            dt.Columns.Add("Action");
            dt.Columns.Add("On Key");
            foreach (var bind in Binds)
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
                (IntrosCB.Checked ? 1 : 0).ToString(), IniManager.KeyType.Normal);
            SetMainMenu(MainMenus[MainMenuCB.SelectedIndex]);
            IniManager.Ini(ref IniManager.SysIni, "Camera", "Distance Max", (AltCamCB.Checked ? 70 : 350).ToString(),
                IniManager.KeyType.Normal);
            IniManager.Ini(ref IniManager.SysIni, "Camera", "Distance Min", (AltCamCB.Checked ? 70 : 350).ToString(),
                IniManager.KeyType.Normal);
            IniManager.Ini(ref IniManager.SysIni, "Camera", "FOV Speed", (CameraCB.Checked ? 10d : 32.5d).ToString(),
                IniManager.KeyType.Normal);
            IniManager.Ini(ref IniManager.SysIni, "Camera", "Scroll Speed", (CameraCB.Checked ? 250 : 96).ToString(),
                IniManager.KeyType.Normal);
            IniManager.Ini(ref IniManager.SysIni, "Camera", "FOV Min",
                (CameraCB.Checked ? 0.5d : AltCamCB.Checked ? 30 : 6).ToString(), IniManager.KeyType.Normal);
            IniManager.Ini(ref IniManager.SysIni, "Camera", "FOV Max",
                (CameraCB.Checked ? 100 : AltCamCB.Checked ? 75 : 15).ToString(), IniManager.KeyType.Normal);
            IniManager.Ini(ref IniManager.SysIni, "Server", "Start map", NewGameCB.Text, IniManager.KeyType.Normal);
            IniManager.Ini(ref IniManager.SysIni, "Server", "Start map entry point",
                DarkNumericUpDown1.Value.ToString(), IniManager.KeyType.Normal);
            var res = StrToRes(ResolutionCB.Text);
            IniManager.Ini(ref IniManager.F3Ini, "Graphics", "fullscreen", (FullscreenCB.Checked ? 1 : 0).ToString(),
                IniManager.KeyType.Normal);
            IniManager.Ini(ref IniManager.F3Ini, "Graphics", "width", res.Width.ToString(), IniManager.KeyType.Normal);
            IniManager.Ini(ref IniManager.F3Ini, "Graphics", "height", res.Height.ToString(),
                IniManager.KeyType.Normal);
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

            IniManager.Ini(ref dgV2Conf, "DirectX", "Antialiasing", AAModes[AACB.SelectedIndex],
                IniManager.KeyType.Normal);
            IniManager.Ini(ref dgV2Conf, "DirectX", "Filtering", FModes[TextureCB.SelectedIndex],
                IniManager.KeyType.Normal);
            IniManager.Ini(ref dgV2Conf, "DirectX", "Resolution", SSModes[SSFCB.SelectedIndex],
                IniManager.KeyType.Normal);
            IniManager.Ini(ref dgV2Conf, "DirectX", "DisableMipmapping", Conversions.ToString(!MipmapCB.Checked),
                IniManager.KeyType.Normal);
            IniManager.Ini(ref dgV2Conf, "DirectX", "PhongShadingWhenPossible", Conversions.ToString(PhongCB.Checked),
                IniManager.KeyType.Normal);
            IniManager.Ini(ref dgV2Conf, "GeneralExt", "FPSLimit", res.Hz.ToString(), IniManager.KeyType.Normal);
            IniManager.Ini(ref IniManager.F3Ini, "Graphics", "refresh", res.Hz.ToString(), IniManager.KeyType.Normal);
            var NewBinds = (from DataGridViewRow r in DataGridView1.Rows
                            select RowToStr(r)).ToList();

            var SectionStart = Array.FindIndex(IniManager.F3Ini, x => x.StartsWith($"[HotKeys]"));
            var SectionEnd = Array.FindIndex(IniManager.F3Ini, SectionStart + 1, x => x.StartsWith("[")) - 1;
            if (SectionEnd < 0)
                SectionEnd = IniManager.F3Ini.Length - 1;

            for (int i = SectionStart + 1, loopTo = SectionEnd - 1; i <= loopTo; i++)
                General.RemoveAt(ref IniManager.F3Ini, SectionStart + 1);

            for (int i = 1, loopTo1 = NewBinds.Count() - 1; i <= loopTo1; i++)
                General.InsertAt(ref IniManager.F3Ini, SectionStart + i, NewBinds[i - 1]);
            File.WriteAllLines("dgVoodoo.conf", dgV2Conf);
            File.WriteAllLines(IniManager.SysDir, IniManager.SysIni);
            File.WriteAllLines(IniManager.F3Dir, IniManager.F3Ini);
            Hide();
        }

        private void SetupSSCB(object sender, EventArgs e)
        {
            var index = SSFCB.SelectedIndex;
            var res = StrToRes(ResolutionCB.Text);
            SSFCB.Items.Clear();
            var resolutions = new[]
                { ResToStr(res, false), ResToStr(res, false, 2), ResToStr(res, false, 3), ResToStr(res, false, 4) };
            SSFCB.Items.AddRange(resolutions);
            SSFCB.SelectedIndex = index;
        }

        private void NewGameToolTip(object sender, EventArgs e)
        {
            ToolTip1.SetToolTip(NewGameCB, NewGameCB.Text);
        }

        private static string RowToStr(DataGridViewRow row)
        {
            var ch = Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(row.Cells[3].Value, "Up", false))
                ? "-"
                : "+";
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(row.Cells[0].Value, "", false)))
            {
                return $"{ch}{row.Cells[1].Value} = {row.Cells[2].Value}";
            }
            else
            {
                return $"{ch}{row.Cells[0].Value}{ch}{row.Cells[1].Value} = {row.Cells[2].Value}";
            }
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
                        Row = e.RowIndex;
                        DarkLabel1.Show();
                        WantKey = true;
                        break;
                    }
                case 2:
                    {
                        DarkLabel1.Hide();
                        WantKey = false;
                        break;
                    }
                case 3:
                    {
                        if (ReferenceEquals(DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value, DBNull.Value))
                            DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "Up";
                        DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value =
                            Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(
                                DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value, "Up", false))
                                ? "Down"
                                : "Up";
                        DarkLabel1.Hide();
                        WantKey = false;
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
                        if (WantKey)
                        {
                            Modifier = e.Modifiers.ToString();
                            Key = e.KeyCode.ToString();
                            DataGridView1.Rows[Row].Cells[0].Value = ProperName.ContainsKey(Modifier) ? ProperName[Modifier] : Modifier;
                            DataGridView1.Rows[Row].Cells[1].Value = ProperName.ContainsKey(Key) ? ProperName[Key] : Key;
                        }

                        break;
                    }
            }
        }

        private Dictionary<string, string> ProperName = new()
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

    public class MainMenuDef
    {
        public string MapName { get; set; }
        public string TargetX { get; set; }
        public string TargetY { get; set; }
        public string TargetZ { get; set; }
        public string Azimuth { get; set; }
        public string Elevation { get; set; }
        public string FOV { get; set; }
    }

    public class Keybind
    {
        public string Key;
        public string Modifier;
        public string Action;
        public bool OnPress;

        public Keybind(string Key, string Modifier, string Action, bool OnPress = true)
        {
            this.Key = Key;
            this.Modifier = Modifier;
            this.Action = Action;
            this.OnPress = OnPress;
        }
    }
}