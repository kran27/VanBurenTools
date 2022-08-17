<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class KeybindEditor
    Inherits AltUI.Forms.DarkForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KeybindEditor))
        Me.Button1 = New AltUI.Controls.DarkButton()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.DarkScrollBar1 = New AltUI.Controls.DarkScrollBar()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.DarkGroupBox1 = New AltUI.Controls.DarkGroupBox()
        Me.Timer2 = New System.Windows.Forms.Timer(Me.components)
        Me.DarkLabel1 = New AltUI.Controls.DarkLabel()
        Me.DarkButton1 = New AltUI.Controls.DarkButton()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.DarkGroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.FlatBottom = False
        Me.Button1.FlatTop = False
        Me.Button1.HoldColour = False
        Me.Button1.Location = New System.Drawing.Point(283, 385)
        Me.Button1.Name = "Button1"
        Me.Button1.Padding = New System.Windows.Forms.Padding(5)
        Me.Button1.Size = New System.Drawing.Size(115, 24)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "Apply Changes"
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToResizeColumns = False
        Me.DataGridView1.AllowUserToResizeRows = False
        Me.DataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.DataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.GridColor = System.Drawing.SystemColors.ButtonFace
        Me.DataGridView1.Location = New System.Drawing.Point(6, 22)
        Me.DataGridView1.MultiSelect = False
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersVisible = False
        Me.DataGridView1.RowTemplate.Height = 25
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(374, 339)
        Me.DataGridView1.TabIndex = 2
        '
        'DarkScrollBar1
        '
        Me.DarkScrollBar1.Location = New System.Drawing.Point(363, 22)
        Me.DarkScrollBar1.Maximum = 500
        Me.DarkScrollBar1.Name = "DarkScrollBar1"
        Me.DarkScrollBar1.Size = New System.Drawing.Size(22, 339)
        Me.DarkScrollBar1.TabIndex = 4
        Me.DarkScrollBar1.Text = "DarkScrollBar1"
        '
        'Timer1
        '
        Me.Timer1.Interval = 5
        '
        'DarkGroupBox1
        '
        Me.DarkGroupBox1.Controls.Add(Me.DataGridView1)
        Me.DarkGroupBox1.Controls.Add(Me.DarkScrollBar1)
        Me.DarkGroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.DarkGroupBox1.Name = "DarkGroupBox1"
        Me.DarkGroupBox1.Size = New System.Drawing.Size(386, 367)
        Me.DarkGroupBox1.TabIndex = 5
        Me.DarkGroupBox1.TabStop = False
        Me.DarkGroupBox1.Text = "Keybinds"
        '
        'Timer2
        '
        Me.Timer2.Interval = 50
        '
        'DarkLabel1
        '
        Me.DarkLabel1.AutoSize = True
        Me.DarkLabel1.Location = New System.Drawing.Point(12, 390)
        Me.DarkLabel1.Name = "DarkLabel1"
        Me.DarkLabel1.Size = New System.Drawing.Size(145, 15)
        Me.DarkLabel1.TabIndex = 6
        Me.DarkLabel1.Text = "Press any Key/Key Combo"
        '
        'DarkButton1
        '
        Me.DarkButton1.FlatBottom = False
        Me.DarkButton1.FlatTop = False
        Me.DarkButton1.HoldColour = False
        Me.DarkButton1.Location = New System.Drawing.Point(216, 385)
        Me.DarkButton1.Name = "DarkButton1"
        Me.DarkButton1.Padding = New System.Windows.Forms.Padding(5)
        Me.DarkButton1.Size = New System.Drawing.Size(61, 24)
        Me.DarkButton1.TabIndex = 7
        Me.DarkButton1.Text = "Reload"
        '
        'KeybindEditor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(410, 421)
        Me.Controls.Add(Me.DarkButton1)
        Me.Controls.Add(Me.DarkLabel1)
        Me.Controls.Add(Me.DarkGroupBox1)
        Me.Controls.Add(Me.Button1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "KeybindEditor"
        Me.Text = "Keybind Editor"
        Me.TransparencyKey = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(31, Byte), Integer), CType(CType(32, Byte), Integer))
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.DarkGroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Button1 As AltUI.Controls.DarkButton
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents DarkScrollBar1 As AltUI.Controls.DarkScrollBar
    Friend WithEvents Timer1 As Timer
    Friend WithEvents DarkGroupBox1 As AltUI.Controls.DarkGroupBox
    Friend WithEvents Timer2 As Timer
    Friend WithEvents DarkLabel1 As AltUI.Controls.DarkLabel
    Friend WithEvents DarkButton1 As AltUI.Controls.DarkButton
End Class
