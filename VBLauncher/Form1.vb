Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.InteropServices

Public Class Form1
    Public FixDir As String = Application.StartupPath & "\Override\Fixes"
    Public MoveForm As Boolean
    Public MoveFormMousePosition As Point

    Public Sub MoveForm_MouseDown(Sender As Object, E As MouseEventArgs) Handles PictureBox4.MouseDown, PictureBox5.MouseDown
        If E.Button = MouseButtons.Left Then MoveForm = 1 : MoveFormMousePosition = E.Location
    End Sub

    Public Sub MoveForm_MouseMove(Sender As Object, E As MouseEventArgs) Handles PictureBox4.MouseMove, PictureBox5.MouseMove
        If MoveForm Then Location = Location + (E.Location - MoveFormMousePosition)
    End Sub

    Public Sub MoveForm_MouseUp(Sender As Object, E As MouseEventArgs) Handles PictureBox4.MouseUp, PictureBox5.MouseUp
        If E.Button = MouseButtons.Left Then MoveForm = 0
    End Sub

    Private Sub Startup() Handles MyBase.Load
        PictureBox4.Parent = PictureBox5
        Dim Random As New Random
        Select Case Random.Next(1, 9)
            Case 1 : PictureBox5.Image = My.Resources.BG1
            Case 2 : PictureBox5.Image = My.Resources.BG2
            Case 3 : PictureBox5.Image = My.Resources.BG3
            Case 4 : PictureBox5.Image = My.Resources.BG4
            Case 5 : PictureBox5.Image = My.Resources.BG5
            Case 6 : PictureBox5.Image = My.Resources.BG6
            Case 7 : PictureBox5.Image = My.Resources.BG7
            Case 8 : PictureBox5.Image = My.Resources.BG8
            Case 9 : PictureBox5.Image = My.Resources.BG9
        End Select
        If Directory.Exists(FixDir) Then Directory.Delete(FixDir, 1)
        If File.Exists(Application.StartupPath & "\F3.exe") Then
            Directory.CreateDirectory(FixDir)
            File.WriteAllBytes(FixDir & "\Fixes.zip", My.Resources.Fixes)
            ZipFile.ExtractToDirectory(FixDir & "\Fixes.zip", FixDir)
            File.Delete(FixDir & "\Fixes.zip")
            If Not File.Exists(Application.StartupPath & "\dgVoodoo.conf") Then _
                File.WriteAllBytes(Application.StartupPath & "\dgVoodoo.conf", My.Resources.dgV2conf)
            AddFontResource(Application.StartupPath & "\Fonts\TT0807M_.TTF")
            AddFontResource(Application.StartupPath & "\Fonts\r_fallouty.ttf")
        Else
            MsgBox("Please put the launcher in the same directory as the game so you can launch it!",
                   MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "Game Executable Not Found!")
        End If
        If Directory.Exists(Application.StartupPath & "\Override\UnusedThings") Then Directory.Delete(Application.StartupPath & "\Override\UnusedThings", 1)
        If File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\Characters\None.CRT") Then _
            File.Delete(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\Characters\None.CRT")
        If Directory.Exists(Application.StartupPath & "\Override\FemaleFix") Then Directory.Delete(Application.StartupPath & "\Override\FemaleFix", 1)
    End Sub

    Private Sub PictureBox1_Click() Handles PictureBox1.Click
        If Not File.Exists(Application.StartupPath & "\F3.exe") Then
            MsgBox("Please put the launcher in the same directory as the game!", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "Game Executable Not Found!")
        Else
            Process.Start("F3.exe")
            Application.Exit()
        End If
    End Sub

    Private Sub PictureBox2_Click() Handles PictureBox2.Click
        If Not File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini") Then _
            File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini",
                              My.Resources.Default_F3)
        Form2.ShowDialog()
    End Sub

    Private Sub PictureBox3_Click() Handles PictureBox3.Click
        Application.Exit()
    End Sub

    <DllImport("gdi32.dll")>
    Public Shared Function AddFontResource(ByVal FontPath As String) As Integer
    End Function

End Class