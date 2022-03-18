Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.InteropServices
Imports AltUI.Forms.DarkMessageBox
Imports VBLauncher.PShared

Public Class Main
#Region "Move Form"
    Private MoveForm As Boolean
    Private MoveFormMousePosition As Point

    Private Sub MoveForm_MouseDown(Sender As Object, E As MouseEventArgs) Handles Background.MouseDown, Logo.MouseDown
        If E.Button = MouseButtons.Left Then MoveForm = 1 : MoveFormMousePosition = E.Location
    End Sub

    Private Sub MoveForm_MouseMove(Sender As Object, E As MouseEventArgs) Handles Background.MouseMove, Logo.MouseMove
        If MoveForm Then Location = Location + (E.Location - MoveFormMousePosition)
    End Sub

    Private Sub MoveForm_MouseUp(Sender As Object, E As MouseEventArgs) Handles Background.MouseUp, Logo.MouseUp
        If E.Button = MouseButtons.Left Then MoveForm = 0
    End Sub
#End Region
    Private Sub Startup() Handles MyBase.Load
        Icon = My.Resources.F3
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
        Try : File.Delete(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\Characters\None.CRT")
        Catch : End Try
        If Not File.Exists(F3Dir) Then
            File.WriteAllText(F3Dir, My.Resources.Default_F3)
        End If
    End Sub

    Private Shared Sub LaunchGame() Handles LaunchB.Click
        Try
            Process.Start("F3.exe")
            Application.Exit()
        Catch Ex As Exception
            ShowError("Please put the launcher in the same directory as the game so you can launch it!", "Game Executable Not Found!")
        End Try
    End Sub

    Private Sub OpenOptions() Handles OptionsB.Click
        If Not File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini") Then _
            File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini",
                              My.Resources.Default_F3)
        Options.ShowDialog()
    End Sub

    Private Sub ExitLauncher() Handles ExitB.Click
        Application.Exit()
    End Sub

    <DllImport("gdi32.dll")>
    Private Shared Function AddFontResource(FontPath As String) As Integer
    End Function

End Class