﻿Imports System.IO
Imports System.Media
Imports System.Runtime.InteropServices
Imports AltUI.Forms.DarkMessageBox

Public Class Main

#Region "Move Form"

    Private Const WM_NCLBUTTONDOWN As Integer = &HA1
    Private Const HT_CAPTION As Integer = &H2

    <DllImport("user32.dll")>
    Private Shared Sub SendMessage(ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer)
    End Sub

    <DllImport("user32.dll")>
    Private Shared Sub ReleaseCapture()
    End Sub

    Private Sub Form1_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Background.MouseDown, Logo.MouseDown
        If e.Button = MouseButtons.Left Then
            ReleaseCapture()
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0)
        End If
    End Sub

#End Region

    Private _mouseDown As Boolean
    Private ReadOnly sound As SoundPlayer = New SoundPlayer()

    Private Sub Startup(sender As Object, e As EventArgs) Handles MyBase.Load
        Logo.Parent = Background
        Background.BackgroundImage = My.Resources.ResourceManager.GetObject($"BG{New Random().Next(1, 13)}")
        If File.Exists("F3.exe") Then
            Directory.CreateDirectory("Mods")
            File.WriteAllBytes("Mods\Fixes.zip", My.Resources.Fixes)
            File.WriteAllBytes("Mods\SUMM.zip", My.Resources.SUMM)
            File.WriteAllBytes("granny2_x64.dll", My.Resources.granny2_x64) ' granny dll used for parsing game files
            AddFontResource("Fonts\TT0807M_.TTF")
            AddFontResource("Fonts\r_fallouty.ttf")
        Else
            ShowError("Please put the launcher in the same directory as the game so you can launch it!", "Game Executable Not Found!")
        End If

#If DEBUG Then
        Dim x = new GrpBrowser({"skl", "b3d"}, true)
        x.ShowDialog()
#End If

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

    Private Sub LaunchGame(sender As Object, e As EventArgs) Handles LaunchB.Click
        PlayButtonUp(sender, e)
        Try
            Process.Start("F3.exe")
            Close()
        Catch
            ShowError("Please put the launcher in the same directory as the game so you can launch it!", "Game Executable Not Found!")
        End Try
    End Sub

    Private Sub OpenOptions(sender As Object, e As EventArgs) Handles OptionsB.Click
        PlayButtonUp(sender, e)
        Options.ShowDialog()
    End Sub

    Private Sub ExitLauncher(sender As Object, e As EventArgs) Handles ExitB.Click
        PlayButtonUp(sender, e)
        Close()
    End Sub

    Private Sub EditorB_Click(sender As Object, e As EventArgs) Handles EditorB.Click
        PlayButtonUp(sender, e)
        Editor.Show()
    End Sub

    Private Sub PlayButtonDown(sender As Object, e As EventArgs) Handles LaunchB.MouseDown, OptionsB.MouseDown, ExitB.MouseDown, EditorB.MouseDown, ModLoaderB.MouseDown
        If Not _mouseDown Then
            _mouseDown = True
            sound.Stream = My.Resources.f3_button_down_01
            sound.Play()
        End If
    End Sub

    Private Sub PlayButtonUp(sender As Object, e As EventArgs) Handles LaunchB.MouseUp, OptionsB.MouseUp, ExitB.MouseUp, EditorB.MouseUp, ModLoaderB.MouseUp
        If _mouseDown Then
            _mouseDown = False
            sound.Stream = My.Resources.f3_button_up_01
            sound.Play()
        End If
    End Sub

    <DllImport("gdi32.dll")>
    Private Shared Sub AddFontResource(FontPath As String)
    End Sub

    Private Sub ModLoaderB_Click(sender As Object, e As EventArgs) Handles ModLoaderB.Click
        PlayButtonUp(sender, e)
        ModLoader.ShowDialog()
    End Sub

End Class