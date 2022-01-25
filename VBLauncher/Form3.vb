Imports System.Runtime.InteropServices
Imports sm = System.Management

Public Class Form3
    Public IFDir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public Line() As String = IO.File.ReadAllLines(IFDir)
    Public dgV2Line() As String = IO.File.ReadAllLines(Application.StartupPath & "\dgVoodoo.conf")
    Public Hz As Double
    Public bpp As String
    Public VRAM As String
    Public SelH As String
    Public SelW As String

    Public Sub WriteTodgV2(LineNum As Integer, TextValue As String)
        dgV2Line = IO.File.ReadAllLines(Application.StartupPath & "\dgVoodoo.conf")
        dgV2Line(LineNum) = TextValue
        IO.File.WriteAllLines(Application.StartupPath & "\dgVoodoo.conf", dgV2Line)
    End Sub

    Private Sub DetectOptions() Handles MyBase.Shown
        Dim Query As New sm.SelectQuery("Win32_VideoController")
        For Each Mo As sm.ManagementObject In New sm.ManagementObjectSearcher(Query).Get
            Dim CurrentRefreshRate As Object = Mo("CurrentRefreshRate")
            Dim Currentbpp As Object = Mo("CurrentBitsPerPixel")
            Dim OVRAM As Object = Mo("AdapterRAM")
            If CurrentRefreshRate IsNot Nothing Then Hz = CurrentRefreshRate.ToString
            If Currentbpp IsNot Nothing Then bpp = Currentbpp.ToString
            If OVRAM IsNot Nothing And OVRAM > VRAM Then VRAM = OVRAM
        Next
        ComboBox4.Items.Clear()
        ComboBox4.Items.AddRange(SupportedScreenSizes.GetSizesAsStrings)
        ComboBox4.SelectedItem = Line(35).Remove(0, 8) & "x" & Line(29).Remove(0, 9)
        If Line(28) = "fullscreen = 1" Then CheckBox1.Checked = True Else CheckBox1.Checked = False
        If Not IO.File.Exists(Application.StartupPath & "\d3d8.dll") Then : ComboBox1.SelectedIndex = 0
        ElseIf IO.File.Exists(Application.StartupPath & "\wined3d.dll") Then : ComboBox1.SelectedIndex = 4
        ElseIf dgV2Line(3) = "OutputAPI = d3d11_fl10_1" Then : ComboBox1.SelectedIndex = 1
        ElseIf dgV2Line(3) = "OutputAPI = d3d11_fl11_0" Then : ComboBox1.SelectedIndex = 2
        ElseIf dgV2Line(3) = "OutputAPI = d3d12_fl12_0" Then : ComboBox1.SelectedIndex = 3 : End If
        Select Case dgV2Line(41)
            Case "Antialiasing = off" : ComboBox2.SelectedIndex = 0
            Case "Antialiasing = 2x" : ComboBox2.SelectedIndex = 1
            Case "Antialiasing = 4x" : ComboBox2.SelectedIndex = 2
            Case "Antialiasing = 8x" : ComboBox2.SelectedIndex = 3
        End Select
        Select Case dgV2Line(37)
            Case "Filtering = appdriven" : ComboBox3.SelectedIndex = 0
            Case "Filtering = pointsampled" : ComboBox3.SelectedIndex = 1
            Case "Filtering = Linearmip" : ComboBox3.SelectedIndex = 2
            Case "Filtering = 2" : ComboBox3.SelectedIndex = 3
            Case "Filtering = 4" : ComboBox3.SelectedIndex = 4
            Case "Filtering = 8" : ComboBox3.SelectedIndex = 5
            Case "Filtering = 16" : ComboBox3.SelectedIndex = 6
        End Select
        If dgV2Line(39) = "DisableMipmapping = 0" Then CheckBox3.Checked = True Else CheckBox3.Checked = False
        If dgV2Line(45) = "PhongShadingWhenPossible = 1" Then CheckBox4.Checked = True Else CheckBox4.Checked = False
        VRAM /= 1048576
        If VRAM > 4096 Then VRAM = 4096
        WriteTodgV2(36, "VRAM = " & VRAM)
        WriteTodgV2(29, "FPSLimit = " & Math.Floor(Hz))
    End Sub

    Private Sub ApplyChanges() Handles Button1.Click
        Dim Iteration As Integer = 0
        For Each S As Size In SupportedScreenSizes.GetSizes()
            If Iteration = ComboBox4.SelectedIndex Then
                SelW = S.Width.ToString()
                SelH = S.Height.ToString()
            End If
            Iteration += 1
        Next
        If bpp >= 32 Then Line(30) = "mode32bpp = 1" : IO.File.WriteAllLines(IFDir, Line)
        If CheckBox1.Checked Then
            Line(28) = "fullscreen = 1"
            Line(31) = "refresh = " & Hz
            IO.File.WriteAllLines(IFDir, Line)
        Else
            Line(28) = "fullscreen = 0"
            IO.File.WriteAllLines(IFDir, Line)
        End If
        Line(29) = "height = " & SelH : Line(35) = "width = " & SelW
        IO.File.WriteAllLines(IFDir, Line)
        Select Case ComboBox1.SelectedIndex
            Case 0
                IO.File.Delete(Application.StartupPath & "\d3d8.dll")
                IO.File.Delete(Application.StartupPath & "\wined3d.dll")
            Case 1
                IO.File.Delete(Application.StartupPath & "\wined3d.dll")
                IO.File.WriteAllBytes(Application.StartupPath & "\d3d8.dll", My.Resources.DXd3d8)
                WriteTodgV2(3, "OutputAPI = d3d11_fl10_1")
            Case 2
                IO.File.Delete(Application.StartupPath & "\wined3d.dll")
                IO.File.WriteAllBytes(Application.StartupPath & "\d3d8.dll", My.Resources.DXd3d8)
                WriteTodgV2(3, "OutputAPI = d3d11_fl11_0")
            Case 3
                IO.File.Delete(Application.StartupPath & "\wined3d.dll")
                IO.File.WriteAllBytes(Application.StartupPath & "\d3d8.dll", My.Resources.DXd3d8)
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
            Case 2 : WriteTodgV2(37, "Filtering = Linearmip")
            Case 3 : WriteTodgV2(37, "Filtering = 2")
            Case 4 : WriteTodgV2(37, "Filtering = 4")
            Case 5 : WriteTodgV2(37, "Filtering = 8")
            Case 6 : WriteTodgV2(37, "Filtering = 16")
        End Select
        If CheckBox3.Checked Then WriteTodgV2(39, "DisableMipmapping = 0") Else WriteTodgV2(39, "DisableMipmapping = 1")
        If CheckBox4.Checked Then WriteTodgV2(45, "PhongShadingWhenPossible = 1") Else WriteTodgV2(45, "PhongShadingWhenPossible = 0")
        Hide()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged() Handles ComboBox1.SelectedIndexChanged
        Select Case ComboBox1.SelectedIndex
            Case 1, 2, 3
                Label2.Enabled = 1 : Label3.Enabled = 1
                ComboBox2.Enabled = 1 : ComboBox3.Enabled = 1
                CheckBox3.Enabled = 1 : CheckBox4.Enabled = 1
            Case Else
                Label2.Enabled = 0 : Label3.Enabled = 0
                ComboBox2.Enabled = 0 : ComboBox3.Enabled = 0
                CheckBox3.Enabled = 0 : CheckBox4.Enabled = 0
        End Select
        If ComboBox1.SelectedIndex = 1 Then CheckBox4.Enabled = 0
    End Sub

End Class

Public Class SupportedScreenSizes
    Private Const DMPelsWidth As Integer = &H80000
    Private Const DMPelsHeight As Integer = &H100000

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure DevModeW
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)> Private ReadOnly dmDeviceName As String
        Private ReadOnly dmSpecVersion As UShort
        Private ReadOnly dmDriverVersion As UShort
        Public dmSize As UShort
        Private ReadOnly dmDriverExtra As UShort
        Public dmFields As UInteger
        Private ReadOnly Union1 As S1S2
        Private ReadOnly dmColor As Short
        Private ReadOnly dmDuplex As Short
        Private ReadOnly dmYResolution As Short
        Private ReadOnly dmTTOption As Short
        Private ReadOnly dmCollate As Short
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)> Private ReadOnly dmFormName As String
        Private ReadOnly dmLogPixels As UShort
        Private ReadOnly dmBitsPerPel As UInteger
        Public ReadOnly dmPelsWidth As UInteger
        Public ReadOnly dmPelsHeight As UInteger
    End Structure

    <StructLayout(LayoutKind.Explicit)>
    Private Structure S1S2
        <FieldOffset(0)> Private ReadOnly Struct1 As PDODFO
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure PDODFO
        Private ReadOnly dmPosition As PointL
        Private ReadOnly dmDisplayOrientation As UInteger
        Private ReadOnly dmDisplayFixedOutput As UInteger
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure PointL
        Private ReadOnly x As Integer
        Private ReadOnly y As Integer
    End Structure

    <DllImport("user32.dll", EntryPoint:="EnumDisplaySettingsExW")>
    Private Shared Function EnumDisplaySettingsExW(<MarshalAs(UnmanagedType.LPWStr)> DeviceName As String, ModeNum As Integer, ByRef DevMode As DevModeW, Flags As UInteger) As Boolean
    End Function

    Public Shared Function GetSizesAsStrings() As String()
        Dim SizeList As List(Of String) = (From S In GetSizes() Select S.Width.ToString & "x" & S.Height.ToString).ToList()
        Return SizeList.ToArray
    End Function

    Public Shared Function GetSizes() As Size()
        Dim SizeList As New List(Of Size)
        Dim Index As Integer = 0
        Dim DM As New DevModeW
        DM.dmFields = DMPelsWidth Or DMPelsHeight
        DM.dmSize = CUShort(Marshal.SizeOf(GetType(DevModeW)))
        While EnumDisplaySettingsExW(Screen.PrimaryScreen.DeviceName, Index, DM, 0)
            Dim Size As New Size(CInt(DM.dmPelsWidth), CInt(DM.dmPelsHeight))
            If Not SizeList.Contains(Size) Then SizeList.Add(Size)
            Index += 1
        End While
        Return SizeList.ToArray
    End Function

End Class