Imports System.ComponentModel
Imports DarkUI2.Controls
Imports DarkUI2.Forms
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()>
Partial Class Form3
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
        Me.GroupBox1 = New DarkUI2.Controls.DarkGroupBox()
        Me.ComboBox4 = New DarkUI2.Controls.DarkComboBox()
        Me.Label4 = New DarkUI2.Controls.DarkLabel()
        Me.CheckBox3 = New DarkUI2.Controls.DarkCheckBox()
        Me.CheckBox4 = New DarkUI2.Controls.DarkCheckBox()
        Me.Label3 = New DarkUI2.Controls.DarkLabel()
        Me.ComboBox3 = New DarkUI2.Controls.DarkComboBox()
        Me.Label2 = New DarkUI2.Controls.DarkLabel()
        Me.ComboBox2 = New DarkUI2.Controls.DarkComboBox()
        Me.ComboBox1 = New DarkUI2.Controls.DarkComboBox()
        Me.Label1 = New DarkUI2.Controls.DarkLabel()
        Me.CheckBox1 = New DarkUI2.Controls.DarkCheckBox()
        Me.Button2 = New DarkUI2.Controls.DarkButton()
        Me.Button1 = New DarkUI2.Controls.DarkButton()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.BorderColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.GroupBox1.Controls.Add(Me.ComboBox4)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.CheckBox3)
        Me.GroupBox1.Controls.Add(Me.CheckBox4)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.ComboBox3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.ComboBox2)
        Me.GroupBox1.Controls.Add(Me.ComboBox1)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.CheckBox1)
        Me.GroupBox1.Location = New System.Drawing.Point(13, 12)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.GroupBox1.Size = New System.Drawing.Size(221, 214)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Options"
        '
        'ComboBox4
        '
        Me.ComboBox4.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.ComboBox4.FormattingEnabled = True
        Me.ComboBox4.Location = New System.Drawing.Point(77, 109)
        Me.ComboBox4.Margin = New System.Windows.Forms.Padding(2)
        Me.ComboBox4.Name = "ComboBox4"
        Me.ComboBox4.Size = New System.Drawing.Size(138, 24)
        Me.ComboBox4.TabIndex = 3
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer))
        Me.Label4.Location = New System.Drawing.Point(8, 112)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(63, 16)
        Me.Label4.TabIndex = 22
        Me.Label4.Text = "Resolution"
        '
        'CheckBox3
        '
        Me.CheckBox3.AutoSize = True
        Me.CheckBox3.Location = New System.Drawing.Point(8, 164)
        Me.CheckBox3.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CheckBox3.Name = "CheckBox3"
        Me.CheckBox3.Size = New System.Drawing.Size(132, 20)
        Me.CheckBox3.TabIndex = 5
        Me.CheckBox3.Text = "Enable Mipmapping"
        '
        'CheckBox4
        '
        Me.CheckBox4.AutoSize = True
        Me.CheckBox4.Location = New System.Drawing.Point(8, 189)
        Me.CheckBox4.Margin = New System.Windows.Forms.Padding(2)
        Me.CheckBox4.Name = "CheckBox4"
        Me.CheckBox4.Size = New System.Drawing.Size(146, 20)
        Me.CheckBox4.TabIndex = 6
        Me.CheckBox4.Text = "Enable Phong Shading"
        Me.ToolTip1.SetToolTip(Me.CheckBox4, "A different shading method," & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "very slightly darkens shadows in some places.")
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer))
        Me.Label3.Location = New System.Drawing.Point(8, 83)
        Me.Label3.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(90, 16)
        Me.Label3.TabIndex = 19
        Me.Label3.Text = "Texture Filtering"
        '
        'ComboBox3
        '
        Me.ComboBox3.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.ComboBox3.FormattingEnabled = True
        Me.ComboBox3.Items.AddRange(New Object() {"Default (Bilinear)", "Point Sampled", "Linear Mip", "Anisotropic 2x", "Anisotropic 4x", "Anisotropic 8x", "Anisotropic 16x"})
        Me.ComboBox3.Location = New System.Drawing.Point(102, 80)
        Me.ComboBox3.Margin = New System.Windows.Forms.Padding(2)
        Me.ComboBox3.Name = "ComboBox3"
        Me.ComboBox3.Size = New System.Drawing.Size(113, 24)
        Me.ComboBox3.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer))
        Me.Label2.Location = New System.Drawing.Point(8, 54)
        Me.Label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(74, 16)
        Me.Label2.TabIndex = 17
        Me.Label2.Text = "Anti Aliasing"
        '
        'ComboBox2
        '
        Me.ComboBox2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.ComboBox2.FormattingEnabled = True
        Me.ComboBox2.Items.AddRange(New Object() {"Off", "2x MSAA", "4x MSAA", "8x MSAA"})
        Me.ComboBox2.Location = New System.Drawing.Point(86, 51)
        Me.ComboBox2.Margin = New System.Windows.Forms.Padding(2)
        Me.ComboBox2.Name = "ComboBox2"
        Me.ComboBox2.Size = New System.Drawing.Size(129, 24)
        Me.ComboBox2.TabIndex = 1
        '
        'ComboBox1
        '
        Me.ComboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Items.AddRange(New Object() {"DirectX 8 (Default)", "DirectX 11", "OpenGL", "Vulkan"})
        Me.ComboBox1.Location = New System.Drawing.Point(98, 22)
        Me.ComboBox1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(117, 24)
        Me.ComboBox1.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.ComboBox1, "DX11 + Vulkan modes use dgVoodoo2" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "dege.fw.hu" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "OpenGL uses WineD3D (GNU LGPL V2+)" &
        "" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "fdossena.com/?p=wined3d/index.frag" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Vulkan uses DXVK (zlib/libpng)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "github.com" &
        "/doitsujin/dxvk")
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(8, 25)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(82, 16)
        Me.Label1.TabIndex = 14
        Me.Label1.Text = "Rendering API"
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(8, 138)
        Me.CheckBox1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(79, 20)
        Me.CheckBox1.TabIndex = 4
        Me.CheckBox1.Text = "Fullscreen"
        '
        'Button2
        '
        Me.Button2.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button2.Location = New System.Drawing.Point(104, 232)
        Me.Button2.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Button2.Name = "Button2"
        Me.Button2.Padding = New System.Windows.Forms.Padding(5)
        Me.Button2.Size = New System.Drawing.Size(61, 27)
        Me.Button2.TabIndex = 7
        Me.Button2.Text = "Close"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(173, 232)
        Me.Button1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Button1.Name = "Button1"
        Me.Button1.Padding = New System.Windows.Forms.Padding(5)
        Me.Button1.Size = New System.Drawing.Size(61, 27)
        Me.Button1.TabIndex = 8
        Me.Button1.Text = "Apply"
        '
        'Form3
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(247, 271)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form3"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Video Options"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents GroupBox1 As DarkGroupBox
    Friend WithEvents Button2 As DarkButton
    Friend WithEvents Button1 As DarkButton
    Friend WithEvents CheckBox1 As DarkCheckBox
    Friend WithEvents ComboBox1 As DarkComboBox
    Friend WithEvents Label1 As DarkLabel
    Friend WithEvents Label2 As DarkLabel
    Friend WithEvents ComboBox2 As DarkComboBox
    Friend WithEvents Label3 As DarkLabel
    Friend WithEvents ComboBox3 As DarkComboBox
    Friend WithEvents CheckBox4 As DarkCheckBox
    Friend WithEvents CheckBox3 As DarkCheckBox
    Friend WithEvents ComboBox4 As DarkComboBox
    Friend WithEvents Label4 As DarkLabel
End Class
