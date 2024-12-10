using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using AltUI.Config;
using ImGuiNET;
using VBLauncher.My.Resources;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace VBLauncher
{
    public partial class EditorWindow
    {
        private Sdl2Window _window;
        private GraphicsDevice _graphicsDevice;
        private CommandList _commandList;
        private ImGuiRenderer _imguiRenderer;
        private ImFontPtr? _font;

        private string _filename;
        private string _extension;
        private dynamic _currentFile;
        private string[] _stf;

        public void Run()
        {
            // Initialize the window
            var windowCreateInfo = new WindowCreateInfo
            {
                X = 100,
                Y = 100,
                WindowWidth = 1280,
                WindowHeight = 720,
                WindowTitle = "Van Buren Editor"
            };
            _window = VeldridStartup.CreateWindow(ref windowCreateInfo);
            _window.Resized += OnWindowResized;
            // reuse DWM code from AltUI
            ThemeProvider.SetupWindow(_window.Handle, 2);

            // Initialize the graphics device
            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(_window);
            _commandList = _graphicsDevice.ResourceFactory.CreateCommandList();

            // Initialize ImGui
            _imguiRenderer = new ImGuiRenderer(
                _graphicsDevice,
                _graphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
                _window.Width,
                _window.Height);
            ImGui.StyleColorsDark();
            ApplyAltUIColours();

            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            if (File.Exists(@"C:\Windows\Fonts\segoeui.ttf"))
            {
                _font = io.Fonts.AddFontFromFileTTF(@"C:\Windows\Fonts\segoeui.ttf", 17f);
                _imguiRenderer.RecreateFontDeviceTexture();
            }

            MainLoop();
        }

        private void MainLoop()
        {
            var stopwatch = Stopwatch.StartNew();
            const double targetFrameTime = 1.0 / 60.0;

            // set window icon
            var stream = new MemoryStream();
            var bmp = Resources.VBEditor.ToBitmap();
            bmp.Save(stream, ImageFormat.Bmp);
            Sdl2WindowExtensions.SetWindowIcon(_window, stream.ToArray());

            while (_window.Exists)
            {
                var frameStartTime = stopwatch.Elapsed.TotalSeconds;

                // Handle input
                var inputSnapshot = _window.PumpEvents();
                if (!_window.Exists) break;

                // Update ImGui
                _imguiRenderer.Update((float)targetFrameTime, inputSnapshot);
                if (_font is { } font)
                {
                    ImGui.PushFont(font);
                }

                DrawImGui();

                // Render frame
                _commandList.Begin();
                _commandList.SetFramebuffer(_graphicsDevice.MainSwapchain.Framebuffer);
                _commandList.ClearColorTarget(0, RgbaFloat.CornflowerBlue);
                _imguiRenderer.Render(_graphicsDevice, _commandList);
                _commandList.End();
                _graphicsDevice.SubmitCommands(_commandList);
                _graphicsDevice.SwapBuffers(_graphicsDevice.MainSwapchain);

                // Frame timing control
                var frameEndTime = stopwatch.Elapsed.TotalSeconds;
                var frameDuration = frameEndTime - frameStartTime;
                if (!(frameDuration < targetFrameTime)) continue;
                var sleepTime = (int)((targetFrameTime - frameDuration) * 1000);
                Thread.Sleep(sleepTime);
            }

            DisposeResources();
        }

        private void OnWindowResized()
        {
            _graphicsDevice.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
            _imguiRenderer.WindowResized(_window.Width, _window.Height);
        }

        private void DisposeResources()
        {
            _commandList.Dispose();
            _imguiRenderer.Dispose();
            _graphicsDevice.Dispose();
        }

        // Enumeration for PushStyleColor() / PopStyleColor()
        private enum ImGuiCol
        {
            Text,
            TextDisabled,
            WindowBg,              // Background of normal windows
            ChildBg,               // Background of child windows
            PopupBg,               // Background of popups, menus, tooltips windows
            Border,
            BorderShadow,
            FrameBg,               // Background of checkbox, radio button, plot, slider, text input
            FrameBgHovered,
            FrameBgActive,
            TitleBg,               // Title bar
            TitleBgActive,         // Title bar when focused
            TitleBgCollapsed,      // Title bar when collapsed
            MenuBarBg,
            ScrollbarBg,
            ScrollbarGrab,
            ScrollbarGrabHovered,
            ScrollbarGrabActive,
            CheckMark,             // Checkbox tick and RadioButton circle
            SliderGrab,
            SliderGrabActive,
            Button,
            ButtonHovered,
            ButtonActive,
            Header,                // Header* colors are used for CollapsingHeader, TreeNode, Selectable, MenuItem
            HeaderHovered,
            HeaderActive,
            Separator,
            SeparatorHovered,
            SeparatorActive,
            ResizeGrip,            // Resize grip in lower-right and lower-left corners of windows.
            ResizeGripHovered,
            ResizeGripActive,
            Tab,                   // TabItem in a TabBar
            TabHovered,
            TabActive,
            TabUnfocused,
            TabUnfocusedActive,
            PlotLines,
            PlotLinesHovered,
            PlotHistogram,
            PlotHistogramHovered,
            TableHeaderBg,         // Table header background
            TableBorderStrong,     // Table outer and header borders (prefer using Alpha=1.0 here)
            TableBorderLight,      // Table inner borders (prefer using Alpha=1.0 here)
            TableRowBg,            // Table row background (even rows)
            TableRowBgAlt,         // Table row background (odd rows)
            TextSelectedBg,
            DragDropTarget,        // Rectangle highlighting a drop target
            NavHighlight,          // Gamepad/keyboard: current highlighted item
            NavWindowingHighlight, // Highlight window when using CTRL+TAB
            NavWindowingDimBg,     // Darken/colorize entire screen behind the CTRL+TAB window list, when active
            ModalWindowDimBg,      // Darken/colorize entire screen behind a modal window, when one is active
        }

        private Vector4 ColToVec(Color col) => new(col.R / 255f, col.G / 255f, col.B / 255f, col.A / 255f);

        // TODO: ensure all used colors are set from ThemeProvider
        private void ApplyAltUIColours()
        {
            var colors = ImGui.GetStyle().Colors;
            colors[(int)ImGuiCol.Text] = ColToVec(ThemeProvider.Theme.Colors.LightText);
            colors[(int)ImGuiCol.TextDisabled] = ColToVec(ThemeProvider.Theme.Colors.DisabledText);
            colors[(int)ImGuiCol.WindowBg] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
            colors[(int)ImGuiCol.ChildBg] = new Vector4(1f, 0f, 0f, 1f);
            colors[(int)ImGuiCol.PopupBg] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
            colors[(int)ImGuiCol.Border] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
            colors[(int)ImGuiCol.BorderShadow] = new Vector4(0f, 0f, 0f, 0f);
            colors[(int)ImGuiCol.FrameBg] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
            colors[(int)ImGuiCol.FrameBgHovered] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
            colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.2f, 0.22f, 0.23f, 1f);
            colors[(int)ImGuiCol.TitleBg] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
            colors[(int)ImGuiCol.TitleBgActive] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
            colors[(int)ImGuiCol.TitleBgCollapsed] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
            colors[(int)ImGuiCol.MenuBarBg] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
            colors[(int)ImGuiCol.ScrollbarBg] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
            colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.34f, 0.34f, 0.34f, 1f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
            colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.56f, 0.56f, 0.56f, 1f);
            colors[(int)ImGuiCol.CheckMark] = ColToVec(ThemeProvider.Theme.Colors.LightText);
            colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.34f, 0.34f, 0.34f, 1f);
            colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.56f, 0.56f, 0.56f, 1f);
            colors[(int)ImGuiCol.Button] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
            colors[(int)ImGuiCol.ButtonHovered] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
            colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.2f, 0.22f, 0.23f, 1f);
            colors[(int)ImGuiCol.Header] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
            colors[(int)ImGuiCol.HeaderHovered] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
            colors[(int)ImGuiCol.HeaderActive] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
            colors[(int)ImGuiCol.Separator] = new Vector4(0f, 0f, 0f, 0f);
            colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0f, 0f, 0f, 0f);
            colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0f, 0f, 0f, 0f);
            colors[(int)ImGuiCol.ResizeGrip] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
            colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.44f, 0.44f, 0.44f, 1f);
            colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.4f, 0.44f, 0.47f, 1f);
            colors[(int)ImGuiCol.Tab] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
            colors[(int)ImGuiCol.TabHovered] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
            colors[(int)ImGuiCol.TabActive] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
            colors[(int)ImGuiCol.TabUnfocused] = ColToVec(ThemeProvider.Theme.Colors.HeaderBackground);
            colors[(int)ImGuiCol.TabUnfocusedActive] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
            colors[(int)ImGuiCol.PlotLines] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
            colors[(int)ImGuiCol.PlotLinesHovered] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
            colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.15f, 0.15f, 0.15f, 1f);
            colors[(int)ImGuiCol.PlotHistogramHovered] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
            colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(1f, 0f, 1f, 1f);
            colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0f, 1f, 1f, 1f);
            colors[(int)ImGuiCol.TableBorderLight] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
            colors[(int)ImGuiCol.TableRowBg] = new Vector4(0f, 0f, 0f, 1f);
            colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(1f, 0f, 1f, 1f);
            colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.2f, 0.22f, 0.23f, 1f);
            colors[(int)ImGuiCol.DragDropTarget] = new Vector4(0.33f, 0.67f, 0.86f, 1f);
            colors[(int)ImGuiCol.NavHighlight] = new Vector4(0.24f, 0.24f, 0.24f, 1f);
            colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1f, 0f, 1f, 1f);
            colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0f, 1f, 1f, 1f);
            colors[(int)ImGuiCol.ModalWindowDimBg] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);

            var style = ImGui.GetStyle();
            style.ScrollbarSize = 15;
            style.WindowRounding = 7;
            style.ChildRounding = 4;
            style.FrameRounding = 3;
            style.PopupRounding = 4;
            style.ScrollbarRounding = 9;
            style.GrabRounding = 3;
            style.TabRounding = 4;
            style.WindowBorderSize = 1f;
            // enable borders on buttons, textboxes, etc.
            style.FrameBorderSize = 1f;
        }

        private void DrawImGui()
        {
            // Create menu bar
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.BeginMenu("New"))
                    {
                        var t = new[] { ".amo", ".arm", ".con", ".crt", ".dor", ".int", ".itm", ".map", ".use", ".wea" };
                        foreach (var s in t)
                        {
                            if (ImGui.MenuItem(s))
                            {
                                // Handle new file creation
                            }
                        }
                        ImGui.EndMenu();
                    }
                    if (ImGui.MenuItem("Open\tCtrl+O"))
                        OpenFile();
                    if (ImGui.MenuItem("Open From .grp"))
                        OpenFromGRP();
                    if (ImGui.MenuItem("Save\tCtrl+S"))
                        SaveFile();
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Options"))
                {
                    if (ImGui.MenuItem("Set English.stf Location"))
                        SetEngStfLocation();
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Tools"))
                {
                    if (ImGui.MenuItem(".stf to .txt"))
                        FullSTFToText();
                    if (ImGui.MenuItem(".txt to .stf"))
                        FullTextToSTF();
                    if (ImGui.MenuItem("Extract All .grp Files"))
                        ExtractAllGRPFiles();
                    if (ImGui.MenuItem("Extract + Convert All .grp Files"))
                        ExtractAllGRPFiles(true);
                    if (ImGui.MenuItem("Show .grp Browser"))
                        using (var grpb = new GrpBrowser(null, true))
                            grpb.ShowDialog();
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }

            // Create a fullscreen window for docking
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());

            switch (_currentFile)
            {
                case CRT:
                    DrawCRT();
                    break;
                case ITM:
                    DrawITM();
                    break;
                case Map:
                    DrawMap();
                    break;
            }
        }

        private void STFBox(string text, ref int sr)
        {
            ImGui.Text(text);
            ImGui.InputInt("##" + text, ref sr);
            if (sr == 0) return;
            ImGui.SameLine();
            ImGui.PushItemWidth(-1);
            ImGui.InputText("##" + text + "in", ref _stf[sr - 1], 100u);
            ImGui.PopItemWidth();
        }

        private void DrawCRT()
        {
            DrawEEN2();
            DrawGENT();
            DrawGCHR();
            DrawGCRE();
            DrawGWAM();
        }

        private void DrawITM()
        {
            DrawEEN2();
            DrawGENT();
            DrawGITM();
        }

        private void DrawMap()
        {
            DrawEMAP();
            DrawEME2();
            DrawECAM();
            DrawEMEP();
            DrawEMEF();
            DrawEMSD();
            DrawEPTH();
            DrawTrigger();
            Draw2MWT();
        }

        private void DrawEEN2()
        {
            if (_currentFile.EEN2 is not EEN2c een2) return;
            ImGui.Begin("EEN2");
            ImGui.PushItemWidth(-1);
            ImGui.Text("Model Prefix");
            ImGui.InputText("##ModelPrefix", ref een2.skl, 100u);
            ImGui.Text("Inventory Icon");
            ImGui.InputText("##InventoryIcon", ref een2.invtex, 100u);
            ImGui.Text("Action Bar Icon");
            ImGui.InputText("##ActionBarIcon", ref een2.acttex, 100u);
            ImGui.Checkbox("Selectable", ref een2.sel);
            ImGui.PopItemWidth();
            DrawEEOV(een2.EEOV);
            ImGui.End();
        }

        private int _eeovSelected = -1;
        private string _eeovTemp = "";
        private void DrawEEOV(EEOVc eeov)
        {
            ImGui.PushItemWidth(-1);
            ImGui.Text("Entity Name");
            ImGui.InputText("##EntityName", ref eeov.s1, 100u);
            ImGui.Text("Dialog File");
            ImGui.InputText("##DialogFile", ref eeov.s2, 100u);
            ImGui.Text("Script File");
            ImGui.InputText("##ScriptFile", ref eeov.s3, 100u);
            ImGui.Text("Skin Texture");
            ImGui.InputText("##SkinTexture", ref eeov.s4, 100u);
            ImGui.Text("Effect File");
            ImGui.InputText("##EffectFile", ref eeov.s5, 100u);
            ImGui.Text("Inventory");
            ImGui.ListBox("##Inventory", ref _eeovSelected, eeov.inv, eeov.inv.Length);
            ImGui.PopItemWidth();
            ImGui.InputText("##InventoryItem", ref _eeovTemp, 100u);
            ImGui.SameLine();
            if (ImGui.Button(" + ##Inventory"))
            {
                if (string.IsNullOrWhiteSpace(_eeovTemp)) return;
                eeov.inv = eeov.inv.Append(_eeovTemp).ToArray();
                _eeovTemp = "";
            }
            ImGui.SameLine();
            if (ImGui.Button(" - ##Inventory"))
            {
                if (_eeovSelected == -1) return;
                var tmp = eeov.inv.ToList();
                tmp.RemoveAt(_eeovSelected);
                eeov.inv = tmp.ToArray();
                _eeovSelected = -1;
            }
        }

        private void DrawGENT()
        {
            if (_currentFile.GENT is not GENTc gent) return;
            ImGui.Begin("GENT");
            ImGui.PushItemWidth(200f);
            STFBox("Hover String", ref gent.HoverSR);
            STFBox("Look String", ref gent.LookSR);
            STFBox("Name String", ref gent.NameSR);
            STFBox("Unknown String", ref gent.UnkwnSR);
            ImGui.Columns(2, "health");
            ImGui.SetColumnWidth(0, 215);
            ImGui.SetColumnWidth(1, 215);
            ImGui.PushItemWidth(200);
            ImGui.Text("Max Health");
            ImGui.InputInt("##Health", ref gent.MaxHealth);
            ImGui.NextColumn();
            ImGui.PushItemWidth(200);
            ImGui.Text("Initial Health");
            ImGui.InputInt("##InitHealth", ref gent.StartHealth);
            ImGui.Columns();
            ImGui.End();
        }

        private void DrawGCHR()
        {
            if (_currentFile.GCHR is not GCHRc gchr) return;
            ImGui.Begin("GCHR");
            ImGui.Text("Name");
            ImGui.InputText("##Name", ref gchr.name, 100u);
            ImGui.End();
        }

        private int _gcreSpecial;
        private readonly string[] _gcreSpecials = ["Strength", "Perception", "Endurance", "Charisma", "Intelligence", "Agility", "Luck"];
        private int _gcreSkill;
        private readonly string[] _gcreSkills = ["Firearms", "Melee", "Unarmed", "Barter", "Persuasion", "Deception", "Mechanics", "Medic", "Outdoorsman", "Science", "Security", "Sneak", "Steal"];
        private int _gcreTrait;
        private readonly string[] _gcreTraits = ["Bruiser", "Chem Reliant", "Clean Living", "Fast Shot", "Feral Kid", "Finesse", "Gifted", "Good Natured", "Increased Metabolism", "Kamikaze", "Night Person", "One Hander", "One In a Million", "Red Scare", "Skilled", "Small Frame"];
        private int _gcreTag;
        private int _gcreSocket;
        private readonly string[] _gcreSockets = ["Head", "Hair", "Ponytail", "Moustache", "Beard", "Eye", "Body", "Hand", "Feet", "Back", "Shoulder", "Vanity"];
        private int _gcreEquipped;
        private string _gcreTemp = "";
        private void DrawGCRE()
        {
            if (_currentFile.GCRE is not GCREc gcre) return;
            ImGui.Begin("GCRE");
            ImGui.Columns(2, "gcreColumns");
            ImGui.Text("Special");
            ImGui.Combo("##Special", ref _gcreSpecial, _gcreSpecials, _gcreSpecials.Length);
            ImGui.SameLine();
            ImGui.PushItemWidth(-1);
            ImGui.InputInt("##SpecialVal", ref gcre.Special[_gcreSpecial]);
            ImGui.PopItemWidth();
            ImGui.Text("Skills");
            ImGui.Combo("##Skills", ref _gcreSkill, _gcreSkills, _gcreSkills.Length);
            var skill = gcre.Skills.FirstOrDefault(s => s.Index == _gcreSkill);
            ImGui.PushItemWidth(-1);
            ImGui.SameLine();
            if (skill is not null)
            {
                ImGui.InputInt("##SkillVal", ref skill.Value);
            }
            else
            {
                var tmpVal = 0;
                if (ImGui.InputInt("##SkillVal", ref tmpVal) && tmpVal != 0)
                {
                    gcre.Skills.Add(new Skill(_gcreSkill, tmpVal));
                }
            }
            ImGui.PopItemWidth();
            ImGui.Text("Traits");
            ImGui.Combo("##Traits", ref _gcreTrait, _gcreTraits, _gcreTraits.Length);
            var traitSel = gcre.Traits.Contains(_gcreTrait);
            ImGui.SameLine();
            if (ImGui.Checkbox("Trait Active", ref traitSel))
            {
                if (traitSel)
                    gcre.Traits.Add(_gcreTrait);
                else
                    gcre.Traits.Remove(_gcreTrait);
            }
            ImGui.Text("Tag Skills");
            ImGui.Combo("##TagSkills", ref _gcreTag, _gcreSkills, _gcreSkills.Length);
            var tagSel = gcre.TagSkills.Contains(_gcreTag);
            ImGui.SameLine();
            if (ImGui.Checkbox("Tag Active", ref tagSel))
            {
                if (tagSel)
                    gcre.TagSkills.Add(_gcreTag);
                else
                    gcre.TagSkills.Remove(_gcreTag);
            }
            ImGui.Text("Portrait");
            ImGui.InputText("##Portrait", ref gcre.PortStr, 100u);
            ImGui.Text("Age");
            ImGui.InputInt("##Age", ref gcre.Age);
            ImGui.NextColumn();
            ImGui.Text("Socket");
            ImGui.Combo("##Socket", ref _gcreSocket, _gcreSockets, _gcreSockets.Length);
            var sock = _gcreSocket switch
            {
                0 => gcre.Hea,
                1 => gcre.Hai,
                2 => gcre.Pon,
                3 => gcre.Mus,
                4 => gcre.Bea,
                5 => gcre.Eye,
                6 => gcre.Bod,
                7 => gcre.Han,
                8 => gcre.Fee,
                9 => gcre.Bac,
                10 => gcre.Sho,
                11 => gcre.Van,
                _ => throw new ArgumentOutOfRangeException()
            };
            ImGui.Text("Socket Model");
            ImGui.InputText("##Model", ref sock.Model, 100u);
            ImGui.Text("Socket Texture");
            ImGui.InputText("##Texture", ref sock.Tex, 100u);
            ImGui.Text("Equipped Items");
            ImGui.ListBox("##Equipped", ref _gcreEquipped, gcre.Inventory, gcre.Inventory.Length);
            ImGui.InputText("##EquippedItem", ref _gcreTemp, 100u);
            ImGui.SameLine();
            if (ImGui.Button(" + ##Equipped"))
            {
                if (string.IsNullOrWhiteSpace(_gcreTemp)) return;
                gcre.Inventory = gcre.Inventory.Append(_gcreTemp).ToArray();
                _gcreTemp = "";
            }
            ImGui.SameLine();
            if (ImGui.Button(" - ##Equipped"))
            {
                if (_gcreEquipped == -1) return;
                var tmp = gcre.Inventory.ToList();
                tmp.RemoveAt(_gcreEquipped);
                gcre.Inventory = tmp.ToArray();
                _gcreEquipped = -1;
            }
            _gwamNames = gcre.GWAM.Select(i => _stf[i.NameSR - 1]).ToArray();
            ImGui.End();
        }

        private readonly string[] _gwamDamageTypes = ["Ballistic", "Bio", "Electric", "EMP", "General", "Heat"];
        private string[] _gwamNames;
        private int _gwamIndex;
        private void DrawGWAM()
        {
            if (_currentFile.GCRE is not GCREc gcre) return;
            ImGui.Begin("GWAM");
            // i believe every isntance of GWAM can have more than one.
            ImGui.Combo("##gwams", ref _gwamIndex, _gwamNames, _gwamNames.Length);
            ImGui.SameLine();
            if (ImGui.Button(" + "))
            {
                gcre.GWAM.Add(new GWAMc());
                _gwamIndex = gcre.GWAM.Count - 1;
            }
            ImGui.SameLine();
            if (ImGui.Button(" - "))
                gcre.GWAM.RemoveAt(_gwamIndex);
            if (_gwamIndex >= gcre.GWAM.Count)
                _gwamIndex = gcre.GWAM.Count - 1;
            if (_gwamNames.Length == 0)
            {
                ImGui.End();
                return;
            }
            GWAMc gwam = gcre.GWAM[_gwamIndex];
            // .wea also has GWAM but isn't implemented yet. get later.
            ImGui.Columns(3, "gwamColumns");
            ImGui.Text("Animation");
            ImGui.InputInt("##Animation", ref gwam.Anim);
            ImGui.Text("Range");
            ImGui.InputInt("##Range", ref gwam.Range);
            ImGui.Text("Action Points");
            ImGui.InputInt("##AP", ref gwam.AP);
            ImGui.NextColumn();
            ImGui.Text("Damage Type");
            ImGui.Combo("##DamageType", ref gwam.DmgType, _gwamDamageTypes, _gwamDamageTypes.Length);
            ImGui.Text("Min Dmg");
            ImGui.InputInt("##MinDmg", ref gwam.MinDmg);
            ImGui.NextColumn();
            ImGui.Text("Shots Fired");
            ImGui.InputInt("##ShotsFired", ref gwam.ShotsFired);
            ImGui.Text("Max Dmg");
            ImGui.InputInt("##MaxDmg", ref gwam.MaxDmg);
            ImGui.Columns();
            STFBox("Attack Name", ref gwam.NameSR);
            ImGui.Text("Effect");
            ImGui.InputText("##Effect", ref gwam.VegName, 100u);
            ImGui.End();
        }

        private int _gitmSocket;
        private readonly string[] _gitmSockets = ["Eye", "Body", "Back", "Hands", "Feet", "Shoes", "Vanity", "In-Hand"];
        private readonly string[] _gitmTypes = ["ITM", "AMO", "ARM", "???", "WEA"];
        private readonly string[] _gitmSlots = ["Body", "Head", "Hands"];
        private void DrawGITM()
        {
            if (_currentFile.GITM is not GITMc gitm) return;
            ImGui.Begin("GITM");
            ImGui.Columns(3, "gitmColumns");
            ImGui.Text("Type");
            ImGui.Combo("##Type", ref gitm.type, _gitmTypes, _gitmTypes.Length);
            ImGui.NextColumn();
            ImGui.Text("Slot");
            ImGui.Combo("##Slot", ref gitm.eqslot, _gitmSlots, _gitmSlots.Length);
            ImGui.SameLine();
            ImGui.Checkbox("Equippable", ref gitm.equip);
            ImGui.NextColumn();
            ImGui.Text("Reload");
            ImGui.InputInt("##Reload", ref gitm.reload);
            ImGui.SameLine();
            // TODO: figure out how melee works
            var tempBool = false;
            ImGui.Checkbox("Melee", ref tempBool);
            ImGui.Columns();

            ImGui.Text("Hide Socket");
            ImGui.Checkbox("Hair", ref gitm.hHai);
            ImGui.SameLine();
            ImGui.Checkbox("Beard", ref gitm.hBea);
            ImGui.SameLine();
            ImGui.Checkbox("Moustache", ref gitm.hMus);
            ImGui.SameLine();
            ImGui.Checkbox("Eye", ref gitm.hEye);
            ImGui.SameLine();
            ImGui.Checkbox("Ponytail", ref gitm.hPon);
            ImGui.SameLine();
            ImGui.Checkbox("Vanity", ref gitm.hVan);

            ImGui.Text("Socket");
            ImGui.Combo("##Socket", ref _gitmSocket, _gitmSockets, _gitmSockets.Length);
            var sock = _gitmSocket switch
            {
                0 => gitm.Eye,
                1 => gitm.Bod,
                2 => gitm.Bac,
                3 => gitm.Han,
                4 => gitm.Fee,
                5 => gitm.Sho,
                6 => gitm.Van,
                7 => gitm.IHS,
                _ => throw new ArgumentOutOfRangeException()
            };
            ImGui.Text("Socket Model");
            ImGui.InputText("##Model", ref sock.Model, 100u);
            ImGui.Text("Socket Texture");
            ImGui.InputText("##Texture", ref sock.Tex, 100u);

            ImGui.End();
        }

        private Vector3 _tempColor = new(1f, 1f, 1f);
        private void DrawEMAP()
        {
            if (_currentFile.EMAP is not EMAPc emap) return;
            _tempColor = new Vector3(emap.col.R / 255f, emap.col.G / 255f, emap.col.B / 255f);
            ImGui.Begin("EMAP");
            ImGui.PushItemWidth(-1);
            ImGui.Text("Map Mesh");
            ImGui.InputText("##MapMesh", ref emap.s1, 100u);
            ImGui.Text("Height Map");
            ImGui.InputText("##HeightMap", ref emap.s2, 100u);
            ImGui.Text("Minimap Texture");
            ImGui.InputText("##MinimapTexture", ref emap.s3, 100u);
            ImGui.Text("Lighting Colour");
            ImGui.ColorEdit3("Lighting Colour", ref _tempColor);
            emap.col = Color.FromArgb(255, (int)(_tempColor.X * 255), (int)(_tempColor.Y * 255), (int)(_tempColor.Z * 255));
            ImGui.Checkbox("Ignore Lighting", ref emap.il);
            ImGui.End();
            if (_currentFile is not Map map) return;

            if (_currentFile.EME2.Count > 0)
                _eme2Names = map.EME2.Select(e => e.name).ToArray();
            if (_currentFile.EMEP.Count > 0)
                _emepNames = map.EMEP.Select(e => e.index.ToString()).ToArray();
            if (_currentFile.EMEF.Count > 0)
                _emefNames = map.EMEF.Select(e => e.s1).ToArray();
            if (_currentFile.EMSD.Count > 0)
                _emsdNames = map.EMSD.Select(e => e.s1).ToArray();
            if (_currentFile.EPTH.Count > 0)
                _epthNames = map.EPTH.Select(e => e.name).ToArray();
            if (_currentFile.Triggers.Count <= 0) return;
            _triggerNames = new string[_currentFile.Triggers.Count];
            for (var i = 0; i < _currentFile.Triggers.Count; i++)
                _triggerNames[i] = i.ToString();
        }

        private int _eme2Index;
        private string[] _eme2Names;
        private void DrawEME2()
        {
            if (_currentFile is not Map map) return;
            ImGui.Begin("EME2");
            ImGui.Combo("##EME2", ref _eme2Index, _eme2Names, _eme2Names.Length);
            ImGui.SameLine();
            if (ImGui.Button(" + "))
            {
                map.EME2.Add(new EME2c());
                _eme2Index = map.EME2.Count - 1;
            }
            ImGui.SameLine();
            if (ImGui.Button(" - "))
                map.EME2.RemoveAt(_eme2Index);
            if (_eme2Index >= map.EME2.Count)
                _eme2Index = map.EME2.Count - 1;
            if (_eme2Names.Length == 0)
            {
                ImGui.End();
                return;
            }
            var eme2 = map.EME2[_eme2Index];
            ImGui.Text("Creature File");
            ImGui.InputText("##CreatureFile", ref eme2.name, 100u);
            DrawEEOV(eme2.EEOV);
            ImGui.InputFloat4("XYZR", ref eme2.l);
            ImGui.End();
        }

        private void DrawECAM()
        {
            if (_currentFile.ECAM is not ECAMc ecam) return;
            ImGui.Begin("ECAM");
            ImGui.Text("Camera Position");
            ImGui.InputFloat4("##XYZR", ref ecam.p);
            ImGui.End();
        }

        private int _emepIndex;
        private string[] _emepNames;
        private void DrawEMEP()
        {
            if (_currentFile is not Map map) return;
            ImGui.Begin("EMEP");
            ImGui.Combo("##EMEP", ref _emepIndex, _emepNames, _emepNames.Length);
            ImGui.SameLine();
            if (ImGui.Button(" + "))
            {
                map.EMEP.Add(new EMEPc());
                _emepIndex = map.EMEP.Count - 1;
            }
            ImGui.SameLine();
            if (ImGui.Button(" - "))
                map.EMEP.RemoveAt(_emepIndex);
            if (_emepIndex >= map.EMEP.Count)
                _emepIndex = map.EMEP.Count - 1;
            if (_emepNames.Length == 0)
            {
                ImGui.End();
                return;
            }
            var emep = map.EMEP[_emepIndex];
            ImGui.Text("Entry Point ID");
            ImGui.InputInt("##EntryPointID", ref emep.index);
            ImGui.Text("Position");
            ImGui.InputFloat3("##XYZ", ref emep.p);
            ImGui.SameLine();
            ImGui.InputFloat("##R", ref emep.r);
            ImGui.End();
        }

        private int _emefIndex;
        private string[] _emefNames;
        private void DrawEMEF()
        {
            if (_currentFile is not Map map) return;
            ImGui.Begin("EMEF");
            ImGui.Combo("##EMEF", ref _emefIndex, _emefNames, _emefNames.Length);
            ImGui.SameLine();
            if (ImGui.Button(" + "))
            {
                map.EMEF.Add(new EMEFc());
                _emefIndex = map.EMEF.Count - 1;
            }
            ImGui.SameLine();
            if (ImGui.Button(" - "))
                map.EMEF.RemoveAt(_emefIndex);
            if (_emefIndex >= map.EMEF.Count)
                _emefIndex = map.EMEF.Count - 1;
            if (_emefNames.Length == 0)
            {
                ImGui.End();
                return;
            }
            var emef = map.EMEF[_emefIndex];
            ImGui.Text("Effect ID");
            ImGui.InputText("##EffectID", ref emef.s1, 100u);
            ImGui.Text("Effect File");
            ImGui.InputText("##EffectFile", ref emef.s2, 100u);
            ImGui.Text("Position");
            ImGui.InputFloat4("##XYZR", ref emef.l);
            ImGui.End();
        }

        private int _emsdIndex;
        private string[] _emsdNames;
        private void DrawEMSD()
        {
            if (_currentFile is not Map map) return;
            ImGui.Begin("EMSD");
            ImGui.Combo("##EMSD", ref _emsdIndex, _emsdNames, _emsdNames.Length);
            ImGui.SameLine();
            if (ImGui.Button(" + "))
            {
                map.EMSD.Add(new EMSDc());
                _emsdIndex = map.EMSD.Count - 1;
            }
            ImGui.SameLine();
            if (ImGui.Button(" - "))
                map.EMSD.RemoveAt(_emsdIndex);
            if (_emsdIndex >= map.EMSD.Count)
                _emsdIndex = map.EMSD.Count - 1;
            if (_emsdNames.Length == 0)
            {
                ImGui.End();
                return;
            }
            var emsd = map.EMSD[_emsdIndex];
            ImGui.Text("Sound ID");
            ImGui.InputText("##SoundID", ref emsd.s1, 100u);
            ImGui.Text("Sound File");
            ImGui.InputText("##SoundFile", ref emsd.s2, 100u);
            ImGui.Text("Position");
            ImGui.InputFloat3("##XYZ", ref emsd.l);
            ImGui.End();
        }

        private int _epthIndex;
        private string[] _epthNames;
        private int _epthPoint;
        private void DrawEPTH()
        {
            if (_currentFile is not Map map) return;
            ImGui.Begin("EPTH");
            ImGui.Combo("##EPTH", ref _epthIndex, _epthNames, _epthNames.Length);
            ImGui.SameLine();
            if (ImGui.Button(" + "))
            {
                map.EPTH.Add(new EPTHc());
                _epthIndex = map.EPTH.Count - 1;
            }
            ImGui.SameLine();
            if (ImGui.Button(" - "))
                map.EPTH.RemoveAt(_epthIndex);
            if (_epthIndex >= map.EPTH.Count)
                _epthIndex = map.EPTH.Count - 1;
            if (_epthNames.Length == 0)
            {
                ImGui.End();
                return;
            }
            EPTHc epth = map.EPTH[_epthIndex];
            ImGui.Text("Path ID");
            ImGui.InputText("##PathID", ref epth.name, 100u);
            ImGui.Text("Point");
            // create string array of point indices
            var points = new string[epth.p.Count];
            for (var i = 0; i < epth.p.Count; i++)
                points[i] = i.ToString();
            ImGui.Combo("##Point", ref _epthPoint, points, points.Length);
            ImGui.SameLine();
            if (ImGui.Button(" + ##Point"))
            {
                epth.p.Add(new Vector4());
                _epthPoint = epth.p.Count - 1;
            }
            ImGui.SameLine();
            if (ImGui.Button(" - ##Point"))
                epth.p.RemoveAt(_epthPoint);
            if (_epthPoint >= epth.p.Count)
                _epthPoint = epth.p.Count - 1;
            var point = epth.p[_epthPoint];
            ImGui.Text("Position");
            ImGui.InputFloat4("##XYZR", ref point);
        }

        private int _triggerIndex;
        private string[] _triggerNames;
        private readonly string[] _triggerTypes = ["B (?)", "Script", "Level Transition"];
        private int _triggerPoint;
        private void DrawTrigger()
        {
            if (_currentFile is not Map map) return;
            ImGui.Begin("Trigger");
            ImGui.Combo("##Trigger", ref _triggerIndex, _triggerNames, _triggerNames.Length);
            ImGui.SameLine();
            if (ImGui.Button(" + "))
            {
                map.Triggers.Add(new Trigger());
                _triggerIndex = map.Triggers.Count - 1;
            }
            ImGui.SameLine();
            if (ImGui.Button(" - "))
                map.Triggers.RemoveAt(_triggerIndex);
            if (_triggerIndex >= map.Triggers.Count)
                _triggerIndex = map.Triggers.Count - 1;
            if (_triggerNames.Length == 0)
            {
                ImGui.End();
                return;
            }

            var trigger = map.Triggers[_triggerIndex];
            ImGui.Text("Trigger Type");
            var triggerIndex = trigger.ExTR.type switch
            {
                "B" => 0,
                "S" => 1,
                "T" => 2,
                _ => -1
            };
            if (ImGui.Combo("##TriggerType", ref triggerIndex, _triggerTypes, _triggerTypes.Length))
            {
                trigger.ExTR.type = triggerIndex switch
                {
                    0 => "B",
                    1 => "S",
                    2 => "T",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            ImGui.Text("Trigger Value");
            ImGui.InputText("##TriggerValue", ref trigger.ExTR.s, 100u);
            // TODO: what is EMTR.n?
            // create string array of point indices
            var points = new string[trigger.EMTR.r.Count];
            for (var i = 0; i < trigger.EMTR.r.Count; i++)
                points[i] = i.ToString();
            ImGui.Text("Point");
            ImGui.Combo("##Point", ref _triggerPoint, points, points.Length);
            ImGui.SameLine();
            if (ImGui.Button(" + ##Point"))
            {
                trigger.EMTR.r.Add(new Vector3());
                _triggerPoint = trigger.EMTR.r.Count - 1;
            }
            ImGui.SameLine();
            if (ImGui.Button(" - ##Point"))
                trigger.EMTR.r.RemoveAt(_triggerPoint);
            if (_triggerPoint >= trigger.EMTR.r.Count)
                _triggerPoint = trigger.EMTR.r.Count - 1;
            var point = trigger.EMTR.r[_triggerPoint];
            ImGui.Text("Position");
            ImGui.InputFloat3("##XYZR", ref point);
            ImGui.End();
        }

        // ReSharper disable once InconsistentNaming - 2 is part of the abbreviation
        private int _2mwtChunk;
        private void Draw2MWT()
        {
            if (_currentFile._2MWT is not _2MWTc _2mwt) return;
            ImGui.Begin("2MWT");
            ImGui.PushItemWidth(-1);
            ImGui.Text("Water Mesh"); // i think?
            ImGui.InputText("##WaterMesh", ref _2mwt.mpf, 100u);
            ImGui.Checkbox("Static Water", ref _2mwt.frozen);
            ImGui.SameLine();
            ImGui.Checkbox("Dark Water", ref _2mwt.dark);
            // create string array of location indices
            var chunks = new string[_2mwt.chunks.Count];
            for (var i = 0; i < _2mwt.chunks.Count; i++)
                chunks[i] = i.ToString();
            ImGui.Text("Water Chunk");
            ImGui.Combo("##Chunk", ref _2mwtChunk, chunks, chunks.Length);
            ImGui.SameLine();
            if (ImGui.Button(" + "))
            {
                _2mwt.chunks.Add(new _2MWTChunk());
                _2mwtChunk = _2mwt.chunks.Count - 1;
            }
            ImGui.SameLine();
            if (ImGui.Button(" - "))
                _2mwt.chunks.RemoveAt(_2mwtChunk);
            if (_2mwtChunk >= _2mwt.chunks.Count)
                _2mwtChunk = _2mwt.chunks.Count - 1;
            var chunk = _2mwt.chunks[_2mwtChunk];
            ImGui.Text("Water Lightmap");
            ImGui.InputText("##WaterTexture", ref chunk.tex, 100u);
            ImGui.Text("Position");
            ImGui.InputFloat3("##XYZ", ref chunk.loc);
            ImGui.Text("Lightmap UV");
            ImGui.InputFloat2("##UV", ref chunk.texloc);
            ImGui.End();
        }
    }
}
