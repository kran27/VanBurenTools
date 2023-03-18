Imports System.ComponentModel
Imports AltUI.Controls
Imports AltUI.Forms
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()>
Partial Class Options
    Inherits DarkForm

    'Form overrides dispose to clean up the component list.
    <DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Options))
        Me.ApplyB = New AltUI.Controls.DarkButton()
        Me.IntrosCB = New AltUI.Controls.DarkCheckBox()
        Me.CameraCB = New AltUI.Controls.DarkCheckBox()
        Me.AltCamCB = New AltUI.Controls.DarkCheckBox()
        Me.NewGameCB = New AltUI.Controls.DarkComboBox()
        Me.NewGameL = New AltUI.Controls.DarkLabel()
        Me.MainMenuCB = New AltUI.Controls.DarkComboBox()
        Me.MainMenuL = New AltUI.Controls.DarkLabel()
        Me.ToolTip1 = New AltUI.Controls.DarkToolTip()
        Me.SSFCB = New AltUI.Controls.DarkComboBox()
        Me.MipmapCB = New AltUI.Controls.DarkCheckBox()
        Me.PhongCB = New AltUI.Controls.DarkCheckBox()
        Me.APICB = New AltUI.Controls.DarkComboBox()
        Me.CloseB = New AltUI.Controls.DarkButton()
        Me.DarkTabControl1 = New AltUI.Controls.DarkTabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.DarkNumericUpDown1 = New AltUI.Controls.DarkNumericUpDown()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.SSFL = New AltUI.Controls.DarkLabel()
        Me.ResolutionCB = New AltUI.Controls.DarkComboBox()
        Me.ResolutionL = New AltUI.Controls.DarkLabel()
        Me.TextureL = New AltUI.Controls.DarkLabel()
        Me.TextureCB = New AltUI.Controls.DarkComboBox()
        Me.AAL = New AltUI.Controls.DarkLabel()
        Me.AACB = New AltUI.Controls.DarkComboBox()
        Me.APIL = New AltUI.Controls.DarkLabel()
        Me.FullscreenCB = New AltUI.Controls.DarkCheckBox()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.DarkScrollBar1 = New AltUI.Controls.DarkScrollBar()
        Me.DarkButton2 = New AltUI.Controls.DarkButton()
        Me.DarkLabel1 = New AltUI.Controls.DarkLabel()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.DarkTabControl1.SuspendLayout
        Me.TabPage1.SuspendLayout
        CType(Me.DarkNumericUpDown1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.TabPage2.SuspendLayout
        Me.TabPage3.SuspendLayout
        CType(Me.DataGridView1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SuspendLayout
        '
        'ApplyB
        '
        Me.ApplyB.BorderColour = System.Drawing.Color.Empty
        Me.ApplyB.CustomColour = false
        Me.ApplyB.FlatBottom = false
        Me.ApplyB.FlatTop = false
        Me.ApplyB.Location = New System.Drawing.Point(331, 346)
        Me.ApplyB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.ApplyB.Name = "ApplyB"
        Me.ApplyB.Padding = New System.Windows.Forms.Padding(5)
        Me.ApplyB.Size = New System.Drawing.Size(61, 25)
        Me.ApplyB.TabIndex = 10
        Me.ApplyB.Text = "Apply"
        '
        'IntrosCB
        '
        Me.IntrosCB.AutoSize = true
        Me.IntrosCB.Location = New System.Drawing.Point(7, 6)
        Me.IntrosCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.IntrosCB.Name = "IntrosCB"
        Me.IntrosCB.Offset = 1
        Me.IntrosCB.Size = New System.Drawing.Size(127, 19)
        Me.IntrosCB.TabIndex = 1
        Me.IntrosCB.Text = "Enable Intro Videos"
        '
        'CameraCB
        '
        Me.CameraCB.AutoSize = true
        Me.CameraCB.Location = New System.Drawing.Point(7, 31)
        Me.CameraCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CameraCB.Name = "CameraCB"
        Me.CameraCB.Offset = 1
        Me.CameraCB.Size = New System.Drawing.Size(139, 19)
        Me.CameraCB.TabIndex = 4
        Me.CameraCB.Text = "Uncap Camera Zoom"
        '
        'AltCamCB
        '
        Me.AltCamCB.AutoSize = true
        Me.AltCamCB.Location = New System.Drawing.Point(154, 31)
        Me.AltCamCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.AltCamCB.Name = "AltCamCB"
        Me.AltCamCB.Offset = 1
        Me.AltCamCB.Size = New System.Drawing.Size(157, 19)
        Me.AltCamCB.TabIndex = 3
        Me.AltCamCB.Text = "Alternate Camera Angles"
        '
        'NewGameCB
        '
        Me.NewGameCB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.NewGameCB.FormattingEnabled = true
        Me.NewGameCB.Items.AddRange(New Object() {"00_03_Tutorial_Junktown.map", "00_04_Tutorial_Vault.map", "04_0202_Spelunking.map", "98_Canyon_Random_01.map", "98_Canyon_Random_02.map", "Default_StartMap.map", "Mainmenu.map", "zz_TestMapsScottE_Test1.map", "zz_TestMapsScottE_Test2.map", "zz_TestMapsScottE_Test4.map", "zz_TestMapsTest_City_Building01.map", "zz_TestMapsTest_City_Building02.map", "zz_TestMapsTest_City_Building03.map", "zz_TestMapsTest_City_Building04.map", "zz_TestMapsTest_City_Fences.map", "zz_TestMapsTest_Junktown_Shacks.map", "zz_TestMapsaarontemp2.map"})
        Me.NewGameCB.Location = New System.Drawing.Point(7, 71)
        Me.NewGameCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.NewGameCB.Name = "NewGameCB"
        Me.NewGameCB.Size = New System.Drawing.Size(241, 24)
        Me.NewGameCB.TabIndex = 5
        '
        'NewGameL
        '
        Me.NewGameL.AutoSize = true
        Me.NewGameL.ForeColor = System.Drawing.Color.RebeccaPurple
        Me.NewGameL.Location = New System.Drawing.Point(7, 53)
        Me.NewGameL.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.NewGameL.Name = "NewGameL"
        Me.NewGameL.Size = New System.Drawing.Size(58, 15)
        Me.NewGameL.TabIndex = 0
        Me.NewGameL.Text = "Start Map"
        '
        'MainMenuCB
        '
        Me.MainMenuCB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.MainMenuCB.FormattingEnabled = true
        Me.MainMenuCB.Items.AddRange(New Object() {"Default", "Aaron Map 2", "Building 1", "Building 2", "Building 3", "Building 4", "Canyon 1", "Canyon 2", "Cave", "Fences", "Scott E Map 1", "Scott E Map 2", "Scott E Map 4", "Shacks", "Start Map", "Tutorial", "Vault"})
        Me.MainMenuCB.Location = New System.Drawing.Point(7, 116)
        Me.MainMenuCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MainMenuCB.Name = "MainMenuCB"
        Me.MainMenuCB.Size = New System.Drawing.Size(358, 24)
        Me.MainMenuCB.TabIndex = 6
        '
        'MainMenuL
        '
        Me.MainMenuL.AutoSize = true
        Me.MainMenuL.Location = New System.Drawing.Point(7, 98)
        Me.MainMenuL.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.MainMenuL.Name = "MainMenuL"
        Me.MainMenuL.Size = New System.Drawing.Size(68, 15)
        Me.MainMenuL.TabIndex = 0
        Me.MainMenuL.Text = "Main Menu"
        '
        'ToolTip1
        '
        Me.ToolTip1.OwnerDraw = true
        '
        'SSFCB
        '
        Me.SSFCB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.SSFCB.FormattingEnabled = true
        Me.SSFCB.Location = New System.Drawing.Point(188, 108)
        Me.SSFCB.Margin = New System.Windows.Forms.Padding(2)
        Me.SSFCB.Name = "SSFCB"
        Me.SSFCB.Size = New System.Drawing.Size(177, 24)
        Me.SSFCB.TabIndex = 28
        Me.ToolTip1.SetToolTip(Me.SSFCB, "Increases visual quality by"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"rendering the scene above"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"your display resolution")
        '
        'MipmapCB
        '
        Me.MipmapCB.AutoSize = true
        Me.MipmapCB.Location = New System.Drawing.Point(94, 137)
        Me.MipmapCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MipmapCB.Name = "MipmapCB"
        Me.MipmapCB.Offset = 1
        Me.MipmapCB.Size = New System.Drawing.Size(95, 19)
        Me.MipmapCB.TabIndex = 30
        Me.MipmapCB.Text = "Mipmapping"
        Me.ToolTip1.SetToolTip(Me.MipmapCB, "Disabling reduces texture blur at distance")
        '
        'PhongCB
        '
        Me.PhongCB.AutoSize = true
        Me.PhongCB.Location = New System.Drawing.Point(195, 137)
        Me.PhongCB.Margin = New System.Windows.Forms.Padding(2)
        Me.PhongCB.Name = "PhongCB"
        Me.PhongCB.Offset = 1
        Me.PhongCB.Size = New System.Drawing.Size(107, 19)
        Me.PhongCB.TabIndex = 31
        Me.PhongCB.Text = "Phong Shading"
        Me.ToolTip1.SetToolTip(Me.PhongCB, "Alternative shading,"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Slightly improves visuals")
        '
        'APICB
        '
        Me.APICB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.APICB.FormattingEnabled = true
        Me.APICB.Items.AddRange(New Object() {"DirectX 11", "Vulkan"})
        Me.APICB.Location = New System.Drawing.Point(7, 21)
        Me.APICB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.APICB.Name = "APICB"
        Me.APICB.Size = New System.Drawing.Size(358, 24)
        Me.APICB.TabIndex = 24
        Me.ToolTip1.SetToolTip(Me.APICB, "Both modes use dgVoodoo2"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"dege.fw.hu"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Vulkan uses DXVK (zlib/libpng)"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"github.com/"& _ 
        "doitsujin/dxvk")
        '
        'CloseB
        '
        Me.CloseB.BorderColour = System.Drawing.Color.Empty
        Me.CloseB.CustomColour = false
        Me.CloseB.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CloseB.FlatBottom = false
        Me.CloseB.FlatTop = false
        Me.CloseB.Location = New System.Drawing.Point(262, 346)
        Me.CloseB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CloseB.Name = "CloseB"
        Me.CloseB.Padding = New System.Windows.Forms.Padding(5)
        Me.CloseB.Size = New System.Drawing.Size(61, 25)
        Me.CloseB.TabIndex = 9
        Me.CloseB.Text = "Close"
        '
        'DarkTabControl1
        '
        Me.DarkTabControl1.AllowDrop = true
        Me.DarkTabControl1.Controls.Add(Me.TabPage1)
        Me.DarkTabControl1.Controls.Add(Me.TabPage2)
        Me.DarkTabControl1.Controls.Add(Me.TabPage3)
        Me.DarkTabControl1.DisableClose = true
        Me.DarkTabControl1.DisableDragging = true
        Me.DarkTabControl1.Font = New System.Drawing.Font("Segoe UI", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.DarkTabControl1.Location = New System.Drawing.Point(12, 12)
        Me.DarkTabControl1.Name = "DarkTabControl1"
        Me.DarkTabControl1.Padding = New System.Drawing.Point(14, 4)
        Me.DarkTabControl1.SelectedIndex = 0
        Me.DarkTabControl1.Size = New System.Drawing.Size(380, 328)
        Me.DarkTabControl1.TabIndex = 11
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.Color.FromArgb(CType(CType(26,Byte),Integer), CType(CType(26,Byte),Integer), CType(CType(28,Byte),Integer))
        Me.TabPage1.Controls.Add(Me.DarkNumericUpDown1)
        Me.TabPage1.Controls.Add(Me.CameraCB)
        Me.TabPage1.Controls.Add(Me.NewGameCB)
        Me.TabPage1.Controls.Add(Me.NewGameL)
        Me.TabPage1.Controls.Add(Me.IntrosCB)
        Me.TabPage1.Controls.Add(Me.MainMenuCB)
        Me.TabPage1.Controls.Add(Me.MainMenuL)
        Me.TabPage1.Controls.Add(Me.AltCamCB)
        Me.TabPage1.Location = New System.Drawing.Point(4, 27)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(372, 297)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "General"
        '
        'DarkNumericUpDown1
        '
        Me.DarkNumericUpDown1.Location = New System.Drawing.Point(256, 72)
        Me.DarkNumericUpDown1.Name = "DarkNumericUpDown1"
        Me.DarkNumericUpDown1.Size = New System.Drawing.Size(110, 23)
        Me.DarkNumericUpDown1.TabIndex = 7
        '
        'TabPage2
        '
        Me.TabPage2.BackColor = System.Drawing.Color.FromArgb(CType(CType(26,Byte),Integer), CType(CType(26,Byte),Integer), CType(CType(28,Byte),Integer))
        Me.TabPage2.Controls.Add(Me.SSFL)
        Me.TabPage2.Controls.Add(Me.SSFCB)
        Me.TabPage2.Controls.Add(Me.ResolutionCB)
        Me.TabPage2.Controls.Add(Me.ResolutionL)
        Me.TabPage2.Controls.Add(Me.MipmapCB)
        Me.TabPage2.Controls.Add(Me.PhongCB)
        Me.TabPage2.Controls.Add(Me.TextureL)
        Me.TabPage2.Controls.Add(Me.TextureCB)
        Me.TabPage2.Controls.Add(Me.AAL)
        Me.TabPage2.Controls.Add(Me.AACB)
        Me.TabPage2.Controls.Add(Me.APICB)
        Me.TabPage2.Controls.Add(Me.APIL)
        Me.TabPage2.Controls.Add(Me.FullscreenCB)
        Me.TabPage2.Location = New System.Drawing.Point(4, 27)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(372, 297)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Graphics"
        '
        'SSFL
        '
        Me.SSFL.AutoSize = true
        Me.SSFL.Location = New System.Drawing.Point(188, 91)
        Me.SSFL.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.SSFL.Name = "SSFL"
        Me.SSFL.Size = New System.Drawing.Size(103, 15)
        Me.SSFL.TabIndex = 36
        Me.SSFL.Text = "Render Resolution"
        '
        'ResolutionCB
        '
        Me.ResolutionCB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.ResolutionCB.FormattingEnabled = true
        Me.ResolutionCB.Location = New System.Drawing.Point(7, 108)
        Me.ResolutionCB.Margin = New System.Windows.Forms.Padding(2)
        Me.ResolutionCB.Name = "ResolutionCB"
        Me.ResolutionCB.Size = New System.Drawing.Size(177, 24)
        Me.ResolutionCB.TabIndex = 27
        '
        'ResolutionL
        '
        Me.ResolutionL.AutoSize = true
        Me.ResolutionL.Location = New System.Drawing.Point(7, 91)
        Me.ResolutionL.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ResolutionL.Name = "ResolutionL"
        Me.ResolutionL.Size = New System.Drawing.Size(79, 15)
        Me.ResolutionL.TabIndex = 35
        Me.ResolutionL.Text = "Display Mode"
        '
        'TextureL
        '
        Me.TextureL.AutoSize = true
        Me.TextureL.Location = New System.Drawing.Point(188, 48)
        Me.TextureL.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.TextureL.Name = "TextureL"
        Me.TextureL.Size = New System.Drawing.Size(91, 15)
        Me.TextureL.TabIndex = 34
        Me.TextureL.Text = "Texture Filtering"
        '
        'TextureCB
        '
        Me.TextureCB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.TextureCB.FormattingEnabled = true
        Me.TextureCB.Items.AddRange(New Object() {"Bilinear (Default)", "Point Sampled", "Linear", "Anisotropic 2x", "Anisotropic 4x", "Anisotropic 8x", "Anisotropic 16x"})
        Me.TextureCB.Location = New System.Drawing.Point(188, 65)
        Me.TextureCB.Margin = New System.Windows.Forms.Padding(2)
        Me.TextureCB.Name = "TextureCB"
        Me.TextureCB.Size = New System.Drawing.Size(177, 24)
        Me.TextureCB.TabIndex = 26
        '
        'AAL
        '
        Me.AAL.AutoSize = true
        Me.AAL.Location = New System.Drawing.Point(7, 48)
        Me.AAL.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.AAL.Name = "AAL"
        Me.AAL.Size = New System.Drawing.Size(74, 15)
        Me.AAL.TabIndex = 33
        Me.AAL.Text = "Anti Aliasing"
        '
        'AACB
        '
        Me.AACB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.AACB.FormattingEnabled = true
        Me.AACB.Items.AddRange(New Object() {"Off", "2x MSAA", "4x MSAA", "8x MSAA"})
        Me.AACB.Location = New System.Drawing.Point(7, 65)
        Me.AACB.Margin = New System.Windows.Forms.Padding(2)
        Me.AACB.Name = "AACB"
        Me.AACB.Size = New System.Drawing.Size(177, 24)
        Me.AACB.TabIndex = 25
        '
        'APIL
        '
        Me.APIL.AutoSize = true
        Me.APIL.Location = New System.Drawing.Point(7, 3)
        Me.APIL.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.APIL.Name = "APIL"
        Me.APIL.Size = New System.Drawing.Size(82, 15)
        Me.APIL.TabIndex = 32
        Me.APIL.Text = "Rendering API"
        '
        'FullscreenCB
        '
        Me.FullscreenCB.AutoSize = true
        Me.FullscreenCB.Location = New System.Drawing.Point(7, 137)
        Me.FullscreenCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.FullscreenCB.Name = "FullscreenCB"
        Me.FullscreenCB.Offset = 1
        Me.FullscreenCB.Size = New System.Drawing.Size(79, 19)
        Me.FullscreenCB.TabIndex = 29
        Me.FullscreenCB.Text = "Fullscreen"
        '
        'TabPage3
        '
        Me.TabPage3.BackColor = System.Drawing.Color.FromArgb(CType(CType(26,Byte),Integer), CType(CType(26,Byte),Integer), CType(CType(28,Byte),Integer))
        Me.TabPage3.Controls.Add(Me.DarkScrollBar1)
        Me.TabPage3.Controls.Add(Me.DarkButton2)
        Me.TabPage3.Controls.Add(Me.DarkLabel1)
        Me.TabPage3.Controls.Add(Me.DataGridView1)
        Me.TabPage3.Location = New System.Drawing.Point(4, 27)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(372, 297)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Keybinds"
        '
        'DarkScrollBar1
        '
        Me.DarkScrollBar1.Location = New System.Drawing.Point(357, 1)
        Me.DarkScrollBar1.Maximum = 500
        Me.DarkScrollBar1.Name = "DarkScrollBar1"
        Me.DarkScrollBar1.Size = New System.Drawing.Size(22, 259)
        Me.DarkScrollBar1.TabIndex = 10
        Me.DarkScrollBar1.Text = "DarkScrollBar1"
        '
        'DarkButton2
        '
        Me.DarkButton2.BorderColour = System.Drawing.Color.Empty
        Me.DarkButton2.CustomColour = false
        Me.DarkButton2.FlatBottom = false
        Me.DarkButton2.FlatTop = false
        Me.DarkButton2.Location = New System.Drawing.Point(305, 266)
        Me.DarkButton2.Name = "DarkButton2"
        Me.DarkButton2.Padding = New System.Windows.Forms.Padding(5)
        Me.DarkButton2.Size = New System.Drawing.Size(61, 24)
        Me.DarkButton2.TabIndex = 9
        Me.DarkButton2.Text = "Reload"
        '
        'DarkLabel1
        '
        Me.DarkLabel1.AutoSize = true
        Me.DarkLabel1.Location = New System.Drawing.Point(6, 271)
        Me.DarkLabel1.Name = "DarkLabel1"
        Me.DarkLabel1.Size = New System.Drawing.Size(145, 15)
        Me.DarkLabel1.TabIndex = 8
        Me.DarkLabel1.Text = "Press any Key/Key Combo"
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToResizeColumns = false
        Me.DataGridView1.AllowUserToResizeRows = false
        Me.DataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(CType(CType(32,Byte),Integer), CType(CType(32,Byte),Integer), CType(CType(32,Byte),Integer))
        Me.DataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Segoe UI", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Segoe UI", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridView1.DefaultCellStyle = DataGridViewCellStyle2
        Me.DataGridView1.GridColor = System.Drawing.SystemColors.ButtonFace
        Me.DataGridView1.Location = New System.Drawing.Point(1, 1)
        Me.DataGridView1.MultiSelect = false
        Me.DataGridView1.Name = "DataGridView1"
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Segoe UI", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridView1.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        Me.DataGridView1.RowHeadersVisible = false
        Me.DataGridView1.RowTemplate.Height = 25
        Me.DataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(374, 259)
        Me.DataGridView1.TabIndex = 3
        '
        'Timer1
        '
        Me.Timer1.Interval = 15
        '
        'Options
        '
        Me.AcceptButton = Me.ApplyB
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(405, 383)
        Me.Controls.Add(Me.DarkTabControl1)
        Me.Controls.Add(Me.CloseB)
        Me.Controls.Add(Me.ApplyB)
        Me.CornerStyle = AltUI.Forms.DarkForm.CornerPreference.[Default]
        Me.CustomBorder = true
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MaximizeBox = false
        Me.MinimizeBox = false
        Me.Name = "Options"
        Me.ShowInTaskbar = false
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Options"
        Me.TransparencyKey = System.Drawing.Color.FromArgb(CType(CType(31,Byte),Integer), CType(CType(31,Byte),Integer), CType(CType(32,Byte),Integer))
        Me.DarkTabControl1.ResumeLayout(false)
        Me.TabPage1.ResumeLayout(false)
        Me.TabPage1.PerformLayout
        CType(Me.DarkNumericUpDown1,System.ComponentModel.ISupportInitialize).EndInit
        Me.TabPage2.ResumeLayout(false)
        Me.TabPage2.PerformLayout
        Me.TabPage3.ResumeLayout(false)
        Me.TabPage3.PerformLayout
        CType(Me.DataGridView1,System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents ToolTip1 As DarkToolTip
    Friend WithEvents ApplyB As DarkButton
    Friend WithEvents IntrosCB As DarkCheckBox
    Friend WithEvents MainMenuCB As DarkComboBox
    Friend WithEvents MainMenuL As DarkLabel
    Friend WithEvents AltCamCB As DarkCheckBox
    Friend WithEvents NewGameCB As DarkComboBox
    Friend WithEvents NewGameL As DarkLabel
    Friend WithEvents CloseB As DarkButton
    Friend WithEvents CameraCB As DarkCheckBox
    Friend WithEvents DarkTabControl1 As DarkTabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents TabPage3 As TabPage
    Friend WithEvents SSFL As DarkLabel
    Friend WithEvents SSFCB As DarkComboBox
    Friend WithEvents ResolutionCB As DarkComboBox
    Friend WithEvents ResolutionL As DarkLabel
    Friend WithEvents MipmapCB As DarkCheckBox
    Friend WithEvents PhongCB As DarkCheckBox
    Friend WithEvents TextureL As DarkLabel
    Friend WithEvents TextureCB As DarkComboBox
    Friend WithEvents AAL As DarkLabel
    Friend WithEvents AACB As DarkComboBox
    Friend WithEvents APICB As DarkComboBox
    Friend WithEvents APIL As DarkLabel
    Friend WithEvents FullscreenCB As DarkCheckBox
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents DarkButton2 As DarkButton
    Friend WithEvents DarkLabel1 As DarkLabel
    Friend WithEvents DarkScrollBar1 As DarkScrollBar
    Friend WithEvents Timer1 As Timer
    Friend WithEvents DarkNumericUpDown1 As DarkNumericUpDown
End Class
