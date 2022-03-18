Imports System.ComponentModel
Imports AltUI.Controls
Imports AltUI.Forms
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()>
Partial Class VideoOptions
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
        Me.OptionsGB = New AltUI.Controls.DarkGroupBox()
        Me.ResolutionCB = New AltUI.Controls.DarkComboBox()
        Me.ResolutionL = New AltUI.Controls.DarkLabel()
        Me.MipmapCB = New AltUI.Controls.DarkCheckBox()
        Me.PhongCB = New AltUI.Controls.DarkCheckBox()
        Me.TextureL = New AltUI.Controls.DarkLabel()
        Me.TextureCB = New AltUI.Controls.DarkComboBox()
        Me.AAL = New AltUI.Controls.DarkLabel()
        Me.AACB = New AltUI.Controls.DarkComboBox()
        Me.APICB = New AltUI.Controls.DarkComboBox()
        Me.APIL = New AltUI.Controls.DarkLabel()
        Me.FullscreenCB = New AltUI.Controls.DarkCheckBox()
        Me.CloseB = New AltUI.Controls.DarkButton()
        Me.ApplyB = New AltUI.Controls.DarkButton()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.OptionsGB.SuspendLayout()
        Me.SuspendLayout()
        '
        'OptionsGB
        '
        Me.OptionsGB.Controls.Add(Me.ResolutionCB)
        Me.OptionsGB.Controls.Add(Me.ResolutionL)
        Me.OptionsGB.Controls.Add(Me.MipmapCB)
        Me.OptionsGB.Controls.Add(Me.PhongCB)
        Me.OptionsGB.Controls.Add(Me.TextureL)
        Me.OptionsGB.Controls.Add(Me.TextureCB)
        Me.OptionsGB.Controls.Add(Me.AAL)
        Me.OptionsGB.Controls.Add(Me.AACB)
        Me.OptionsGB.Controls.Add(Me.APICB)
        Me.OptionsGB.Controls.Add(Me.APIL)
        Me.OptionsGB.Controls.Add(Me.FullscreenCB)
        Me.OptionsGB.Location = New System.Drawing.Point(13, 11)
        Me.OptionsGB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.OptionsGB.Name = "OptionsGB"
        Me.OptionsGB.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.OptionsGB.Size = New System.Drawing.Size(221, 185)
        Me.OptionsGB.TabIndex = 0
        Me.OptionsGB.TabStop = False
        Me.OptionsGB.Text = "Options"
        '
        'ResolutionCB
        '
        Me.ResolutionCB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.ResolutionCB.FormattingEnabled = True
        Me.ResolutionCB.Location = New System.Drawing.Point(72, 101)
        Me.ResolutionCB.Margin = New System.Windows.Forms.Padding(2)
        Me.ResolutionCB.Name = "ResolutionCB"
        Me.ResolutionCB.Size = New System.Drawing.Size(140, 24)
        Me.ResolutionCB.TabIndex = 3
        '
        'ResolutionL
        '
        Me.ResolutionL.AutoSize = True
        Me.ResolutionL.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer))
        Me.ResolutionL.Location = New System.Drawing.Point(8, 105)
        Me.ResolutionL.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ResolutionL.Name = "ResolutionL"
        Me.ResolutionL.Size = New System.Drawing.Size(63, 15)
        Me.ResolutionL.TabIndex = 22
        Me.ResolutionL.Text = "Resolution"
        '
        'MipmapCB
        '
        Me.MipmapCB.AutoSize = True
        Me.MipmapCB.Location = New System.Drawing.Point(9, 143)
        Me.MipmapCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MipmapCB.Name = "MipmapCB"
        Me.MipmapCB.Size = New System.Drawing.Size(95, 19)
        Me.MipmapCB.TabIndex = 5
        Me.MipmapCB.Text = "Mipmapping"
        Me.ToolTip1.SetToolTip(Me.MipmapCB, "Disabling reduces texture blur at distance")
        '
        'PhongCB
        '
        Me.PhongCB.AutoSize = True
        Me.PhongCB.Location = New System.Drawing.Point(9, 160)
        Me.PhongCB.Margin = New System.Windows.Forms.Padding(2)
        Me.PhongCB.Name = "PhongCB"
        Me.PhongCB.Size = New System.Drawing.Size(107, 19)
        Me.PhongCB.TabIndex = 6
        Me.PhongCB.Text = "Phong Shading"
        Me.ToolTip1.SetToolTip(Me.PhongCB, "Alternative shading," & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Slightly improves visuals")
        '
        'TextureL
        '
        Me.TextureL.AutoSize = True
        Me.TextureL.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer))
        Me.TextureL.Location = New System.Drawing.Point(8, 77)
        Me.TextureL.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.TextureL.Name = "TextureL"
        Me.TextureL.Size = New System.Drawing.Size(94, 15)
        Me.TextureL.TabIndex = 19
        Me.TextureL.Text = "Texture Filtering "
        '
        'TextureCB
        '
        Me.TextureCB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.TextureCB.FormattingEnabled = True
        Me.TextureCB.Items.AddRange(New Object() {"Bilinear (Default)", "Point Sampled", "Linear", "Anisotropic 2x", "Anisotropic 4x", "Anisotropic 8x", "Anisotropic 16x"})
        Me.TextureCB.Location = New System.Drawing.Point(100, 73)
        Me.TextureCB.Margin = New System.Windows.Forms.Padding(2)
        Me.TextureCB.Name = "TextureCB"
        Me.TextureCB.Size = New System.Drawing.Size(112, 24)
        Me.TextureCB.TabIndex = 2
        Me.TextureCB.BringToFront()
        '
        'AAL
        '
        Me.AAL.AutoSize = True
        Me.AAL.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer))
        Me.AAL.Location = New System.Drawing.Point(8, 49)
        Me.AAL.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.AAL.Name = "AAL"
        Me.AAL.Size = New System.Drawing.Size(74, 15)
        Me.AAL.TabIndex = 17
        Me.AAL.Text = "Anti Aliasing"
        '
        'AACB
        '
        Me.AACB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.AACB.FormattingEnabled = True
        Me.AACB.Items.AddRange(New Object() {"Off", "2x MSAA", "4x MSAA", "8x MSAA"})
        Me.AACB.Location = New System.Drawing.Point(83, 45)
        Me.AACB.Margin = New System.Windows.Forms.Padding(2)
        Me.AACB.Name = "AACB"
        Me.AACB.Size = New System.Drawing.Size(129, 24)
        Me.AACB.TabIndex = 1
        '
        'APICB
        '
        Me.APICB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.APICB.FormattingEnabled = True
        Me.APICB.Items.AddRange(New Object() {"DirectX 8 (Default)", "DirectX 11", "OpenGL", "Vulkan"})
        Me.APICB.Location = New System.Drawing.Point(91, 17)
        Me.APICB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.APICB.Name = "APICB"
        Me.APICB.Size = New System.Drawing.Size(121, 24)
        Me.APICB.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.APICB, "DX11 + Vulkan modes use dgVoodoo2" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "dege.fw.hu" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "OpenGL uses WineD3D (GNU LGPL V2+)" &
        "" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "fdossena.com/?p=wined3d/index.frag" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Vulkan uses DXVK (zlib/libpng)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "github.com" &
        "/doitsujin/dxvk")
        '
        'APIL
        '
        Me.APIL.AutoSize = True
        Me.APIL.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer))
        Me.APIL.Location = New System.Drawing.Point(8, 21)
        Me.APIL.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.APIL.Name = "APIL"
        Me.APIL.Size = New System.Drawing.Size(82, 15)
        Me.APIL.TabIndex = 14
        Me.APIL.Text = "Rendering API"
        '
        'FullscreenCB
        '
        Me.FullscreenCB.AutoSize = True
        Me.FullscreenCB.Location = New System.Drawing.Point(9, 126)
        Me.FullscreenCB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.FullscreenCB.Name = "FullscreenCB"
        Me.FullscreenCB.Size = New System.Drawing.Size(79, 19)
        Me.FullscreenCB.TabIndex = 4
        Me.FullscreenCB.Text = "Fullscreen"
        '
        'CloseB
        '
        Me.CloseB.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CloseB.Location = New System.Drawing.Point(104, 202)
        Me.CloseB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CloseB.Name = "CloseB"
        Me.CloseB.Padding = New System.Windows.Forms.Padding(5)
        Me.CloseB.Size = New System.Drawing.Size(61, 25)
        Me.CloseB.TabIndex = 7
        Me.CloseB.Text = "Close"
        '
        'ApplyB
        '
        Me.ApplyB.Location = New System.Drawing.Point(173, 202)
        Me.ApplyB.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.ApplyB.Name = "ApplyB"
        Me.ApplyB.Padding = New System.Windows.Forms.Padding(5)
        Me.ApplyB.Size = New System.Drawing.Size(61, 25)
        Me.ApplyB.TabIndex = 8
        Me.ApplyB.Text = "Apply"
        '
        'VideoOptions
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(247, 239)
        Me.Controls.Add(Me.CloseB)
        Me.Controls.Add(Me.ApplyB)
        Me.Controls.Add(Me.OptionsGB)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "VideoOptions"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Video Options"
        Me.OptionsGB.ResumeLayout(False)
        Me.OptionsGB.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents OptionsGB As DarkGroupBox
    Friend WithEvents CloseB As DarkButton
    Friend WithEvents ApplyB As DarkButton
    Friend WithEvents FullscreenCB As DarkCheckBox
    Friend WithEvents APICB As DarkComboBox
    Friend WithEvents APIL As DarkLabel
    Friend WithEvents AAL As DarkLabel
    Friend WithEvents AACB As DarkComboBox
    Friend WithEvents TextureL As DarkLabel
    Friend WithEvents TextureCB As DarkComboBox
    Friend WithEvents PhongCB As DarkCheckBox
    Friend WithEvents MipmapCB As DarkCheckBox
    Friend WithEvents ResolutionCB As DarkComboBox
    Friend WithEvents ResolutionL As DarkLabel
End Class
