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
        Me.DarkButton1 = New AltUI.Controls.DarkButton()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.DarkTreeView1 = New AltUI.Controls.DarkTreeView()
        Me.TableLayoutPanel1.SuspendLayout
        Me.SuspendLayout
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
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100!))
        Me.TableLayoutPanel1.Controls.Add(Me.DarkTreeView1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.DarkButton1, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(800, 450)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'DarkTreeView1
        '
        Me.DarkTreeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DarkTreeView1.Location = New System.Drawing.Point(3, 3)
        Me.DarkTreeView1.MaxDragChange = 20
        Me.DarkTreeView1.Name = "DarkTreeView1"
        Me.DarkTreeView1.Size = New System.Drawing.Size(794, 415)
        Me.DarkTreeView1.TabIndex = 1
        Me.DarkTreeView1.Text = "DarkTreeView1"
        '
        'GrpBrowser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7!, 15!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.CornerStyle = AltUI.Forms.DarkForm.CornerPreference.[Default]
        Me.Name = "GrpBrowser"
        Me.ShowIcon = false
        Me.ShowInTaskbar = false
        Me.Text = ".grp Browser"
        Me.TransparencyKey = System.Drawing.Color.FromArgb(CType(CType(31,Byte),Integer), CType(CType(31,Byte),Integer), CType(CType(32,Byte),Integer))
        Me.TableLayoutPanel1.ResumeLayout(false)
        Me.ResumeLayout(false)

End Sub

    Friend WithEvents DarkButton1 As AltUI.Controls.DarkButton
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents DarkTreeView1 As AltUI.Controls.DarkTreeView
End Class
