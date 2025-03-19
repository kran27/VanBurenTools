using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using AltUI.Forms;

namespace VBLauncher
{
    public partial class MainForm : DarkForm
    {

        // Form overrides dispose to clean up the component list.
        [DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is not null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Required by the Windows Form Designer
        private IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            var resources = new ComponentResourceManager(typeof(MainForm));
            LaunchB = new PictureBox();
            LaunchB.Click += new EventHandler(LaunchGame);
            LaunchB.MouseDown += new MouseEventHandler(PlayButtonDown);
            LaunchB.MouseUp += new MouseEventHandler(PlayButtonUp);
            OptionsB = new PictureBox();
            OptionsB.Click += new EventHandler(OpenOptions);
            OptionsB.MouseDown += new MouseEventHandler(PlayButtonDown);
            OptionsB.MouseUp += new MouseEventHandler(PlayButtonUp);
            ExitB = new PictureBox();
            ExitB.Click += new EventHandler(ExitLauncher);
            ExitB.MouseDown += new MouseEventHandler(PlayButtonDown);
            ExitB.MouseUp += new MouseEventHandler(PlayButtonUp);
            Logo = new PictureBox();
            Logo.MouseDown += new MouseEventHandler(Form1_MouseDown);
            Background = new PictureBox();
            Background.MouseDown += new MouseEventHandler(Form1_MouseDown);
            EditorB = new PictureBox();
            EditorB.Click += new EventHandler(EditorB_Click);
            //EditorB.MouseDown += new MouseEventHandler(PlayButtonDown);
            //EditorB.MouseUp += new MouseEventHandler(PlayButtonUp);
            ModLoaderB = new PictureBox();
            ModLoaderB.MouseDown += new MouseEventHandler(PlayButtonDown);
            ModLoaderB.MouseUp += new MouseEventHandler(PlayButtonUp);
            ModLoaderB.Click += new EventHandler(ModLoaderB_Click);
            ((ISupportInitialize)LaunchB).BeginInit();
            ((ISupportInitialize)OptionsB).BeginInit();
            ((ISupportInitialize)ExitB).BeginInit();
            ((ISupportInitialize)Logo).BeginInit();
            ((ISupportInitialize)Background).BeginInit();
            ((ISupportInitialize)EditorB).BeginInit();
            ((ISupportInitialize)ModLoaderB).BeginInit();
            SuspendLayout();
            // 
            // LaunchB
            // 
            LaunchB.BackColor = Color.Transparent;
            LaunchB.BackgroundImage = (Image)resources.GetObject("LaunchB.BackgroundImage");
            LaunchB.BackgroundImageLayout = ImageLayout.Stretch;
            LaunchB.Location = new Point(13, 13);
            LaunchB.Margin = new Padding(4);
            LaunchB.Name = "LaunchB";
            LaunchB.Size = new Size(192, 29);
            LaunchB.TabIndex = 0;
            LaunchB.TabStop = false;
            // 
            // OptionsB
            // 
            OptionsB.BackColor = Color.Transparent;
            OptionsB.BackgroundImage = (Image)resources.GetObject("OptionsB.BackgroundImage");
            OptionsB.BackgroundImageLayout = ImageLayout.Stretch;
            OptionsB.Location = new Point(13, 50);
            OptionsB.Margin = new Padding(4);
            OptionsB.Name = "OptionsB";
            OptionsB.Size = new Size(192, 29);
            OptionsB.TabIndex = 1;
            OptionsB.TabStop = false;
            // 
            // ExitB
            // 
            ExitB.BackColor = Color.Transparent;
            ExitB.BackgroundImage = (Image)resources.GetObject("ExitB.BackgroundImage");
            ExitB.BackgroundImageLayout = ImageLayout.Stretch;
            ExitB.Location = new Point(13, 161);
            ExitB.Margin = new Padding(4);
            ExitB.Name = "ExitB";
            ExitB.Size = new Size(192, 29);
            ExitB.TabIndex = 2;
            ExitB.TabStop = false;
            // 
            // Logo
            // 
            Logo.BackColor = Color.Transparent;
            Logo.BackgroundImage = (Image)resources.GetObject("Logo.BackgroundImage");
            Logo.BackgroundImageLayout = ImageLayout.Stretch;
            Logo.Location = new Point(478, 317);
            Logo.Margin = new Padding(4);
            Logo.Name = "Logo";
            Logo.Size = new Size(149, 150);
            Logo.TabIndex = 3;
            Logo.TabStop = false;
            // 
            // Background
            // 
            Background.BackgroundImageLayout = ImageLayout.Stretch;
            Background.Location = new Point(0, 0);
            Background.Name = "Background";
            Background.Size = new Size(640, 480);
            Background.TabIndex = 4;
            Background.TabStop = false;
            // 
            // EditorB
            // 
            EditorB.BackColor = Color.Transparent;
            EditorB.BackgroundImage = (Image)resources.GetObject("EditorB.BackgroundImage");
            EditorB.BackgroundImageLayout = ImageLayout.Stretch;
            EditorB.Location = new Point(13, 124);
            EditorB.Margin = new Padding(4);
            EditorB.Name = "EditorB";
            EditorB.Size = new Size(192, 29);
            EditorB.TabIndex = 6;
            EditorB.TabStop = false;
            // 
            // ModLoaderB
            // 
            ModLoaderB.BackColor = Color.Transparent;
            ModLoaderB.BackgroundImage = (Image)resources.GetObject("ModLoaderB.BackgroundImage");
            ModLoaderB.BackgroundImageLayout = ImageLayout.Stretch;
            ModLoaderB.Location = new Point(13, 87);
            ModLoaderB.Margin = new Padding(4);
            ModLoaderB.Name = "ModLoaderB";
            ModLoaderB.Size = new Size(192, 29);
            ModLoaderB.TabIndex = 7;
            ModLoaderB.TabStop = false;
            // 
            // Main
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.Black;
            ClientSize = new Size(640, 480);
            Controls.Add(Logo);
            Controls.Add(OptionsB);
            Controls.Add(LaunchB);
            Controls.Add(ExitB);
            Controls.Add(EditorB);
            Controls.Add(ModLoaderB);
            Controls.Add(Background);
            CornerStyle = CornerPreference.Default;
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Van Buren Launcher";
            TransparencyKey = Color.FromArgb(31, 31, 32);
            ((ISupportInitialize)LaunchB).EndInit();
            ((ISupportInitialize)OptionsB).EndInit();
            ((ISupportInitialize)ExitB).EndInit();
            ((ISupportInitialize)Logo).EndInit();
            ((ISupportInitialize)Background).EndInit();
            ((ISupportInitialize)EditorB).EndInit();
            ((ISupportInitialize)ModLoaderB).EndInit();
            Load += new EventHandler(Startup);
            ResumeLayout(false);

        }

        internal PictureBox LaunchB;
        internal PictureBox OptionsB;
        internal PictureBox ExitB;
        internal PictureBox Logo;
        internal PictureBox Background;
        internal PictureBox EditorB;
        internal PictureBox ModLoaderB;
    }
}