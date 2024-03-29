﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static AltUI.Forms.DarkMessageBox;

namespace VBLauncher
{

    public partial class Main
    {

        #region Move Form

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        public Main()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        private static extern void SendMessage(nint hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern void ReleaseCapture();

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #endregion

        private bool _mouseDown;
        private readonly SoundPlayer sound = new SoundPlayer();

        private void Startup(object sender, EventArgs e)
        {
            Logo.Parent = Background;
            Background.BackgroundImage = (Image)My.Resources.Resources.ResourceManager.GetObject($"BG{new Random().Next(1, 13)}");
            if (File.Exists("F3.exe"))
            {
                Directory.CreateDirectory("Mods");
                File.WriteAllBytes(@"Mods\Fixes.zip", My.Resources.Resources.Fixes);
                File.WriteAllBytes(@"Mods\SUMM.zip", My.Resources.Resources.SUMM);
                File.WriteAllBytes("granny2_x64.dll", My.Resources.Resources.granny2_x64); // granny dll used for parsing game files
                AddFontResource(@"Fonts\TT0807M_.TTF");
                AddFontResource(@"Fonts\r_fallouty.ttf");
            }
            else
            {
                ShowError("Please put the launcher in the same directory as the game so you can launch it!", "Game Executable Not Found!");
            }

            #if DEBUG
            var x = new GrpBrowser(new[] { "skl" }, true);
            x.ShowDialog();
            #endif
            AllowTransparency = false;
            try
            {
                File.Delete($@"{Environment.GetFolderPath(Environment.SpecialFolder.Personal)}\F3\Characters\None.CRT");
            }
            catch
            {
            }
            try
            {
                IniManager.F3Ini = File.ReadAllLines(IniManager.F3Dir);
            }
            catch
            {
                Directory.CreateDirectory($@"{Environment.GetFolderPath(Environment.SpecialFolder.Personal)}\F3");
                File.WriteAllText(IniManager.F3Dir, My.Resources.Resources.Default_F3);
                IniManager.F3Ini = File.ReadAllLines(IniManager.F3Dir);
            }
            try
            {
                IniManager.SysIni = File.ReadAllLines(IniManager.SysDir);
            }
            catch
            {
                Directory.CreateDirectory(@"Override\MenuMap\Engine");
                File.WriteAllText(IniManager.SysDir, My.Resources.Resources.Default_sys);
                IniManager.SysIni = File.ReadAllLines(IniManager.SysDir);
            }
        }

        private void LaunchGame(object sender, EventArgs e)
        {
            PlayButtonUp(sender, e);
            try
            {
                Process.Start("F3.exe");
                Close();
            }
            catch
            {
                ShowError("Please put the launcher in the same directory as the game so you can launch it!", "Game Executable Not Found!");
            }
        }

        private void OpenOptions(object sender, EventArgs e)
        {
            PlayButtonUp(sender, e);
            My.MyProject.Forms.Options.ShowDialog();
        }

        private void ExitLauncher(object sender, EventArgs e)
        {
            PlayButtonUp(sender, e);
            Close();
        }

        private void EditorB_Click(object sender, EventArgs e)
        {
            PlayButtonUp(sender, e);
            My.MyProject.Forms.Editor.Show();
        }

        private void PlayButtonDown(object sender, EventArgs e)
        {
            if (!_mouseDown)
            {
                _mouseDown = true;
                sound.Stream = My.Resources.Resources.f3_button_down_01;
                sound.Play();
            }
        }

        private void PlayButtonUp(object sender, EventArgs e)
        {
            if (_mouseDown)
            {
                _mouseDown = false;
                sound.Stream = My.Resources.Resources.f3_button_up_01;
                sound.Play();
            }
        }

        [DllImport("gdi32.dll")]
        private static extern void AddFontResource(string FontPath);

        private void ModLoaderB_Click(object sender, EventArgs e)
        {
            PlayButtonUp(sender, e);
            My.MyProject.Forms.ModLoader.ShowDialog();
        }

    }
}