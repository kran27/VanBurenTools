Imports System.IO
Imports VBLauncher.General
Imports VBLauncher.IniManager

Public Class Options
    Private ReadOnly HelmType As String = "Override\Helmet\Helmet.type"
    Private ReadOnly HelmInv As String = "Override\Helmet\Interface\HeaMotorcycle_default_INV.tga"
    Private ReadOnly HelmTex As String = "Override\Helmet\Critters\HeaMotorcycle_default_LG.png"
    Private ReadOnly MainMenus As MainMenuDef() = {MMD("mainmenu.map", 0, 5.5, 0, 2, 0.75, 59.2), MMD("zz_TestMapsaarontemp2.map", 0, 0, 0, 0, 0, 0), MMD("zz_TestMapsTest_City_Building01.map", 0, 5.5, 0, 45, -2.5, 57), MMD("zz_TestMapsTest_City_Building02.map", 0, 5.5, 0, 43, -2.5, 57), MMD("zz_TestMapsTest_City_Building03.map", 0, 5.5, 0, 43, -5, 57), MMD("zz_TestMapsTest_City_Building04.map", 0, 5.5, 0, 43, -5.5, 57), MMD("98_Canyon_Random_01.map", 50, 5, 10, 61, 0, 45), MMD("98_Canyon_Random_02.map", 55, 5, 10, 36, -2.5, 50), MMD("04_0202_Spelunking.map", 70, 5, 45, 15, 5, 50), MMD("zz_TestMapsTest_City_Fences.map", 0, 40, 0, 42, 35, 50), MMD("zz_TestMapsScottE_Test1.map", 85, 30, 30, 255, 39, 60), MMD("zz_TestMapsScottE_Test2.map", 145, 80, -85, 0.5, 25, 75), MMD("zz_TestMapsScottE_Test4.map", 0, 7.5, 0, 45, 12.5, 50), MMD("zz_TestMapsTest_Junktown_Shacks.map", 0, 50, -10, 42, 39, 50), MMD("Default_StartMap.map", 60, 7.5, 25, 270, 8, 27), MMD("00_03_Tutorial_Junktown.map", 80, 7.5, 50, 5, 10, 68), MMD("00_04_Tutorial_Vault.map", 50, 50.5, 0, 36, 25, 68)}
    Private ReadOnly Maps As String() = {"mainmenu.map", "zz_TestMapsaarontemp2.map", "zz_TestMapsTest_City_Building01.map", "zz_TestMapsTest_City_Building02.map", "zz_TestMapsTest_City_Building03.map", "zz_TestMapsTest_City_Building04.map", "98_Canyon_Random_01.map", "98_Canyon_Random_02.map", "04_0202_Spelunking.map", "zz_TestMapsTest_City_Fences.map", "zz_TestMapsScottE_Test1.map", "zz_TestMapsScottE_Test2.map", "zz_TestMapsScottE_Test4.map", "zz_TestMapsTest_Junktown_Shacks.map", "Default_StartMap.map", "00_03_Tutorial_Junktown.map", "00_04_Tutorial_Vault.map"}
    Private ReadOnly Icons As Byte()() = {My.Resources._8_Ball_I, My.Resources.AmericanI, My.Resources.BlackI, My.Resources.BlueI, My.Resources.EyeI, My.Resources.FlamesI, My.Resources.Full_SkullI, My.Resources.GreenI, My.Resources.GreyI, My.Resources.PoliceI, My.Resources.RedI, My.Resources.Shot_SmileyI, My.Resources.SkullI, My.Resources.YellowI}
    Private ReadOnly Textures As Byte()() = {My.Resources._8_Ball, My.Resources.American, My.Resources.Black, My.Resources.Blue, My.Resources.Eye, My.Resources.Flames, My.Resources.Full_Skull, My.Resources.Green, My.Resources.Grey, My.Resources.Police, My.Resources.Red, My.Resources.Shot_Smiley, My.Resources.Skull, My.Resources.Yellow}
    Private ReadOnly PIcons As Bitmap() = {My.Resources.Default_Icon, My.Resources._8_Ball_Icon, My.Resources.American_Icon, My.Resources.Black_Icon, My.Resources.Blue_Icon, My.Resources.Eye_Icon, My.Resources.Flames_Icon, My.Resources.Full_Skull_Icon, My.Resources.Green_Icon, My.Resources.Grey_Icon, My.Resources.Police_Icon, My.Resources.Red_Icon, My.Resources.Shot_Smiley_Icon, My.Resources.Skull_Icon, My.Resources.Yellow_Icon}
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
        ButtonsCB.Checked = Directory.Exists("Override\SUMM")
        MainMenuCB.SelectedIndex = Array.IndexOf(Maps, SysIni.Ini("Mainmenu", "map name"))
        CameraCB.Checked = SysIni.Ini("Camera", "FOV Min") = 0.5
        AltCamCB.Checked = SysIni.Ini("Camera", "Distance Max") = 70
        Dim Files = SearchForFiles("Override", {"*.map"})
        For Each File In Files
            Dim FI As New FileInfo(File)
            If Not NewGameCB.Items.Contains(FI.Name) Then NewGameCB.Items.Add(FI.Name)
        Next
        NewGameCB.Text = SysIni.Ini("Server", "Start map")
        Try : HelmetCB.SelectedItem = File.ReadAllText(HelmType)
        Catch : HelmetCB.SelectedIndex = 0 : End Try
    End Sub

    Private Sub ApplyChanges() Handles ApplyB.Click
        F3Ini.Ini("Graphics", "enable startup movies", If(IntrosCB.Checked, 1, 0))
        If ButtonsCB.Checked Then
            Directory.CreateDirectory("Override\SUMM\Interface")
            File.WriteAllBytes("Override\SUMM\Interface\Mainmenu.int", My.Resources.Mainmenu)
            File.WriteAllBytes("Override\SUMM\Interface\f3_front_end_buttons.tga", My.Resources.f3_front_end_buttons)
        Else
            Try : Directory.Delete("Override\SUMM", 1) : Catch : End Try
        End If
        SetMainMenu(MainMenus(MainMenuCB.SelectedIndex))
        SysIni.Ini("Camera", "Distance Max", If(AltCamCB.Checked, 70, 350))
        SysIni.Ini("Camera", "Distance Min", If(AltCamCB.Checked, 70, 350))
        SysIni.Ini("Camera", "FOV Speed", If(CameraCB.Checked, 10, 32.5))
        SysIni.Ini("Camera", "Scroll Speed", If(CameraCB.Checked, 250, 96))
        SysIni.Ini("Camera", "FOV Min", If(CameraCB.Checked, 0.5, If(AltCamCB.Checked, 30, 6)))
        SysIni.Ini("Camera", "FOV Max", If(CameraCB.Checked, 100, If(AltCamCB.Checked, 75, 15)))
        SysIni.Ini("Server", "Start map", NewGameCB.Text)
        Try : Directory.Delete("Override\Helmet", 1) : Catch : End Try
        If Not HelmetCB.SelectedIndex = 0 Then
            Directory.CreateDirectory("Override\Helmet\Critters")
            Directory.CreateDirectory("Override\Helmet\Interface")
            File.WriteAllText(HelmType, HelmetCB.Text)
            File.WriteAllBytes(HelmInv, Icons(HelmetCB.SelectedIndex + 1)) : File.WriteAllBytes(HelmTex, Textures(HelmetCB.SelectedIndex + 1))
        End If
        File.WriteAllLines(SysDir, SysIni)
        File.WriteAllLines(F3Dir, F3Ini)
        Hide()
    End Sub

    Private Sub HelmetPreview() Handles HelmetCB.SelectedIndexChanged
        HelmetPI.Image = PIcons(HelmetCB.SelectedIndex)
    End Sub

    Private Sub OpenVideoOptions() Handles VideoB.Click
        VideoOptions.ShowDialog()
    End Sub

    Private Sub NewGameToolTip() Handles NewGameCB.SelectedIndexChanged
        ToolTip1.SetToolTip(NewGameCB, NewGameCB.Text)
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