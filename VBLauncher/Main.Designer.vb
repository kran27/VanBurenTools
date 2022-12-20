Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()>
Partial Class Main
    Inherits AltUI.Forms.DarkForm

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.LaunchB = New System.Windows.Forms.PictureBox()
        Me.OptionsB = New System.Windows.Forms.PictureBox()
        Me.ExitB = New System.Windows.Forms.PictureBox()
        Me.Logo = New System.Windows.Forms.PictureBox()
        Me.Background = New System.Windows.Forms.PictureBox()
        Me.KeybindB = New System.Windows.Forms.PictureBox()
        Me.EditorB = New System.Windows.Forms.PictureBox()
        CType(Me.LaunchB,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.OptionsB,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.ExitB,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Logo,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Background,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.KeybindB,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.EditorB,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SuspendLayout
        '
        'LaunchB
        '
        Me.LaunchB.BackColor = System.Drawing.Color.Transparent
        Me.LaunchB.BackgroundImage = CType(resources.GetObject("LaunchB.BackgroundImage"),System.Drawing.Image)
        Me.LaunchB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.LaunchB.Location = New System.Drawing.Point(13, 13)
        Me.LaunchB.Margin = New System.Windows.Forms.Padding(4)
        Me.LaunchB.Name = "LaunchB"
        Me.LaunchB.Size = New System.Drawing.Size(192, 29)
        Me.LaunchB.TabIndex = 0
        Me.LaunchB.TabStop = false
        '
        'OptionsB
        '
        Me.OptionsB.BackColor = System.Drawing.Color.Transparent
        Me.OptionsB.BackgroundImage = CType(resources.GetObject("OptionsB.BackgroundImage"),System.Drawing.Image)
        Me.OptionsB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.OptionsB.Location = New System.Drawing.Point(13, 50)
        Me.OptionsB.Margin = New System.Windows.Forms.Padding(4)
        Me.OptionsB.Name = "OptionsB"
        Me.OptionsB.Size = New System.Drawing.Size(192, 29)
        Me.OptionsB.TabIndex = 1
        Me.OptionsB.TabStop = false
        '
        'ExitB
        '
        Me.ExitB.BackColor = System.Drawing.Color.Transparent
        Me.ExitB.BackgroundImage = CType(resources.GetObject("ExitB.BackgroundImage"),System.Drawing.Image)
        Me.ExitB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ExitB.Location = New System.Drawing.Point(13, 124)
        Me.ExitB.Margin = New System.Windows.Forms.Padding(4)
        Me.ExitB.Name = "ExitB"
        Me.ExitB.Size = New System.Drawing.Size(192, 29)
        Me.ExitB.TabIndex = 2
        Me.ExitB.TabStop = false
        '
        'Logo
        '
        Me.Logo.BackColor = System.Drawing.Color.Transparent
        Me.Logo.BackgroundImage = CType(resources.GetObject("Logo.BackgroundImage"),System.Drawing.Image)
        Me.Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.Logo.Location = New System.Drawing.Point(478, 317)
        Me.Logo.Margin = New System.Windows.Forms.Padding(4)
        Me.Logo.Name = "Logo"
        Me.Logo.Size = New System.Drawing.Size(149, 150)
        Me.Logo.TabIndex = 3
        Me.Logo.TabStop = false
        '
        'Background
        '
        Me.Background.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.Background.Location = New System.Drawing.Point(0, 0)
        Me.Background.Name = "Background"
        Me.Background.Size = New System.Drawing.Size(640, 480)
        Me.Background.TabIndex = 4
        Me.Background.TabStop = false
        '
        'KeybindB
        '
        Me.KeybindB.BackColor = System.Drawing.Color.Transparent
        Me.KeybindB.BackgroundImage = CType(resources.GetObject("KeybindB.BackgroundImage"),System.Drawing.Image)
        Me.KeybindB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.KeybindB.Location = New System.Drawing.Point(13, 87)
        Me.KeybindB.Margin = New System.Windows.Forms.Padding(4)
        Me.KeybindB.Name = "KeybindB"
        Me.KeybindB.Size = New System.Drawing.Size(192, 29)
        Me.KeybindB.TabIndex = 5
        Me.KeybindB.TabStop = false
        '
        'EditorB
        '
        Me.EditorB.BackColor = System.Drawing.Color.Transparent
        Me.EditorB.BackgroundImage = CType(resources.GetObject("EditorB.BackgroundImage"),System.Drawing.Image)
        Me.EditorB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.EditorB.Location = New System.Drawing.Point(13, 124)
        Me.EditorB.Margin = New System.Windows.Forms.Padding(4)
        Me.EditorB.Name = "EditorB"
        Me.EditorB.Size = New System.Drawing.Size(192, 29)
        Me.EditorB.TabIndex = 6
        Me.EditorB.TabStop = false
        '
        'Main
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(640, 480)
        Me.Controls.Add(Me.KeybindB)
        Me.Controls.Add(Me.Logo)
        Me.Controls.Add(Me.OptionsB)
        Me.Controls.Add(Me.LaunchB)
        Me.Controls.Add(Me.ExitB)
        Me.Controls.Add(Me.EditorB)
        Me.Controls.Add(Me.Background)
        Me.CornerStyle = AltUI.Forms.DarkForm.CornerPreference.[Default]
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "Main"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Van Buren Launcher"
        Me.TransparencyKey = System.Drawing.Color.FromArgb(CType(CType(31,Byte),Integer), CType(CType(31,Byte),Integer), CType(CType(32,Byte),Integer))
        CType(Me.LaunchB,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.OptionsB,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.ExitB,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Logo,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Background,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.KeybindB,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.EditorB,System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)

End Sub

    Friend WithEvents LaunchB As PictureBox
    Friend WithEvents OptionsB As PictureBox
    Friend WithEvents ExitB As PictureBox
    Friend WithEvents Logo As PictureBox
    Friend WithEvents Background As PictureBox
    Friend WithEvents KeybindB As PictureBox
    Friend WithEvents EditorB As PictureBox
End Class
