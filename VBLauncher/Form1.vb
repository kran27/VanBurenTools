Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.InteropServices

Public Class Form1
    Public MoveForm As Boolean
    Public MoveFormMousePosition As Point

    Public Sub MoveForm_MouseDown(Sender As Object, E As MouseEventArgs) Handles PictureBox5.MouseDown, PictureBox4.MouseDown
        If E.Button = MouseButtons.Left Then MoveForm = 1 : MoveFormMousePosition = E.Location
    End Sub

    Public Sub MoveForm_MouseMove(Sender As Object, E As MouseEventArgs) Handles PictureBox5.MouseMove, PictureBox4.MouseMove
        If MoveForm Then Location = Location + (E.Location - MoveFormMousePosition)
    End Sub

    Public Sub MoveForm_MouseUp(Sender As Object, E As MouseEventArgs) Handles PictureBox5.MouseUp, PictureBox4.MouseUp
        If E.Button = MouseButtons.Left Then MoveForm = 0
    End Sub

    Private Sub Startup() Handles MyBase.Load
        Icon = My.Resources.F3
        DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
                              DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DEFAULT, 0)
        PictureBox1.BackgroundImage = My.Resources.Launch
        PictureBox2.BackgroundImage = My.Resources.Options
        PictureBox3.BackgroundImage = My.Resources._Exit
        PictureBox4.BackgroundImage = My.Resources.Logo
        PictureBox4.Parent = PictureBox5
        Dim Random As New Random
        Select Case Random.Next(1, 12)
            Case 1 : PictureBox5.BackgroundImage = My.Resources.BG1
            Case 2 : PictureBox5.BackgroundImage = My.Resources.BG2
            Case 3 : PictureBox5.BackgroundImage = My.Resources.BG3
            Case 4 : PictureBox5.BackgroundImage = My.Resources.BG4
            Case 5 : PictureBox5.BackgroundImage = My.Resources.BG5
            Case 6 : PictureBox5.BackgroundImage = My.Resources.BG6
            Case 7 : PictureBox5.BackgroundImage = My.Resources.BG7
            Case 8 : PictureBox5.BackgroundImage = My.Resources.BG8
            Case 9 : PictureBox5.BackgroundImage = My.Resources.BG9
            Case 10 : PictureBox5.BackgroundImage = My.Resources.BG10
            Case 11 : PictureBox5.BackgroundImage = My.Resources.BG11
            Case 12 : PictureBox5.BackgroundImage = My.Resources.BG12
        End Select
        Try : Directory.Delete("Override\Fixes", 1) : Catch : End Try
        If File.Exists("F3.exe") Then
            Directory.CreateDirectory("Override\Fixes")
            File.WriteAllBytes("Override\Fixes\Fixes.zip", My.Resources.Fixes)
            ZipFile.ExtractToDirectory("Override\Fixes\Fixes.zip", "Override\Fixes")
            File.Delete("Override\Fixes\Fixes.zip")
            AddFontResource("Fonts\TT0807M_.TTF")
            AddFontResource("Fonts\r_fallouty.ttf")
        Else
            Form4.ShowError("Please put the launcher in the same directory as the game so you can launch it!", "Game Executable Not Found!")
        End If
        If File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\Characters\None.CRT") Then _
            File.Delete(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\Characters\None.CRT")
    End Sub

    Private Shared Sub PictureBox1_Click() Handles PictureBox1.Click
        Try
            Process.Start("F3.exe")
            Application.Exit()
        Catch Ex As Exception
            Form4.ShowError("Please put the launcher in the same directory as the game so you can launch it!", "Game Executable Not Found!")
        End Try
    End Sub

    Private Sub PictureBox2_Click() Handles PictureBox2.Click
        If Not File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini") Then _
            File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini",
                              My.Resources.Default_F3)
        Form2.ShowDialog()
    End Sub

    Private Shared Sub PictureBox3_Click() Handles PictureBox3.Click
        Application.Exit()
    End Sub

    <DllImport("gdi32.dll")>
    Public Shared Function AddFontResource(FontPath As String) As Integer
    End Function

    Public Enum DWMWINDOWATTRIBUTE
        DWMWA_WINDOW_CORNER_PREFERENCE = 33
    End Enum

    Public Enum DWM_WINDOW_CORNER_PREFERENCE
        DWMWCP_DEFAULT = 0
    End Enum

    <DllImport("dwmapi.dll")>
    Private Shared Function DwmSetWindowAttribute(hwnd As IntPtr, Attribute As DWMWINDOWATTRIBUTE,
                                                  ByRef pvAttribute As DWM_WINDOW_CORNER_PREFERENCE,
                                                  cbAttribute As UInteger)
    End Function

End Class