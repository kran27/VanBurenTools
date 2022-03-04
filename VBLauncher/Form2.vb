Imports System.IO
Imports System.IO.Compression

Public Class Form2
    Public IFDir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public Line() As String = File.ReadAllLines(IFDir)
    Public SysLine() As String
    Public HelmType As String = "Override\Helmet\Helmet.type"
    Public HelmInv As String = "Override\Helmet\Interface\HeaMotorcycle_default_INV.tga"
    Public HelmTex As String = "Override\Helmet\Critters\HeaMotorcycle_default_LG.png"

    Function SearchForFiles(RootFolder As String, FileFilter() As String) As List(Of String)
        Dim ReturnedData As New List(Of String)
        Dim FolderStack As New Stack(Of String)
        FolderStack.Push(RootFolder)
        Do While FolderStack.Count > 0
            Dim ThisFolder As String = FolderStack.Pop
            Try
                For Each SubFolder In Directory.GetDirectories(ThisFolder)
                    FolderStack.Push(SubFolder)
                Next
                For Each FileExt In FileFilter
                    ReturnedData.AddRange(Directory.GetFiles(ThisFolder, FileExt))
                Next
            Catch Ex As Exception
            End Try
        Loop
        Return ReturnedData
    End Function

    Public Sub WriteToF3Ini(LineNum As Integer, TextValue As String)
        Line = File.ReadAllLines(IFDir)
        Line(LineNum) = TextValue
        File.WriteAllLines(IFDir, Line)
    End Sub

    Public Sub WriteMainMenu(MapName As String, TargetX As String, TargetY As String, TargetZ As String,
                             Azimuth As String, Elevation As String, FOV As String)
        SysLine(19) = "map name = " & MapName
        SysLine(20) = "target x = " & TargetX
        SysLine(21) = "target y = " & TargetY
        SysLine(22) = "target z = " & TargetZ
        SysLine(23) = "azimuth = " & Azimuth
        SysLine(24) = "elevation = " & Elevation
        SysLine(26) = "fov = " & FOV
        File.WriteAllLines("Override\MenuMap\Engine\sys.ini", SysLine)
    End Sub

    Private Sub CheckOptions() Handles MyBase.Load
        Icon = My.Resources.F3
        CheckBox1.Checked = Line(25) = "enable startup movies = 1"
        CheckBox2.Checked = Directory.Exists("Override\SUMM")
        If Not File.Exists("Override\MenuMap\Engine\sys.ini") Then Directory.CreateDirectory("Override\MenuMap\Engine") : _
            File.WriteAllBytes("Override\MenuMap\Engine\sys.ini", My.Resources.Default_sys)
        SysLine = File.ReadAllLines("Override\MenuMap\Engine\sys.ini")
        Select Case SysLine(19)
            Case "map name = mainmenu.map" : ComboBox1.SelectedIndex = 0
            Case "map name = zz_TestMapsaarontemp2.map" : ComboBox1.SelectedIndex = 1
            Case "map name = zz_TestMapsTest_City_Building01.map" : ComboBox1.SelectedIndex = 2
            Case "map name = zz_TestMapsTest_City_Building02.map" : ComboBox1.SelectedIndex = 3
            Case "map name = zz_TestMapsTest_City_Building03.map" : ComboBox1.SelectedIndex = 4
            Case "map name = zz_TestMapsTest_City_Building04.map" : ComboBox1.SelectedIndex = 5
            Case "map name = 98_Canyon_Random_01.map" : ComboBox1.SelectedIndex = 6
            Case "map name = 98_Canyon_Random_02.map" : ComboBox1.SelectedIndex = 7
            Case "map name = 04_0202_Spelunking.map" : ComboBox1.SelectedIndex = 8
            Case "map name = zz_TestMapsTest_City_Fences.map" : ComboBox1.SelectedIndex = 9
            Case "map name = zz_TestMapsScottE_Test1.map" : ComboBox1.SelectedIndex = 10
            Case "map name = zz_TestMapsScottE_Test2.map" : ComboBox1.SelectedIndex = 11
            Case "map name = zz_TestMapsScottE_Test4.map" : ComboBox1.SelectedIndex = 12
            Case "map name = zz_TestMapsTest_Junktown_Shacks.map" : ComboBox1.SelectedIndex = 13
            Case "map name = Default_StartMap.map" : ComboBox1.SelectedIndex = 14
            Case "map name = 00_03_Tutorial_Junktown.map" : ComboBox1.SelectedIndex = 15
            Case "map name = 00_04_Tutorial_Vault.map" : ComboBox1.SelectedIndex = 16
        End Select
        CheckBox3.Checked = SysLine(13) = "FOV Min = 0.5"
        Dim Files = SearchForFiles("Override", {"*.map"})
        For Each File As Object In Files
            Dim FI As New FileInfo(File)
            If Not ComboBox3.Items.Contains(FI.Name) Then
                ComboBox3.Items.Add(FI.Name)
            End If
        Next
        SysLine = File.ReadAllLines("Override\MenuMap\Engine\sys.ini")
        ComboBox3.Text = SysLine(52).Remove(0, 12)
        Try : ComboBox2.SelectedItem = File.ReadAllText(HelmType)
        Catch : ComboBox2.SelectedIndex = 0 : End Try
    End Sub

    Private Sub ApplyChanges() Handles Button1.Click
        If CheckBox1.Checked Then WriteToF3Ini(25, "enable startup movies = 1") Else _
            WriteToF3Ini(25, "enable startup movies = 0")
        If CheckBox2.Checked Then
            Directory.CreateDirectory("Override\SUMM\Interface")
            File.WriteAllBytes("Override\SUMM\Interface\Mainmenu.int", My.Resources.Mainmenu)
            File.WriteAllBytes("Override\SUMM\Interface\f3_front_end_buttons.tga", My.Resources.f3_front_end_buttons)
        Else
            Try : Directory.Delete("Override\SUMM", 1) : Catch : End Try
        End If
        Select Case ComboBox1.SelectedIndex
            Case 0 : WriteMainMenu("mainmenu.map", 0, 5.5, 0, 2, 0.75, 59.2)
            Case 1 : WriteMainMenu("zz_TestMapsaarontemp2.map", 0, 0, 0, 0, 0, 0)
            Case 2 : WriteMainMenu("zz_TestMapsTest_City_Building01.map", 0, 5.5, 0, 45, -2.5, 57)
            Case 3 : WriteMainMenu("zz_TestMapsTest_City_Building02.map", 0, 5.5, 0, 43, -2.5, 57)
            Case 4 : WriteMainMenu("zz_TestMapsTest_City_Building03.map", 0, 5.5, 0, 43, -5, 57)
            Case 5 : WriteMainMenu("zz_TestMapsTest_City_Building04.map", 0, 5.5, 0, 43, -5.5, 57)
            Case 6 : WriteMainMenu("98_Canyon_Random_01.map", 50, 5, 10, 61, 0, 45)
            Case 7 : WriteMainMenu("98_Canyon_Random_02.map", 55, 5, 10, 36, -2.5, 50)
            Case 8 : WriteMainMenu("04_0202_Spelunking.map", 70, 5, 45, 15, 5, 50)
            Case 9 : WriteMainMenu("zz_TestMapsTest_City_Fences.map", 0, 40, 0, 42, 35, 50)
            Case 10 : WriteMainMenu("zz_TestMapsScottE_Test1.map", 85, 30, 30, 255, 39, 60)
            Case 11 : WriteMainMenu("zz_TestMapsScottE_Test2.map", 145, 80, -85, 0.5, 25, 75)
            Case 12 : WriteMainMenu("zz_TestMapsScottE_Test4.map", 0, 7.5, 0, 45, 12.5, 50)
            Case 13 : WriteMainMenu("zz_TestMapsTest_Junktown_Shacks.map", 0, 50, -10, 42, 39, 50)
            Case 14 : WriteMainMenu("Default_StartMap.map", 60, 7.5, 25, 270, 8, 27)
            Case 15 : WriteMainMenu("00_03_Tutorial_Junktown.map", 80, 7.5, 50, 5, 10, 68)
            Case 16 : WriteMainMenu("00_04_Tutorial_Vault.map", 50, 50.5, 0, 36, 25, 68)
        End Select
        SysLine = File.ReadAllLines("Override\MenuMap\Engine\sys.ini")
        If CheckBox3.Checked Then
            SysLine(12) = "FOV Speed = 10"
            SysLine(16) = "Scroll Speed = 125"
            SysLine(13) = "FOV Min = 0.5"
            SysLine(14) = "FOV Max = 100"
        Else
            SysLine(12) = "FOV Speed = 6.5"
            SysLine(16) = "Scroll Speed = 96"
            SysLine(13) = "FOV Min = 6"
            SysLine(14) = "FOV Max = 15"
        End If
        SysLine(52) = "Start map = " & ComboBox3.Text
        File.WriteAllLines("Override\MenuMap\Engine\sys.ini", SysLine)
        Try : Directory.Delete("Override\Helmet", 1) : Catch : End Try
        If Not ComboBox2.SelectedIndex = 0 Then
            Directory.CreateDirectory("Override\Helmet\Critters")
            Directory.CreateDirectory("Override\Helmet\Interface")
            File.WriteAllText(HelmType, ComboBox2.Text)
            Select Case ComboBox2.SelectedIndex
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
        Hide()
    End Sub

    Private Sub HelmetPreview() Handles ComboBox2.SelectedIndexChanged
        Select Case ComboBox2.SelectedIndex
            Case 0 : PictureBox1.Image = My.Resources.Default_Icon
            Case 1 : PictureBox1.Image = My.Resources._8_Ball_Icon
            Case 2 : PictureBox1.Image = My.Resources.American_Icon
            Case 3 : PictureBox1.Image = My.Resources.Black_Icon
            Case 4 : PictureBox1.Image = My.Resources.Blue_Icon
            Case 5 : PictureBox1.Image = My.Resources.Eye_Icon
            Case 6 : PictureBox1.Image = My.Resources.Flames_Icon
            Case 7 : PictureBox1.Image = My.Resources.Full_Skull_Icon
            Case 8 : PictureBox1.Image = My.Resources.Green_Icon
            Case 9 : PictureBox1.Image = My.Resources.Grey_Icon
            Case 10 : PictureBox1.Image = My.Resources.Police_Icon
            Case 11 : PictureBox1.Image = My.Resources.Red_Icon
            Case 12 : PictureBox1.Image = My.Resources.Shot_Smiley_Icon
            Case 13 : PictureBox1.Image = My.Resources.Skull_Icon
            Case 14 : PictureBox1.Image = My.Resources.Yellow_Icon
        End Select
    End Sub

    Private Sub Button3_Click() Handles Button3.Click
        Form3.ShowDialog()
    End Sub

    Private Sub ComboBox3_SelectedIndexChanged() Handles ComboBox3.SelectedIndexChanged
        ToolTip1.SetToolTip(ComboBox3, ComboBox3.Text)
    End Sub

End Class