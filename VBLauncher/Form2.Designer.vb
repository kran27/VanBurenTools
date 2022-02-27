Imports System.ComponentModel
Imports DarkUI2.Controls
Imports DarkUI2.Forms
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()>
Partial Class Form2
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
        Me.Button1 = New DarkUI2.Controls.DarkButton()
        Me.CheckBox1 = New DarkUI2.Controls.DarkCheckBox()
        Me.GroupBox1 = New DarkUI2.Controls.DarkGroupBox()
        Me.CheckBox4 = New DarkUI2.Controls.DarkCheckBox()
        Me.CheckBox3 = New DarkUI2.Controls.DarkCheckBox()
        Me.CheckBox2 = New DarkUI2.Controls.DarkCheckBox()
        Me.GroupBox2 = New DarkUI2.Controls.DarkGroupBox()
        Me.ComboBox3 = New DarkUI2.Controls.DarkComboBox()
        Me.Label4 = New DarkUI2.Controls.DarkLabel()
        Me.ComboBox1 = New DarkUI2.Controls.DarkComboBox()
        Me.Label3 = New DarkUI2.Controls.DarkLabel()
        Me.ComboBox2 = New DarkUI2.Controls.DarkComboBox()
        Me.Label2 = New DarkUI2.Controls.DarkLabel()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Label1 = New DarkUI2.Controls.DarkLabel()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Button3 = New DarkUI2.Controls.DarkButton()
        Me.Button2 = New DarkUI2.Controls.DarkButton()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(209, 278)
        Me.Button1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Button1.Name = "Button1"
        Me.Button1.Padding = New System.Windows.Forms.Padding(5)
        Me.Button1.Size = New System.Drawing.Size(61, 25)
        Me.Button1.TabIndex = 11
        Me.Button1.Text = "Apply"
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(8, 21)
        Me.CheckBox1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(140, 19)
        Me.CheckBox1.TabIndex = 1
        Me.CheckBox1.Text = "Enable Startup Videos"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.CheckBox4)
        Me.GroupBox1.Controls.Add(Me.CheckBox3)
        Me.GroupBox1.Controls.Add(Me.CheckBox2)
        Me.GroupBox1.Controls.Add(Me.CheckBox1)
        Me.GroupBox1.Location = New System.Drawing.Point(13, 11)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.GroupBox1.Size = New System.Drawing.Size(257, 118)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "General Tweaks"
        '
        'CheckBox4
        '
        Me.CheckBox4.AutoSize = True
        Me.CheckBox4.Location = New System.Drawing.Point(8, 94)
        Me.CheckBox4.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CheckBox4.Name = "CheckBox4"
        Me.CheckBox4.Size = New System.Drawing.Size(115, 19)
        Me.CheckBox4.TabIndex = 4
        Me.CheckBox4.Text = "Fix Map Lighting"
        '
        'CheckBox3
        '
        Me.CheckBox3.AutoSize = True
        Me.CheckBox3.Location = New System.Drawing.Point(8, 69)
        Me.CheckBox3.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CheckBox3.Name = "CheckBox3"
        Me.CheckBox3.Size = New System.Drawing.Size(139, 19)
        Me.CheckBox3.TabIndex = 3
        Me.CheckBox3.Text = "Uncap Camera Zoom"
        '
        'CheckBox2
        '
        Me.CheckBox2.AutoSize = True
        Me.CheckBox2.Location = New System.Drawing.Point(8, 45)
        Me.CheckBox2.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CheckBox2.Name = "CheckBox2"
        Me.CheckBox2.Size = New System.Drawing.Size(189, 19)
        Me.CheckBox2.TabIndex = 2
        Me.CheckBox2.Text = "Remove Useless Menu Options"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.ComboBox3)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.ComboBox1)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.ComboBox2)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.PictureBox1)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Location = New System.Drawing.Point(13, 135)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.GroupBox2.Size = New System.Drawing.Size(257, 137)
        Me.GroupBox2.TabIndex = 5
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Fun Tweaks"
        '
        'ComboBox3
        '
        Me.ComboBox3.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.ComboBox3.FormattingEnabled = True
        Me.ComboBox3.Items.AddRange(New Object() {"00_03_Tutorial_Junktown.map", "00_04_Tutorial_Vault.map", "04_0202_Spelunking.map", "98_Canyon_Random_01.map", "98_Canyon_Random_02.map", "Default_StartMap.map", "Mainmenu.map", "zz_TestMapsScottE_Test1.map", "zz_TestMapsScottE_Test2.map", "zz_TestMapsScottE_Test4.map", "zz_TestMapsTest_City_Building01.map", "zz_TestMapsTest_City_Building02.map", "zz_TestMapsTest_City_Building03.map", "zz_TestMapsTest_City_Building04.map", "zz_TestMapsTest_City_Fences.map", "zz_TestMapsTest_Junktown_Shacks.map", "zz_TestMapsaarontemp2.map"})
        Me.ComboBox3.Location = New System.Drawing.Point(104, 21)
        Me.ComboBox3.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.ComboBox3.Name = "ComboBox3"
        Me.ComboBox3.Size = New System.Drawing.Size(145, 24)
        Me.ComboBox3.TabIndex = 6
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer))
        Me.Label4.Location = New System.Drawing.Point(8, 23)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(89, 15)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "NewGame Map"
        '
        'ComboBox1
        '
        Me.ComboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Items.AddRange(New Object() {"Default", "Aaron Map 2", "Building 1", "Building 2", "Building 3", "Building 4", "Canyon 1", "Canyon 2", "Cave", "Fences", "Scott E Map 1", "Scott E Map 2", "Scott E Map 4", "Shacks", "Start Map", "Tutorial", "Vault"})
        Me.ComboBox1.Location = New System.Drawing.Point(111, 49)
        Me.ComboBox1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(138, 24)
        Me.ComboBox1.TabIndex = 7
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer))
        Me.Label3.Location = New System.Drawing.Point(123, 102)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(51, 15)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Preview:"
        '
        'ComboBox2
        '
        Me.ComboBox2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.ComboBox2.FormattingEnabled = True
        Me.ComboBox2.Items.AddRange(New Object() {"Default", "8-Ball", "American", "Black", "Blue", "Eye", "Flames", "Full Skull", "Green", "Grey", "Police", "Red", "Shot Smiley", "Skull", "Yellow"})
        Me.ComboBox2.Location = New System.Drawing.Point(60, 77)
        Me.ComboBox2.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.ComboBox2.Name = "ComboBox2"
        Me.ComboBox2.Size = New System.Drawing.Size(113, 24)
        Me.ComboBox2.TabIndex = 8
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer))
        Me.Label2.Location = New System.Drawing.Point(8, 80)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(46, 15)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Helmet"
        '
        'PictureBox1
        '
        Me.PictureBox1.Location = New System.Drawing.Point(181, 77)
        Me.PictureBox1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(75, 58)
        Me.PictureBox1.TabIndex = 3
        Me.PictureBox1.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(8, 52)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(95, 15)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Main Menu Map"
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(13, 278)
        Me.Button3.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Button3.Name = "Button3"
        Me.Button3.Padding = New System.Windows.Forms.Padding(5)
        Me.Button3.Size = New System.Drawing.Size(97, 25)
        Me.Button3.TabIndex = 9
        Me.Button3.Text = "Video Options"
        '
        'Button2
        '
        Me.Button2.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button2.Location = New System.Drawing.Point(140, 278)
        Me.Button2.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Button2.Name = "Button2"
        Me.Button2.Padding = New System.Windows.Forms.Padding(5)
        Me.Button2.Size = New System.Drawing.Size(61, 25)
        Me.Button2.TabIndex = 10
        Me.Button2.Text = "Close"
        '
        'Form2
        '
        Me.AcceptButton = Me.Button1
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(283, 314)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Button1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form2"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Options"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents Button1 As DarkButton
    Friend WithEvents CheckBox1 As DarkCheckBox
    Friend WithEvents GroupBox1 As DarkGroupBox
    Friend WithEvents GroupBox2 As DarkGroupBox
    Friend WithEvents ComboBox1 As DarkComboBox
    Friend WithEvents Label1 As DarkLabel
    Friend WithEvents Label3 As DarkLabel
    Friend WithEvents ComboBox2 As DarkComboBox
    Friend WithEvents Label2 As DarkLabel
    Friend WithEvents CheckBox2 As DarkCheckBox
    Friend WithEvents CheckBox3 As DarkCheckBox
    Friend WithEvents ComboBox3 As DarkComboBox
    Friend WithEvents Label4 As DarkLabel
    Friend WithEvents Button3 As DarkButton
    Friend WithEvents CheckBox4 As DarkCheckBox
    Friend WithEvents Button2 As DarkButton
End Class
