Imports System.IO
Imports VBLauncher.General
Imports VBLauncher.IniManager

Public Class Options
    Private ReadOnly HelmType As String = "Override\Helmet\Helmet.type"
    Private ReadOnly HelmInv As String = "Override\Helmet\Interface\HeaMotorcycle_default_INV.tga"
    Private ReadOnly HelmTex As String = "Override\Helmet\Critters\HeaMotorcycle_default_LG.png"

    Private Sub SetMainMenu(MapName As String, TargetX As String, TargetY As String, TargetZ As String,
                             Azimuth As String, Elevation As String, FOV As String)
        Ini(SysIni, "Mainmenu", "map name", MapName)
        Ini(SysIni, "Mainmenu", "target x", TargetX)
        Ini(SysIni, "Mainmenu", "target y", TargetY)
        Ini(SysIni, "Mainmenu", "target z", TargetZ)
        Ini(SysIni, "Mainmenu", "azimuth", Azimuth)
        Ini(SysIni, "Mainmenu", "elevation", Elevation)
        Ini(SysIni, "Mainmenu", "fov", FOV)
    End Sub

    Private Sub CheckOptions() Handles MyBase.Load
        Icon = My.Resources.F3
        Try
            F3Ini = File.ReadAllLines(F3Dir)
        Catch
            Directory.CreateDirectory(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3")
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
        IntrosCB.Checked = Ini(F3Ini, "Graphics", "enable startup movies") = 1
        ButtonsCB.Checked = Directory.Exists("Override\SUMM")
        Select Case Ini(SysIni, "Mainmenu", "map name")
            Case "mainmenu.map" : MainMenuCB.SelectedIndex = 0
            Case "zz_TestMapsaarontemp2.map" : MainMenuCB.SelectedIndex = 1
            Case "zz_TestMapsTest_City_Building01.map" : MainMenuCB.SelectedIndex = 2
            Case "zz_TestMapsTest_City_Building02.map" : MainMenuCB.SelectedIndex = 3
            Case "zz_TestMapsTest_City_Building03.map" : MainMenuCB.SelectedIndex = 4
            Case "zz_TestMapsTest_City_Building04.map" : MainMenuCB.SelectedIndex = 5
            Case "98_Canyon_Random_01.map" : MainMenuCB.SelectedIndex = 6
            Case "98_Canyon_Random_02.map" : MainMenuCB.SelectedIndex = 7
            Case "04_0202_Spelunking.map" : MainMenuCB.SelectedIndex = 8
            Case "zz_TestMapsTest_City_Fences.map" : MainMenuCB.SelectedIndex = 9
            Case "zz_TestMapsScottE_Test1.map" : MainMenuCB.SelectedIndex = 10
            Case "zz_TestMapsScottE_Test2.map" : MainMenuCB.SelectedIndex = 11
            Case "zz_TestMapsScottE_Test4.map" : MainMenuCB.SelectedIndex = 12
            Case "zz_TestMapsTest_Junktown_Shacks.map" : MainMenuCB.SelectedIndex = 13
            Case "Default_StartMap.map" : MainMenuCB.SelectedIndex = 14
            Case "00_03_Tutorial_Junktown.map" : MainMenuCB.SelectedIndex = 15
            Case "00_04_Tutorial_Vault.map" : MainMenuCB.SelectedIndex = 16
        End Select
        CameraCB.Checked = Ini(SysIni, "Camera", "FOV Min") = 0.5
        Dim Files = SearchForFiles("Override", {"*.map"})
        For Each File As Object In Files
            Dim FI As New FileInfo(File)
            If Not NewGameCB.Items.Contains(FI.Name) Then
                NewGameCB.Items.Add(FI.Name)
            End If
        Next
        NewGameCB.Text = Ini(SysIni, "Server", "Start map")
        Try : HelmetCB.SelectedItem = File.ReadAllText(HelmType)
        Catch : HelmetCB.SelectedIndex = 0 : End Try
    End Sub

    Private Sub ApplyChanges() Handles ApplyB.Click
        Ini(F3Ini, "Graphics", "enable startup movies", If(IntrosCB.Checked, 1, 0))
        If ButtonsCB.Checked Then
            Directory.CreateDirectory("Override\SUMM\Interface")
            File.WriteAllBytes("Override\SUMM\Interface\Mainmenu.int", My.Resources.Mainmenu)
            File.WriteAllBytes("Override\SUMM\Interface\f3_front_end_buttons.tga", My.Resources.f3_front_end_buttons)
        Else
            Try : Directory.Delete("Override\SUMM", 1) : Catch : End Try
        End If
        Select Case MainMenuCB.SelectedIndex
            Case 0 : SetMainMenu("mainmenu.map", 0, 5.5, 0, 2, 0.75, 59.2)
            Case 1 : SetMainMenu("zz_TestMapsaarontemp2.map", 0, 0, 0, 0, 0, 0)
            Case 2 : SetMainMenu("zz_TestMapsTest_City_Building01.map", 0, 5.5, 0, 45, -2.5, 57)
            Case 3 : SetMainMenu("zz_TestMapsTest_City_Building02.map", 0, 5.5, 0, 43, -2.5, 57)
            Case 4 : SetMainMenu("zz_TestMapsTest_City_Building03.map", 0, 5.5, 0, 43, -5, 57)
            Case 5 : SetMainMenu("zz_TestMapsTest_City_Building04.map", 0, 5.5, 0, 43, -5.5, 57)
            Case 6 : SetMainMenu("98_Canyon_Random_01.map", 50, 5, 10, 61, 0, 45)
            Case 7 : SetMainMenu("98_Canyon_Random_02.map", 55, 5, 10, 36, -2.5, 50)
            Case 8 : SetMainMenu("04_0202_Spelunking.map", 70, 5, 45, 15, 5, 50)
            Case 9 : SetMainMenu("zz_TestMapsTest_City_Fences.map", 0, 40, 0, 42, 35, 50)
            Case 10 : SetMainMenu("zz_TestMapsScottE_Test1.map", 85, 30, 30, 255, 39, 60)
            Case 11 : SetMainMenu("zz_TestMapsScottE_Test2.map", 145, 80, -85, 0.5, 25, 75)
            Case 12 : SetMainMenu("zz_TestMapsScottE_Test4.map", 0, 7.5, 0, 45, 12.5, 50)
            Case 13 : SetMainMenu("zz_TestMapsTest_Junktown_Shacks.map", 0, 50, -10, 42, 39, 50)
            Case 14 : SetMainMenu("Default_StartMap.map", 60, 7.5, 25, 270, 8, 27)
            Case 15 : SetMainMenu("00_03_Tutorial_Junktown.map", 80, 7.5, 50, 5, 10, 68)
            Case 16 : SetMainMenu("00_04_Tutorial_Vault.map", 50, 50.5, 0, 36, 25, 68)
        End Select
        If CameraCB.Checked Then
            Ini(SysIni, "Camera", "FOV Speed", 10)
            Ini(SysIni, "Camera", "Scroll Speed", 250)
            Ini(SysIni, "Camera", "FOV Min", 0.5)
            Ini(SysIni, "Camera", "FOV Max", 100)
        Else
            Ini(SysIni, "Camera", "FOV Speed", 6.5)
            Ini(SysIni, "Camera", "Scroll Speed", 96)
            Ini(SysIni, "Camera", "FOV Min", 6)
            Ini(SysIni, "Camera", "FOV Max", 15)
        End If
        Ini(SysIni, "Server", "Start map", NewGameCB.Text)
        Try : Directory.Delete("Override\Helmet", 1) : Catch : End Try
        If Not HelmetCB.SelectedIndex = 0 Then
            Directory.CreateDirectory("Override\Helmet\Critters")
            Directory.CreateDirectory("Override\Helmet\Interface")
            File.WriteAllText(HelmType, HelmetCB.Text)
            Select Case HelmetCB.SelectedIndex
                Case 1 : File.WriteAllBytes(HelmInv, My.Resources._8_Ball_I) : File.WriteAllBytes(HelmTex, My.Resources._8_Ball)
                Case 2 : File.WriteAllBytes(HelmInv, My.Resources.AmericanI) : File.WriteAllBytes(HelmTex, My.Resources.American)
                Case 3 : File.WriteAllBytes(HelmInv, My.Resources.BlackI) : File.WriteAllBytes(HelmTex, My.Resources.Black)
                Case 4 : File.WriteAllBytes(HelmInv, My.Resources.BlueI) : File.WriteAllBytes(HelmTex, My.Resources.Blue)
                Case 5 : File.WriteAllBytes(HelmInv, My.Resources.EyeI) : File.WriteAllBytes(HelmTex, My.Resources.Eye)
                Case 6 : File.WriteAllBytes(HelmInv, My.Resources.FlamesI) : File.WriteAllBytes(HelmTex, My.Resources.Flames)
                Case 7 : File.WriteAllBytes(HelmInv, My.Resources.Full_SkullI) : File.WriteAllBytes(HelmTex, My.Resources.Full_Skull)
                Case 8 : File.WriteAllBytes(HelmInv, My.Resources.GreenI) : File.WriteAllBytes(HelmTex, My.Resources.Green)
                Case 9 : File.WriteAllBytes(HelmInv, My.Resources.GreyI) : File.WriteAllBytes(HelmTex, My.Resources.Grey)
                Case 10 : File.WriteAllBytes(HelmInv, My.Resources.PoliceI) : File.WriteAllBytes(HelmTex, My.Resources.Police)
                Case 11 : File.WriteAllBytes(HelmInv, My.Resources.RedI) : File.WriteAllBytes(HelmTex, My.Resources.Red)
                Case 12 : File.WriteAllBytes(HelmInv, My.Resources.Shot_SmileyI) : File.WriteAllBytes(HelmTex, My.Resources.Shot_Smiley)
                Case 13 : File.WriteAllBytes(HelmInv, My.Resources.SkullI) : File.WriteAllBytes(HelmTex, My.Resources.Skull)
                Case 14 : File.WriteAllBytes(HelmInv, My.Resources.YellowI) : File.WriteAllBytes(HelmTex, My.Resources.Yellow)
            End Select
        End If
        File.WriteAllLines(SysDir, SysIni)
        File.WriteAllLines(F3Dir, F3Ini)
        Hide()
    End Sub

    Private Sub HelmetPreview() Handles HelmetCB.SelectedIndexChanged
        Select Case HelmetCB.SelectedIndex
            Case 0 : HelmetPI.Image = My.Resources.Default_Icon
            Case 1 : HelmetPI.Image = My.Resources._8_Ball_Icon
            Case 2 : HelmetPI.Image = My.Resources.American_Icon
            Case 3 : HelmetPI.Image = My.Resources.Black_Icon
            Case 4 : HelmetPI.Image = My.Resources.Blue_Icon
            Case 5 : HelmetPI.Image = My.Resources.Eye_Icon
            Case 6 : HelmetPI.Image = My.Resources.Flames_Icon
            Case 7 : HelmetPI.Image = My.Resources.Full_Skull_Icon
            Case 8 : HelmetPI.Image = My.Resources.Green_Icon
            Case 9 : HelmetPI.Image = My.Resources.Grey_Icon
            Case 10 : HelmetPI.Image = My.Resources.Police_Icon
            Case 11 : HelmetPI.Image = My.Resources.Red_Icon
            Case 12 : HelmetPI.Image = My.Resources.Shot_Smiley_Icon
            Case 13 : HelmetPI.Image = My.Resources.Skull_Icon
            Case 14 : HelmetPI.Image = My.Resources.Yellow_Icon
        End Select
    End Sub

    Private Sub OpenVideoOptions() Handles VideoB.Click
        VideoOptions.ShowDialog()
    End Sub

    Private Sub NewGameToolTip() Handles NewGameCB.SelectedIndexChanged
        ToolTip1.SetToolTip(NewGameCB, NewGameCB.Text)
    End Sub

End Class