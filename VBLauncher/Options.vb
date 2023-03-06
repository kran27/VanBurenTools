Imports System.IO
Imports VBLauncher.General
Imports VBLauncher.IniManager

Public Class Options
    Private ReadOnly MainMenus As MainMenuDef() = {MMD("mainmenu.map", 0, 5.5, 0, 2, 0.75, 59.2), MMD("zz_TestMapsaarontemp2.map", 0, 0, 0, 0, 0, 0), MMD("zz_TestMapsTest_City_Building01.map", 0, 5.5, 0, 45, -2.5, 57), MMD("zz_TestMapsTest_City_Building02.map", 0, 5.5, 0, 43, -2.5, 57), MMD("zz_TestMapsTest_City_Building03.map", 0, 5.5, 0, 43, -5, 57), MMD("zz_TestMapsTest_City_Building04.map", 0, 5.5, 0, 43, -5.5, 57), MMD("98_Canyon_Random_01.map", 50, 5, 10, 61, 0, 45), MMD("98_Canyon_Random_02.map", 55, 5, 10, 36, -2.5, 50), MMD("04_0202_Spelunking.map", 70, 5, 45, 15, 5, 50), MMD("zz_TestMapsTest_City_Fences.map", 0, 40, 0, 42, 35, 50), MMD("zz_TestMapsScottE_Test1.map", 85, 30, 30, 255, 39, 60), MMD("zz_TestMapsScottE_Test2.map", 145, 80, -85, 0.5, 25, 75), MMD("zz_TestMapsScottE_Test4.map", 0, 7.5, 0, 45, 12.5, 50), MMD("zz_TestMapsTest_Junktown_Shacks.map", 0, 50, -10, 42, 39, 50), MMD("Default_StartMap.map", 60, 7.5, 25, 270, 8, 27), MMD("00_03_Tutorial_Junktown.map", 80, 7.5, 50, 5, 10, 68), MMD("00_04_Tutorial_Vault.map", 50, 50.5, 0, 36, 25, 68)}
    Private ReadOnly Maps As String() = {"mainmenu.map", "zz_TestMapsaarontemp2.map", "zz_TestMapsTest_City_Building01.map", "zz_TestMapsTest_City_Building02.map", "zz_TestMapsTest_City_Building03.map", "zz_TestMapsTest_City_Building04.map", "98_Canyon_Random_01.map", "98_Canyon_Random_02.map", "04_0202_Spelunking.map", "zz_TestMapsTest_City_Fences.map", "zz_TestMapsScottE_Test1.map", "zz_TestMapsScottE_Test2.map", "zz_TestMapsScottE_Test4.map", "zz_TestMapsTest_Junktown_Shacks.map", "Default_StartMap.map", "00_03_Tutorial_Junktown.map", "00_04_Tutorial_Vault.map"}
    Private Shared Sub SetMainMenu(MMD As MainMenuDef)
        SysIni.Ini("Mainmenu", "map name", MMD.MapName)
        SysIni.Ini("Mainmenu", "target x", MMD.TargetX)
        SysIni.Ini("Mainmenu", "target y", MMD.TargetY)
        SysIni.Ini("Mainmenu", "target z", MMD.TargetZ)
        SysIni.Ini("Mainmenu", "azimuth", MMD.Azimuth)
        SysIni.Ini("Mainmenu", "elevation", MMD.Elevation)
        SysIni.Ini("Mainmenu", "fov", MMD.FOV)
    End Sub

    Private Shared Function MMD(MapName As String, TargetX As String, TargetY As String, TargetZ As String, Azimuth As String, Elevation As String, FOV As String) As MainMenuDef
        Return New MainMenuDef With {.MapName = MapName, .TargetX = TargetX, .TargetY = TargetY, .TargetZ = TargetZ, .Azimuth = Azimuth, .Elevation = Elevation, .FOV = FOV}
    End Function

    Private Sub CheckOptions() Handles MyBase.Load
        IntrosCB.Checked = F3Ini.Ini("Graphics", "enable startup movies") = 1
        MainMenuCB.SelectedIndex = Array.IndexOf(Maps, SysIni.Ini("Mainmenu", "map name"))
        CameraCB.Checked = SysIni.Ini("Camera", "FOV Min") = 0.5d
        AltCamCB.Checked = SysIni.Ini("Camera", "Distance Max") = 70
        Dim Files = SearchForFiles("Override", {"*.map"})
        For Each File In Files
            Dim FI As New FileInfo(File)
            If Not NewGameCB.Items.Contains(FI.Name) Then NewGameCB.Items.Add(FI.Name)
        Next
        NewGameCB.Text = SysIni.Ini("Server", "Start map")
    End Sub

    Private Sub ApplyChanges() Handles ApplyB.Click
        F3Ini.Ini("Graphics", "enable startup movies", If(IntrosCB.Checked, 1, 0), KeyType.Normal)
        SetMainMenu(MainMenus(MainMenuCB.SelectedIndex))
        SysIni.Ini("Camera", "Distance Max", If(AltCamCB.Checked, 70, 350), KeyType.Normal)
        SysIni.Ini("Camera", "Distance Min", If(AltCamCB.Checked, 70, 350), KeyType.Normal)
        SysIni.Ini("Camera", "FOV Speed", If(CameraCB.Checked, 10, 32.5), KeyType.Normal)
        SysIni.Ini("Camera", "Scroll Speed", If(CameraCB.Checked, 250, 96), KeyType.Normal)
        SysIni.Ini("Camera", "FOV Min", If(CameraCB.Checked, 0.5, If(AltCamCB.Checked, 30, 6)), KeyType.Normal)
        SysIni.Ini("Camera", "FOV Max", If(CameraCB.Checked, 100, If(AltCamCB.Checked, 75, 15)), KeyType.Normal)
        SysIni.Ini("Server", "Start map", NewGameCB.Text, KeyType.Normal)
        File.WriteAllLines(SysDir, SysIni)
        File.WriteAllLines(F3Dir, F3Ini)
        Hide()
    End Sub

    Private Sub OpenVideoOptions() Handles VideoB.Click
        VideoOptions.ShowDialog()
    End Sub

    Private Sub NewGameToolTip() Handles NewGameCB.SelectedIndexChanged
        ToolTip1.SetToolTip(NewGameCB, NewGameCB.Text)
    End Sub

    Private Sub OpenKeybinds(sender As Object, e As EventArgs) Handles KeybindB.Click
        KeybindEditor.ShowDialog()
    End Sub
End Class

Public Class MainMenuDef
    Public Property MapName As String
    Public Property TargetX As String
    Public Property TargetY As String
    Public Property TargetZ As String
    Public Property Azimuth As String
    Public Property Elevation As String
    Public Property FOV As String
End Class