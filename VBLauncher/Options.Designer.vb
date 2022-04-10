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
        Me.ApplyB = New AltUI.Controls.DarkButton()
        Me.IntrosCB = New AltUI.Controls.DarkCheckBox()
        Me.TweaksGB = New AltUI.Controls.DarkGroupBox()
        Me.AltCamCB = New AltUI.Controls.DarkCheckBox()
        Me.ButtonsCB = New AltUI.Controls.DarkCheckBox()
        Me.PreferencesGB = New AltUI.Controls.DarkGroupBox()
        Me.NewGameCB = New AltUI.Controls.DarkComboBox()
        Me.NewGameL = New AltUI.Controls.DarkLabel()
        Me.MainMenuCB = New AltUI.Controls.DarkComboBox()
        Me.HelmetPL = New AltUI.Controls.DarkLabel()
        Me.HelmetCB = New AltUI.Controls.DarkComboBox()
        Me.HelmetL = New AltUI.Controls.DarkLabel()
        Me.HelmetPI = New System.Windows.Forms.PictureBox()
        Me.MainMenuL = New AltUI.Controls.DarkLabel()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.VideoB = New AltUI.Controls.DarkButton()
        Me.CloseB = New AltUI.Controls.DarkButton()
        Me.CameraCB = New AltUI.Controls.DarkCheckBox()
        Me.TweaksGB.SuspendLayout()
        Me.PreferencesGB.SuspendLayout()
        CType(Me.HelmetPI, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ApplyB
        '
        Me.ApplyB.Location = New System.Drawing.Point(209, 279)
        Me.ApplyB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.ApplyB.Name = "ApplyB"
        Me.ApplyB.Padding = New System.Windows.Forms.Padding(5)
        Me.ApplyB.Size = New System.Drawing.Size(61, 27)
        Me.ApplyB.TabIndex = 10
        Me.ApplyB.Text = "Apply"
        '
        'IntrosCB
        '
        Me.IntrosCB.AutoSize = True
        Me.IntrosCB.Location = New System.Drawing.Point(9, 19)
        Me.IntrosCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.IntrosCB.Name = "IntrosCB"
        Me.IntrosCB.Size = New System.Drawing.Size(127, 20)
        Me.IntrosCB.TabIndex = 1
        Me.IntrosCB.Text = "Enable Intro Videos"
        '
        'TweaksGB
        '
        Me.TweaksGB.Controls.Add(Me.CameraCB)
        Me.TweaksGB.Controls.Add(Me.AltCamCB)
        Me.TweaksGB.Controls.Add(Me.ButtonsCB)
        Me.TweaksGB.Controls.Add(Me.IntrosCB)
        Me.TweaksGB.Location = New System.Drawing.Point(13, 12)
        Me.TweaksGB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TweaksGB.Name = "TweaksGB"
        Me.TweaksGB.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TweaksGB.Size = New System.Drawing.Size(257, 113)
        Me.TweaksGB.TabIndex = 0
        Me.TweaksGB.TabStop = False
        Me.TweaksGB.Text = "Tweaks"
        '
        'AltCamCB
        '
        Me.AltCamCB.AutoSize = True
        Me.AltCamCB.Location = New System.Drawing.Point(9, 64)
        Me.AltCamCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.AltCamCB.Name = "AltCamCB"
        Me.AltCamCB.Size = New System.Drawing.Size(156, 20)
        Me.AltCamCB.TabIndex = 3
        Me.AltCamCB.Text = "Alternate Camera Angles"
        '
        'ButtonsCB
        '
        Me.ButtonsCB.AutoSize = True
        Me.ButtonsCB.Location = New System.Drawing.Point(9, 42)
        Me.ButtonsCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.ButtonsCB.Name = "ButtonsCB"
        Me.ButtonsCB.Size = New System.Drawing.Size(187, 20)
        Me.ButtonsCB.TabIndex = 2
        Me.ButtonsCB.Text = "Remove Useless Menu Buttons"
        '
        'PreferencesGB
        '
        Me.PreferencesGB.Controls.Add(Me.NewGameCB)
        Me.PreferencesGB.Controls.Add(Me.NewGameL)
        Me.PreferencesGB.Controls.Add(Me.MainMenuCB)
        Me.PreferencesGB.Controls.Add(Me.HelmetPL)
        Me.PreferencesGB.Controls.Add(Me.HelmetCB)
        Me.PreferencesGB.Controls.Add(Me.HelmetL)
        Me.PreferencesGB.Controls.Add(Me.HelmetPI)
        Me.PreferencesGB.Controls.Add(Me.MainMenuL)
        Me.PreferencesGB.Location = New System.Drawing.Point(13, 131)
        Me.PreferencesGB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PreferencesGB.Name = "PreferencesGB"
        Me.PreferencesGB.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PreferencesGB.Size = New System.Drawing.Size(257, 142)
        Me.PreferencesGB.TabIndex = 5
        Me.PreferencesGB.TabStop = False
        Me.PreferencesGB.Text = "Preferences"
        '
        'NewGameCB
        '
        Me.NewGameCB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.NewGameCB.FormattingEnabled = True
        Me.NewGameCB.Items.AddRange(New Object() {"00_03_Tutorial_Junktown.map", "00_04_Tutorial_Vault.map", "04_0202_Spelunking.map", "98_Canyon_Random_01.map", "98_Canyon_Random_02.map", "Default_StartMap.map", "Mainmenu.map", "zz_TestMapsScottE_Test1.map", "zz_TestMapsScottE_Test2.map", "zz_TestMapsScottE_Test4.map", "zz_TestMapsTest_City_Building01.map", "zz_TestMapsTest_City_Building02.map", "zz_TestMapsTest_City_Building03.map", "zz_TestMapsTest_City_Building04.map", "zz_TestMapsTest_City_Fences.map", "zz_TestMapsTest_Junktown_Shacks.map", "zz_TestMapsaarontemp2.map"})
        Me.NewGameCB.Location = New System.Drawing.Point(99, 18)
        Me.NewGameCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.NewGameCB.Name = "NewGameCB"
        Me.NewGameCB.Size = New System.Drawing.Size(149, 24)
        Me.NewGameCB.TabIndex = 5
        '
        'NewGameL
        '
        Me.NewGameL.AutoSize = True
        Me.NewGameL.Location = New System.Drawing.Point(7, 22)
        Me.NewGameL.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.NewGameL.Name = "NewGameL"
        Me.NewGameL.Size = New System.Drawing.Size(91, 16)
        Me.NewGameL.TabIndex = 0
        Me.NewGameL.Text = "New Game Map"
        '
        'MainMenuCB
        '
        Me.MainMenuCB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.MainMenuCB.FormattingEnabled = True
        Me.MainMenuCB.Items.AddRange(New Object() {"Default", "Aaron Map 2", "Building 1", "Building 2", "Building 3", "Building 4", "Canyon 1", "Canyon 2", "Cave", "Fences", "Scott E Map 1", "Scott E Map 2", "Scott E Map 4", "Shacks", "Start Map", "Tutorial", "Vault"})
        Me.MainMenuCB.Location = New System.Drawing.Point(103, 48)
        Me.MainMenuCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MainMenuCB.Name = "MainMenuCB"
        Me.MainMenuCB.Size = New System.Drawing.Size(145, 24)
        Me.MainMenuCB.TabIndex = 6
        '
        'HelmetPL
        '
        Me.HelmetPL.AutoSize = True
        Me.HelmetPL.Location = New System.Drawing.Point(126, 107)
        Me.HelmetPL.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.HelmetPL.Name = "HelmetPL"
        Me.HelmetPL.Size = New System.Drawing.Size(50, 16)
        Me.HelmetPL.TabIndex = 6
        Me.HelmetPL.Text = "Preview:"
        '
        'HelmetCB
        '
        Me.HelmetCB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.HelmetCB.FormattingEnabled = True
        Me.HelmetCB.Items.AddRange(New Object() {"Default", "8-Ball", "American", "Black", "Blue", "Eye", "Flames", "Full Skull", "Green", "Grey", "Police", "Red", "Shot Smiley", "Skull", "Yellow"})
        Me.HelmetCB.Location = New System.Drawing.Point(54, 78)
        Me.HelmetCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.HelmetCB.Name = "HelmetCB"
        Me.HelmetCB.Size = New System.Drawing.Size(123, 24)
        Me.HelmetCB.TabIndex = 7
        '
        'HelmetL
        '
        Me.HelmetL.AutoSize = True
        Me.HelmetL.Location = New System.Drawing.Point(7, 83)
        Me.HelmetL.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.HelmetL.Name = "HelmetL"
        Me.HelmetL.Size = New System.Drawing.Size(44, 16)
        Me.HelmetL.TabIndex = 4
        Me.HelmetL.Text = "Helmet"
        '
        'HelmetPI
        '
        Me.HelmetPI.Location = New System.Drawing.Point(180, 78)
        Me.HelmetPI.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.HelmetPI.Name = "HelmetPI"
        Me.HelmetPI.Size = New System.Drawing.Size(75, 62)
        Me.HelmetPI.TabIndex = 3
        Me.HelmetPI.TabStop = False
        '
        'MainMenuL
        '
        Me.MainMenuL.AutoSize = True
        Me.MainMenuL.Location = New System.Drawing.Point(7, 53)
        Me.MainMenuL.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.MainMenuL.Name = "MainMenuL"
        Me.MainMenuL.Size = New System.Drawing.Size(95, 16)
        Me.MainMenuL.TabIndex = 0
        Me.MainMenuL.Text = "Main Menu Map"
        '
        'VideoB
        '
        Me.VideoB.Location = New System.Drawing.Point(13, 279)
        Me.VideoB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.VideoB.Name = "VideoB"
        Me.VideoB.Padding = New System.Windows.Forms.Padding(5)
        Me.VideoB.Size = New System.Drawing.Size(97, 27)
        Me.VideoB.TabIndex = 8
        Me.VideoB.Text = "Video Options"
        '
        'CloseB
        '
        Me.CloseB.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CloseB.Location = New System.Drawing.Point(140, 279)
        Me.CloseB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CloseB.Name = "CloseB"
        Me.CloseB.Padding = New System.Windows.Forms.Padding(5)
        Me.CloseB.Size = New System.Drawing.Size(61, 27)
        Me.CloseB.TabIndex = 9
        Me.CloseB.Text = "Close"
        '
        'CameraCB
        '
        Me.CameraCB.AutoSize = True
        Me.CameraCB.Location = New System.Drawing.Point(9, 86)
        Me.CameraCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CameraCB.Name = "CameraCB"
        Me.CameraCB.Size = New System.Drawing.Size(137, 20)
        Me.CameraCB.TabIndex = 4
        Me.CameraCB.Text = "Uncap Camera Zoom"
        '
        'Options
        '
        Me.AcceptButton = Me.ApplyB
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(283, 319)
        Me.Controls.Add(Me.CloseB)
        Me.Controls.Add(Me.VideoB)
        Me.Controls.Add(Me.PreferencesGB)
        Me.Controls.Add(Me.TweaksGB)
        Me.Controls.Add(Me.ApplyB)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Options"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Options"
        Me.TweaksGB.ResumeLayout(False)
        Me.TweaksGB.PerformLayout()
        Me.PreferencesGB.ResumeLayout(False)
        Me.PreferencesGB.PerformLayout()
        CType(Me.HelmetPI, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents HelmetPI As PictureBox
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents ApplyB As DarkButton
    Friend WithEvents IntrosCB As DarkCheckBox
    Friend WithEvents TweaksGB As DarkGroupBox
    Friend WithEvents PreferencesGB As DarkGroupBox
    Friend WithEvents MainMenuCB As DarkComboBox
    Friend WithEvents MainMenuL As DarkLabel
    Friend WithEvents HelmetPL As DarkLabel
    Friend WithEvents HelmetCB As DarkComboBox
    Friend WithEvents HelmetL As DarkLabel
    Friend WithEvents ButtonsCB As DarkCheckBox
    Friend WithEvents AltCamCB As DarkCheckBox
    Friend WithEvents NewGameCB As DarkComboBox
    Friend WithEvents NewGameL As DarkLabel
    Friend WithEvents VideoB As DarkButton
    Friend WithEvents CloseB As DarkButton
    Friend WithEvents CameraCB As DarkCheckBox
End Class
