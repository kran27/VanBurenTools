Imports System.IO.Directory

Public Class Form2
    Public ovrdir As String = Application.StartupPath & "\Override"
    Public ifdir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public line() As String = IO.File.ReadAllLines(ifdir)
    Public sysline() As String
    Public file As IO.FileInfo

    Function SearchForFiles(ByVal RootFolder As String, ByVal FileFilter() As String) As List(Of String)
        Dim ReturnedData As New List(Of String)
        Dim FolderStack As New Stack(Of String)
        FolderStack.Push(RootFolder)
        Do While FolderStack.Count > 0
            Dim ThisFolder As String = FolderStack.Pop
            Try
                For Each SubFolder In GetDirectories(ThisFolder)
                    FolderStack.Push(SubFolder)
                Next
                For Each FileExt In FileFilter
                    ReturnedData.AddRange(GetFiles(ThisFolder, FileExt))
                Next
            Catch ex As Exception
            End Try
        Loop
        Return ReturnedData
    End Function

    Public Sub WriteToF3Ini(ByVal LineNum As Integer, ByVal TextValue As String)
        line = IO.File.ReadAllLines(ifdir)
        line(LineNum) = TextValue
        IO.File.WriteAllLines(ifdir, line)
    End Sub

    Public Sub WriteMainMenu(ByVal MapName As String, ByVal targetX As String, ByVal targetY As String, ByVal targetZ As String, ByVal Azimuth As String, ByVal Elevation As String, ByVal FOV As String)
        sysline(19) = "map name = " & MapName
        sysline(20) = "target x = " & targetX
        sysline(21) = "target y = " & targetY
        sysline(22) = "target z = " & targetZ
        sysline(23) = "azimuth = " & Azimuth
        sysline(24) = "elevation = " & Elevation
        sysline(26) = "fov = " & FOV
        IO.File.WriteAllLines(ovrdir & "\MenuMap\Engine\sys.ini", sysline)
    End Sub

#Region "Auto Detect Options"

    Private Sub CheckOptions() Handles MyBase.Load
        If line(25) = "enable startup movies = 1" Then
            CheckBox2.CheckState = 1
        End If
        If IO.Directory.Exists(ovrdir & "\SUMM") Then
            CheckBox4.CheckState = 1
        End If

#Region "Maps"

        If Not IO.File.Exists(ovrdir & "\MenuMap\Engine\sys.ini") Then
            IO.Directory.CreateDirectory(ovrdir & "\MenuMap\Engine")
            IO.File.WriteAllBytes(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.Default_sys)
        End If
        sysline = IO.File.ReadAllLines(ovrdir & "\MenuMap\Engine\sys.ini")
        Select Case sysline(19)
            Case "map name = mainmenu.map"
                ComboBox1.SelectedIndex = 0
            Case "map name = zz_TestMapsaarontemp2.map"
                ComboBox1.SelectedIndex = 1
            Case "map name = zz_TestMapsTest_City_Building01.map"
                ComboBox1.SelectedIndex = 2
            Case "map name = zz_TestMapsTest_City_Building02.map"
                ComboBox1.SelectedIndex = 3
            Case "map name = zz_TestMapsTest_City_Building03.map"
                ComboBox1.SelectedIndex = 4
            Case "map name = zz_TestMapsTest_City_Building04.map"
                ComboBox1.SelectedIndex = 5
            Case "map name = 98_Canyon_Random_01.map"
                ComboBox1.SelectedIndex = 6
            Case "map name = 98_Canyon_Random_02.map"
                ComboBox1.SelectedIndex = 7
            Case "map name = 04_0202_Spelunking.map"
                ComboBox1.SelectedIndex = 8
            Case "map name = zz_TestMapsTest_City_Fences.map"
                ComboBox1.SelectedIndex = 9
            Case "map name = zz_TestMapsScottE_Test1.map"
                ComboBox1.SelectedIndex = 10
            Case "map name = zz_TestMapsScottE_Test2.map"
                ComboBox1.SelectedIndex = 11
            Case "map name = zz_TestMapsScottE_Test4.map"
                ComboBox1.SelectedIndex = 12
            Case "map name = zz_TestMapsTest_Junktown_Shacks.map"
                ComboBox1.SelectedIndex = 13
            Case "map name = Default_StartMap.map"
                ComboBox1.SelectedIndex = 14
            Case "map name = 00_03_Tutorial_Junktown.map"
                ComboBox1.SelectedIndex = 15
            Case "map name = 00_04_Tutorial_Vault.map"
                ComboBox1.SelectedIndex = 16
        End Select
        If sysline(13) = "FOV Min = 0.5" Then
            CheckBox5.Checked = True
        End If
        Dim Files = SearchForFiles(ovrdir, {"*.map"})
        For Each file As Object In Files
            Dim fi As New IO.FileInfo(file)
            If Not ComboBox3.Items.Contains(fi.Name) Then
                ComboBox3.Items.Add(fi.Name)
            End If
        Next
        sysline = IO.File.ReadAllLines(ovrdir & "\MenuMap\Engine\sys.ini")
        Dim newgamemap As String = sysline(52).Remove(0, 12)
        ComboBox3.Text = newgamemap

#End Region

#Region "Helmets"

        If IO.File.Exists(ovrdir & "\Helmet\Helmet.type") Then
            Select Case IO.File.ReadAllText(ovrdir & "\Helmet\Helmet.type")
                Case "8Ball"
                    ComboBox2.SelectedIndex = 1
                Case "American"
                    ComboBox2.SelectedIndex = 2
                Case "Black"
                    ComboBox2.SelectedIndex = 3
                Case "Blue"
                    ComboBox2.SelectedIndex = 4
                Case "Eye"
                    ComboBox2.SelectedIndex = 5
                Case "Flames"
                    ComboBox2.SelectedIndex = 6
                Case "FullSkull"
                    ComboBox2.SelectedIndex = 7
                Case "Green"
                    ComboBox2.SelectedIndex = 8
                Case "Grey"
                    ComboBox2.SelectedIndex = 9
                Case "Police"
                    ComboBox2.SelectedIndex = 10
                Case "Red"
                    ComboBox2.SelectedIndex = 11
                Case "ShotSmiley"
                    ComboBox2.SelectedIndex = 12
                Case "Skull"
                    ComboBox2.SelectedIndex = 13
                Case "Yellow"
                    ComboBox2.SelectedIndex = 14
            End Select
        Else
            ComboBox2.SelectedIndex = 0
        End If

#End Region

    End Sub

#End Region

    Private Sub ApplyChanges(sender As Object, e As EventArgs) Handles Button1.Click
        If CheckBox2.Checked Then
            WriteToF3Ini(25, "enable startup movies = 1")
        Else
            WriteToF3Ini(25, "enable startup movies = 0")
        End If
        If CheckBox4.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\SUMM\Interface")
            IO.File.WriteAllBytes(ovrdir & "\SUMM\Interface\Mainmenu.int", My.Resources.Mainmenu)
            IO.File.WriteAllBytes(ovrdir & "\SUMM\Interface\f3_front_end_buttons.tga", My.Resources.f3_front_end_buttons)
        ElseIf IO.Directory.Exists(ovrdir & "\SUMM") Then
            IO.Directory.Delete(ovrdir & "\SUMM", True)
        End If

#Region "Load Maps"

        Select Case ComboBox1.SelectedIndex
            Case 0
                If line(29) = "height = 768" Then
                    WriteMainMenu("mainmenu.map", 0, 5.5, 0, 0, 0, 68)
                Else
                    WriteMainMenu("mainmenu.map", 0, 5.5, 0, 2, 0.75, 59.2)
                End If
            Case 1
                WriteMainMenu("zz_TestMapsaarontemp2.map", 0, 0, 0, 0, 0, 0)
            Case 2
                WriteMainMenu("zz_TestMapsTest_City_Building01.map", 0, 5.5, 0, 45, -2.5, 57)
            Case 3
                WriteMainMenu("zz_TestMapsTest_City_Building02.map", 0, 5.5, 0, 43, -2.5, 57)
            Case 4
                WriteMainMenu("zz_TestMapsTest_City_Building03.map", 0, 5.5, 0, 43, -5, 57)
            Case 5
                WriteMainMenu("zz_TestMapsTest_City_Building04.map", 0, 5.5, 0, 43, -5.5, 57)
            Case 6
                WriteMainMenu("98_Canyon_Random_01.map", 50, 5, 10, 61, 0, 45)
            Case 7
                WriteMainMenu("98_Canyon_Random_02.map", 55, 5, 10, 36, -2.5, 50)
            Case 8
                WriteMainMenu("04_0202_Spelunking.map", 70, 5, 45, 15, 5, 50)
            Case 9
                WriteMainMenu("zz_TestMapsTest_City_Fences.map", 0, 40, 0, 42, 35, 50)
            Case 10
                WriteMainMenu("zz_TestMapsScottE_Test1.map", 85, 30, 30, 255, 39, 60)
            Case 11
                WriteMainMenu("zz_TestMapsScottE_Test2.map", 145, 80, -85, 0.5, 25, 75)
            Case 12
                WriteMainMenu("zz_TestMapsScottE_Test4.map", 0, 7.5, 0, 45, 12.5, 50)
            Case 13
                WriteMainMenu("zz_TestMapsTest_Junktown_Shacks.map", 0, 50, -10, 42, 39, 50)
            Case 14
                WriteMainMenu("Default_StartMap.map", 60, 7.5, 25, 270, 8, 27)
            Case 15
                WriteMainMenu("00_03_Tutorial_Junktown.map", 80, 7.5, 50, 5, 10, 68)
            Case 16
                WriteMainMenu("00_04_Tutorial_Vault.map", 50, 50.5, 0, 36, 25, 68)
        End Select
        sysline = IO.File.ReadAllLines(ovrdir & "\MenuMap\Engine\sys.ini")
        If CheckBox5.Checked Then
            sysline(12) = "FOV Speed = 10"
            sysline(13) = "FOV Min = 0.5"
            sysline(14) = "FOV Max = 100"
            sysline(16) = "Scroll Speed = 125"
        Else
            sysline(12) = "FOV Speed = 6.5"
            sysline(13) = "FOV Min = 6"
            sysline(14) = "FOV Max = 15"
            sysline(16) = "Scroll Speed = 96"
        End If
        sysline(52) = "Start map = " & ComboBox3.Text
        IO.File.WriteAllLines(ovrdir & "\MenuMap\Engine\sys.ini", sysline)

#End Region

#Region "Load Helmets"

        If IO.Directory.Exists(ovrdir & "\Helmet") Then
            IO.Directory.Delete(ovrdir & "\Helmet", True)
        End If
        If Not ComboBox2.SelectedIndex = 0 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
        End If
        Select Case ComboBox2.SelectedIndex
            Case 1
                IO.File.WriteAllText(ovrdir & "\Helmet\Helmet.type", "8Ball")
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources._8_Ball_I)
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources._8_Ball)
            Case 2
                IO.File.WriteAllText(ovrdir & "\Helmet\Helmet.type", "American")
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.AmericanI)
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.American)
            Case 3
                IO.File.WriteAllText(ovrdir & "\Helmet\Helmet.type", "Black")
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.BlackI)
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Black)
            Case 4
                IO.File.WriteAllText(ovrdir & "\Helmet\Helmet.type", "Blue")
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.BlueI)
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Blue)
            Case 5
                IO.File.WriteAllText(ovrdir & "\Helmet\Helmet.type", "Eye")
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.EyeI)
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Eye)
            Case 6
                IO.File.WriteAllText(ovrdir & "\Helmet\Helmet.type", "Flames")
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.FlamesI)
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Flames)
            Case 7
                IO.File.WriteAllText(ovrdir & "\Helmet\Helmet.type", "FullSkull")
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.Full_SkullI)
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Full_Skull)
            Case 8
                IO.File.WriteAllText(ovrdir & "\Helmet\Helmet.type", "Green")
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.GreenI)
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Green)
            Case 9
                IO.File.WriteAllText(ovrdir & "\Helmet\Helmet.type", "Grey")
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.GreyI)
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Grey)
            Case 10
                IO.File.WriteAllText(ovrdir & "\Helmet\Helmet.type", "Police")
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.PoliceI)
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Police)
            Case 11
                IO.File.WriteAllText(ovrdir & "\Helmet\Helmet.type", "Red")
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.RedI)
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Red)
            Case 12
                IO.File.WriteAllText(ovrdir & "\Helmet\Helmet.type", "ShotSmiley")
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.Shot_SmileyI)
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Shot_Smiley)
            Case 13
                IO.File.WriteAllText(ovrdir & "\Helmet\Helmet.type", "Skull")
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.SkullI)
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Skull)
            Case 14
                IO.File.WriteAllText(ovrdir & "\Helmet\Helmet.type", "Yellow")
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.YellowI)
                IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Yellow)
        End Select

#End Region

        Hide()
    End Sub

    Private Sub HelmetPreview() Handles ComboBox2.SelectedIndexChanged
        Select Case ComboBox2.SelectedIndex
            Case 0
                PictureBox1.Image = My.Resources.Default_Icon
            Case 1
                PictureBox1.Image = My.Resources._8_Ball_Icon
            Case 2
                PictureBox1.Image = My.Resources.American_Icon
            Case 3
                PictureBox1.Image = My.Resources.Black_Icon
            Case 4
                PictureBox1.Image = My.Resources.Blue_Icon
            Case 5
                PictureBox1.Image = My.Resources.Eye_Icon
            Case 6
                PictureBox1.Image = My.Resources.Flames_Icon
            Case 7
                PictureBox1.Image = My.Resources.Full_Skull_Icon
            Case 8
                PictureBox1.Image = My.Resources.Green_Icon
            Case 9
                PictureBox1.Image = My.Resources.Grey_Icon
            Case 10
                PictureBox1.Image = My.Resources.Police_Icon
            Case 11
                PictureBox1.Image = My.Resources.Red_Icon
            Case 12
                PictureBox1.Image = My.Resources.Shot_Smiley_Icon
            Case 13
                PictureBox1.Image = My.Resources.Skull_Icon
            Case 14
                PictureBox1.Image = My.Resources.Yellow_Icon
        End Select
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form3.ShowDialog()
    End Sub

End Class