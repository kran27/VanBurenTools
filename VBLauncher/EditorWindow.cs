using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AltUI.Config;
using ImGuiColorTextEditNet;
using ImGuiNET;
using VBLauncher.My.Resources;
using VBLauncher.Properties;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace VBLauncher;

public partial class EditorWindow
{
    private static readonly uint[] _vegPalette =

    [
        //AABBGGRR
        0xff7f7f7f, // Default
        0xffeb956c, // Keyword (Done)
        0xffc094ed, // Number (Done)
        0xff6da2c9, // String (Done)
        0xff6da2c9, // Char literal (Done)
        0xffbdbdbd, // Punctuation (Done)
        0xff408080, // Preprocessor
        0xffaaaaaa, // Identifier
        0xffffbfe1, // Known identifier (Done)
        0xffc040a0, // Preproc identifier
        0xff6cc485, // Comment (single line) (Done)
        0xff6cc485, // Comment (multi line) (Done)
        0xff101010, // Background
        0xffe0e0e0, // Cursor
        0x80a06020, // Selection
        0x800020ff, // ErrorMarker
        0x40f08000, // Breakpoint
        0xff707000, // Line number
        0x40000000, // Current line fill
        0x40808080, // Current line fill (inactive)
        0x40a0a0a0, // Current line edge
        0xa0a0a0a0, // Executing Line
        0xffccc366, // Custom (enum value)
        0xffff91c1, // Custom 2 (header)
        0xff9bcc39 // Custom 3 (func)
    ];

// not named specifically for GIAM as it's used in GIWP
    private readonly string[] _ammoTypes =
    [
        "None", "15mm EC", ".223", ".22/.22 Injector", "Empty", "2mm EC", ".30", "40mm", ".44", ".45", ".50", "7.62mm",
        "9mm", "BB", "Microfusion Cell", "Naphate", "Heavy Rivet", "Lil' Rivet", "Rocket", "Small Energy Cell"
    ];

    private readonly string[] _gcreSkills =
    [
        "Firearms", "Melee", "Unarmed", "Barter", "Persuasion", "Deception", "Mechanics", "Medic", "Outdoorsman",
        "Science", "Security", "Sneak", "Steal"
    ];

    private readonly string[] _gcreSockets =
        ["Head", "Hair", "Ponytail", "Moustache", "Beard", "Eye", "Body", "Hand", "Feet", "Back", "Shoulder", "Vanity"];

    private readonly string[] _gcreSpecials =
        ["Strength", "Perception", "Endurance", "Charisma", "Intelligence", "Agility", "Luck"];

    private readonly string[] _gcreTraits =
    [
        "Bruiser", "Chem Reliant", "Clean Living", "Fast Shot", "Feral Kid", "Finesse", "Gifted", "Good Natured",
        "Increased Metabolism", "Kamikaze", "Night Person", "One Hander", "One In a Million", "Red Scare", "Skilled",
        "Small Frame"
    ];

    private readonly string[] _gitmSlots = ["Body", "Head", "Hands"];
    private readonly string[] _gitmSockets = ["Eye", "Body", "Back", "Hands", "Feet", "Shoes", "Vanity", "In-Hand"];
    private readonly string[] _gitmTypes = ["ITM", "AMO", "ARM", "???", "WEA"];

    private readonly string[] _gobjTypes = [".use", ".dor", ".con"];

    private readonly string[] _gwamDamageTypes = ["Ballistic", "Bio", "Electric", "EMP", "General", "Heat"];
    private readonly string[] _triggerTypes = ["Building", "Script", "Transition"];

    // ReSharper disable once InconsistentNaming - 2 is part of the abbreviation
    private int _2mwtChunk;
    private CommandList _commandList = null!;
    private ImFontPtr? _consoleFont;
    private dynamic _currentFile = null!;

    private int _eeovSelected = -1;
    private string _eeovTemp = "";

    private Vector3 _emapColor = new(1f, 1f, 1f);

    private int _eme2Index;
    private string[] _eme2Names = [];

    private int _emefIndex;
    private string[] _emefNames = [];

    private int _emepIndex;
    private string[] _emepNames = [];

    private Vector3 _emfgColor = new(1f, 1f, 1f);

    private int _emnoIndex;
    private string[] _emnoNames = [];

// EMNP is most similar to 2MWT, single EMNP section with multiple "chunks"
    private int _emnpChunk;

    private int _emsdIndex;
    private string[] _emsdNames = [];

    private int _epthIndex;
    private string[] _epthNames = [];
    private int _epthPoint;
    private string _extension = null!;

    private string _filename = null!;
    private ImFontPtr? _font;
    private int _gcreEquipped;
    private int _gcreSkill;
    private int _gcreSocket;

    private int _gcreSpecial;
    private int _gcreTag;
    private string _gcreTemp = "";
    private int _gcreTrait;

    private int _gitmSocket;
    private GraphicsDevice _graphicsDevice = null!;
    private int _gwamIndex;
    private string[] _gwamNames = [];
    private ImGuiRenderer _imguiRenderer = null!;
    private string[] _stf = null!;

    private int _triggerIndex;
    private string[] _triggerNames = [];
    private int _triggerPoint;

    private TextEditor _vegTextEditor = null!;
    private Sdl2Window _window = null!;

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
        _window.KeyDown += OnKeyDown;
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

        if (File.Exists(@"C:\Windows\Fonts\CascadiaCode.ttf"))
        {
            _consoleFont = io.Fonts.AddFontFromFileTTF(@"C:\Windows\Fonts\CascadiaCode.ttf", 16f);
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
            if (_font is { } font) ImGui.PushFont(font);

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

    private void OnKeyDown(KeyEvent keyEvent)
    {
        switch (keyEvent)
        {
            case { Key: Key.O, Modifiers: ModifierKeys.Control }:
                OpenFile();
                break;
            case { Key: Key.S, Modifiers: ModifierKeys.Control }:
                SaveFile();
                break;
        }
    }

    private void DisposeResources()
    {
        _commandList.Dispose();
        _imguiRenderer.Dispose();
        _graphicsDevice.Dispose();
    }

    private Vector4 ColToVec(Color col)
    {
        return new Vector4(col.R / 255f, col.G / 255f, col.B / 255f, col.A / 255f);
    }

    private Vector3 ColToVec3(Color col)
    {
        return new Vector3(col.R / 255f, col.G / 255f, col.B / 255f);
    }

    // TODO: ensure all used colors are set from ThemeProvider
    private void ApplyAltUIColours()
    {
        var colors = ImGui.GetStyle().Colors;
        colors[0] = ColToVec(ThemeProvider.Theme.Colors.LightText);
        colors[1] = ColToVec(ThemeProvider.Theme.Colors.DisabledText);
        colors[2] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
        colors[3] = new Vector4(1f, 0f, 0f, 1f);
        colors[4] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
        colors[5] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
        colors[6] = new Vector4(0f, 0f, 0f, 0f);
        colors[7] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
        colors[8] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
        colors[9] = new Vector4(0.2f, 0.22f, 0.23f, 1f);
        colors[10] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
        colors[11] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
        colors[12] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
        colors[13] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
        colors[14] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
        colors[15] = new Vector4(0.34f, 0.34f, 0.34f, 1f);
        colors[16] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
        colors[17] = new Vector4(0.56f, 0.56f, 0.56f, 1f);
        colors[18] = ColToVec(ThemeProvider.Theme.Colors.LightText);
        colors[19] = new Vector4(0.34f, 0.34f, 0.34f, 1f);
        colors[20] = new Vector4(0.56f, 0.56f, 0.56f, 1f);
        colors[21] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
        colors[22] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
        colors[23] = new Vector4(0.2f, 0.22f, 0.23f, 1f);
        colors[24] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
        colors[25] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
        colors[26] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
        colors[27] = new Vector4(0f, 0f, 0f, 0f);
        colors[28] = new Vector4(0f, 0f, 0f, 0f);
        colors[29] = new Vector4(0f, 0f, 0f, 0f);
        colors[30] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
        colors[31] = new Vector4(0.44f, 0.44f, 0.44f, 1f);
        colors[32] = new Vector4(0.4f, 0.44f, 0.47f, 1f);
        colors[33] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
        colors[34] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
        colors[35] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
        colors[36] = ColToVec(ThemeProvider.Theme.Colors.HeaderBackground);
        colors[37] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
        colors[38] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
        colors[39] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
        colors[40] = new Vector4(0.15f, 0.15f, 0.15f, 1f);
        colors[41] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
        colors[42] = new Vector4(1f, 0f, 1f, 1f);
        colors[43] = new Vector4(0f, 1f, 1f, 1f);
        colors[44] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
        colors[45] = new Vector4(0f, 0f, 0f, 1f);
        colors[46] = new Vector4(1f, 0f, 1f, 1f);
        colors[47] = new Vector4(0.2f, 0.22f, 0.23f, 1f);
        colors[48] = new Vector4(0.33f, 0.67f, 0.86f, 1f);
        colors[49] = new Vector4(0.24f, 0.24f, 0.24f, 1f);
        colors[50] = new Vector4(1f, 0f, 1f, 1f);
        colors[51] = new Vector4(0f, 1f, 1f, 1f);
        colors[52] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);

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
        style.FrameBorderSize = 1f;
    }

    private static string GetColourCode()
    {
        var sb = new StringBuilder();
        sb.AppendLine("void embraceTheDarkness()");
        sb.AppendLine("{");
        sb.AppendLine("\tImVec4* colors = ImGui::GetStyle().Colors;");
        var colors = ImGui.GetStyle().Colors;
        for (var i = 0; i < (int)ImGuiCol.COUNT; i++)
        {
            var col = colors[i];
            sb.AppendLine($"\tcolors[{i}] = ImVec4({col.X}f, {col.Y}f, {col.Z}f, {col.W}f);");
        }

        sb.AppendLine();
        // style settings
        sb.AppendLine("\tImGuiStyle& style = ImGui::GetStyle();");
        // get style settings
        var style = ImGui.GetStyle();
        sb.AppendLine($"\tstyle.ScrollbarSize = {style.ScrollbarSize};");
        sb.AppendLine($"\tstyle.WindowRounding = {style.WindowRounding};");
        sb.AppendLine($"\tstyle.ChildRounding = {style.ChildRounding};");
        sb.AppendLine($"\tstyle.FrameRounding = {style.FrameRounding};");
        sb.AppendLine($"\tstyle.PopupRounding = {style.PopupRounding};");
        sb.AppendLine($"\tstyle.ScrollbarRounding = {style.ScrollbarRounding};");
        sb.AppendLine($"\tstyle.GrabRounding = {style.GrabRounding};");
        sb.AppendLine($"\tstyle.TabRounding = {style.TabRounding};");
        sb.AppendLine($"\tstyle.WindowBorderSize = {style.WindowBorderSize}f;");
        sb.AppendLine($"\tstyle.FrameBorderSize = {style.FrameBorderSize}f;");
        sb.AppendLine("}");
        return sb.ToString();
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
                    var t = new[]
                        { ".amo", ".arm", ".con", ".crt", ".dor", ".itm", ".map", ".use", ".veg", ".wea" };
                    foreach (var s in t)
                    {
                        if (!ImGui.MenuItem(s)) continue;
                        ResetTempValues();
                        _currentFile = s switch
                        {
                            ".amo" => new AMO(),
                            ".arm" => new ARM(),
                            ".con" => new USE(USEType.CON),
                            ".crt" => new CRT(),
                            ".dor" => new USE(USEType.DOR),
                            //".int" => new INT(), // TODO: how to handle new INT?
                            ".itm" => new ITM(),
                            ".map" => new Map(),
                            ".use" => new USE(),
                            ".veg" => new VEG([]),
                            ".wea" => new WEA(),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        if (s == ".veg") InitVEG();
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.MenuItem("Open", "Ctrl+O"))
                    OpenFile();
                if (ImGui.MenuItem("Open From .grp"))
                    OpenFromGRP();
                if (ImGui.MenuItem("Save", "Ctrl+S"))
                    SaveFile();
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Options"))
            {
                if (ImGui.MenuItem("Set English.stf Location"))
                    SetEngStfLocation();
                if (ImGui.MenuItem("Enable STF Editing", "", Settings.Default.STFEditEnabled))
                {
                    Settings.Default.STFEditEnabled = !Settings.Default.STFEditEnabled;
                    Settings.Default.Save();
                }

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
                {
                    using var grpb = new GrpBrowser(null, true);
                    grpb.ShowDialog();
                }

                if (ImGui.MenuItem(".png to .rle"))
                {
                    var ofd = new OpenFileDialog
                    {
                        Filter = "PNG Files|*.png",
                        Title = "Select a PNG file to convert to RLE"
                    };
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        var bmp = new Bitmap(ofd.FileName);
                        var rle = GrpBrowser.EncodeRLE(bmp);
                        var sfd = new SaveFileDialog
                        {
                            Filter = "RLE Files|*.rle",
                            Title = "Save the RLE file"
                        };
                        if (sfd.ShowDialog() == DialogResult.OK)
                            File.WriteAllBytes(sfd.FileName, rle);
                    }
                }

                //if (ImGui.MenuItem("Get ImGui Colours"))
                //    File.WriteAllText("ImGuiColours.txt", GetColourCode());
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
            case USE:
                DrawUSE();
                break;
            case ARM:
                DrawARM();
                break;
            case AMO:
                DrawAMO();
                break;
            case WEA:
                DrawWEA();
                break;
            case VEG:
                DrawVEG();
                break;
            case INT:
                DrawINT();
                break;
        }
    }

// reset all temporary values used in the editor (indexes, lists, etc.)
    private void ResetTempValues()
    {
        _eeovSelected = -1;
        _eeovTemp = "";
        _gcreSpecial = 0;
        _gcreSkill = 0;
        _gcreTrait = 0;
        _gcreTag = 0;
        _gcreSocket = 0;
        _gcreEquipped = -1;
        _gcreTemp = "";
        _gwamIndex = 0;
        _gwamNames = [];
        _eme2Index = 0;
        _eme2Names = [];
        _emepIndex = 0;
        _emepNames = [];
        _emefIndex = 0;
        _emefNames = [];
        _emsdIndex = 0;
        _emsdNames = [];
        _epthIndex = 0;
        _epthNames = [];
        _epthPoint = 0;
        _triggerIndex = 0;
        _triggerNames = [];
        _triggerPoint = 0;
        _2mwtChunk = 0;
    }

    private void STFBox(string text, ref int sr)
    {
        ImGui.Text(text);
        ImGui.InputInt("##" + text, ref sr);
        if (sr == 0) return;
        ImGui.SameLine();
        ImGui.PushItemWidth(-1);
        if (!Settings.Default.STFEditEnabled)
            ImGui.BeginDisabled();
        if (sr == _stf.Length)
        {
            Array.Resize(ref _stf, _stf.Length + 1);
            _stf[sr] = "";
        }

        ImGui.InputText("##" + text + "in", ref _stf[sr - 1], 100u);
        if (!Settings.Default.STFEditEnabled)
            ImGui.EndDisabled();
        ImGui.PopItemWidth();
    }

    private static void HelpMarker(string desc)
    {
        ImGui.TextDisabled("(?)");
        if (!ImGui.IsItemHovered(ImGuiHoveredFlags.DelayShort)) return;
        ImGui.BeginTooltip();
        ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
        ImGui.TextUnformatted(desc);
        ImGui.PopTextWrapPos();
        ImGui.EndTooltip();
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
        DrawEMFG();
        DrawEMNO();
        DrawEMNP();
    }

    private void DrawUSE()
    {
        DrawEEN2();
        DrawGENT();
        DrawGOBJ();
    }

    private void DrawARM()
    {
        DrawITM();
        DrawGIAR();
    }

    private void DrawAMO()
    {
        DrawITM();
        DrawGIAM();
    }

    private void DrawWEA()
    {
        DrawITM();
        DrawGIWP();
    }

    private void InitVEG()
    {
        var pushed = false;
        if (_consoleFont is { } font)
        {
            ImGui.PushFont(font);
            pushed = true;
        }

        _vegTextEditor = new TextEditor
        {
            Options =
            {
                IsColorizerEnabled = true
            },
            Renderer =
            {
                IsShowingWhitespace = false,
                Palette = _vegPalette
            },
            SyntaxHighlighter = new VEGSyntaxHighlighter()
        };
        if (pushed) ImGui.PopFont();
    }

    private void DrawVEG()
    {
        ImGui.Begin("VEG Editor");
        // VEG is stored as binary but edited in a custom textual language (based on .G3D)
        var pushed = false;
        if (_consoleFont is { } font)
        {
            ImGui.PushFont(font);
            pushed = true;
        }

        _vegTextEditor.Render("VEG Editor", new Vector2(0, 0));
        if (pushed) ImGui.PopFont();
    }
    
    private void DrawINT()
    {
        ImGui.Begin("INT");
        if (_currentFile is not INT gui) return;
        // create preview of the INT file
        if (ImGui.Button("Preview"))
        {
            var ip = new IntViewer();
            ip.Width = 1040;
            ip.Height = 807;
            ip.Show();
            ip.LoadData(gui);
        }
        ImGui.End();
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
        ImGui.PushItemWidth(ImGui.GetWindowWidth() - 78);
        ImGui.InputText("##InventoryItem", ref _eeovTemp, 100u);
        ImGui.PopItemWidth();
        ImGui.SameLine();
        if (ImGui.Button(" + ##Inventory"))
        {
            if (string.IsNullOrWhiteSpace(_eeovTemp)) return;
            eeov.inv = eeov.inv.Append(_eeovTemp).ToArray();
            _eeovTemp = "";
        }

        ImGui.SameLine();
        if (!ImGui.Button(" - ##Inventory")) return;
        if (_eeovSelected == -1) return;
        var tmp = eeov.inv.ToList();
        tmp.RemoveAt(_eeovSelected);
        eeov.inv = tmp.ToArray();
        _eeovSelected = -1;
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
            if (ImGui.InputInt("##SkillVal", ref tmpVal) && tmpVal != 0) gcre.Skills.Add(new Skill(_gcreSkill, tmpVal));
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

        _gwamNames = gcre.GWAM.Select(i => i.NameSR == 0 ? "" : _stf[i.NameSR - 1]).ToArray();
        ImGui.End();
    }

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
        if (ImGui.Button(" - ") && gcre.GWAM.Count > 0)
            gcre.GWAM.RemoveAt(_gwamIndex);
        if (_gwamIndex >= gcre.GWAM.Count)
            _gwamIndex = gcre.GWAM.Count - 1;
        if (gcre.GWAM.Count == 0)
        {
            ImGui.End();
            return;
        }

        var gwam = gcre.GWAM[_gwamIndex];
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

    private void DrawGITM()
    {
        if (_currentFile.GITM is not GITMc gitm) return;
        ImGui.Begin("GITM");
        ImGui.PushItemWidth(-1);
        ImGui.Text("Type");
        ImGui.Combo("##Type", ref gitm.type, _gitmTypes, _gitmTypes.Length);
        ImGui.Text("Slot");
        ImGui.Combo("##Slot", ref gitm.eqslot, _gitmSlots, _gitmSlots.Length);
        ImGui.Checkbox("Equippable", ref gitm.equip);
        ImGui.Text("Reload");
        ImGui.InputInt("##Reload", ref gitm.reload);
        // TODO: figure out how melee works
        var tempBool = false;
        ImGui.Checkbox("Melee", ref tempBool);

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
        ImGui.PopItemWidth();
        ImGui.End();
    }

    private void DrawEMAP()
    {
        if (_currentFile.EMAP is not EMAPc emap) return;
        _emapColor = ColToVec3(emap.col);
        ImGui.Begin("EMAP");
        ImGui.PushItemWidth(-1);
        ImGui.Text("Map Mesh");
        ImGui.InputText("##MapMesh", ref emap.s1, 100u);
        ImGui.Text("Height Map");
        ImGui.InputText("##HeightMap", ref emap.s2, 100u);
        ImGui.Text("Minimap Texture");
        ImGui.InputText("##MinimapTexture", ref emap.s3, 100u);
        ImGui.Text("Lighting Colour");
        ImGui.ColorEdit3("Lighting Colour", ref _emapColor);
        emap.col = Color.FromArgb(255, (int)(_emapColor.X * 255), (int)(_emapColor.Y * 255), (int)(_emapColor.Z * 255));
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
        if (ImGui.Button(" - ") && map.EME2.Count > 0)
            map.EME2.RemoveAt(_eme2Index);
        if (_eme2Index >= map.EME2.Count)
            _eme2Index = map.EME2.Count - 1;
        if (map.EME2.Count == 0)
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
        if (ImGui.Button(" - ") && map.EMEP.Count > 0)
            map.EMEP.RemoveAt(_emepIndex);
        if (_emepIndex >= map.EMEP.Count)
            _emepIndex = map.EMEP.Count - 1;
        if (map.EMEP.Count == 0)
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
        if (ImGui.Button(" - ") && map.EMEF.Count > 0)
            map.EMEF.RemoveAt(_emefIndex);
        if (_emefIndex >= map.EMEF.Count)
            _emefIndex = map.EMEF.Count - 1;
        if (map.EMEF.Count == 0)
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
        if (ImGui.Button(" - ") && map.EMSD.Count > 0)
            map.EMSD.RemoveAt(_emsdIndex);
        if (_emsdIndex >= map.EMSD.Count)
            _emsdIndex = map.EMSD.Count - 1;
        if (map.EMSD.Count == 0)
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

    private void DrawEMNO()
    {
        if (_currentFile is not Map map) return;
        ImGui.Begin("EMNO");
        // get all the names of the EMNOs (use string references)
        _emnoNames = map.EMNO.Select(e => e.sr == 0 ? map.EMNO.IndexOf(e).ToString() : _stf[e.sr - 1]).ToArray();
        ImGui.Combo("##EMNO", ref _emnoIndex, _emnoNames, _emnoNames.Length);
        ImGui.PushItemWidth(-1);
        ImGui.SameLine();
        if (ImGui.Button(" + "))
        {
            map.EMNO.Add(new EMNOc());
            _emnoIndex = map.EMNO.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ") && map.EMNO.Count > 0)
            map.EMNO.RemoveAt(_emnoIndex);
        if (_emnoIndex >= map.EMNO.Count)
            _emnoIndex = map.EMNO.Count - 1;
        if (map.EMNO.Count == 0)
        {
            ImGui.End();
            return;
        }

        var emno = map.EMNO[_emnoIndex];
        ImGui.Text("Position");
        ImGui.InputFloat2("##XY", ref emno.l);
        ImGui.Text("Icon Texture");
        ImGui.InputText("##IconTexture", ref emno.tex, 100u);
        ImGui.PopItemWidth();
        STFBox("Name", ref emno.sr);
        ImGui.End();
    }

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
        if (ImGui.Button(" - ") && map.EPTH.Count > 0)
        {
            map.EPTH.RemoveAt(_epthIndex);
            _epthPoint = 0;
        }

        if (_epthIndex >= map.EPTH.Count)
            _epthIndex = map.EPTH.Count - 1;
        if (map.EPTH.Count == 0)
        {
            ImGui.End();
            return;
        }

        var epth = map.EPTH[_epthIndex];
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
        if (ImGui.Button(" - ##Point") && epth.p.Count > 0)
            epth.p.RemoveAt(_epthPoint);
        if (_epthPoint >= epth.p.Count)
            _epthPoint = epth.p.Count - 1;
        if (epth.p.Count == 0)
        {
            ImGui.End();
            return;
        }

        var point = epth.p[_epthPoint];
        ImGui.Text("Position");
        ImGui.InputFloat4("##XYZR", ref point);
        epth.p[_epthPoint] = point; // can't pass by ref so we must reassign
        ImGui.End();
    }

    private void DrawTrigger()
    {
        if (_currentFile is not Map map) return;
        ImGui.Begin("EMTR");
        ImGui.Combo("##Trigger", ref _triggerIndex, _triggerNames, _triggerNames.Length);
        ImGui.SameLine();
        if (ImGui.Button(" + "))
        {
            map.Triggers.Add(new Trigger());
            _triggerIndex = map.Triggers.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ") && map.Triggers.Count > 0)
        {
            map.Triggers.RemoveAt(_triggerIndex);
            _triggerPoint = 0;
        }

        if (_triggerIndex >= map.Triggers.Count)
            _triggerIndex = map.Triggers.Count - 1;
        if (map.Triggers.Count == 0)
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
            trigger.ExTR.type = triggerIndex switch
            {
                0 => "B",
                1 => "S",
                2 => "T",
                _ => throw new ArgumentOutOfRangeException()
            };
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
        if (ImGui.Button(" - ##Point") && trigger.EMTR.r.Count > 0)
            trigger.EMTR.r.RemoveAt(_triggerPoint);
        if (_triggerPoint >= trigger.EMTR.r.Count)
            _triggerPoint = trigger.EMTR.r.Count - 1;
        if (trigger.EMTR.r.Count == 0)
        {
            ImGui.End();
            return;
        }

        var point = trigger.EMTR.r[_triggerPoint];
        ImGui.Text("Position");
        ImGui.InputFloat3("##XYZR", ref point);
        trigger.EMTR.r[_triggerPoint] = point; // can't pass by ref so we must reassign
        ImGui.End();
    }

    private void Draw2MWT()
    {
        if (_currentFile._2MWT is not _2MWTc)
        {
            _currentFile._2MWT = new _2MWTc();
            return;
        }

        _2MWTc _2mwt = _currentFile._2MWT;
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
        ImGui.PushItemWidth(250);
        ImGui.Combo("##Chunk", ref _2mwtChunk, chunks, chunks.Length);
        ImGui.PopItemWidth();
        ImGui.SameLine();
        if (ImGui.Button(" + "))
        {
            _2mwt.chunks.Add(new _2MWTChunk());
            _2mwtChunk = _2mwt.chunks.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ") && _2mwt.chunks.Count > 0)
            _2mwt.chunks.RemoveAt(_2mwtChunk);
        if (_2mwtChunk >= _2mwt.chunks.Count)
            _2mwtChunk = _2mwt.chunks.Count - 1;
        if (_2mwt.chunks.Count == 0)
        {
            ImGui.End();
            return;
        }

        var chunk = _2mwt.chunks[_2mwtChunk];
        ImGui.Text("Water Lightmap");
        ImGui.InputText("##WaterTexture", ref chunk.tex, 100u);
        ImGui.Text("Position");
        ImGui.InputFloat3("##XYZ", ref chunk.loc);
        ImGui.Text("Lightmap UV");
        ImGui.InputFloat2("##UV", ref chunk.texloc);
        ImGui.End();
    }

    private void DrawEMNP()
    {
        if (_currentFile.EMNP is not EMNPc)
        {
            _currentFile.EMNP = new EMNPc();
            return;
        }

        EMNPc emnp = _currentFile.EMNP;
        ImGui.Begin("EMNP");
        // make list of indices
        var chunks = new string[emnp.chunks.Count];
        for (var i = 0; i < emnp.chunks.Count; i++)
            chunks[i] = i.ToString();
        ImGui.Text("EMNP Chunk");
        ImGui.Combo("##Chunk", ref _emnpChunk, chunks, chunks.Length);
        ImGui.SameLine();
        if (ImGui.Button(" + "))
        {
            emnp.chunks.Add(new EMNPChunk());
            _emnpChunk = emnp.chunks.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ") && emnp.chunks.Count > 0)
            emnp.chunks.RemoveAt(_emnpChunk);
        if (_emnpChunk >= emnp.chunks.Count)
            _emnpChunk = emnp.chunks.Count - 1;
        if (emnp.chunks.Count == 0)
        {
            ImGui.End();
            return;
        }

        ImGui.PushItemWidth(-1);
        var chunk = emnp.chunks[_emnpChunk];
        ImGui.Text("Value 1");
        var temp1 = (int)chunk.@bool;
        ImGui.InputInt("##Value1", ref temp1);
        ImGui.Text("Position");
        ImGui.InputFloat3("##XYZ", ref chunk.l);
        ImGui.Text("Value 2");
        var temp2 = (int)chunk.b1;
        ImGui.InputInt("##Value2", ref temp2);
        ImGui.Text("Value 3");
        var temp3 = (int)chunk.b2;
        ImGui.InputInt("##Value3", ref temp3);
        ImGui.Text("Value 4");
        var temp4 = (int)chunk.b3;
        ImGui.InputInt("##Value4", ref temp4);
        ImGui.Text("Value 5");
        var temp5 = (int)chunk.b4;
        ImGui.InputInt("##Value5", ref temp5);
        ImGui.Text("Value 6");
        var temp6 = (int)chunk.b5;
        ImGui.InputInt("##Value6", ref temp6);
        chunk.@bool = (byte)temp1;
        chunk.b1 = (byte)temp2;
        chunk.b2 = (byte)temp3;
        chunk.b3 = (byte)temp4;
        chunk.b4 = (byte)temp5;
        chunk.b5 = (byte)temp6;
        ImGui.PopItemWidth();
        ImGui.End();
    }

    private void DrawGOBJ()
    {
        if (_currentFile.GOBJ is not GOBJc gobj) return;
        ImGui.Begin("GOBJ");
        ImGui.PushItemWidth(-1);
        ImGui.Text("Type");
        if (ImGui.Combo("##Type", ref gobj.Type, _gobjTypes, _gobjTypes.Length))
            _extension = _gobjTypes[gobj.Type]; // update the extension (used when saving)
        ImGui.End();
    }

    private void DrawGIAR()
    {
        if (_currentFile.GIAR is not GIARc giar) return;
        ImGui.Begin("GIAR");
        ImGui.PushItemWidth(-1);
        ImGui.Text("Ballistic Resistance");
        ImGui.InputInt("##BallisticResistance", ref giar.BallR);
        ImGui.Text("Bio Resistance");
        ImGui.InputInt("##BioResistance", ref giar.BioR);
        ImGui.Text("Electric Resistance");
        ImGui.InputInt("##ElectricResistance", ref giar.ElecR);
        ImGui.Text("EMP Resistance");
        ImGui.InputInt("##EMPResistance", ref giar.EMPR);
        ImGui.Text("Normal Resistance");
        ImGui.InputInt("##NormalResistance", ref giar.NormR);
        ImGui.Text("Heat Resistance");
        ImGui.InputInt("##HeatResistance", ref giar.HeatR);
        ImGui.End();
    }

    private void DrawEMFG()
    {
        if (_currentFile.EMFG is not EMFGc)
        {
            _currentFile.EMFG = new EMFGc();
            return;
        }

        EMFGc emfg = _currentFile.EMFG;
        _emfgColor = ColToVec3(emfg.colour);
        ImGui.Begin("EMFG");
        ImGui.Checkbox("Enabled", ref emfg.enabled);
        ImGui.PushItemWidth(-1);
        ImGui.Text("Colour");
        ImGui.ColorEdit3("##Colour", ref _emfgColor);
        emfg.colour = Color.FromArgb(255, (int)(_emfgColor.X * 255), (int)(_emfgColor.Y * 255),
            (int)(_emfgColor.Z * 255));
        ImGui.Text("Base Height");
        ImGui.InputFloat("##BaseHeight", ref emfg.base_height);
        ImGui.Text("Anim1 Speed");
        ImGui.InputFloat("##Anim1Speed", ref emfg.anim1Speed);
        ImGui.Text("Anim1 Height");
        ImGui.InputFloat("##Anim1Height", ref emfg.anim1Height);
        ImGui.Text("Total Height");
        ImGui.InputFloat("##TotalHeight", ref emfg.total_height);
        ImGui.Text("Anim2 Speed");
        ImGui.InputFloat("##Anim2Speed", ref emfg.anim2Speed);
        ImGui.Text("Anim2 Height");
        ImGui.InputFloat("##Anim2Height", ref emfg.anim2Height);
        ImGui.Text("Vertical Offset");
        ImGui.InputFloat("##VerticalOffset", ref emfg.verticalOffset);
        ImGui.Text("Max Fog Density");
        ImGui.InputFloat("##MaxFogDensity", ref emfg.max_fog_density);
        ImGui.PopItemWidth();
        ImGui.End();
    }

    private void DrawGIAM()
    {
        if (_currentFile.GIAM is not GIAMc giam) return;
        ImGui.Begin("GIAM");
        ImGui.PushItemWidth(-1);
        ImGui.Text("Ammo Type");
        ImGui.Combo("##AmmoType", ref giam.ammoType, _ammoTypes, _ammoTypes.Length);
        ImGui.Text("Minimum Damage");
        ImGui.InputInt("##MinDamage", ref giam.minDmg);
        ImGui.Text("Maximum Damage");
        ImGui.InputInt("##MaxDamage", ref giam.maxDmg);
        ImGui.Text("Range Modifier");
        ImGui.InputInt("##Range", ref giam.unk1);
        ImGui.Text("Crit Chance");
        ImGui.InputInt("##CritChance", ref giam.critChance);
        ImGui.Text("Unknown");
        ImGui.InputInt("##Unknown", ref giam.engUnk1);
        ImGui.Text("Unknown");
        ImGui.InputInt("##Unknown2", ref giam.unk2);
        ImGui.Text("Bullets Per Shot");
        ImGui.InputInt("##Unknown3", ref giam.unk3);
        ImGui.PopItemWidth();
        ImGui.End();
    }

    private void DrawGIWP()
    {
        ImGui.Begin("GIWP");
        ImGui.Text("GIWP not implemented yet.");
        ImGui.End();
    }
}