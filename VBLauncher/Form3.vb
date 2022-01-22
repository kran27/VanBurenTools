Imports sm = System.Management

Public Class Form3
    Public ifdir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public line() As String = IO.File.ReadAllLines(ifdir)
    Public dgV2line() As String = IO.File.ReadAllLines(Application.StartupPath & "\dgVoodoo.conf")
    Public hz As Double
    Public bpp As String
    Public vram As String

    Public Sub WriteTodgV2(ByVal LineNum As Integer, ByVal TextValue As String)
        dgV2line = IO.File.ReadAllLines(Application.StartupPath & "\dgVoodoo.conf")
        dgV2line(LineNum) = TextValue
        IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
    End Sub

    Private Sub DetectOptions() Handles MyBase.Shown
        Dim query As New sm.SelectQuery("Win32_VideoController")
        For Each mo As sm.ManagementObject In New sm.ManagementObjectSearcher(query).Get
            Dim CurrentRefreshRate As Object = mo("CurrentRefreshRate")
            Dim Currentbpp As Object = mo("CurrentBitsPerPixel")
            Dim oVRAM As Object = mo("AdapterRAM")
            If CurrentRefreshRate IsNot Nothing Then
                hz = CurrentRefreshRate.ToString
            End If
            If Currentbpp IsNot Nothing Then
                bpp = Currentbpp.ToString
            End If
            If oVRAM IsNot Nothing And oVRAM > vram Then
                vram = oVRAM
            End If
        Next
        If line(28) = "fullscreen = 1" Then
            CheckBox1.Checked = True
        ElseIf line(29) = "height = " & 768 Then
            CheckBox2.Checked = True
        End If
        If Not IO.File.Exists(Application.StartupPath & "\d3d8.dll") Then
            ComboBox1.SelectedIndex = 0
        ElseIf IO.File.Exists(Application.StartupPath & "\wined3d.dll") Then
            ComboBox1.SelectedIndex = 4
        ElseIf dgV2line(3) = "OutputAPI = d3d11_fl10_1" Then
            ComboBox1.SelectedIndex = 1
        ElseIf dgV2line(3) = "OutputAPI = d3d11_fl11_0" Then
            ComboBox1.SelectedIndex = 2
        ElseIf dgV2line(3) = "OutputAPI = d3d12_fl12_0" Then
            ComboBox1.SelectedIndex = 3
        End If
        Select Case dgV2line(41)
            Case "Antialiasing = off"
                ComboBox2.SelectedIndex = 0
            Case "Antialiasing = 2x"
                ComboBox2.SelectedIndex = 1
            Case "Antialiasing = 4x"
                ComboBox2.SelectedIndex = 2
            Case "Antialiasing = 8x"
                ComboBox2.SelectedIndex = 3
        End Select
        Select Case dgV2line(37)
            Case "Filtering = appdriven"
                ComboBox3.SelectedIndex = 0
            Case "Filtering = pointsampled"
                ComboBox3.SelectedIndex = 1
            Case "Filtering = linearmip"
                ComboBox3.SelectedIndex = 2
            Case "Filtering = 2"
                ComboBox3.SelectedIndex = 3
            Case "Filtering = 4"
                ComboBox3.SelectedIndex = 4
            Case "Filtering = 8"
                ComboBox3.SelectedIndex = 5
            Case "Filtering = 16"
                ComboBox3.SelectedIndex = 6
        End Select
        If dgV2line(39) = "DisableMipmapping = false" Then
            CheckBox3.Checked = True
        Else
            CheckBox3.Checked = False
        End If
        If dgV2line(45) = "PhongShadingWhenPossible = true" Then
            CheckBox4.Checked = True
        Else
            CheckBox4.Checked = False
        End If
        vram /= 1048576
        If vram > 4096 Then
            vram = 4096
        End If
        WriteTodgV2(36, "VRAM = " & vram)
        WriteTodgV2(29, "FPSLimit = " & Math.Floor(hz))
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            CheckBox2.Checked = False
            CheckBox2.Enabled = False
        Else
            CheckBox2.Enabled = True
        End If
    End Sub

    Private Sub ApplyChanges(sender As Object, e As EventArgs) Handles Button1.Click
        If bpp >= 32 Then
            line(30) = "mode32bpp = 1"
            IO.File.WriteAllLines(ifdir, line)
        End If
        If CheckBox1.Checked Then
            line(28) = "fullscreen = 1"
            line(29) = "height = " & My.Computer.Screen.Bounds.Height
            line(35) = "width = " & My.Computer.Screen.Bounds.Width
            line(31) = "refresh = " & hz
            IO.File.WriteAllLines(ifdir, line)
        ElseIf CheckBox2.Checked Then
            line(28) = "fullscreen = 0"
            line(29) = "height = " & 768
            line(35) = "width = " & 1024
            IO.File.WriteAllLines(ifdir, line)
        Else
            line(28) = "fullscreen = 0"
            line(29) = "height = " & 900
            line(35) = "width = " & 1600
            IO.File.WriteAllLines(ifdir, line)
        End If
        Select Case ComboBox1.SelectedIndex
            Case 0
                IO.File.Delete(Application.StartupPath & "\d3d8.dll")
                IO.File.Delete(Application.StartupPath & "\wined3d.dll")
            Case 1
                IO.File.Delete(Application.StartupPath & "\wined3d.dll")
                IO.File.WriteAllBytes(Application.StartupPath & "\d3d8.dll", My.Resources.DXD3D8)
                WriteTodgV2(3, "OutputAPI = d3d11_fl10_1")
            Case 2
                IO.File.Delete(Application.StartupPath & "\wined3d.dll")
                IO.File.WriteAllBytes(Application.StartupPath & "\d3d8.dll", My.Resources.DXD3D8)
                WriteTodgV2(3, "OutputAPI = d3d11_fl11_0")
            Case 3
                IO.File.Delete(Application.StartupPath & "\wined3d.dll")
                IO.File.WriteAllBytes(Application.StartupPath & "\d3d8.dll", My.Resources.DXD3D8)
                WriteTodgV2(3, "OutputAPI = d3d12_fl12_0")
            Case 4
                IO.File.WriteAllBytes(Application.StartupPath & "\d3d8.dll", My.Resources.GLd3d8)
                IO.File.WriteAllBytes(Application.StartupPath & "\wined3d.dll", My.Resources.GLwined3d)
        End Select
        Select Case ComboBox2.SelectedIndex
            Case 0
                WriteTodgV2(41, "Antialiasing = off")
            Case 1
                WriteTodgV2(41, "Antialiasing = 2x")
            Case 2
                WriteTodgV2(41, "Antialiasing = 4x")
            Case 3
                WriteTodgV2(41, "Antialiasing = 8x")
        End Select
        Select Case ComboBox3.SelectedIndex
            Case 0
                WriteTodgV2(37, "Filtering = appdriven")
            Case 1
                WriteTodgV2(37, "Filtering = pointsampled")
            Case 2
                WriteTodgV2(37, "Filtering = linearmip")
            Case 3
                WriteTodgV2(37, "Filtering = 2")
            Case 4
                WriteTodgV2(37, "Filtering = 4")
            Case 5
                WriteTodgV2(37, "Filtering = 8")
            Case 6
                WriteTodgV2(37, "Filtering = 16")
        End Select
        If CheckBox3.Checked Then
            WriteTodgV2(39, "DisableMipmapping = false")
        Else
            WriteTodgV2(39, "DisableMipmapping = true")
        End If
        If CheckBox4.Checked Then
            WriteTodgV2(45, "PhongShadingWhenPossible = true")
        Else
            WriteTodgV2(45, "PhongShadingWhenPossible = false")
        End If
        Hide()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex = 1 Or ComboBox1.SelectedIndex = 2 Or ComboBox1.SelectedIndex = 3 Then
            Label2.Enabled = True
            Label3.Enabled = True
            ComboBox2.Enabled = True
            ComboBox3.Enabled = True
            CheckBox3.Enabled = True
            CheckBox4.Enabled = True
        Else
            Label2.Enabled = False
            Label3.Enabled = False
            ComboBox2.Enabled = False
            ComboBox3.Enabled = False
            CheckBox3.Enabled = False
            CheckBox4.Enabled = False
        End If
        If ComboBox1.SelectedIndex = 1 Then
            CheckBox4.Enabled = False
        End If
    End Sub

End Class