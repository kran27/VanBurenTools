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
        Me.LaunchB = New System.Windows.Forms.PictureBox()
        Me.OptionsB = New System.Windows.Forms.PictureBox()
        Me.ExitB = New System.Windows.Forms.PictureBox()
        Me.Logo = New System.Windows.Forms.PictureBox()
        Me.Background = New System.Windows.Forms.PictureBox()
        CType(Me.LaunchB, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.OptionsB, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ExitB, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Logo, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Background, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LaunchB
        '
        Me.LaunchB.BackColor = System.Drawing.Color.Transparent
        Me.LaunchB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.LaunchB.Location = New System.Drawing.Point(13, 13)
        Me.LaunchB.Margin = New System.Windows.Forms.Padding(4)
        Me.LaunchB.Name = "LaunchB"
        Me.LaunchB.Size = New System.Drawing.Size(192, 29)
        Me.LaunchB.TabIndex = 0
        Me.LaunchB.TabStop = False
        '
        'OptionsB
        '
        Me.OptionsB.BackColor = System.Drawing.Color.Transparent
        Me.OptionsB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.OptionsB.Location = New System.Drawing.Point(13, 50)
        Me.OptionsB.Margin = New System.Windows.Forms.Padding(4)
        Me.OptionsB.Name = "OptionsB"
        Me.OptionsB.Size = New System.Drawing.Size(192, 29)
        Me.OptionsB.TabIndex = 1
        Me.OptionsB.TabStop = False
        '
        'ExitB
        '
        Me.ExitB.BackColor = System.Drawing.Color.Transparent
        Me.ExitB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ExitB.Location = New System.Drawing.Point(13, 87)
        Me.ExitB.Margin = New System.Windows.Forms.Padding(4)
        Me.ExitB.Name = "ExitB"
        Me.ExitB.Size = New System.Drawing.Size(192, 29)
        Me.ExitB.TabIndex = 2
        Me.ExitB.TabStop = False
        '
        'Logo
        '
        Me.Logo.BackColor = System.Drawing.Color.Transparent
        Me.Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.Logo.Location = New System.Drawing.Point(478, 317)
        Me.Logo.Margin = New System.Windows.Forms.Padding(4)
        Me.Logo.Name = "Logo"
        Me.Logo.Size = New System.Drawing.Size(149, 150)
        Me.Logo.TabIndex = 3
        Me.Logo.TabStop = False
        '
        'Background
        '
        Me.Background.Location = New System.Drawing.Point(0, 0)
        Me.Background.Name = "Background"
        Me.Background.Size = New System.Drawing.Size(640, 480)
        Me.Background.TabIndex = 4
        Me.Background.TabStop = False
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(640, 480)
        Me.Controls.Add(Me.Logo)
        Me.Controls.Add(Me.ExitB)
        Me.Controls.Add(Me.OptionsB)
        Me.Controls.Add(Me.LaunchB)
        Me.Controls.Add(Me.Background)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "Main"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Van Buren Launcher"
        CType(Me.LaunchB, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.OptionsB, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ExitB, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Logo, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Background, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LaunchB As PictureBox
    Friend WithEvents OptionsB As PictureBox
    Friend WithEvents ExitB As PictureBox
    Friend WithEvents Logo As PictureBox
    Friend WithEvents Background As PictureBox
End Class
