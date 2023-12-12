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
        HexBox1 = New Be.Windows.Forms.HexBox()
        DarkButton1 = New AltUI.Controls.DarkButton()
        DarkTreeView1 = New AltUI.Controls.DarkTreeView()
        TableLayoutPanel1 = New TableLayoutPanel()
        DarkButton2 = New AltUI.Controls.DarkButton()
        DarkSectionPanel1 = New AltUI.Controls.DarkSectionPanel()
        DarkSectionPanel2 = New AltUI.Controls.DarkSectionPanel()
        PictureBox1 = New PictureBox()
        DarkRichTextBox1 = New AltUI.Controls.DarkRichTextBox()
        TableLayoutPanel2 = New TableLayoutPanel()
        TrackBar1 = New TrackBar()
        TableLayoutPanel1.SuspendLayout()
        DarkSectionPanel1.SuspendLayout()
        DarkSectionPanel2.SuspendLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        TableLayoutPanel2.SuspendLayout()
        CType(TrackBar1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' HexBox1
        ' 
        HexBox1.BackColor = Color.FromArgb(CByte(32), CByte(32), CByte(32))
        HexBox1.Dock = DockStyle.Fill
        HexBox1.Font = New Font("Cascadia Mono", 9F, FontStyle.Regular, GraphicsUnit.Point)
        HexBox1.LineInfoVisible = True
        HexBox1.Location = New Point(403, 3)
        HexBox1.Name = "HexBox1"
        HexBox1.ReadOnly = True
        HexBox1.ShadowSelectionColor = Color.FromArgb(CByte(100), CByte(60), CByte(188), CByte(255))
        HexBox1.Size = New Size(394, 415)
        HexBox1.StringViewVisible = True
        HexBox1.TabIndex = 2
        HexBox1.VScrollBarVisible = True
        ' 
        ' DarkButton1
        ' 
        DarkButton1.BorderColour = Color.Empty
        DarkButton1.CustomColour = False
        DarkButton1.Dock = DockStyle.Right
        DarkButton1.FlatBottom = False
        DarkButton1.FlatTop = False
        DarkButton1.Location = New Point(722, 424)
        DarkButton1.Name = "DarkButton1"
        DarkButton1.Padding = New Padding(5)
        DarkButton1.Size = New Size(75, 23)
        DarkButton1.TabIndex = 0
        DarkButton1.Text = "Open"
        ' 
        ' DarkTreeView1
        ' 
        DarkTreeView1.Dock = DockStyle.Fill
        DarkTreeView1.Location = New Point(1, 25)
        DarkTreeView1.MaxDragChange = 20
        DarkTreeView1.Name = "DarkTreeView1"
        DarkTreeView1.Padding = New Padding(0, 0, 0, 2)
        DarkTreeView1.Size = New Size(392, 389)
        DarkTreeView1.TabIndex = 1
        DarkTreeView1.Text = "DarkTreeView1"
        ' 
        ' TableLayoutPanel1
        ' 
        TableLayoutPanel1.ColumnCount = 2
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.Controls.Add(DarkButton1, 1, 1)
        TableLayoutPanel1.Controls.Add(DarkButton2, 0, 1)
        TableLayoutPanel1.Controls.Add(DarkSectionPanel1, 0, 0)
        TableLayoutPanel1.Controls.Add(DarkSectionPanel2, 1, 0)
        TableLayoutPanel1.Dock = DockStyle.Fill
        TableLayoutPanel1.Location = New Point(0, 0)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 2
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
        TableLayoutPanel1.RowStyles.Add(New RowStyle())
        TableLayoutPanel1.Size = New Size(800, 450)
        TableLayoutPanel1.TabIndex = 2
        ' 
        ' DarkButton2
        ' 
        DarkButton2.BorderColour = Color.Empty
        DarkButton2.CustomColour = False
        DarkButton2.FlatBottom = False
        DarkButton2.FlatTop = False
        DarkButton2.Location = New Point(3, 424)
        DarkButton2.Name = "DarkButton2"
        DarkButton2.Padding = New Padding(5)
        DarkButton2.Size = New Size(75, 23)
        DarkButton2.TabIndex = 2
        DarkButton2.Text = "Extract"
        ' 
        ' DarkSectionPanel1
        ' 
        DarkSectionPanel1.BackColor = Color.FromArgb(CByte(28), CByte(28), CByte(30))
        DarkSectionPanel1.Controls.Add(DarkTreeView1)
        DarkSectionPanel1.Dock = DockStyle.Fill
        DarkSectionPanel1.Location = New Point(3, 3)
        DarkSectionPanel1.Name = "DarkSectionPanel1"
        DarkSectionPanel1.SectionHeader = Nothing
        DarkSectionPanel1.Size = New Size(394, 415)
        DarkSectionPanel1.TabIndex = 3
        ' 
        ' DarkSectionPanel2
        ' 
        DarkSectionPanel2.BackColor = Color.FromArgb(CByte(28), CByte(28), CByte(30))
        DarkSectionPanel2.Dock = DockStyle.Fill
        DarkSectionPanel2.Location = New Point(403, 3)
        DarkSectionPanel2.Name = "DarkSectionPanel2"
        DarkSectionPanel2.SectionHeader = ""
        DarkSectionPanel2.Size = New Size(394, 415)
        DarkSectionPanel2.TabIndex = 4
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackgroundImageLayout = ImageLayout.Center
        PictureBox1.Dock = DockStyle.Fill
        PictureBox1.Location = New Point(403, 3)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(394, 415)
        PictureBox1.TabIndex = 2
        PictureBox1.TabStop = False
        ' 
        ' DarkRichTextBox1
        ' 
        DarkRichTextBox1.BackColor = Color.FromArgb(CByte(31), CByte(31), CByte(32))
        DarkRichTextBox1.BorderStyle = BorderStyle.None
        DarkRichTextBox1.Dock = DockStyle.Fill
        DarkRichTextBox1.ForeColor = Color.FromArgb(CByte(213), CByte(213), CByte(213))
        DarkRichTextBox1.Location = New Point(403, 3)
        DarkRichTextBox1.Name = "DarkRichTextBox1"
        DarkRichTextBox1.Size = New Size(394, 415)
        DarkRichTextBox1.TabIndex = 2
        DarkRichTextBox1.Text = ""
        ' 
        ' TableLayoutPanel2
        ' 
        TableLayoutPanel2.ColumnCount = 1
        TableLayoutPanel2.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
        TableLayoutPanel2.Controls.Add(TrackBar1, 0, 1)
        TableLayoutPanel2.Dock = DockStyle.Fill
        TableLayoutPanel2.Location = New Point(1, 25)
        TableLayoutPanel2.Name = "TableLayoutPanel2"
        TableLayoutPanel2.RowCount = 2
        TableLayoutPanel2.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
        TableLayoutPanel2.RowStyles.Add(New RowStyle())
        TableLayoutPanel2.Size = New Size(392, 389)
        TableLayoutPanel2.TabIndex = 0
        ' 
        ' TrackBar1
        ' 
        TrackBar1.Dock = DockStyle.Fill
        TrackBar1.Location = New Point(3, 341)
        TrackBar1.Name = "TrackBar1"
        TrackBar1.RightToLeft = RightToLeft.No
        TrackBar1.Size = New Size(386, 45)
        TrackBar1.TabIndex = 0
        ' 
        ' GrpBrowser
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(TableLayoutPanel1)
        CornerStyle = CornerPreference.Default
        Name = "GrpBrowser"
        Text = "GrpBrowser"
        TableLayoutPanel1.ResumeLayout(False)
        DarkSectionPanel1.ResumeLayout(False)
        DarkSectionPanel2.ResumeLayout(False)
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        TableLayoutPanel2.ResumeLayout(False)
        TableLayoutPanel2.PerformLayout()
        CType(TrackBar1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub
    Friend WithEvents HexBox1 As Be.Windows.Forms.HexBox
    Friend WithEvents DarkButton1 As AltUI.Controls.DarkButton
    Friend WithEvents DarkTreeView1 As AltUI.Controls.DarkTreeView
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents DarkRichTextBox1 As AltUI.Controls.DarkRichTextBox
    Friend WithEvents DarkButton2 As AltUI.Controls.DarkButton
    Friend WithEvents DarkSectionPanel1 As AltUI.Controls.DarkSectionPanel
    Friend WithEvents DarkSectionPanel2 As AltUI.Controls.DarkSectionPanel
    Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
    Friend WithEvents TrackBar1 As TrackBar
End Class
