Imports System.IO.Compression
Public Class Form1
    Public fixdir As String = Application.StartupPath & "\Override\Fixes"
#Region " Move Form "
    Public MoveForm As Boolean
    Public MoveForm_MousePosition As Point
    Public Sub MoveForm_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown, PictureBox1.MouseDown, PictureBox2.MouseDown, PictureBox3.MouseDown, PictureBox4.MouseDown, PictureBox5.MouseDown
        If e.Button = MouseButtons.Left Then
            MoveForm = True
            MoveForm_MousePosition = e.Location
        End If
    End Sub
    Public Sub MoveForm_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove, PictureBox1.MouseMove, PictureBox2.MouseMove, PictureBox3.MouseMove, PictureBox4.MouseMove, PictureBox5.MouseMove
        If MoveForm Then
            Me.Location = Me.Location + (e.Location - MoveForm_MousePosition)
        End If
    End Sub
    Public Sub MoveForm_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp, PictureBox1.MouseUp, PictureBox2.MouseUp, PictureBox3.MouseUp, PictureBox4.MouseUp, PictureBox5.MouseUp
        If e.Button = MouseButtons.Left Then
            MoveForm = False
        End If
    End Sub
#End Region
    Private Sub Startup() Handles MyBase.Load
        Dim random As New Random
        Dim rand As String = random.Next(1, 9)
        PictureBox4.Parent = PictureBox5
        If rand = "1" Then
            PictureBox5.Image = My.Resources.BG1
        ElseIf rand = "2" Then
            PictureBox5.Image = My.Resources.BG2
        ElseIf rand = "3" Then
            PictureBox5.Image = My.Resources.BG3
        ElseIf rand = "4" Then
            PictureBox5.Image = My.Resources.BG4
        ElseIf rand = "5" Then
            PictureBox5.Image = My.Resources.BG5
        ElseIf rand = "6" Then
            PictureBox5.Image = My.Resources.BG6
        ElseIf rand = "7" Then
            PictureBox5.Image = My.Resources.BG7
        ElseIf rand = "8" Then
            PictureBox5.Image = My.Resources.BG8
        ElseIf rand = "9" Then
            PictureBox5.Image = My.Resources.BG9
        End If
        PictureBox5.SizeMode = PictureBoxSizeMode.StretchImage
        If Not IO.File.Exists(Application.StartupPath & "\F3.exe") Then
            MsgBox("Please put the launcher in the same directory as the game so you can launch it!", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "Game Executable Not Found!")
        ElseIf IO.Directory.Exists(fixdir) Then
            System.IO.Directory.Delete(fixdir, True)
        End If
        If IO.Directory.Exists(Application.StartupPath & "\Override\UnusedThings") Then
            IO.Directory.Delete(Application.StartupPath & "\Override\UnusedThings", True)
        End If
        IO.Directory.CreateDirectory(fixdir)
            IO.File.WriteAllBytes(fixdir & "\Fixes.zip", My.Resources.Fixes)
            IO.Compression.ZipFile.ExtractToDirectory(fixdir & "\Fixes.zip", fixdir)
            If IO.File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\Characters\None.CRT") Then
            IO.File.Delete(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\Characters\None.CRT")
        End If
        If IO.Directory.Exists(Application.StartupPath & "\Override\FemaleFix") Then
            IO.Directory.Delete(Application.StartupPath & "\Override\FemaleFix", True)
        End If
    End Sub
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        If Not IO.File.Exists(Application.StartupPath & "\F3.exe") Then
            MsgBox("Please put the launcher in the same directory as the game!", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "Game Executable Not Found!")
        Else
            Process.Start("F3.exe")
            Application.Exit()
        End If
    End Sub
    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        If Not IO.File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini") Then
            IO.File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini", My.Resources.Default_F3)
        End If
        MoveForm = False
        Form2.ShowDialog()
    End Sub
    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        Application.Exit()
    End Sub
End Class
