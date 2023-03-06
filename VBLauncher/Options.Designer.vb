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
        Me.components = New Container()
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(Options))
        Me.ApplyB = New DarkButton()
        Me.IntrosCB = New DarkCheckBox()
        Me.TweaksGB = New DarkGroupBox()
        Me.CameraCB = New DarkCheckBox()
        Me.AltCamCB = New DarkCheckBox()
        Me.PreferencesGB = New DarkGroupBox()
        Me.KeybindB = New DarkButton()
        Me.VideoB = New DarkButton()
        Me.NewGameCB = New DarkComboBox()
        Me.NewGameL = New DarkLabel()
        Me.MainMenuCB = New DarkComboBox()
        Me.MainMenuL = New DarkLabel()
        Me.ToolTip1 = New ToolTip(Me.components)
        Me.CloseB = New DarkButton()
        Me.TweaksGB.SuspendLayout
        Me.PreferencesGB.SuspendLayout
        Me.SuspendLayout
        '
        'ApplyB
        '
        Me.ApplyB.BorderColour = Color.Empty
        Me.ApplyB.CustomColour = false
        Me.ApplyB.FlatBottom = false
        Me.ApplyB.FlatTop = false
        Me.ApplyB.Location = New Point(209, 260)
        Me.ApplyB.Margin = New Padding(4, 3, 4, 3)
        Me.ApplyB.Name = "ApplyB"
        Me.ApplyB.Padding = New Padding(5)
        Me.ApplyB.Size = New Size(61, 25)
        Me.ApplyB.TabIndex = 10
        Me.ApplyB.Text = "Apply"
        '
        'IntrosCB
        '
        Me.IntrosCB.AutoSize = true
        Me.IntrosCB.Location = New Point(8, 22)
        Me.IntrosCB.Margin = New Padding(4, 3, 4, 3)
        Me.IntrosCB.Name = "IntrosCB"
        Me.IntrosCB.Offset = 1
        Me.IntrosCB.Size = New Size(127, 19)
        Me.IntrosCB.TabIndex = 1
        Me.IntrosCB.Text = "Enable Intro Videos"
        '
        'TweaksGB
        '
        Me.TweaksGB.Controls.Add(Me.CameraCB)
        Me.TweaksGB.Controls.Add(Me.AltCamCB)
        Me.TweaksGB.Controls.Add(Me.IntrosCB)
        Me.TweaksGB.Location = New Point(13, 12)
        Me.TweaksGB.Margin = New Padding(4, 3, 4, 3)
        Me.TweaksGB.Name = "TweaksGB"
        Me.TweaksGB.Padding = New Padding(4, 3, 4, 3)
        Me.TweaksGB.Size = New Size(257, 90)
        Me.TweaksGB.TabIndex = 0
        Me.TweaksGB.TabStop = false
        Me.TweaksGB.Text = "Tweaks"
        '
        'CameraCB
        '
        Me.CameraCB.AutoSize = true
        Me.CameraCB.Location = New Point(8, 66)
        Me.CameraCB.Margin = New Padding(4, 3, 4, 3)
        Me.CameraCB.Name = "CameraCB"
        Me.CameraCB.Offset = 1
        Me.CameraCB.Size = New Size(139, 19)
        Me.CameraCB.TabIndex = 4
        Me.CameraCB.Text = "Uncap Camera Zoom"
        '
        'AltCamCB
        '
        Me.AltCamCB.AutoSize = true
        Me.AltCamCB.Location = New Point(8, 44)
        Me.AltCamCB.Margin = New Padding(4, 3, 4, 3)
        Me.AltCamCB.Name = "AltCamCB"
        Me.AltCamCB.Offset = 1
        Me.AltCamCB.Size = New Size(157, 19)
        Me.AltCamCB.TabIndex = 3
        Me.AltCamCB.Text = "Alternate Camera Angles"
        '
        'PreferencesGB
        '
        Me.PreferencesGB.Controls.Add(Me.KeybindB)
        Me.PreferencesGB.Controls.Add(Me.VideoB)
        Me.PreferencesGB.Controls.Add(Me.NewGameCB)
        Me.PreferencesGB.Controls.Add(Me.NewGameL)
        Me.PreferencesGB.Controls.Add(Me.MainMenuCB)
        Me.PreferencesGB.Controls.Add(Me.MainMenuL)
        Me.PreferencesGB.Location = New Point(13, 108)
        Me.PreferencesGB.Margin = New Padding(4, 3, 4, 3)
        Me.PreferencesGB.Name = "PreferencesGB"
        Me.PreferencesGB.Padding = New Padding(4, 3, 4, 3)
        Me.PreferencesGB.Size = New Size(257, 146)
        Me.PreferencesGB.TabIndex = 5
        Me.PreferencesGB.TabStop = false
        Me.PreferencesGB.Text = "Preferences"
        '
        'KeybindB
        '
        Me.KeybindB.BorderColour = Color.Empty
        Me.KeybindB.CustomColour = false
        Me.KeybindB.FlatBottom = false
        Me.KeybindB.FlatTop = false
        Me.KeybindB.Location = New Point(132, 112)
        Me.KeybindB.Name = "KeybindB"
        Me.KeybindB.Padding = New Padding(5)
        Me.KeybindB.Size = New Size(117, 24)
        Me.KeybindB.TabIndex = 7
        Me.KeybindB.Text = "Keybind Editor"
        '
        'VideoB
        '
        Me.VideoB.BorderColour = Color.Empty
        Me.VideoB.CustomColour = false
        Me.VideoB.FlatBottom = false
        Me.VideoB.FlatTop = false
        Me.VideoB.Location = New Point(8, 112)
        Me.VideoB.Margin = New Padding(4, 3, 4, 3)
        Me.VideoB.Name = "VideoB"
        Me.VideoB.Padding = New Padding(5)
        Me.VideoB.Size = New Size(117, 24)
        Me.VideoB.TabIndex = 8
        Me.VideoB.Text = "Video Options"
        '
        'NewGameCB
        '
        Me.NewGameCB.DrawMode = DrawMode.OwnerDrawVariable
        Me.NewGameCB.FormattingEnabled = true
        Me.NewGameCB.Items.AddRange(New Object() {"00_03_Tutorial_Junktown.map", "00_04_Tutorial_Vault.map", "04_0202_Spelunking.map", "98_Canyon_Random_01.map", "98_Canyon_Random_02.map", "Default_StartMap.map", "Mainmenu.map", "zz_TestMapsScottE_Test1.map", "zz_TestMapsScottE_Test2.map", "zz_TestMapsScottE_Test4.map", "zz_TestMapsTest_City_Building01.map", "zz_TestMapsTest_City_Building02.map", "zz_TestMapsTest_City_Building03.map", "zz_TestMapsTest_City_Building04.map", "zz_TestMapsTest_City_Fences.map", "zz_TestMapsTest_Junktown_Shacks.map", "zz_TestMapsaarontemp2.map"})
        Me.NewGameCB.Location = New Point(8, 37)
        Me.NewGameCB.Margin = New Padding(4, 3, 4, 3)
        Me.NewGameCB.Name = "NewGameCB"
        Me.NewGameCB.Size = New Size(241, 24)
        Me.NewGameCB.TabIndex = 5
        '
        'NewGameL
        '
        Me.NewGameL.AutoSize = true
        Me.NewGameL.ForeColor = Color.RebeccaPurple
        Me.NewGameL.Location = New Point(8, 19)
        Me.NewGameL.Margin = New Padding(4, 0, 4, 0)
        Me.NewGameL.Name = "NewGameL"
        Me.NewGameL.Size = New Size(58, 15)
        Me.NewGameL.TabIndex = 0
        Me.NewGameL.Text = "Start Map"
        '
        'MainMenuCB
        '
        Me.MainMenuCB.DrawMode = DrawMode.OwnerDrawVariable
        Me.MainMenuCB.FormattingEnabled = true
        Me.MainMenuCB.Items.AddRange(New Object() {"Default", "Aaron Map 2", "Building 1", "Building 2", "Building 3", "Building 4", "Canyon 1", "Canyon 2", "Cave", "Fences", "Scott E Map 1", "Scott E Map 2", "Scott E Map 4", "Shacks", "Start Map", "Tutorial", "Vault"})
        Me.MainMenuCB.Location = New Point(8, 82)
        Me.MainMenuCB.Margin = New Padding(4, 3, 4, 3)
        Me.MainMenuCB.Name = "MainMenuCB"
        Me.MainMenuCB.Size = New Size(241, 24)
        Me.MainMenuCB.TabIndex = 6
        '
        'MainMenuL
        '
        Me.MainMenuL.AutoSize = true
        Me.MainMenuL.Location = New Point(8, 64)
        Me.MainMenuL.Margin = New Padding(4, 0, 4, 0)
        Me.MainMenuL.Name = "MainMenuL"
        Me.MainMenuL.Size = New Size(68, 15)
        Me.MainMenuL.TabIndex = 0
        Me.MainMenuL.Text = "Main Menu"
        '
        'CloseB
        '
        Me.CloseB.BorderColour = Color.Empty
        Me.CloseB.CustomColour = false
        Me.CloseB.DialogResult = DialogResult.Cancel
        Me.CloseB.FlatBottom = false
        Me.CloseB.FlatTop = false
        Me.CloseB.Location = New Point(140, 260)
        Me.CloseB.Margin = New Padding(4, 3, 4, 3)
        Me.CloseB.Name = "CloseB"
        Me.CloseB.Padding = New Padding(5)
        Me.CloseB.Size = New Size(61, 25)
        Me.CloseB.TabIndex = 9
        Me.CloseB.Text = "Close"
        '
        'Options
        '
        Me.AcceptButton = Me.ApplyB
        Me.AutoScaleMode = AutoScaleMode.None
        Me.ClientSize = New Size(283, 297)
        Me.Controls.Add(Me.CloseB)
        Me.Controls.Add(Me.PreferencesGB)
        Me.Controls.Add(Me.TweaksGB)
        Me.Controls.Add(Me.ApplyB)
        Me.CornerStyle = CornerPreference.[Default]
        Me.CustomBorder = true
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"),Icon)
        Me.Margin = New Padding(4, 3, 4, 3)
        Me.MaximizeBox = false
        Me.MinimizeBox = false
        Me.Name = "Options"
        Me.ShowInTaskbar = false
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "Options"
        Me.TransparencyKey = Color.FromArgb(CType(CType(31,Byte),Integer), CType(CType(31,Byte),Integer), CType(CType(32,Byte),Integer))
        Me.TweaksGB.ResumeLayout(false)
        Me.TweaksGB.PerformLayout
        Me.PreferencesGB.ResumeLayout(false)
        Me.PreferencesGB.PerformLayout
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents ApplyB As DarkButton
    Friend WithEvents IntrosCB As DarkCheckBox
    Friend WithEvents TweaksGB As DarkGroupBox
    Friend WithEvents PreferencesGB As DarkGroupBox
    Friend WithEvents MainMenuCB As DarkComboBox
    Friend WithEvents MainMenuL As DarkLabel
    Friend WithEvents AltCamCB As DarkCheckBox
    Friend WithEvents NewGameCB As DarkComboBox
    Friend WithEvents NewGameL As DarkLabel
    Friend WithEvents VideoB As DarkButton
    Friend WithEvents CloseB As DarkButton
    Friend WithEvents CameraCB As DarkCheckBox
    Friend WithEvents KeybindB As DarkButton
End Class
