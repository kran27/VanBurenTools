using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using AltUI.Controls;
using AltUI.Forms;

namespace VBLauncher
{

    public partial class Options : DarkForm
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
            components = new Container();
            var DataGridViewCellStyle1 = new DataGridViewCellStyle();
            var DataGridViewCellStyle2 = new DataGridViewCellStyle();
            var DataGridViewCellStyle3 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(Options));
            ApplyB = new DarkButton();
            ApplyB.Click += new EventHandler(ApplyChanges);
            IntrosCB = new DarkCheckBox();
            CameraCB = new DarkCheckBox();
            AltCamCB = new DarkCheckBox();
            NewGameCB = new DarkComboBox();
            NewGameCB.SelectedIndexChanged += new EventHandler(NewGameToolTip);
            NewGameL = new DarkLabel();
            MainMenuCB = new DarkComboBox();
            MainMenuL = new DarkLabel();
            ToolTip1 = new DarkToolTip();
            SSFCB = new DarkComboBox();
            MipmapCB = new DarkCheckBox();
            PhongCB = new DarkCheckBox();
            APICB = new DarkComboBox();
            CloseB = new DarkButton();
            DarkTabControl1 = new DarkTabControl();
            TabPage1 = new TabPage();
            DarkNumericUpDown1 = new DarkNumericUpDown();
            TabPage2 = new TabPage();
            SSFL = new DarkLabel();
            ResolutionCB = new DarkComboBox();
            ResolutionCB.SelectedIndexChanged += new EventHandler(SetupSSCB);
            ResolutionL = new DarkLabel();
            TextureL = new DarkLabel();
            TextureCB = new DarkComboBox();
            AAL = new DarkLabel();
            AACB = new DarkComboBox();
            APIL = new DarkLabel();
            FullscreenCB = new DarkCheckBox();
            TabPage3 = new TabPage();
            DarkScrollBar1 = new DarkScrollBar();
            DarkScrollBar1.MouseDown += new MouseEventHandler(DarkScrollBar1_Click);
            DarkScrollBar1.MouseUp += new MouseEventHandler(DarkScrollBar1_MouseUp);
            DarkButton2 = new DarkButton();
            DarkButton2.Click += new EventHandler(LoadKeybinds);
            DarkLabel1 = new DarkLabel();
            DataGridView1 = new DataGridView();
            DataGridView1.CellClick += new DataGridViewCellEventHandler(DataGridView1_CellContentClick);
            DataGridView1.Scroll += new ScrollEventHandler(DataGridView1_ScrollChanged);
            DataGridView1.KeyDown += new KeyEventHandler(DataGridView1_KeyDown);
            Timer1 = new Timer(components);
            Timer1.Tick += new EventHandler(Timer1_Tick);
            DarkTabControl1.SuspendLayout();
            TabPage1.SuspendLayout();
            ((ISupportInitialize)DarkNumericUpDown1).BeginInit();
            TabPage2.SuspendLayout();
            TabPage3.SuspendLayout();
            ((ISupportInitialize)DataGridView1).BeginInit();
            SuspendLayout();
            // 
            // ApplyB
            // 
            ApplyB.BorderColour = Color.Empty;
            ApplyB.CustomColour = false;
            ApplyB.FlatBottom = false;
            ApplyB.FlatTop = false;
            ApplyB.Location = new Point(331, 346);
            ApplyB.Margin = new Padding(4, 3, 4, 3);
            ApplyB.Name = "ApplyB";
            ApplyB.Padding = new Padding(5);
            ApplyB.Size = new Size(61, 25);
            ApplyB.TabIndex = 10;
            ApplyB.Text = "Apply";
            // 
            // IntrosCB
            // 
            IntrosCB.AutoSize = true;
            IntrosCB.Location = new Point(7, 6);
            IntrosCB.Margin = new Padding(4, 3, 4, 3);
            IntrosCB.Name = "IntrosCB";
            IntrosCB.Offset = 1;
            IntrosCB.Size = new Size(127, 19);
            IntrosCB.TabIndex = 1;
            IntrosCB.Text = "Enable Intro Videos";
            // 
            // CameraCB
            // 
            CameraCB.AutoSize = true;
            CameraCB.Location = new Point(7, 31);
            CameraCB.Margin = new Padding(4, 3, 4, 3);
            CameraCB.Name = "CameraCB";
            CameraCB.Offset = 1;
            CameraCB.Size = new Size(139, 19);
            CameraCB.TabIndex = 4;
            CameraCB.Text = "Uncap Camera Zoom";
            // 
            // AltCamCB
            // 
            AltCamCB.AutoSize = true;
            AltCamCB.Location = new Point(154, 31);
            AltCamCB.Margin = new Padding(4, 3, 4, 3);
            AltCamCB.Name = "AltCamCB";
            AltCamCB.Offset = 1;
            AltCamCB.Size = new Size(157, 19);
            AltCamCB.TabIndex = 3;
            AltCamCB.Text = "Alternate Camera Angles";
            // 
            // NewGameCB
            // 
            NewGameCB.DrawMode = DrawMode.OwnerDrawVariable;
            NewGameCB.FormattingEnabled = true;
            NewGameCB.Items.AddRange(new object[] { "00_03_Tutorial_Junktown.map", "00_04_Tutorial_Vault.map", "04_0202_Spelunking.map", "98_Canyon_Random_01.map", "98_Canyon_Random_02.map", "Default_StartMap.map", "Mainmenu.map", "zz_TestMapsScottE_Test1.map", "zz_TestMapsScottE_Test2.map", "zz_TestMapsScottE_Test4.map", "zz_TestMapsTest_City_Building01.map", "zz_TestMapsTest_City_Building02.map", "zz_TestMapsTest_City_Building03.map", "zz_TestMapsTest_City_Building04.map", "zz_TestMapsTest_City_Fences.map", "zz_TestMapsTest_Junktown_Shacks.map", "zz_TestMapsaarontemp2.map" });
            NewGameCB.Location = new Point(7, 71);
            NewGameCB.Margin = new Padding(4, 3, 4, 3);
            NewGameCB.Name = "NewGameCB";
            NewGameCB.Size = new Size(241, 24);
            NewGameCB.TabIndex = 5;
            // 
            // NewGameL
            // 
            NewGameL.AutoSize = true;
            NewGameL.ForeColor = Color.RebeccaPurple;
            NewGameL.Location = new Point(7, 53);
            NewGameL.Margin = new Padding(4, 0, 4, 0);
            NewGameL.Name = "NewGameL";
            NewGameL.Size = new Size(58, 15);
            NewGameL.TabIndex = 0;
            NewGameL.Text = "Start Map";
            // 
            // MainMenuCB
            // 
            MainMenuCB.DrawMode = DrawMode.OwnerDrawVariable;
            MainMenuCB.FormattingEnabled = true;
            MainMenuCB.Items.AddRange(new object[] { "Default", "Aaron Map 2", "Building 1", "Building 2", "Building 3", "Building 4", "Canyon 1", "Canyon 2", "Cave", "Fences", "Scott E Map 1", "Scott E Map 2", "Scott E Map 4", "Shacks", "Start Map", "Tutorial", "Vault" });
            MainMenuCB.Location = new Point(7, 116);
            MainMenuCB.Margin = new Padding(4, 3, 4, 3);
            MainMenuCB.Name = "MainMenuCB";
            MainMenuCB.Size = new Size(358, 24);
            MainMenuCB.TabIndex = 6;
            // 
            // MainMenuL
            // 
            MainMenuL.AutoSize = true;
            MainMenuL.Location = new Point(7, 98);
            MainMenuL.Margin = new Padding(4, 0, 4, 0);
            MainMenuL.Name = "MainMenuL";
            MainMenuL.Size = new Size(68, 15);
            MainMenuL.TabIndex = 0;
            MainMenuL.Text = "Main Menu";
            // 
            // ToolTip1
            // 
            ToolTip1.OwnerDraw = true;
            // 
            // SSFCB
            // 
            SSFCB.DrawMode = DrawMode.OwnerDrawVariable;
            SSFCB.FormattingEnabled = true;
            SSFCB.Location = new Point(188, 108);
            SSFCB.Margin = new Padding(2);
            SSFCB.Name = "SSFCB";
            SSFCB.Size = new Size(177, 24);
            SSFCB.TabIndex = 28;
            ToolTip1.SetToolTip(SSFCB, "Increases visual quality by" + '\r' + '\n' + "rendering the scene above" + '\r' + '\n' + "your display resolution");
            // 
            // MipmapCB
            // 
            MipmapCB.AutoSize = true;
            MipmapCB.Location = new Point(94, 137);
            MipmapCB.Margin = new Padding(4, 3, 4, 3);
            MipmapCB.Name = "MipmapCB";
            MipmapCB.Offset = 1;
            MipmapCB.Size = new Size(95, 19);
            MipmapCB.TabIndex = 30;
            MipmapCB.Text = "Mipmapping";
            ToolTip1.SetToolTip(MipmapCB, "Disabling reduces texture blur at distance");
            // 
            // PhongCB
            // 
            PhongCB.AutoSize = true;
            PhongCB.Location = new Point(195, 137);
            PhongCB.Margin = new Padding(2);
            PhongCB.Name = "PhongCB";
            PhongCB.Offset = 1;
            PhongCB.Size = new Size(107, 19);
            PhongCB.TabIndex = 31;
            PhongCB.Text = "Phong Shading";
            ToolTip1.SetToolTip(PhongCB, "Alternative shading," + '\r' + '\n' + "Slightly improves visuals");
            // 
            // APICB
            // 
            APICB.DrawMode = DrawMode.OwnerDrawVariable;
            APICB.FormattingEnabled = true;
            APICB.Items.AddRange(new object[] { "DirectX 11", "Vulkan" });
            APICB.Location = new Point(7, 21);
            APICB.Margin = new Padding(4, 3, 4, 3);
            APICB.Name = "APICB";
            APICB.Size = new Size(358, 24);
            APICB.TabIndex = 24;
            ToolTip1.SetToolTip(APICB, "Both modes use dgVoodoo2" + '\r' + '\n' + "dege.fw.hu" + '\r' + '\n' + "Vulkan uses DXVK (zlib/libpng)" + '\r' + '\n' + "github.com/" + "doitsujin/dxvk");
            // 
            // CloseB
            // 
            CloseB.BorderColour = Color.Empty;
            CloseB.CustomColour = false;
            CloseB.DialogResult = DialogResult.Cancel;
            CloseB.FlatBottom = false;
            CloseB.FlatTop = false;
            CloseB.Location = new Point(262, 346);
            CloseB.Margin = new Padding(4, 3, 4, 3);
            CloseB.Name = "CloseB";
            CloseB.Padding = new Padding(5);
            CloseB.Size = new Size(61, 25);
            CloseB.TabIndex = 9;
            CloseB.Text = "Close";
            // 
            // DarkTabControl1
            // 
            DarkTabControl1.AllowDrop = true;
            DarkTabControl1.Controls.Add(TabPage1);
            DarkTabControl1.Controls.Add(TabPage2);
            DarkTabControl1.Controls.Add(TabPage3);
            DarkTabControl1.DisableClose = true;
            DarkTabControl1.DisableDragging = true;
            DarkTabControl1.Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);
            DarkTabControl1.Location = new Point(12, 12);
            DarkTabControl1.Name = "DarkTabControl1";
            DarkTabControl1.Padding = new Point(14, 4);
            DarkTabControl1.SelectedIndex = 0;
            DarkTabControl1.Size = new Size(380, 328);
            DarkTabControl1.TabIndex = 11;
            // 
            // TabPage1
            // 
            TabPage1.BackColor = Color.FromArgb(26, 26, 28);
            TabPage1.Controls.Add(DarkNumericUpDown1);
            TabPage1.Controls.Add(CameraCB);
            TabPage1.Controls.Add(NewGameCB);
            TabPage1.Controls.Add(NewGameL);
            TabPage1.Controls.Add(IntrosCB);
            TabPage1.Controls.Add(MainMenuCB);
            TabPage1.Controls.Add(MainMenuL);
            TabPage1.Controls.Add(AltCamCB);
            TabPage1.Location = new Point(4, 27);
            TabPage1.Name = "TabPage1";
            TabPage1.Padding = new Padding(3);
            TabPage1.Size = new Size(372, 297);
            TabPage1.TabIndex = 0;
            TabPage1.Text = "General";
            // 
            // DarkNumericUpDown1
            // 
            DarkNumericUpDown1.Location = new Point(256, 72);
            DarkNumericUpDown1.Name = "DarkNumericUpDown1";
            DarkNumericUpDown1.Size = new Size(110, 23);
            DarkNumericUpDown1.TabIndex = 7;
            // 
            // TabPage2
            // 
            TabPage2.BackColor = Color.FromArgb(26, 26, 28);
            TabPage2.Controls.Add(SSFL);
            TabPage2.Controls.Add(SSFCB);
            TabPage2.Controls.Add(ResolutionCB);
            TabPage2.Controls.Add(ResolutionL);
            TabPage2.Controls.Add(MipmapCB);
            TabPage2.Controls.Add(PhongCB);
            TabPage2.Controls.Add(TextureL);
            TabPage2.Controls.Add(TextureCB);
            TabPage2.Controls.Add(AAL);
            TabPage2.Controls.Add(AACB);
            TabPage2.Controls.Add(APICB);
            TabPage2.Controls.Add(APIL);
            TabPage2.Controls.Add(FullscreenCB);
            TabPage2.Location = new Point(4, 27);
            TabPage2.Name = "TabPage2";
            TabPage2.Padding = new Padding(3);
            TabPage2.Size = new Size(372, 297);
            TabPage2.TabIndex = 1;
            TabPage2.Text = "Graphics";
            // 
            // SSFL
            // 
            SSFL.AutoSize = true;
            SSFL.Location = new Point(188, 91);
            SSFL.Margin = new Padding(4, 0, 4, 0);
            SSFL.Name = "SSFL";
            SSFL.Size = new Size(103, 15);
            SSFL.TabIndex = 36;
            SSFL.Text = "Render Resolution";
            // 
            // ResolutionCB
            // 
            ResolutionCB.DrawMode = DrawMode.OwnerDrawVariable;
            ResolutionCB.FormattingEnabled = true;
            ResolutionCB.Location = new Point(7, 108);
            ResolutionCB.Margin = new Padding(2);
            ResolutionCB.Name = "ResolutionCB";
            ResolutionCB.Size = new Size(177, 24);
            ResolutionCB.TabIndex = 27;
            // 
            // ResolutionL
            // 
            ResolutionL.AutoSize = true;
            ResolutionL.Location = new Point(7, 91);
            ResolutionL.Margin = new Padding(4, 0, 4, 0);
            ResolutionL.Name = "ResolutionL";
            ResolutionL.Size = new Size(79, 15);
            ResolutionL.TabIndex = 35;
            ResolutionL.Text = "Display Mode";
            // 
            // TextureL
            // 
            TextureL.AutoSize = true;
            TextureL.Location = new Point(188, 48);
            TextureL.Margin = new Padding(2, 0, 2, 0);
            TextureL.Name = "TextureL";
            TextureL.Size = new Size(91, 15);
            TextureL.TabIndex = 34;
            TextureL.Text = "Texture Filtering";
            // 
            // TextureCB
            // 
            TextureCB.DrawMode = DrawMode.OwnerDrawVariable;
            TextureCB.FormattingEnabled = true;
            TextureCB.Items.AddRange(new object[] { "Bilinear (Default)", "Point Sampled", "Linear", "Anisotropic 2x", "Anisotropic 4x", "Anisotropic 8x", "Anisotropic 16x" });
            TextureCB.Location = new Point(188, 65);
            TextureCB.Margin = new Padding(2);
            TextureCB.Name = "TextureCB";
            TextureCB.Size = new Size(177, 24);
            TextureCB.TabIndex = 26;
            // 
            // AAL
            // 
            AAL.AutoSize = true;
            AAL.Location = new Point(7, 48);
            AAL.Margin = new Padding(2, 0, 2, 0);
            AAL.Name = "AAL";
            AAL.Size = new Size(74, 15);
            AAL.TabIndex = 33;
            AAL.Text = "Anti Aliasing";
            // 
            // AACB
            // 
            AACB.DrawMode = DrawMode.OwnerDrawVariable;
            AACB.FormattingEnabled = true;
            AACB.Items.AddRange(new object[] { "Off", "2x MSAA", "4x MSAA", "8x MSAA" });
            AACB.Location = new Point(7, 65);
            AACB.Margin = new Padding(2);
            AACB.Name = "AACB";
            AACB.Size = new Size(177, 24);
            AACB.TabIndex = 25;
            // 
            // APIL
            // 
            APIL.AutoSize = true;
            APIL.Location = new Point(7, 3);
            APIL.Margin = new Padding(4, 0, 4, 0);
            APIL.Name = "APIL";
            APIL.Size = new Size(82, 15);
            APIL.TabIndex = 32;
            APIL.Text = "Rendering API";
            // 
            // FullscreenCB
            // 
            FullscreenCB.AutoSize = true;
            FullscreenCB.Location = new Point(7, 137);
            FullscreenCB.Margin = new Padding(4, 3, 4, 3);
            FullscreenCB.Name = "FullscreenCB";
            FullscreenCB.Offset = 1;
            FullscreenCB.Size = new Size(79, 19);
            FullscreenCB.TabIndex = 29;
            FullscreenCB.Text = "Fullscreen";
            // 
            // TabPage3
            // 
            TabPage3.BackColor = Color.FromArgb(26, 26, 28);
            TabPage3.Controls.Add(DarkScrollBar1);
            TabPage3.Controls.Add(DarkButton2);
            TabPage3.Controls.Add(DarkLabel1);
            TabPage3.Controls.Add(DataGridView1);
            TabPage3.Location = new Point(4, 27);
            TabPage3.Name = "TabPage3";
            TabPage3.Padding = new Padding(3);
            TabPage3.Size = new Size(372, 297);
            TabPage3.TabIndex = 2;
            TabPage3.Text = "Keybinds";
            // 
            // DarkScrollBar1
            // 
            DarkScrollBar1.Location = new Point(357, 1);
            DarkScrollBar1.Maximum = 500;
            DarkScrollBar1.Name = "DarkScrollBar1";
            DarkScrollBar1.Size = new Size(22, 259);
            DarkScrollBar1.TabIndex = 10;
            DarkScrollBar1.Text = "DarkScrollBar1";
            // 
            // DarkButton2
            // 
            DarkButton2.BorderColour = Color.Empty;
            DarkButton2.CustomColour = false;
            DarkButton2.FlatBottom = false;
            DarkButton2.FlatTop = false;
            DarkButton2.Location = new Point(305, 266);
            DarkButton2.Name = "DarkButton2";
            DarkButton2.Padding = new Padding(5);
            DarkButton2.Size = new Size(61, 24);
            DarkButton2.TabIndex = 9;
            DarkButton2.Text = "Reload";
            // 
            // DarkLabel1
            // 
            DarkLabel1.AutoSize = true;
            DarkLabel1.Location = new Point(6, 271);
            DarkLabel1.Name = "DarkLabel1";
            DarkLabel1.Size = new Size(145, 15);
            DarkLabel1.TabIndex = 8;
            DarkLabel1.Text = "Press any Key/Key Combo";
            // 
            // DataGridView1
            // 
            DataGridView1.AllowUserToResizeColumns = false;
            DataGridView1.AllowUserToResizeRows = false;
            DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            DataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            DataGridView1.BackgroundColor = Color.FromArgb(32, 32, 32);
            DataGridView1.BorderStyle = BorderStyle.None;
            DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DataGridViewCellStyle1.BackColor = SystemColors.Control;
            DataGridViewCellStyle1.Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);
            DataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            DataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            DataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            DataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            DataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1;
            DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DataGridViewCellStyle2.BackColor = SystemColors.Window;
            DataGridViewCellStyle2.Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);
            DataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            DataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            DataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            DataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            DataGridView1.DefaultCellStyle = DataGridViewCellStyle2;
            DataGridView1.GridColor = SystemColors.ButtonFace;
            DataGridView1.Location = new Point(1, 1);
            DataGridView1.MultiSelect = false;
            DataGridView1.Name = "DataGridView1";
            DataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DataGridViewCellStyle3.BackColor = SystemColors.Control;
            DataGridViewCellStyle3.Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);
            DataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            DataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            DataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            DataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            DataGridView1.RowHeadersDefaultCellStyle = DataGridViewCellStyle3;
            DataGridView1.RowHeadersVisible = false;
            DataGridView1.RowTemplate.Height = 25;
            DataGridView1.ScrollBars = ScrollBars.Vertical;
            DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DataGridView1.Size = new Size(374, 259);
            DataGridView1.TabIndex = 3;
            // 
            // Timer1
            // 
            Timer1.Interval = 15;
            // 
            // Options
            // 
            AcceptButton = ApplyB;
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(405, 383);
            Controls.Add(DarkTabControl1);
            Controls.Add(CloseB);
            Controls.Add(ApplyB);
            CornerStyle = CornerPreference.Default;
            CustomBorder = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Options";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Options";
            TransparencyKey = Color.FromArgb(31, 31, 32);
            DarkTabControl1.ResumeLayout(false);
            TabPage1.ResumeLayout(false);
            TabPage1.PerformLayout();
            ((ISupportInitialize)DarkNumericUpDown1).EndInit();
            TabPage2.ResumeLayout(false);
            TabPage2.PerformLayout();
            TabPage3.ResumeLayout(false);
            TabPage3.PerformLayout();
            ((ISupportInitialize)DataGridView1).EndInit();
            Load += new EventHandler(CheckOptions);
            ResumeLayout(false);

        }
        internal DarkToolTip ToolTip1;
        internal DarkButton ApplyB;
        internal DarkCheckBox IntrosCB;
        internal DarkComboBox MainMenuCB;
        internal DarkLabel MainMenuL;
        internal DarkCheckBox AltCamCB;
        internal DarkComboBox NewGameCB;
        internal DarkLabel NewGameL;
        internal DarkButton CloseB;
        internal DarkCheckBox CameraCB;
        internal DarkTabControl DarkTabControl1;
        internal TabPage TabPage1;
        internal TabPage TabPage2;
        internal TabPage TabPage3;
        internal DarkLabel SSFL;
        internal DarkComboBox SSFCB;
        internal DarkComboBox ResolutionCB;
        internal DarkLabel ResolutionL;
        internal DarkCheckBox MipmapCB;
        internal DarkCheckBox PhongCB;
        internal DarkLabel TextureL;
        internal DarkComboBox TextureCB;
        internal DarkLabel AAL;
        internal DarkComboBox AACB;
        internal DarkComboBox APICB;
        internal DarkLabel APIL;
        internal DarkCheckBox FullscreenCB;
        internal DataGridView DataGridView1;
        internal DarkButton DarkButton2;
        internal DarkLabel DarkLabel1;
        internal DarkScrollBar DarkScrollBar1;
        internal Timer Timer1;
        internal DarkNumericUpDown DarkNumericUpDown1;
    }
}