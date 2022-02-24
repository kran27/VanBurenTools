Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports DarkUI2.Forms
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()>
Partial Class Form4
    Inherits DarkMessageBox

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
        Me.SuspendLayout()
        '
        'btnCancel
        '
        Me.btnCancel.Location = New Point(12, 12)
        '
        'btnClose
        '
        Me.btnClose.Location = New Point(12, 12)
        '
        'btnYes
        '
        Me.btnYes.Location = New Point(12, 12)
        '
        'btnNo
        '
        Me.btnNo.Location = New Point(12, 12)
        '
        'btnRetry
        '
        Me.btnRetry.Location = New Point(452, 12)
        '
        'btnIgnore
        '
        Me.btnIgnore.Location = New Point(452, 12)
        '
        'Form4
        '
        Me.AutoScaleDimensions = New SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(800, 450)
        Me.Name = "Form4"
        Me.Text = "Form4"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

End Class
