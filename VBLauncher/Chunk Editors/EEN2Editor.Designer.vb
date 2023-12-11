<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EEN2Editor
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        TableLayoutPanel1 = New TableLayoutPanel()
        DarkLabel1 = New AltUI.Controls.DarkLabel()
        DarkTextBox1 = New AltUI.Controls.DarkTextBox()
        DarkLabel2 = New AltUI.Controls.DarkLabel()
        DarkLabel3 = New AltUI.Controls.DarkLabel()
        DarkTextBox2 = New AltUI.Controls.DarkTextBox()
        DarkTextBox3 = New AltUI.Controls.DarkTextBox()
        DarkCheckBox1 = New AltUI.Controls.DarkCheckBox()
        TableLayoutPanel1.SuspendLayout()
        SuspendLayout()
        ' 
        ' TableLayoutPanel1
        ' 
        TableLayoutPanel1.BackColor = Color.FromArgb(CByte(16), CByte(16), CByte(17))
        TableLayoutPanel1.ColumnCount = 1
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
        TableLayoutPanel1.Controls.Add(DarkLabel1, 0, 0)
        TableLayoutPanel1.Controls.Add(DarkTextBox1, 0, 1)
        TableLayoutPanel1.Controls.Add(DarkLabel2, 0, 2)
        TableLayoutPanel1.Controls.Add(DarkLabel3, 0, 4)
        TableLayoutPanel1.Controls.Add(DarkTextBox2, 0, 3)
        TableLayoutPanel1.Controls.Add(DarkTextBox3, 0, 5)
        TableLayoutPanel1.Controls.Add(DarkCheckBox1, 0, 6)
        TableLayoutPanel1.Dock = DockStyle.Fill
        TableLayoutPanel1.Location = New Point(0, 0)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 7
        TableLayoutPanel1.RowStyles.Add(New RowStyle())
        TableLayoutPanel1.RowStyles.Add(New RowStyle())
        TableLayoutPanel1.RowStyles.Add(New RowStyle())
        TableLayoutPanel1.RowStyles.Add(New RowStyle())
        TableLayoutPanel1.RowStyles.Add(New RowStyle())
        TableLayoutPanel1.RowStyles.Add(New RowStyle())
        TableLayoutPanel1.RowStyles.Add(New RowStyle())
        TableLayoutPanel1.Size = New Size(308, 157)
        TableLayoutPanel1.TabIndex = 0
        ' 
        ' DarkLabel1
        ' 
        DarkLabel1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        DarkLabel1.AutoSize = True
        DarkLabel1.Location = New Point(3, 0)
        DarkLabel1.Name = "DarkLabel1"
        DarkLabel1.Size = New Size(74, 15)
        DarkLabel1.TabIndex = 0
        DarkLabel1.Text = "Model Prefix"
        ' 
        ' DarkTextBox1
        ' 
        DarkTextBox1.BackColor = Color.FromArgb(CByte(26), CByte(26), CByte(28))
        DarkTextBox1.BorderStyle = BorderStyle.FixedSingle
        DarkTextBox1.Dock = DockStyle.Fill
        DarkTextBox1.ForeColor = Color.FromArgb(CByte(213), CByte(213), CByte(213))
        DarkTextBox1.Location = New Point(3, 18)
        DarkTextBox1.Name = "DarkTextBox1"
        DarkTextBox1.Size = New Size(302, 23)
        DarkTextBox1.TabIndex = 1
        ' 
        ' DarkLabel2
        ' 
        DarkLabel2.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        DarkLabel2.AutoSize = True
        DarkLabel2.Location = New Point(3, 44)
        DarkLabel2.Name = "DarkLabel2"
        DarkLabel2.Size = New Size(98, 15)
        DarkLabel2.TabIndex = 2
        DarkLabel2.Text = "Inventory Texture"
        ' 
        ' DarkLabel3
        ' 
        DarkLabel3.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        DarkLabel3.AutoSize = True
        DarkLabel3.Location = New Point(3, 88)
        DarkLabel3.Name = "DarkLabel3"
        DarkLabel3.Size = New Size(103, 15)
        DarkLabel3.TabIndex = 3
        DarkLabel3.Text = "Action Bar Texture"
        ' 
        ' DarkTextBox2
        ' 
        DarkTextBox2.BackColor = Color.FromArgb(CByte(26), CByte(26), CByte(28))
        DarkTextBox2.BorderStyle = BorderStyle.FixedSingle
        DarkTextBox2.Dock = DockStyle.Fill
        DarkTextBox2.ForeColor = Color.FromArgb(CByte(213), CByte(213), CByte(213))
        DarkTextBox2.Location = New Point(3, 62)
        DarkTextBox2.Name = "DarkTextBox2"
        DarkTextBox2.Size = New Size(302, 23)
        DarkTextBox2.TabIndex = 4
        ' 
        ' DarkTextBox3
        ' 
        DarkTextBox3.BackColor = Color.FromArgb(CByte(26), CByte(26), CByte(28))
        DarkTextBox3.BorderStyle = BorderStyle.FixedSingle
        DarkTextBox3.Dock = DockStyle.Fill
        DarkTextBox3.ForeColor = Color.FromArgb(CByte(213), CByte(213), CByte(213))
        DarkTextBox3.Location = New Point(3, 106)
        DarkTextBox3.Name = "DarkTextBox3"
        DarkTextBox3.Size = New Size(302, 23)
        DarkTextBox3.TabIndex = 5
        ' 
        ' DarkCheckBox1
        ' 
        DarkCheckBox1.AutoSize = True
        DarkCheckBox1.Location = New Point(3, 135)
        DarkCheckBox1.Name = "DarkCheckBox1"
        DarkCheckBox1.Offset = 1
        DarkCheckBox1.Size = New Size(79, 19)
        DarkCheckBox1.TabIndex = 6
        DarkCheckBox1.Text = "Selectable"
        ' 
        ' EEN2Editor
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        Controls.Add(TableLayoutPanel1)
        Name = "EEN2Editor"
        Size = New Size(308, 157)
        TableLayoutPanel1.ResumeLayout(False)
        TableLayoutPanel1.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents DarkLabel1 As AltUI.Controls.DarkLabel
    Friend WithEvents DarkTextBox1 As AltUI.Controls.DarkTextBox
    Friend WithEvents DarkLabel2 As AltUI.Controls.DarkLabel
    Friend WithEvents DarkLabel3 As AltUI.Controls.DarkLabel
    Friend WithEvents DarkTextBox2 As AltUI.Controls.DarkTextBox
    Friend WithEvents DarkTextBox3 As AltUI.Controls.DarkTextBox
    Friend WithEvents DarkCheckBox1 As AltUI.Controls.DarkCheckBox

End Class
