Imports sm = System.Management
Public Class Form3
    Public ifdir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public line() As String = IO.File.ReadAllLines(ifdir)
    Public dgV2line() As String = IO.File.ReadAllLines(Application.StartupPath & "\dgVoodoo.conf")
    Public hz As Double
    Public bpp As String
    Public vram As String
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
        Else
            ComboBox1.SelectedIndex = 0
        End If
        If dgV2line(41) = "Antialiasing = off" Then
            ComboBox2.SelectedIndex = 0
        ElseIf dgV2line(41) = "Antialiasing = 2x" Then
            ComboBox2.SelectedIndex = 1
        ElseIf dgV2line(41) = "Antialiasing = 4x" Then
            ComboBox2.SelectedIndex = 2
        ElseIf dgV2line(41) = "Antialiasing = 8x" Then
            ComboBox2.SelectedIndex = 3
        End If
        If dgV2line(37) = "Filtering = appdriven" Then
            ComboBox3.SelectedIndex = 0
        ElseIf dgV2line(37) = "Filtering = pointsampled" Then
            ComboBox3.SelectedIndex = 1
        ElseIf dgV2line(37) = "Filtering = linearmip" Then
            ComboBox3.SelectedIndex = 2
        ElseIf dgV2line(37) = "Filtering = 2" Then
            ComboBox3.SelectedIndex = 3
        ElseIf dgV2line(37) = "Filtering = 4" Then
            ComboBox3.SelectedIndex = 4
        ElseIf dgV2line(37) = "Filtering = 8" Then
            ComboBox3.SelectedIndex = 5
        ElseIf dgV2line(37) = "Filtering = 16" Then
            ComboBox3.SelectedIndex = 6
        End If
        If dgV2line(3) = "OutputAPI = d3d11_fl10_1" Then
            ComboBox1.SelectedIndex = 1
        ElseIf dgV2line(3) = "OutputAPI = d3d12_fl11_0" Then
            ComboBox1.SelectedIndex = 2
        ElseIf dgV2line(3) = "OutputAPI = d3d12_fl12_0" Then
            ComboBox1.SelectedIndex = 3
        End If
        If dgV2line(45) = "PhongShadingWhenPossible = true" Then
            CheckBox3.Checked = True
        Else
            checkbox3.Checked = False
        End If
        vram /= 1048576
        If vram > 4096 Then
            vram = 4096
        End If
        dgV2line(36) = "VRAM = " & vram
        dgV2line(29) = "FPSLimit = " & Math.Floor(hz)
        IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
        dgV2line = IO.File.ReadAllLines(Application.StartupPath & "\dgVoodoo.conf")
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
        Else
            If CheckBox2.Checked Then
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
        End If
        If ComboBox1.SelectedIndex = 0 Then
            IO.File.Delete(Application.StartupPath & "\d3d8.dll")
            IO.File.Delete(Application.StartupPath & "\wined3d.dll")
        ElseIf ComboBox1.SelectedIndex = 1 Or ComboBox1.SelectedIndex = 2 Or ComboBox1.SelectedIndex = 3 Then
            IO.File.Delete(Application.StartupPath & "\wined3d.dll")
            IO.File.WriteAllBytes(Application.StartupPath & "\d3d8.dll", My.Resources.DXD3D8)
            If ComboBox1.SelectedIndex = 1 Then
                dgV2line(3) = "OutputAPI = d3d11_fl10_1"
                IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
            ElseIf ComboBox1.SelectedIndex = 2 Then
                dgV2line(3) = "OutputAPI = d3d12_fl11_0"
                IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
            Else
                dgV2line(3) = "OutputAPI = d3d12_fl12_0"
                IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
            End If
        ElseIf ComboBox1.SelectedIndex = 4 Then
            IO.File.WriteAllBytes(Application.StartupPath & "\d3d8.dll", My.Resources.GLd3d8)
            IO.File.WriteAllBytes(Application.StartupPath & "\wined3d.dll", My.Resources.GLwined3d)
        End If
        If ComboBox2.SelectedIndex = 0 Then
            dgV2line(41) = "Antialiasing = off"
            IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
        ElseIf ComboBox2.SelectedIndex = 1 Then
            dgV2line(41) = "Antialiasing = 2x"
            IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
        ElseIf ComboBox2.SelectedIndex = 2 Then
            dgV2line(41) = "Antialiasing = 4x"
            IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
        ElseIf ComboBox2.SelectedIndex = 3 Then
            dgV2line(41) = "Antialiasing = 8x"
            IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
        End If
        If ComboBox3.SelectedIndex = 0 Then
            dgV2line(37) = "Filtering = appdriven"
            IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
        ElseIf ComboBox3.SelectedIndex = 1 Then
            dgV2line(37) = "Filtering = pointsampled"
            IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
        ElseIf ComboBox3.SelectedIndex = 2 Then
            dgV2line(37) = "Filtering = linearmip"
            IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
        ElseIf ComboBox3.SelectedIndex = 3 Then
            dgV2line(37) = "Filtering = 2"
            IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
        ElseIf ComboBox3.SelectedIndex = 4 Then
            dgV2line(37) = "Filtering = 4"
            IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
        ElseIf ComboBox3.SelectedIndex = 5 Then
            dgV2line(37) = "Filtering = 8"
            IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
        ElseIf ComboBox3.SelectedIndex = 6 Then
            dgV2line(37) = "Filtering = 16"
            IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
        End If
        If CheckBox3.Checked Then
            dgV2line(45) = "PhongShadingWhenPossible = true"
            IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
        Else
            dgV2line(45) = "PhongShadingWhenPossible = false"
            IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2line)
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
        Else
            Label2.Enabled = False
            Label3.Enabled = False
            ComboBox2.Enabled = False
            ComboBox3.Enabled = False
            CheckBox3.Enabled = False
        End If
        If ComboBox1.SelectedIndex = 1 Then
            CheckBox3.Enabled = False
        End If
    End Sub
End Class