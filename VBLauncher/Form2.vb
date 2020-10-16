Public Class Form2
    Public ovrdir As String = Application.StartupPath & "\Override"
    Public ifdir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public line() As String = IO.File.ReadAllLines(ifdir)
    Public mapline() As String
#Region "Auto Tick Boxes"
    Private Sub CheckOptions() Handles MyBase.VisibleChanged
        If IO.File.Exists(ovrdir & "\FemaleFix\_CRT\PCFemale.CRT") Then
            CheckBox1.CheckState = 1
        Else
            CheckBox1.CheckState = 0
        End If
        If line(27) = "enable wireframe = 1" Then
            CheckBox2.CheckState = 1
        Else
            CheckBox2.CheckState = 0
        End If
        If line(25) = "enable startup movies = 1" Then
            CheckBox3.CheckState = 1
        Else
            CheckBox3.CheckState = 0
        End If
        If IO.File.Exists(ovrdir & "\MapChange\Engine\sys.ini") Then
            mapline = IO.File.ReadAllLines(ovrdir & "\MapChange\Engine\sys.ini")
            If mapline(52) = "Start map = zz_TestMapsaarontemp2.map" Then
                CheckBox4.CheckState = 1
            ElseIf mapline(52) = "Start map = zz_TestMapsTest_City_Building02.map" Then
                CheckBox5.CheckState = 1
            ElseIf mapline(52) = "Start map = zz_TestMapsTest_City_Building04.map" Then
                CheckBox6.CheckState = 1
            ElseIf mapline(52) = "Start map = 98_Canyon_Random_02.map" Then
                CheckBox7.CheckState = 1
            ElseIf mapline(52) = "Start map = zz_TestMapsTest_City_Fences.map" Then
                CheckBox8.CheckState = 1
            ElseIf mapline(52) = "Start map = zz_TestMapsScottE_Test1.map" Then
                CheckBox9.CheckState = 1
            ElseIf mapline(52) = "Start map = zz_TestMapsScottE_Test4.map" Then
                CheckBox10.CheckState = 1
            ElseIf mapline(52) = "Start map = 00_04_Tutorial_Vault.map" Then
                CheckBox11.CheckState = 1
            ElseIf mapline(52) = "zz_TestMapsTest_City_Building01.map" Then
                CheckBox12.CheckState = 1
            ElseIf mapline(52) = "zz_TestMapsTest_City_Building03.map" Then
                CheckBox13.CheckState = 1
            ElseIf mapline(52) = "Start map = 98_Canyon_Random_01.map" Then
                CheckBox14.CheckState = 1
            ElseIf mapline(52) = "Start map = 04_0202_Spelunking.map" Then
                CheckBox15.CheckState = 1
            ElseIf mapline(52) = "Start map = Mainmenu.map" Then
                CheckBox16.CheckState = 1
            ElseIf mapline(52) = "Start map = zz_TestMapsScottE_Test2.map" Then
                CheckBox17.CheckState = 1
            ElseIf mapline(52) = "Start map = zz_TestMapsTest_Junktown_Shacks.map" Then
                CheckBox18.CheckState = 1
            End If
        End If
    End Sub
#Region "Literal Toxic Waste"
    Private Sub box4() Handles CheckBox4.CheckedChanged
        If CheckBox4.Checked = True Then
            CheckBox5.CheckState = 0
            CheckBox6.CheckState = 0
            CheckBox7.CheckState = 0
            CheckBox8.CheckState = 0
            CheckBox9.CheckState = 0
            CheckBox10.CheckState = 0
            CheckBox11.CheckState = 0
            CheckBox12.CheckState = 0
            CheckBox13.CheckState = 0
            CheckBox14.CheckState = 0
            CheckBox15.CheckState = 0
            CheckBox16.CheckState = 0
            CheckBox17.CheckState = 0
            CheckBox18.CheckState = 0
        End If
    End Sub
    Private Sub box5() Handles CheckBox5.CheckedChanged
        If CheckBox5.Checked = True Then
            CheckBox4.CheckState = 0
            CheckBox6.CheckState = 0
            CheckBox7.CheckState = 0
            CheckBox8.CheckState = 0
            CheckBox9.CheckState = 0
            CheckBox10.CheckState = 0
            CheckBox11.CheckState = 0
            CheckBox12.CheckState = 0
            CheckBox13.CheckState = 0
            CheckBox14.CheckState = 0
            CheckBox15.CheckState = 0
            CheckBox16.CheckState = 0
            CheckBox17.CheckState = 0
            CheckBox18.CheckState = 0
        End If
    End Sub
    Private Sub box6() Handles CheckBox6.CheckedChanged
        If CheckBox6.Checked = True Then
            CheckBox4.CheckState = 0
            CheckBox5.CheckState = 0
            CheckBox7.CheckState = 0
            CheckBox8.CheckState = 0
            CheckBox9.CheckState = 0
            CheckBox10.CheckState = 0
            CheckBox11.CheckState = 0
            CheckBox12.CheckState = 0
            CheckBox13.CheckState = 0
            CheckBox14.CheckState = 0
            CheckBox15.CheckState = 0
            CheckBox16.CheckState = 0
            CheckBox17.CheckState = 0
            CheckBox18.CheckState = 0
        End If
    End Sub
    Private Sub box7() Handles CheckBox7.CheckedChanged
        If CheckBox7.Checked = True Then
            CheckBox4.CheckState = 0
            CheckBox5.CheckState = 0
            CheckBox6.CheckState = 0
            CheckBox8.CheckState = 0
            CheckBox9.CheckState = 0
            CheckBox10.CheckState = 0
            CheckBox11.CheckState = 0
            CheckBox12.CheckState = 0
            CheckBox13.CheckState = 0
            CheckBox14.CheckState = 0
            CheckBox15.CheckState = 0
            CheckBox16.CheckState = 0
            CheckBox17.CheckState = 0
            CheckBox18.CheckState = 0
        End If
    End Sub
    Private Sub box8() Handles CheckBox8.CheckedChanged
        If CheckBox8.Checked = True Then
            CheckBox4.CheckState = 0
            CheckBox5.CheckState = 0
            CheckBox6.CheckState = 0
            CheckBox7.CheckState = 0
            CheckBox9.CheckState = 0
            CheckBox10.CheckState = 0
            CheckBox11.CheckState = 0
            CheckBox12.CheckState = 0
            CheckBox13.CheckState = 0
            CheckBox14.CheckState = 0
            CheckBox15.CheckState = 0
            CheckBox16.CheckState = 0
            CheckBox17.CheckState = 0
            CheckBox18.CheckState = 0
        End If
    End Sub
    Private Sub box9() Handles CheckBox9.CheckedChanged
        If CheckBox9.Checked = True Then
            CheckBox4.CheckState = 0
            CheckBox5.CheckState = 0
            CheckBox6.CheckState = 0
            CheckBox7.CheckState = 0
            CheckBox8.CheckState = 0
            CheckBox10.CheckState = 0
            CheckBox11.CheckState = 0
            CheckBox12.CheckState = 0
            CheckBox13.CheckState = 0
            CheckBox14.CheckState = 0
            CheckBox15.CheckState = 0
            CheckBox16.CheckState = 0
            CheckBox17.CheckState = 0
            CheckBox18.CheckState = 0
        End If
    End Sub
    Private Sub box10() Handles CheckBox10.CheckedChanged
        If CheckBox10.Checked = True Then
            CheckBox4.CheckState = 0
            CheckBox5.CheckState = 0
            CheckBox6.CheckState = 0
            CheckBox7.CheckState = 0
            CheckBox8.CheckState = 0
            CheckBox9.CheckState = 0
            CheckBox11.CheckState = 0
            CheckBox12.CheckState = 0
            CheckBox13.CheckState = 0
            CheckBox14.CheckState = 0
            CheckBox15.CheckState = 0
            CheckBox16.CheckState = 0
            CheckBox17.CheckState = 0
            CheckBox18.CheckState = 0
        End If
    End Sub
    Private Sub box11() Handles CheckBox11.CheckedChanged
        If CheckBox11.Checked = True Then
            CheckBox4.CheckState = 0
            CheckBox5.CheckState = 0
            CheckBox6.CheckState = 0
            CheckBox7.CheckState = 0
            CheckBox8.CheckState = 0
            CheckBox9.CheckState = 0
            CheckBox10.CheckState = 0
            CheckBox12.CheckState = 0
            CheckBox13.CheckState = 0
            CheckBox14.CheckState = 0
            CheckBox15.CheckState = 0
            CheckBox16.CheckState = 0
            CheckBox17.CheckState = 0
            CheckBox18.CheckState = 0
        End If
    End Sub
    Private Sub box12() Handles CheckBox12.CheckedChanged
        If CheckBox12.Checked = True Then
            CheckBox4.CheckState = 0
            CheckBox5.CheckState = 0
            CheckBox6.CheckState = 0
            CheckBox7.CheckState = 0
            CheckBox8.CheckState = 0
            CheckBox9.CheckState = 0
            CheckBox10.CheckState = 0
            CheckBox11.CheckState = 0
            CheckBox13.CheckState = 0
            CheckBox14.CheckState = 0
            CheckBox15.CheckState = 0
            CheckBox16.CheckState = 0
            CheckBox17.CheckState = 0
            CheckBox18.CheckState = 0
        End If
    End Sub
    Private Sub box13() Handles CheckBox13.CheckedChanged
        If CheckBox13.Checked = True Then
            CheckBox4.CheckState = 0
            CheckBox5.CheckState = 0
            CheckBox6.CheckState = 0
            CheckBox7.CheckState = 0
            CheckBox8.CheckState = 0
            CheckBox9.CheckState = 0
            CheckBox10.CheckState = 0
            CheckBox11.CheckState = 0
            CheckBox12.CheckState = 0
            CheckBox14.CheckState = 0
            CheckBox15.CheckState = 0
            CheckBox16.CheckState = 0
            CheckBox17.CheckState = 0
            CheckBox18.CheckState = 0
        End If
    End Sub
    Private Sub box14() Handles CheckBox14.CheckedChanged
        If CheckBox14.Checked = True Then
            CheckBox4.CheckState = 0
            CheckBox5.CheckState = 0
            CheckBox6.CheckState = 0
            CheckBox7.CheckState = 0
            CheckBox8.CheckState = 0
            CheckBox9.CheckState = 0
            CheckBox10.CheckState = 0
            CheckBox11.CheckState = 0
            CheckBox12.CheckState = 0
            CheckBox13.CheckState = 0
            CheckBox15.CheckState = 0
            CheckBox16.CheckState = 0
            CheckBox17.CheckState = 0
            CheckBox18.CheckState = 0
        End If
    End Sub
    Private Sub box15() Handles CheckBox15.CheckedChanged
        If CheckBox15.Checked = True Then
            CheckBox4.CheckState = 0
            CheckBox5.CheckState = 0
            CheckBox6.CheckState = 0
            CheckBox7.CheckState = 0
            CheckBox8.CheckState = 0
            CheckBox9.CheckState = 0
            CheckBox10.CheckState = 0
            CheckBox11.CheckState = 0
            CheckBox12.CheckState = 0
            CheckBox13.CheckState = 0
            CheckBox14.CheckState = 0
            CheckBox16.CheckState = 0
            CheckBox17.CheckState = 0
            CheckBox18.CheckState = 0
        End If
    End Sub
    Private Sub box16() Handles CheckBox16.CheckedChanged
        If CheckBox16.Checked = True Then
            CheckBox4.CheckState = 0
            CheckBox5.CheckState = 0
            CheckBox6.CheckState = 0
            CheckBox7.CheckState = 0
            CheckBox8.CheckState = 0
            CheckBox9.CheckState = 0
            CheckBox10.CheckState = 0
            CheckBox11.CheckState = 0
            CheckBox12.CheckState = 0
            CheckBox13.CheckState = 0
            CheckBox14.CheckState = 0
            CheckBox15.CheckState = 0
            CheckBox17.CheckState = 0
            CheckBox18.CheckState = 0
        End If
    End Sub
    Private Sub box17() Handles CheckBox17.CheckedChanged
        If CheckBox17.Checked = True Then
            CheckBox4.CheckState = 0
            CheckBox5.CheckState = 0
            CheckBox6.CheckState = 0
            CheckBox7.CheckState = 0
            CheckBox8.CheckState = 0
            CheckBox9.CheckState = 0
            CheckBox10.CheckState = 0
            CheckBox11.CheckState = 0
            CheckBox12.CheckState = 0
            CheckBox13.CheckState = 0
            CheckBox14.CheckState = 0
            CheckBox15.CheckState = 0
            CheckBox16.CheckState = 0
            CheckBox18.CheckState = 0
        End If
    End Sub
    Private Sub box18() Handles CheckBox18.CheckedChanged
        If CheckBox18.Checked = True Then
            CheckBox4.CheckState = 0
            CheckBox5.CheckState = 0
            CheckBox6.CheckState = 0
            CheckBox7.CheckState = 0
            CheckBox8.CheckState = 0
            CheckBox9.CheckState = 0
            CheckBox10.CheckState = 0
            CheckBox11.CheckState = 0
            CheckBox12.CheckState = 0
            CheckBox13.CheckState = 0
            CheckBox14.CheckState = 0
            CheckBox15.CheckState = 0
            CheckBox16.CheckState = 0
            CheckBox17.CheckState = 0
        End If
    End Sub
#End Region
#End Region
    Private Sub ApplyChanges(sender As Object, e As EventArgs) Handles Button1.Click
        If CheckBox1.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\FemaleFix\_CRT\")
            IO.File.WriteAllBytes(ovrdir & "\FemaleFix\_CRT\PCFemale.CRT", My.Resources.PCFemale)
        ElseIf IO.File.Exists(ovrdir & "\FemaleFix\_CRT\PCFemale.CRT") Then
            IO.File.Delete(ovrdir & "\FemaleFix\_CRT\PCFemale.CRT")
            IO.Directory.Delete(ovrdir & "\FemaleFix\_CRT\")
            IO.Directory.Delete(ovrdir & "\FemaleFix")
        End If
        If CheckBox2.Checked Then
            line(27) = "enable wireframe = 1"
            IO.File.WriteAllLines(ifdir, line)
        Else
            line(27) = "enable wireframe = 0"
            IO.File.WriteAllLines(ifdir, line)
        End If
        If CheckBox3.Checked Then
            line(25) = "enable startup movies = 1"
            IO.File.WriteAllLines(ifdir, line)
        Else
            line(25) = "enable startup movies = 0"
            IO.File.WriteAllLines(ifdir, line)
        End If
#Region "Maps"
        If CheckBox4.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.AaronMap2)
        End If
        If CheckBox5.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.Building2)
        End If
        If CheckBox6.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.Building4)
        End If
        If CheckBox7.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.Canyon2)
        End If
        If CheckBox8.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.Fences)
        End If
        If CheckBox9.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.ScottEMap1)
        End If
        If CheckBox10.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.ScottEMap4)
        End If
        If CheckBox11.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.Vault)
        End If
        If CheckBox12.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.Building1)
        End If
        If CheckBox13.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.Building3)
        End If
        If CheckBox14.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.Canyon)
        End If
        If CheckBox15.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.Cave)
        End If
        If CheckBox16.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.MainMenu)
        End If
        If CheckBox17.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.ScottEMap2)
        End If
        If CheckBox18.Checked Then
            IO.Directory.CreateDirectory(ovrdir & "\MapChange\Engine\")
            IO.File.WriteAllBytes(ovrdir & "\MapChange\Engine\sys.ini", My.Resources.Shacks)
        End If
        If Not CheckBox4.Checked And Not CheckBox5.Checked And Not CheckBox6.Checked And Not CheckBox7.Checked And Not CheckBox8.Checked And Not CheckBox9.Checked And Not CheckBox10.Checked And Not CheckBox11.Checked And Not CheckBox12.Checked And Not CheckBox13.Checked And Not CheckBox14.Checked And Not CheckBox15.Checked And Not CheckBox16.Checked And Not CheckBox17.Checked And Not CheckBox18.Checked And IO.Directory.Exists(ovrdir & "/MapChange") Then
            IO.File.Delete(ovrdir & "\MapChange\Engine\sys.ini")
            IO.Directory.Delete(ovrdir & "\MapChange\Engine")
            IO.Directory.Delete(ovrdir & "\MapChange")
        End If
#End Region
        Hide()
    End Sub
    Private Sub SetResolution(sender As Object, e As EventArgs) Handles Button3.Click
        line(28) = "fullscreen = 1"
        line(29) = "height = " & My.Computer.Screen.Bounds.Height
        line(35) = "width = " & My.Computer.Screen.Bounds.Width
        IO.File.WriteAllLines(ifdir, line)
        MsgBox("Changed Settings to match your monitor!", MsgBoxStyle.Information Or MsgBoxStyle.OkOnly, "Success!")
    End Sub
End Class