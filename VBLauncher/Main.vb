Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.InteropServices
Imports AltUI.Forms.DarkMessageBox

Public Class Main
    Public MoveForm As Boolean
    Public MoveFormMousePosition As Point

    Public Sub MoveForm_MouseDown(Sender As Object, E As MouseEventArgs) Handles Background.MouseDown, Logo.MouseDown
        If E.Button = MouseButtons.Left Then MoveForm = 1 : MoveFormMousePosition = E.Location
    End Sub

    Public Sub MoveForm_MouseMove(Sender As Object, E As MouseEventArgs) Handles Background.MouseMove, Logo.MouseMove
        If MoveForm Then Location = Location + (E.Location - MoveFormMousePosition)
    End Sub

    Public Sub MoveForm_MouseUp(Sender As Object, E As MouseEventArgs) Handles Background.MouseUp, Logo.MouseUp
        If E.Button = MouseButtons.Left Then MoveForm = 0
    End Sub

    Private Sub Startup() Handles MyBase.Load
        Icon = My.Resources.F3
        Height = 480
        LaunchB.Height = 29
        OptionsB.Height = 29
        ExitB.Height = 29
        Logo.Height = 150
        Background.Height = 480
        LaunchB.Location = New Point(13, 13)
        OptionsB.Location = New Point(13, 50)
        ExitB.Location = New Point(13, 87)
        Logo.Location = New Point(478, 317)
        DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
                              DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND, 8)
        LaunchB.BackgroundImage = My.Resources.Launch
        OptionsB.BackgroundImage = My.Resources.Options
        ExitB.BackgroundImage = My.Resources._Exit
        Logo.BackgroundImage = My.Resources.Logo
        Logo.Parent = Background
        Dim Random As New Random
        Select Case Random.Next(1, 12)
            Case 1 : Background.BackgroundImage = My.Resources.BG1
            Case 2 : Background.BackgroundImage = My.Resources.BG2
            Case 3 : Background.BackgroundImage = My.Resources.BG3
            Case 4 : Background.BackgroundImage = My.Resources.BG4
            Case 5 : Background.BackgroundImage = My.Resources.BG5
            Case 6 : Background.BackgroundImage = My.Resources.BG6
            Case 7 : Background.BackgroundImage = My.Resources.BG7
            Case 8 : Background.BackgroundImage = My.Resources.BG8
            Case 9 : Background.BackgroundImage = My.Resources.BG9
            Case 10 : Background.BackgroundImage = My.Resources.BG10
            Case 11 : Background.BackgroundImage = My.Resources.BG11
            Case 12 : Background.BackgroundImage = My.Resources.BG12
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
            ShowError("Please put the launcher in the same directory as the game so you can launch it!", "Game Executable Not Found!")
        End If
        If File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\Characters\None.CRT") Then _
            File.Delete(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\Characters\None.CRT")
    End Sub

    Private Shared Sub PictureBox1_Click() Handles LaunchB.Click
        Try
            Process.Start("F3.exe")
            Application.Exit()
        Catch Ex As Exception
            ShowError("Please put the launcher in the same directory as the game so you can launch it!", "Game Executable Not Found!")
        End Try
    End Sub

    Private Sub PictureBox2_Click() Handles OptionsB.Click
        If Not File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini") Then _
            File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini",
                              My.Resources.Default_F3)
        Options.ShowDialog()
    End Sub

    Private Shared Sub PictureBox3_Click() Handles ExitB.Click
        Application.Exit()
    End Sub

    <DllImport("gdi32.dll")>
    Public Shared Function AddFontResource(FontPath As String) As Integer
    End Function

    Public Enum DWMWINDOWATTRIBUTE
        DWMWA_WINDOW_CORNER_PREFERENCE = 33
    End Enum

    Public Enum DWM_WINDOW_CORNER_PREFERENCE
        DWMWCP_ROUND = 2
    End Enum

    <DllImport("dwmapi.dll")>
    Private Shared Function DwmSetWindowAttribute(hwnd As IntPtr, Attribute As DWMWINDOWATTRIBUTE,
                                                  ByRef pvAttribute As DWM_WINDOW_CORNER_PREFERENCE,
                                                  cbAttribute As UInteger) As Long
    End Function

End Class