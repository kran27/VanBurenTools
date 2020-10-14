Public Class Form2
    Public ffdir As String = Application.StartupPath & "\Override\FemaleFix\_CRT\"
    Public ifdir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public line() As String = System.IO.File.ReadAllLines(ifdir)
    Private Sub CheckOptions() Handles Me.Activated
        If IO.File.Exists(ffdir & "PCFemale.CRT") Then
            CheckBox1.CheckState = 1
        End If
        If line(27) = "enable wireframe = 1" Then
            CheckBox3.CheckState = 1
        End If
        If line(25) = "enable startup movies = 1" Then
            CheckBox4.CheckState = 1
        End If
    End Sub
    Private Sub FemaleFix(sender As Object, e As EventArgs) Handles Button1.Click
        If CheckBox1.Checked = True Then
            System.IO.Directory.CreateDirectory(ffdir)
            System.IO.File.WriteAllBytes(ffdir & "PCFemale.CRT", My.Resources.PCFemale)
        ElseIf IO.File.Exists(ffdir & "PCFemale.CRT") Then
            System.IO.File.Delete(ffdir & "PCFemale.CRT")
            System.IO.Directory.Delete(ffdir)
            System.IO.Directory.Delete(Application.StartupPath & "\Override\FemaleFix\")
        End If
        If CheckBox2.Checked = True Then
            line(28) = "fullscreen = 1"
            line(29) = "height = " & My.Computer.Screen.Bounds.Height
            line(35) = "width = " & My.Computer.Screen.Bounds.Width
            System.IO.File.WriteAllLines(ifdir, line)
        End If
        If CheckBox3.Checked = True Then
            line(27) = "enable wireframe = 1"
            System.IO.File.WriteAllLines(ifdir, line)
        Else
            line(27) = "enable wireframe = 0"
            System.IO.File.WriteAllLines(ifdir, line)
        End If
        If CheckBox4.Checked = True Then
            line(25) = "enable startup movies = 1"
            System.IO.File.WriteAllLines(ifdir, line)
        Else
            line(25) = "enable startup movies = 0"
            System.IO.File.WriteAllLines(ifdir, line)
        End If
        Hide()
    End Sub
End Class