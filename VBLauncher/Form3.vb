Imports System.Runtime.InteropServices
Imports sm = System.Management

Public Class Form3
    Public ifdir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public line() As String = IO.File.ReadAllLines(ifdir)
    Public dgV2line() As String = IO.File.ReadAllLines(Application.StartupPath & "\dgVoodoo.conf")
    Public hz As Double
    Public bpp As String
    Public vram As String
    Public SelH As String
    Public SelW As String

    Public Sub WriteTodgV2(LineNum As Integer, TextValue As String)
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
            If CurrentRefreshRate IsNot Nothing Then hz = CurrentRefreshRate.ToString
            If Currentbpp IsNot Nothing Then bpp = Currentbpp.ToString
            If oVRAM IsNot Nothing And oVRAM > vram Then vram = oVRAM
        Next
        ComboBox4.Items.AddRange(SupportedScreenSizes.GetSizesAsStrings)
        ComboBox4.SelectedItem = line(35).Remove(0, 8) & "x" & line(29).Remove(0, 9)
        If line(28) = "fullscreen = 1" Then CheckBox1.Checked = True Else CheckBox1.Checked = False
        If Not IO.File.Exists(Application.StartupPath & "\d3d8.dll") Then : ComboBox1.SelectedIndex = 0
        ElseIf IO.File.Exists(Application.StartupPath & "\wined3d.dll") Then : ComboBox1.SelectedIndex = 4
        ElseIf dgV2line(3) = "OutputAPI = d3d11_fl10_1" Then : ComboBox1.SelectedIndex = 1
        ElseIf dgV2line(3) = "OutputAPI = d3d11_fl11_0" Then : ComboBox1.SelectedIndex = 2
        ElseIf dgV2line(3) = "OutputAPI = d3d12_fl12_0" Then : ComboBox1.SelectedIndex = 3 : End If
        Select Case dgV2line(41)
            Case "Antialiasing = off" : ComboBox2.SelectedIndex = 0
            Case "Antialiasing = 2x" : ComboBox2.SelectedIndex = 1
            Case "Antialiasing = 4x" : ComboBox2.SelectedIndex = 2
            Case "Antialiasing = 8x" : ComboBox2.SelectedIndex = 3
        End Select
        Select Case dgV2line(37)
            Case "Filtering = appdriven" : ComboBox3.SelectedIndex = 0
            Case "Filtering = pointsampled" : ComboBox3.SelectedIndex = 1
            Case "Filtering = linearmip" : ComboBox3.SelectedIndex = 2
            Case "Filtering = 2" : ComboBox3.SelectedIndex = 3
            Case "Filtering = 4" : ComboBox3.SelectedIndex = 4
            Case "Filtering = 8" : ComboBox3.SelectedIndex = 5
            Case "Filtering = 16" : ComboBox3.SelectedIndex = 6
        End Select
        If dgV2line(39) = "DisableMipmapping = 0" Then CheckBox3.Checked = True Else CheckBox3.Checked = False
        If dgV2line(45) = "PhongShadingWhenPossible = 1" Then CheckBox4.Checked = True Else CheckBox4.Checked = False
        vram /= 1048576
        If vram > 4096 Then vram = 4096
        WriteTodgV2(36, "VRAM = " & vram)
        WriteTodgV2(29, "FPSLimit = " & Math.Floor(hz))
    End Sub

    Private Sub ApplyChanges(sender As Object, e As EventArgs) Handles Button1.Click
        Dim iteration As Integer = 0
        For Each s As Size In SupportedScreenSizes.GetSizes()
            If iteration = ComboBox4.SelectedIndex Then
                SelW = s.Width.ToString()
                SelH = s.Height.ToString()
            End If
            iteration += 1
        Next
        If bpp >= 32 Then line(30) = "mode32bpp = 1" : IO.File.WriteAllLines(ifdir, line)
        If CheckBox1.Checked Then
            line(28) = "fullscreen = 1"
            line(31) = "refresh = " & hz
            IO.File.WriteAllLines(ifdir, line)
        Else
            line(28) = "fullscreen = 0"
            IO.File.WriteAllLines(ifdir, line)
        End If
        line(29) = "height = " & SelH : line(35) = "width = " & SelW
        IO.File.WriteAllLines(ifdir, line)
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
            Case 0 : WriteTodgV2(41, "Antialiasing = off")
            Case 1 : WriteTodgV2(41, "Antialiasing = 2x")
            Case 2 : WriteTodgV2(41, "Antialiasing = 4x")
            Case 3 : WriteTodgV2(41, "Antialiasing = 8x")
        End Select
        Select Case ComboBox3.SelectedIndex
            Case 0 : WriteTodgV2(37, "Filtering = appdriven")
            Case 1 : WriteTodgV2(37, "Filtering = pointsampled")
            Case 2 : WriteTodgV2(37, "Filtering = linearmip")
            Case 3 : WriteTodgV2(37, "Filtering = 2")
            Case 4 : WriteTodgV2(37, "Filtering = 4")
            Case 5 : WriteTodgV2(37, "Filtering = 8")
            Case 6 : WriteTodgV2(37, "Filtering = 16")
        End Select
        If CheckBox3.Checked Then WriteTodgV2(39, "DisableMipmapping = 0") Else WriteTodgV2(39, "DisableMipmapping = 1")
        If CheckBox4.Checked Then WriteTodgV2(45, "PhongShadingWhenPossible = 1") Else WriteTodgV2(45, "PhongShadingWhenPossible = 0")
        Hide()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex = 1 Or ComboBox1.SelectedIndex = 2 Or ComboBox1.SelectedIndex = 3 Then
            Label2.Enabled = 1
            Label3.Enabled = 1
            ComboBox2.Enabled = 1
            ComboBox3.Enabled = 1
            CheckBox3.Enabled = 1
            CheckBox4.Enabled = 1
        Else
            Label2.Enabled = 0
            Label3.Enabled = 0
            ComboBox2.Enabled = 0
            ComboBox3.Enabled = 0
            CheckBox3.Enabled = 0
            CheckBox4.Enabled = 0
        End If
        If ComboBox1.SelectedIndex = 1 Then CheckBox4.Enabled = 0
    End Sub

End Class

Public Class SupportedScreenSizes
    Private Const DM_PELSWIDTH As Integer = &H80000
    Private Const DM_PELSHEIGHT As Integer = &H100000

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure DEVMODEW
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)> Public dmDeviceName As String
        Public dmSpecVersion As UShort
        Public dmDriverVersion As UShort
        Public dmSize As UShort
        Public dmDriverExtra As UShort
        Public dmFields As UInteger
        Public Union1 As Anonymous_7a7460d9_d99f_4e9a_9ebb_cdd10c08463d
        Public dmColor As Short
        Public dmDuplex As Short
        Public dmYResolution As Short
        Public dmTTOption As Short
        Public dmCollate As Short
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)> Public dmFormName As String
        Public dmLogPixels As UShort 'The number of pixels per logical inch. Printer drivers do not use this member.
        Public dmBitsPerPel As UInteger 'Specifies the color resolution, in bits per pixel, of the display device.
        Public dmPelsWidth As UInteger 'Specifies the width, in pixels, of the visible device surface.
        Public dmPelsHeight As UInteger 'Specifies the height, in pixels, of the visible device surface.
        Public Union2 As Anonymous_084dbe97_5806_4c28_a299_ed6037f61d90
        Public dmDisplayFrequency As UInteger 'Specifies the frequency, in hertz (cycles per second), of the display device in a particular mode.
        Public dmICMMethod As UInteger
        Public dmICMIntent As UInteger
        Public dmMediaType As UInteger
        Public dmDitherType As UInteger
        Public dmReserved1 As UInteger
        Public dmReserved2 As UInteger
        Public dmPanningWidth As UInteger
        Public dmPanningHeight As UInteger
    End Structure

    <StructLayout(LayoutKind.Explicit)>
    Private Structure Anonymous_7a7460d9_d99f_4e9a_9ebb_cdd10c08463d
        <FieldOffset(0)> Public Struct1 As Anonymous_865d3c92_fe8c_4ee6_9601_a9eb2536957e
        <FieldOffset(0)> Public Struct2 As Anonymous_1b5f787e_41ca_472c_8595_3484490ffe0c
    End Structure

    <StructLayout(LayoutKind.Explicit)>
    Private Structure Anonymous_084dbe97_5806_4c28_a299_ed6037f61d90
        <FieldOffset(0)> Public dmDisplayFlags As UInteger
        <FieldOffset(0)> Public dmNup As UInteger
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure Anonymous_865d3c92_fe8c_4ee6_9601_a9eb2536957e
        Public dmOrientation As Short
        Public dmPaperSize As Short
        Public dmPaperLength As Short
        Public dmPaperWidth As Short
        Public dmScale As Short
        Public dmCopies As Short
        Public dmDefaultSource As Short
        Public dmPrintQuality As Short
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure Anonymous_1b5f787e_41ca_472c_8595_3484490ffe0c
        Public dmPosition As POINTL
        Public dmDisplayOrientation As UInteger
        Public dmDisplayFixedOutput As UInteger
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure POINTL
        Public x As Integer
        Public y As Integer
    End Structure

    <DllImport("user32.dll", EntryPoint:="EnumDisplaySettingsExW")>
    Private Shared Function EnumDisplaySettingsExW(<MarshalAs(UnmanagedType.LPWStr)> lpszDeviceName As String, iModeNum As Integer, ByRef lpDevMode As DEVMODEW, dwFlags As UInteger) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    Public Shared Function GetSizesAsStrings() As String()
        Dim sizelist As New List(Of String)
        For Each s As Size In GetSizes()
            sizelist.Add(s.Width.ToString & "x" & s.Height.ToString)
        Next
        Return sizelist.ToArray
    End Function

    Public Shared Function GetSizes() As Size()
        Dim sizelist As New List(Of Size)
        Dim indx As Integer = 0
        Dim dm As New DEVMODEW
        dm.dmFields = DM_PELSWIDTH Or DM_PELSHEIGHT
        dm.dmSize = CUShort(Marshal.SizeOf(GetType(DEVMODEW)))
        While EnumDisplaySettingsExW(Screen.PrimaryScreen.DeviceName, indx, dm, 0)
            Dim sz As New Size(CInt(dm.dmPelsWidth), CInt(dm.dmPelsHeight))
            If Not sizelist.Contains(sz) Then sizelist.Add(sz)
            indx += 1
        End While
        Return sizelist.ToArray
    End Function

End Class