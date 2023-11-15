<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GrpBrowser
    Inherits AltUI.Forms.DarkForm

    'Form overrides dispose to clean up the component list.
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
        Me.HexBox1 = New Be.Windows.Forms.HexBox()
        Me.DarkButton1 = New AltUI.Controls.DarkButton()
        Me.DarkTreeView1 = New AltUI.Controls.DarkTreeView()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.DarkRichTextBox1 = New AltUI.Controls.DarkRichTextBox()
        Me.DarkButton2 = New AltUI.Controls.DarkButton()
        Me.TableLayoutPanel1.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SuspendLayout
        '
        'HexBox1
        '
        Me.HexBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(32,Byte),Integer), CType(CType(32,Byte),Integer), CType(CType(32,Byte),Integer))
        Me.HexBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.HexBox1.Font = New System.Drawing.Font("Cascadia Mono", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.HexBox1.LineInfoVisible = true
        Me.HexBox1.Location = New System.Drawing.Point(403, 3)
        Me.HexBox1.Name = "HexBox1"
        Me.HexBox1.ReadOnly = true
        Me.HexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(CType(CType(100,Byte),Integer), CType(CType(60,Byte),Integer), CType(CType(188,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.HexBox1.Size = New System.Drawing.Size(394, 415)
        Me.HexBox1.StringViewVisible = true
        Me.HexBox1.TabIndex = 2
        Me.HexBox1.VScrollBarVisible = true
        '
        'DarkButton1
        '
        Me.DarkButton1.BorderColour = System.Drawing.Color.Empty
        Me.DarkButton1.CustomColour = false
        Me.DarkButton1.Dock = System.Windows.Forms.DockStyle.Right
        Me.DarkButton1.FlatBottom = false
        Me.DarkButton1.FlatTop = false
        Me.DarkButton1.Location = New System.Drawing.Point(722, 424)
        Me.DarkButton1.Name = "DarkButton1"
        Me.DarkButton1.Padding = New System.Windows.Forms.Padding(5)
        Me.DarkButton1.Size = New System.Drawing.Size(75, 23)
        Me.DarkButton1.TabIndex = 0
        Me.DarkButton1.Text = "Open"
        '
        'DarkTreeView1
        '
        Me.DarkTreeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DarkTreeView1.Location = New System.Drawing.Point(3, 3)
        Me.DarkTreeView1.MaxDragChange = 20
        Me.DarkTreeView1.Name = "DarkTreeView1"
        Me.DarkTreeView1.Size = New System.Drawing.Size(394, 415)
        Me.DarkTreeView1.TabIndex = 1
        Me.DarkTreeView1.Text = "DarkTreeView1"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.Controls.Add(Me.DarkTreeView1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.DarkButton1, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.DarkButton2, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(800, 450)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Location = New System.Drawing.Point(403, 3)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(394, 415)
        Me.PictureBox1.TabIndex = 2
        Me.PictureBox1.TabStop = false
        '
        'DarkRichTextBox1
        '
        Me.DarkRichTextBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(31,Byte),Integer), CType(CType(31,Byte),Integer), CType(CType(32,Byte),Integer))
        Me.DarkRichTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DarkRichTextBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DarkRichTextBox1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213,Byte),Integer), CType(CType(213,Byte),Integer), CType(CType(213,Byte),Integer))
        Me.DarkRichTextBox1.Location = New System.Drawing.Point(403, 3)
        Me.DarkRichTextBox1.Name = "DarkRichTextBox1"
        Me.DarkRichTextBox1.Size = New System.Drawing.Size(394, 415)
        Me.DarkRichTextBox1.TabIndex = 2
        Me.DarkRichTextBox1.Text = ""
        '
        'DarkButton2
        '
        Me.DarkButton2.BorderColour = System.Drawing.Color.Empty
        Me.DarkButton2.CustomColour = false
        Me.DarkButton2.FlatBottom = false
        Me.DarkButton2.FlatTop = false
        Me.DarkButton2.Location = New System.Drawing.Point(3, 424)
        Me.DarkButton2.Name = "DarkButton2"
        Me.DarkButton2.Padding = New System.Windows.Forms.Padding(5)
        Me.DarkButton2.Size = New System.Drawing.Size(75, 23)
        Me.DarkButton2.TabIndex = 2
        Me.DarkButton2.Text = "Extract"
        '
        'GrpBrowser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7!, 15!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.CornerStyle = AltUI.Forms.DarkForm.CornerPreference.[Default]
        Me.Name = "GrpBrowser"
        Me.Text = "GrpBrowser"
        Me.TransparencyKey = System.Drawing.Color.FromArgb(CType(CType(31,Byte),Integer), CType(CType(31,Byte),Integer), CType(CType(32,Byte),Integer))
        Me.TableLayoutPanel1.ResumeLayout(false)
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents HexBox1 As Be.Windows.Forms.HexBox
    Friend WithEvents DarkButton1 As AltUI.Controls.DarkButton
    Friend WithEvents DarkTreeView1 As AltUI.Controls.DarkTreeView
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents DarkRichTextBox1 As AltUI.Controls.DarkRichTextBox
    Friend WithEvents DarkButton2 As AltUI.Controls.DarkButton
End Class
