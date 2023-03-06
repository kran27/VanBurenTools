Imports System.ComponentModel
Imports AltUI.Controls
Imports AltUI.Forms
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class ModLoader
    Inherits DarkForm

    'Form overrides dispose to clean up the component list.
    <DebuggerNonUserCode()> _
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
    <DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ModLoader))
        Me.DarkToolStrip1 = New AltUI.Controls.DarkToolStrip()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.AddModToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EnableAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DisableAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripDropDownButton2 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.OpenGameFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenModsFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ListView1 = New VBLauncher.ModLoader.DoubleBufferedListView()
        Me.ModName = New System.Windows.Forms.ColumnHeader()
        Me.Conflicts = New System.Windows.Forms.ColumnHeader()
        Me.Priority = New System.Windows.Forms.ColumnHeader()
        Me.DarkToolStrip2 = New AltUI.Controls.DarkToolStrip()
        Me.ToolStripLabel2 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripLabel3 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.DarkLabel3 = New AltUI.Controls.DarkLabel()
        Me.DarkLabel1 = New AltUI.Controls.DarkLabel()
        Me.DarkLabel2 = New AltUI.Controls.DarkLabel()
        Me.DarkRichTextBox1 = New AltUI.Controls.DarkRichTextBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.DarkToolStrip1.SuspendLayout()
        Me.DarkToolStrip2.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DarkToolStrip1
        '
        Me.DarkToolStrip1.AutoSize = False
        Me.DarkToolStrip1.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.DarkToolStrip1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.DarkToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripButton1, Me.ToolStripDropDownButton1, Me.ToolStripDropDownButton2, Me.ToolStripLabel1})
        Me.DarkToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.DarkToolStrip1.Name = "DarkToolStrip1"
        Me.DarkToolStrip1.Padding = New System.Windows.Forms.Padding(5, 0, 1, 0)
        Me.DarkToolStrip1.Size = New System.Drawing.Size(800, 28)
        Me.DarkToolStrip1.TabIndex = 2
        Me.DarkToolStrip1.Text = "DarkToolStrip1"
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"), System.Drawing.Image)
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(23, 25)
        Me.ToolStripButton1.Text = "ToolStripButton1"
        Me.ToolStripButton1.ToolTipText = "Refresh"
        '
        'ToolStripDropDownButton1
        '
        Me.ToolStripDropDownButton1.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.ToolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripDropDownButton1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddModToolStripMenuItem, Me.EnableAllToolStripMenuItem, Me.DisableAllToolStripMenuItem})
        Me.ToolStripDropDownButton1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.ToolStripDropDownButton1.Image = CType(resources.GetObject("ToolStripDropDownButton1.Image"), System.Drawing.Image)
        Me.ToolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButton1.Name = "ToolStripDropDownButton1"
        Me.ToolStripDropDownButton1.Size = New System.Drawing.Size(29, 25)
        Me.ToolStripDropDownButton1.Text = "ToolStripDropDownButton1"
        Me.ToolStripDropDownButton1.ToolTipText = "List Options"
        '
        'AddModToolStripMenuItem
        '
        Me.AddModToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.AddModToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.AddModToolStripMenuItem.Name = "AddModToolStripMenuItem"
        Me.AddModToolStripMenuItem.Size = New System.Drawing.Size(129, 22)
        Me.AddModToolStripMenuItem.Text = "Add Mod"
        '
        'EnableAllToolStripMenuItem
        '
        Me.EnableAllToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.EnableAllToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.EnableAllToolStripMenuItem.Name = "EnableAllToolStripMenuItem"
        Me.EnableAllToolStripMenuItem.Size = New System.Drawing.Size(129, 22)
        Me.EnableAllToolStripMenuItem.Text = "Enable All"
        '
        'DisableAllToolStripMenuItem
        '
        Me.DisableAllToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.DisableAllToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.DisableAllToolStripMenuItem.Name = "DisableAllToolStripMenuItem"
        Me.DisableAllToolStripMenuItem.Size = New System.Drawing.Size(129, 22)
        Me.DisableAllToolStripMenuItem.Text = "Disable All"
        '
        'ToolStripDropDownButton2
        '
        Me.ToolStripDropDownButton2.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.ToolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripDropDownButton2.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenGameFolderToolStripMenuItem, Me.OpenModsFolderToolStripMenuItem})
        Me.ToolStripDropDownButton2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.ToolStripDropDownButton2.Image = CType(resources.GetObject("ToolStripDropDownButton2.Image"), System.Drawing.Image)
        Me.ToolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButton2.Name = "ToolStripDropDownButton2"
        Me.ToolStripDropDownButton2.Size = New System.Drawing.Size(29, 25)
        Me.ToolStripDropDownButton2.Text = "ToolStripDropDownButton2"
        Me.ToolStripDropDownButton2.ToolTipText = "Folder Menu"
        '
        'OpenGameFolderToolStripMenuItem
        '
        Me.OpenGameFolderToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.OpenGameFolderToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.OpenGameFolderToolStripMenuItem.Name = "OpenGameFolderToolStripMenuItem"
        Me.OpenGameFolderToolStripMenuItem.Size = New System.Drawing.Size(173, 22)
        Me.OpenGameFolderToolStripMenuItem.Text = "Open Game Folder"
        '
        'OpenModsFolderToolStripMenuItem
        '
        Me.OpenModsFolderToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.OpenModsFolderToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.OpenModsFolderToolStripMenuItem.Name = "OpenModsFolderToolStripMenuItem"
        Me.OpenModsFolderToolStripMenuItem.Size = New System.Drawing.Size(173, 22)
        Me.OpenModsFolderToolStripMenuItem.Text = "Open Mods Folder"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripLabel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.ToolStripLabel1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(52, 25)
        Me.ToolStripLabel1.Text = "Active: 0"
        '
        'ListView1
        '
        Me.ListView1.AllowDrop = True
        Me.ListView1.BackColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.ListView1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ListView1.CheckBoxes = True
        Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ModName, Me.Conflicts, Me.Priority})
        Me.ListView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListView1.FullRowSelect = True
        Me.ListView1.HideSelection = True
        Me.ListView1.Location = New System.Drawing.Point(0, 28)
        Me.ListView1.MultiSelect = False
        Me.ListView1.Name = "ListView1"
        Me.ListView1.OwnerDraw = True
        Me.ListView1.Size = New System.Drawing.Size(600, 397)
        Me.ListView1.TabIndex = 3
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'ModName
        '
        Me.ModName.Text = "Mod Name"
        Me.ModName.Width = 300
        '
        'Conflicts
        '
        Me.Conflicts.Text = "Conflicts"
        Me.Conflicts.Width = 150
        '
        'Priority
        '
        Me.Priority.Text = "Priority"
        Me.Priority.Width = 150
        '
        'DarkToolStrip2
        '
        Me.DarkToolStrip2.AutoSize = False
        Me.DarkToolStrip2.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.DarkToolStrip2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.DarkToolStrip2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.DarkToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.DarkToolStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel2, Me.ToolStripSeparator1, Me.ToolStripLabel3, Me.ToolStripSeparator2})
        Me.DarkToolStrip2.Location = New System.Drawing.Point(0, 425)
        Me.DarkToolStrip2.Name = "DarkToolStrip2"
        Me.DarkToolStrip2.Padding = New System.Windows.Forms.Padding(5, 0, 1, 0)
        Me.DarkToolStrip2.Size = New System.Drawing.Size(800, 25)
        Me.DarkToolStrip2.TabIndex = 4
        Me.DarkToolStrip2.Text = "DarkToolStrip2"
        '
        'ToolStripLabel2
        '
        Me.ToolStripLabel2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripLabel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.ToolStripLabel2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.ToolStripLabel2.Image = CType(resources.GetObject("ToolStripLabel2.Image"), System.Drawing.Image)
        Me.ToolStripLabel2.Name = "ToolStripLabel2"
        Me.ToolStripLabel2.Size = New System.Drawing.Size(96, 22)
        Me.ToolStripLabel2.Text = " Deploy Mods"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripSeparator1.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.ToolStripSeparator1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.ToolStripSeparator1.Margin = New System.Windows.Forms.Padding(0, 0, 2, 0)
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripLabel3
        '
        Me.ToolStripLabel3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripLabel3.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.ToolStripLabel3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.ToolStripLabel3.Image = CType(resources.GetObject("ToolStripLabel3.Image"), System.Drawing.Image)
        Me.ToolStripLabel3.Name = "ToolStripLabel3"
        Me.ToolStripLabel3.Size = New System.Drawing.Size(45, 22)
        Me.ToolStripLabel3.Text = " Exit"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripSeparator2.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.ToolStripSeparator2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.ToolStripSeparator2.Margin = New System.Windows.Forms.Padding(0, 0, 2, 0)
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.DarkLabel3, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.DarkLabel1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.DarkLabel2, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.DarkRichTextBox1, 0, 2)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(600, 28)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.Padding = New System.Windows.Forms.Padding(0, 0, 0, 6)
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(200, 397)
        Me.TableLayoutPanel1.TabIndex = 5
        '
        'DarkLabel3
        '
        Me.DarkLabel3.AutoSize = True
        Me.DarkLabel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DarkLabel3.Location = New System.Drawing.Point(9, 377)
        Me.DarkLabel3.Margin = New System.Windows.Forms.Padding(9, 6, 9, 0)
        Me.DarkLabel3.Name = "DarkLabel3"
        Me.DarkLabel3.Size = New System.Drawing.Size(182, 14)
        Me.DarkLabel3.TabIndex = 3
        Me.DarkLabel3.Text = "Version:"
        '
        'DarkLabel1
        '
        Me.DarkLabel1.AutoSize = True
        Me.DarkLabel1.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.DarkLabel1.Location = New System.Drawing.Point(3, 2)
        Me.DarkLabel1.Margin = New System.Windows.Forms.Padding(3, 2, 3, 0)
        Me.DarkLabel1.Name = "DarkLabel1"
        Me.DarkLabel1.Size = New System.Drawing.Size(83, 21)
        Me.DarkLabel1.TabIndex = 1
        Me.DarkLabel1.Text = "Mod Title"
        '
        'DarkLabel2
        '
        Me.DarkLabel2.AutoSize = True
        Me.DarkLabel2.Location = New System.Drawing.Point(9, 29)
        Me.DarkLabel2.Margin = New System.Windows.Forms.Padding(9, 6, 3, 0)
        Me.DarkLabel2.Name = "DarkLabel2"
        Me.DarkLabel2.Size = New System.Drawing.Size(70, 15)
        Me.DarkLabel2.TabIndex = 2
        Me.DarkLabel2.Text = "Description:"
        '
        'DarkRichTextBox1
        '
        Me.DarkRichTextBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(16, Byte), Integer), CType(CType(16, Byte), Integer), CType(CType(17, Byte), Integer))
        Me.DarkRichTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DarkRichTextBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DarkRichTextBox1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer), CType(CType(213, Byte), Integer))
        Me.DarkRichTextBox1.Location = New System.Drawing.Point(12, 47)
        Me.DarkRichTextBox1.Margin = New System.Windows.Forms.Padding(12, 3, 12, 12)
        Me.DarkRichTextBox1.Name = "DarkRichTextBox1"
        Me.DarkRichTextBox1.ReadOnly = True
        Me.DarkRichTextBox1.Size = New System.Drawing.Size(176, 312)
        Me.DarkRichTextBox1.TabIndex = 0
        Me.DarkRichTextBox1.Text = "This is a description of the text that has a very long line to see how the box ha" &
    "ndles wrapping to make sure my method will work" & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(10) & "here's a shorter line" & Global.Microsoft.VisualBasic.ChrW(10) & "v1.0"
        '
        'Timer1
        '
        Me.Timer1.Interval = 30
        '
        'ModLoader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7!, 15!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.DarkToolStrip2)
        Me.Controls.Add(Me.DarkToolStrip1)
        Me.CornerStyle = AltUI.Forms.DarkForm.CornerPreference.[Default]
        Me.Name = "ModLoader"
        Me.Text = "ModLoader"
        Me.TransparencyKey = System.Drawing.Color.FromArgb(CType(CType(31,Byte),Integer), CType(CType(31,Byte),Integer), CType(CType(32,Byte),Integer))
        Me.DarkToolStrip1.ResumeLayout(false)
        Me.DarkToolStrip1.PerformLayout
        Me.DarkToolStrip2.ResumeLayout(false)
        Me.DarkToolStrip2.PerformLayout
        Me.TableLayoutPanel1.ResumeLayout(false)
        Me.TableLayoutPanel1.PerformLayout
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents DarkToolStrip1 As DarkToolStrip
    Friend WithEvents ToolStripButton1 As ToolStripButton
    Friend WithEvents ListView1 As DoubleBufferedListView
    Friend WithEvents ToolStripSplitButton1 As ToolStripSplitButton
    Friend WithEvents ToolStripDropDownButton1 As ToolStripDropDownButton
    Friend WithEvents AaaToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AaaaToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripDropDownButton2 As ToolStripDropDownButton
    Friend WithEvents AddModToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EnableAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DisableAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenGameFolderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenModsFolderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents DarkToolStrip2 As DarkToolStrip
    Friend WithEvents ToolStripLabel2 As ToolStripLabel
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripLabel3 As ToolStripLabel
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents ModName As ColumnHeader
    Friend WithEvents Conflicts As ColumnHeader
    Friend WithEvents Priority As ColumnHeader
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents Timer1 As Timer
    Friend WithEvents DarkLabel1 As DarkLabel
    Friend WithEvents DarkLabel2 As DarkLabel
    Friend WithEvents DarkRichTextBox1 As DarkRichTextBox
    Friend WithEvents DarkLabel3 As DarkLabel
End Class
