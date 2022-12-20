Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.InteropServices
Imports AltUI.Forms.DarkMessageBox
Public Class Main
#Region "Move Form"
    Public Const WM_NCLBUTTONDOWN As Integer = &HA1
    Public Const HT_CAPTION As Integer = &H2

    <DllImport("user32.dll")>
    Public Shared Sub SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer)
    End Sub

    <DllImport("user32.dll")>
    Public Shared Sub ReleaseCapture()
    End Sub

    Private Sub Form1_MouseDown(ByVal sender As Object, ByVal e As Windows.Forms.MouseEventArgs) Handles Background.MouseDown, Logo.MouseDown
        If e.Button = MouseButtons.Left Then
            ReleaseCapture()
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0)
        End If
    End Sub
#End Region
    Private ReadOnly BGs As Bitmap() = {My.Resources.BG1, My.Resources.BG2, My.Resources.BG3, My.Resources.BG4, My.Resources.BG5, My.Resources.BG6, My.Resources.BG7, My.Resources.BG8, My.Resources.BG9, My.Resources.BG10, My.Resources.BG11, My.Resources.BG12}
    Private Sub Startup() Handles MyBase.Load
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
        if file.Exists("VBEditor.exe")
            EditorB.Show()
            ExitB.Location = new Point(13, 161)
            Else 
            EditorB.Hide()
        End If
        AllowTransparency = False
        Try : File.Delete($"{My.Computer.FileSystem.SpecialDirectories.MyDocuments}\F3\Characters\None.CRT")
        Catch : End Try
        Try
            F3Ini = File.ReadAllLines(F3Dir)
        Catch
            Directory.CreateDirectory($"{My.Computer.FileSystem.SpecialDirectories.MyDocuments}\F3")
            File.WriteAllText(F3Dir, My.Resources.Default_F3)
            F3Ini = File.ReadAllLines(F3Dir)
        End Try
        Try
            SysIni = File.ReadAllLines(SysDir)
        Catch
            Directory.CreateDirectory("Override\MenuMap\Engine")
            File.WriteAllText(SysDir, My.Resources.Default_sys)
            SysIni = File.ReadAllLines(SysDir)
        End Try
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

    Private Sub OpenKeybinds(sender As Object, e As EventArgs) Handles KeybindB.Click
        KeybindEditor.ShowDialog()
    End Sub

    Private Sub EditorB_Click(sender As Object, e As EventArgs) Handles EditorB.Click
        Process.Start("VBEditor.exe")
    End Sub
End Class