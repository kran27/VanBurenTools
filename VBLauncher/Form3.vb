Imports sm = System.Management
Public Class Form3
    Public ifdir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public line() As String = IO.File.ReadAllLines(ifdir)
    Public hz As String
    Public bpp As String
    Public allcards As String = ""
    Protected Overrides Sub OnShown(ByVal e As System.EventArgs)
        MyBase.OnShown(e)
        Dim query As New sm.SelectQuery("Win32_VideoController")
        For Each mo As sm.ManagementObject In New sm.ManagementObjectSearcher(query).Get
            Dim CurrentRefreshRate As Object = mo("CurrentRefreshRate")
            Dim Currentbpp As Object = mo("CurrentBitsPerPixel")
            Dim VCName As Object = mo("Name")
            If Not allcards.Contains(VCName) Then
                allcards = allcards & VCName
            End If
            If CurrentRefreshRate IsNot Nothing Then
                hz = CurrentRefreshRate.ToString
            End If
            If Currentbpp IsNot Nothing Then
                bpp = Currentbpp.ToString
            End If
        Next
    End Sub
    Private Sub DetectOptions() Handles MyBase.VisibleChanged
        If line(28) = "fullscreen = 1" Then
            CheckBox1.Checked = True
        ElseIf line(29) = "height = " & 768 Then
            CheckBox2.Checked = True
        End If
        If Not allcards.Contains("NVIDIA") Or Not allcards.Contains("Intel") Then
            If Not ComboBox1.Items.Contains("DirectX 8 (Default)") Then
                ComboBox1.Items.Add("DirectX 8 (Default)")
            End If
            If Not ComboBox1.Items.Contains("Vulkan") Then
                ComboBox1.Items.Add("Vulkan")
            End If
        Else
            If Not ComboBox1.Items.Contains("DirectX 8 (Default)") Then
                ComboBox1.Items.Add("DirectX 8 (Default)")
            End If
            If Not ComboBox1.Items.Contains("OpenGL") Then
                ComboBox1.Items.Add("OpenGL")
            End If
            If Not ComboBox1.Items.Contains("Vulkan") Then
                ComboBox1.Items.Add("Vulkan")
            End If
        End If
        If Not IO.File.Exists(Application.StartupPath & "\d3d8.dll") Then
            ComboBox1.SelectedItem = "DirectX 8 (Default)"
        ElseIf IO.File.Exists(Application.StartupPath & "\libwine.dll") Then
            ComboBox1.SelectedItem = "OpenGL"
        ElseIf IO.File.Exists(Application.StartupPath & "\wined3d.dll") Then
            ComboBox1.SelectedItem = "Vulkan"
        Else
            ComboBox1.SelectedItem = "DirectX 8 (Default)"
        End If
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
            IO.File.Delete(Application.StartupPath & "\libwine.dll")
            IO.File.Delete(Application.StartupPath & "\wined3d.dll")
        ElseIf ComboBox1.SelectedItem = "OpenGL" Then
            IO.File.WriteAllBytes(Application.StartupPath & "\d3d8.dll", My.Resources.GLd3d8)
            IO.File.WriteAllBytes(Application.StartupPath & "\libwine.dll", My.Resources.GLlibwine)
            IO.File.WriteAllBytes(Application.StartupPath & "\wined3d.dll", My.Resources.GLwined3d)
        ElseIf ComboBox1.SelectedItem = "Vulkan" Then
            IO.File.WriteAllBytes(Application.StartupPath & "\d3d8.dll", My.Resources.VKd3d8)
            IO.File.WriteAllBytes(Application.StartupPath & "\wined3d.dll", My.Resources.VKwined3d)
            If IO.File.Exists(Application.StartupPath & "\libwine.dll") Then
                IO.File.Delete(Application.StartupPath & "\libwine.dll")
            End If
        End If
        Hide()
    End Sub

    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class