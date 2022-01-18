Imports System.IO.Directory
Public Class Form2
    Public ovrdir As String = Application.StartupPath & "\Override"
    Public ifdir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public line() As String = IO.File.ReadAllLines(ifdir)
    Public mapline() As String
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
#Region "Auto Detect Options"
    Private Sub CheckOptions() Handles MyBase.Load
        If IO.File.Exists(ovrdir & "\FemaleFix\_CRT\PCFemale.CRT") Then
            CheckBox1.CheckState = 1
        End If
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
        mapline = IO.File.ReadAllLines(ovrdir & "\MenuMap\Engine\sys.ini")
        If IO.File.Exists(ovrdir & "\MenuMap\Engine\sys.ini") Then
            If mapline(19) = "map name = mainmenu.map" Then
                ComboBox1.SelectedIndex = 0
            ElseIf mapline(19) = "map name = zz_TestMapsaarontemp2.map" Then
                ComboBox1.SelectedIndex = 1
            ElseIf mapline(19) = "map name = zz_TestMapsTest_City_Building01.map" Then
                ComboBox1.SelectedIndex = 2
            ElseIf mapline(19) = "map name = zz_TestMapsTest_City_Building02.map" Then
                ComboBox1.SelectedIndex = 3
            ElseIf mapline(19) = "map name = zz_TestMapsTest_City_Building03.map" Then
                ComboBox1.SelectedIndex = 4
            ElseIf mapline(19) = "map name = zz_TestMapsTest_City_Building04.map" Then
                ComboBox1.SelectedIndex = 5
            ElseIf mapline(19) = "map name = 98_Canyon_Random_01.map" Then
                ComboBox1.SelectedIndex = 6
            ElseIf mapline(19) = "map name = 98_Canyon_Random_02.map" Then
                ComboBox1.SelectedIndex = 7
            ElseIf mapline(19) = "map name = 04_0202_Spelunking.map" Then
                ComboBox1.SelectedIndex = 8
            ElseIf mapline(19) = "map name = zz_TestMapsTest_City_Fences.map" Then
                ComboBox1.SelectedIndex = 9
            ElseIf mapline(19) = "map name = zz_TestMapsScottE_Test1.map" Then
                ComboBox1.SelectedIndex = 10
            ElseIf mapline(19) = "map name = zz_TestMapsScottE_Test2.map" Then
                ComboBox1.SelectedIndex = 11
            ElseIf mapline(19) = "map name = zz_TestMapsScottE_Test4.map" Then
                ComboBox1.SelectedIndex = 12
            ElseIf mapline(19) = "map name = zz_TestMapsTest_Junktown_Shacks.map" Then
                ComboBox1.SelectedIndex = 13
            ElseIf mapline(19) = "map name = Default_StartMap.map" Then
                ComboBox1.SelectedIndex = 14
            ElseIf mapline(19) = "map name = 00_03_Tutorial_Junktown.map" Then
                ComboBox1.SelectedIndex = 15
            ElseIf mapline(19) = "map name = 00_04_Tutorial_Vault.map" Then
                ComboBox1.SelectedIndex = 16
            Else
                ComboBox1.SelectedIndex = 0
            End If
        Else
            ComboBox1.SelectedIndex = 0
        End If
        If mapline(13) = "FOV Min = 0.5" Then
            CheckBox5.Checked = True
        End If
        Dim Files = SearchForFiles(ovrdir, {"*.map"})
        For Each file As Object In Files
            Dim fi As New IO.FileInfo(file)
            If Not ComboBox3.Items.Contains(fi.Name) Then
                ComboBox3.Items.Add(fi.Name)
            End If
        Next
        mapline = IO.File.ReadAllLines(ovrdir & "\MenuMap\Engine\sys.ini")
        Dim newgamemap As String = mapline(52)
        ComboBox3.Text = newgamemap.Remove(0, 12)
#End Region
#Region "Helmets"
        If IO.File.Exists(ovrdir & "\Helmet\8Ball") Then
            ComboBox2.SelectedIndex = 1
        ElseIf IO.File.Exists(ovrdir & "\Helmet\American") Then
            ComboBox2.SelectedIndex = 2
        ElseIf IO.File.Exists(ovrdir & "\Helmet\Black") Then
            ComboBox2.SelectedIndex = 3
        ElseIf IO.File.Exists(ovrdir & "\Helmet\Blue") Then
            ComboBox2.SelectedIndex = 4
        ElseIf IO.File.Exists(ovrdir & "\Helmet\Eye") Then
            ComboBox2.SelectedIndex = 5
        ElseIf IO.File.Exists(ovrdir & "\Helmet\Flames") Then
            ComboBox2.SelectedIndex = 6
        ElseIf IO.File.Exists(ovrdir & "\Helmet\FullSkull") Then
            ComboBox2.SelectedIndex = 7
        ElseIf IO.File.Exists(ovrdir & "\Helmet\Green") Then
            ComboBox2.SelectedIndex = 8
        ElseIf IO.File.Exists(ovrdir & "\Helmet\Grey") Then
            ComboBox2.SelectedIndex = 9
        ElseIf IO.File.Exists(ovrdir & "\Helmet\Police") Then
            ComboBox2.SelectedIndex = 10
        ElseIf IO.File.Exists(ovrdir & "\Helmet\Red") Then
            ComboBox2.SelectedIndex = 11
        ElseIf IO.File.Exists(ovrdir & "\Helmet\ShotSmiley") Then
            ComboBox2.SelectedIndex = 12
        ElseIf IO.File.Exists(ovrdir & "\Helmet\Skull") Then
            ComboBox2.SelectedIndex = 13
        ElseIf IO.File.Exists(ovrdir & "\Helmet\Yellow") Then
            ComboBox2.SelectedIndex = 14
        Else
            ComboBox2.SelectedIndex = 0
        End If
#End Region
    End Sub
#End Region
    Private Sub ApplyChanges(sender As Object, e As EventArgs) Handles Button1.Click
        line = IO.File.ReadAllLines(ifdir)
        If CheckBox1.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\FemaleFix\_CRT")
            IO.File.WriteAllBytes(ovrdir & "\FemaleFix\_CRT\PCFemale.CRT", My.Resources.PCFemale)
        ElseIf IO.File.Exists(ovrdir & "\FemaleFix\_CRT\PCFemale.CRT") Then
            IO.File.Delete(ovrdir & "\FemaleFix\_CRT\PCFemale.CRT")
            IO.Directory.Delete(ovrdir & "\FemaleFix\_CRT")
            IO.Directory.Delete(ovrdir & "\FemaleFix")
        End If
        If CheckBox2.Checked Then
            line(25) = "enable startup movies = 1"
            IO.File.WriteAllLines(ifdir, line)
        Else
            line(25) = "enable startup movies = 0"
            IO.File.WriteAllLines(ifdir, line)
        End If
        If CheckBox4.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\SUMM\Interface")
            IO.File.WriteAllBytes(ovrdir & "\SUMM\Interface\Mainmenu.int", My.Resources.Mainmenu)
            IO.File.WriteAllBytes(ovrdir & "\SUMM\Interface\f3_front_end_buttons.tga", My.Resources.f3_front_end_buttons)
        ElseIf IO.Directory.Exists(ovrdir & "\SUMM\Interface") Then
            IO.File.Delete(ovrdir & "\SUMM\Interface\Mainmenu.int")
            IO.File.Delete(ovrdir & "\SUMM\Interface\f3_front_end_buttons.tga")
            IO.Directory.Delete(ovrdir & "\SUMM\Interface")
            IO.Directory.Delete(ovrdir & "\SUMM")
        End If
#Region "Load Maps"
        If ComboBox1.SelectedIndex = 0 And line(29) = "height = " & 872 Then
            IO.File.WriteAllBytes(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.Default_sys)
        ElseIf ComboBox1.SelectedIndex = 0 And Not line(29) = "height = " & 872 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources._Default)
        ElseIf ComboBox1.SelectedIndex = 1 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.AaronMap2)
        ElseIf ComboBox1.SelectedIndex = 2 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.Building1)
        ElseIf ComboBox1.SelectedIndex = 3 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.Building2)
        ElseIf ComboBox1.SelectedIndex = 4 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.Building3)
        ElseIf ComboBox1.SelectedIndex = 5 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.Building4)
        ElseIf ComboBox1.SelectedIndex = 6 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.Canyon)
        ElseIf ComboBox1.SelectedIndex = 7 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.Canyon2)
        ElseIf ComboBox1.SelectedIndex = 8 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.Cave)
        ElseIf ComboBox1.SelectedIndex = 9 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.Fences)
        ElseIf ComboBox1.SelectedIndex = 10 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.ScottEMap1)
        ElseIf ComboBox1.SelectedIndex = 11 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.ScottEMap2)
        ElseIf ComboBox1.SelectedIndex = 12 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.ScottEMap4)
        ElseIf ComboBox1.SelectedIndex = 13 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.Shacks)
        ElseIf ComboBox1.SelectedIndex = 14 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.StartMap)
        ElseIf ComboBox1.SelectedIndex = 15 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.Tutorial)
        ElseIf ComboBox1.SelectedIndex = 16 Then
            IO.File.WriteAllText(ovrdir & "\MenuMap\Engine\sys.ini", My.Resources.Vault)
        End If
        mapline = IO.File.ReadAllLines(ovrdir & "\MenuMap\Engine\sys.ini")
        If CheckBox5.Checked Then
            mapline(12) = "FOV Speed = 10"
            mapline(13) = "FOV Min = 0.5"
            mapline(14) = "FOV Max = 100"
            mapline(16) = "Scroll Speed = 125"
            mapline(52) = "Start map = " & ComboBox3.Text
        Else
            mapline(12) = "FOV Speed = 6.5"
            mapline(13) = "FOV Min = 6"
            mapline(14) = "FOV Max = 15"
            mapline(16) = "Scroll Speed = 96"
            mapline(52) = "Start map = " & ComboBox3.Text
        End If
        IO.File.WriteAllLines(ovrdir & "\MenuMap\Engine\sys.ini", mapline)
#End Region
#Region "Load Helmets"
        IO.Directory.CreateDirectory(ovrdir & "\Helmet")
        Dim helmdi As New IO.DirectoryInfo(ovrdir & "\Helmet")
        Dim helmfi As IO.FileInfo() = helmdi.GetFiles()
        For Each file In helmfi
            Try
                IO.File.Delete(file.FullName)
            Catch ex As Exception
            End Try
        Next
        If ComboBox2.SelectedIndex = 0 Then
            If IO.Directory.Exists(ovrdir & "\Helmet\Interface") Then
                IO.File.Delete(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga")
                IO.File.Delete(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga")
                IO.Directory.Delete(ovrdir & "\Helmet\Interface")
                IO.Directory.Delete(ovrdir & "\Helmet\Critters")
                IO.Directory.Delete(ovrdir & "\Helmet")
            End If
        ElseIf ComboBox2.SelectedIndex = 1 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
            IO.File.Create(ovrdir & "\Helmet\8Ball")
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources._8_Ball_I)
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources._8_Ball)
        ElseIf ComboBox2.SelectedIndex = 2 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
            IO.File.Create(ovrdir & "\Helmet\American")
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.AmericanI)
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.American)
        ElseIf ComboBox2.SelectedIndex = 3 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
            IO.File.Create(ovrdir & "\Helmet\Black")
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.BlackI)
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Black)
        ElseIf ComboBox2.SelectedIndex = 4 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
            IO.File.Create(ovrdir & "\Helmet\Blue")
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.BlueI)
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Blue)
        ElseIf ComboBox2.SelectedIndex = 5 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
            IO.File.Create(ovrdir & "\Helmet\Eye")
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.EyeI)
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Eye)
        ElseIf ComboBox2.SelectedIndex = 6 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
            IO.File.Create(ovrdir & "\Helmet\Flames")
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.FlamesI)
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Flames)
        ElseIf ComboBox2.SelectedIndex = 7 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
            IO.File.Create(ovrdir & "\Helmet\FullSkull")
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.Full_SkullI)
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Full_Skull)
        ElseIf ComboBox2.SelectedIndex = 8 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
            IO.File.Create(ovrdir & "\Helmet\Green")
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.GreenI)
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Green)
        ElseIf ComboBox2.SelectedIndex = 9 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
            IO.File.Create(ovrdir & "\Helmet\Grey")
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.GreyI)
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Grey)
        ElseIf ComboBox2.SelectedIndex = 10 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
            IO.File.Create(ovrdir & "\Helmet\Police")
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.PoliceI)
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Police)
        ElseIf ComboBox2.SelectedIndex = 11 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
            IO.File.Create(ovrdir & "\Helmet\Red")
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.RedI)
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Red)
        ElseIf ComboBox2.SelectedIndex = 12 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
            IO.File.Create(ovrdir & "\Helmet\ShotSmiley")
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.Shot_SmileyI)
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Shot_Smiley)
        ElseIf ComboBox2.SelectedIndex = 13 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
            IO.File.Create(ovrdir & "\Helmet\Skull")
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.SkullI)
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Skull)
        ElseIf ComboBox2.SelectedIndex = 14 Then
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Critters")
            IO.Directory.CreateDirectory(ovrdir & "\Helmet\Interface")
            IO.File.Create(ovrdir & "\Helmet\Yellow")
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Interface\HeaMotorcycle_default_INV.tga", My.Resources.YellowI)
            IO.File.WriteAllBytes(ovrdir & "\Helmet\Critters\HeaMotorcycle_default_LG.tga", My.Resources.Yellow)
        End If
#End Region
        Hide()
    End Sub
    Private Sub HelmetPreview() Handles ComboBox2.SelectedIndexChanged
        If ComboBox2.SelectedIndex = 0 Then
            PictureBox1.Image = My.Resources.Default_Icon
        ElseIf ComboBox2.SelectedIndex = 1 Then
            PictureBox1.Image = My.Resources._8_Ball_Icon
        ElseIf ComboBox2.SelectedIndex = 2 Then
            PictureBox1.Image = My.Resources.American_Icon
        ElseIf ComboBox2.SelectedIndex = 3 Then
            PictureBox1.Image = My.Resources.Black_Icon
        ElseIf ComboBox2.SelectedIndex = 4 Then
            PictureBox1.Image = My.Resources.Blue_Icon
        ElseIf ComboBox2.SelectedIndex = 5 Then
            PictureBox1.Image = My.Resources.Eye_Icon
        ElseIf ComboBox2.SelectedIndex = 6 Then
            PictureBox1.Image = My.Resources.Flames_Icon
        ElseIf ComboBox2.SelectedIndex = 7 Then
            PictureBox1.Image = My.Resources.Full_Skull_Icon
        ElseIf ComboBox2.SelectedIndex = 8 Then
            PictureBox1.Image = My.Resources.Green_Icon
        ElseIf ComboBox2.SelectedIndex = 9 Then
            PictureBox1.Image = My.Resources.Grey_Icon
        ElseIf ComboBox2.SelectedIndex = 10 Then
            PictureBox1.Image = My.Resources.Police_Icon
        ElseIf ComboBox2.SelectedIndex = 11 Then
            PictureBox1.Image = My.Resources.Red_Icon
        ElseIf ComboBox2.SelectedIndex = 12 Then
            PictureBox1.Image = My.Resources.Shot_Smiley_Icon
        ElseIf ComboBox2.SelectedIndex = 13 Then
            PictureBox1.Image = My.Resources.Skull_Icon
        ElseIf ComboBox2.SelectedIndex = 14 Then
            PictureBox1.Image = My.Resources.Yellow_Icon
        End If
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form3.Location = Location
        Form3.ShowDialog()
    End Sub
End Class