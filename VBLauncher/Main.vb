Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.InteropServices
Imports AltUI.Forms.DarkMessageBox
Public Class Main
#Region "Move Form"
    Private MoveForm As Boolean
    Private MoveFormMousePosition As Point

    Private Sub MoveForm_MouseDown(Sender As Object, E As MouseEventArgs) Handles Background.MouseDown, Logo.MouseDown
        If E.Button = MouseButtons.Left Then
            MoveForm = 1
            MoveFormMousePosition = E.Location
        End If
    End Sub

    Private Sub MoveForm_MouseMove(Sender As Object, E As MouseEventArgs) Handles Background.MouseMove, Logo.MouseMove
        If MoveForm Then Location = Location + (E.Location - MoveFormMousePosition)
    End Sub

    Private Sub MoveForm_MouseUp(Sender As Object, E As MouseEventArgs) Handles Background.MouseUp, Logo.MouseUp
        If E.Button = MouseButtons.Left Then MoveForm = 0
    End Sub
#End Region
    Private ReadOnly BGs As Bitmap() = {My.Resources.BG1, My.Resources.BG2, My.Resources.BG3, My.Resources.BG4, My.Resources.BG5, My.Resources.BG6, My.Resources.BG7, My.Resources.BG8, My.Resources.BG9, My.Resources.BG10, My.Resources.BG11, My.Resources.BG12}
    Private Sub Startup() Handles MyBase.Load
        Icon = My.Resources.F3
        LaunchB.BackgroundImage = My.Resources.Launch
        OptionsB.BackgroundImage = My.Resources.Options
        ExitB.BackgroundImage = My.Resources._Exit
        Logo.BackgroundImage = My.Resources.Logo
        Logo.Parent = Background
        Background.BackgroundImage = BGs(New Random().Next(0, BGs.Length))
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
        AllowTransparency = False
        Try : File.Delete($"{My.Computer.FileSystem.SpecialDirectories.MyDocuments}\F3\Characters\None.CRT")
        Catch : End Try
    End Sub

    Private Shared Sub LaunchGame() Handles LaunchB.Click
        Try
            Process.Start("F3.exe")
            Application.Exit()
        Catch
            ShowError("Please put the launcher in the same directory as the game so you can launch it!", "Game Executable Not Found!")
        End Try
    End Sub

    Private Sub OpenOptions() Handles OptionsB.Click
        Options.ShowDialog()
    End Sub

    Private Sub ExitLauncher() Handles ExitB.Click
        Application.Exit()
    End Sub

    <DllImport("gdi32.dll")>
    Private Shared Sub AddFontResource(FontPath As String)
    End Sub

End Class