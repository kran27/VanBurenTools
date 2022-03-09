Imports System.IO
Imports System.Runtime.InteropServices

Public Class VideoOptions
    Public IFDir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public Line() As String = File.ReadAllLines(IFDir)
    Public dgV2Line() As String
    Public SelH As String
    Public SelW As String

    Public Sub WriteTodgV2(LineNum As Integer, TextValue As String)
        dgV2Line = File.ReadAllLines("dgVoodoo.conf")
        dgV2Line(LineNum) = TextValue
        File.WriteAllLines("dgVoodoo.conf", dgV2Line)
    End Sub

    Public Sub WriteToF3Ini(LineNum As Integer, TextValue As String)
        Line = File.ReadAllLines(IFDir)
        Line(LineNum) = TextValue
        File.WriteAllLines(IFDir, Line)
    End Sub

    Private Sub CheckOptions() Handles MyBase.Load
        Icon = My.Resources.F3
        If Not File.Exists("dgVoodoo.conf") Then _
            File.WriteAllBytes("dgVoodoo.conf", My.Resources.dgV2conf)
        dgV2Line = File.ReadAllLines("dgVoodoo.conf")
        ResolutionCB.Items.Clear()
        ResolutionCB.Items.AddRange(EDSEW.GetSizesAsStrings)
        ResolutionCB.SelectedItem = Line(35).Remove(0, 8) & "x" & Line(29).Remove(0, 9)
        FullscreenCB.Checked = Line(28) = "fullscreen = 1"
        If File.Exists("d3d8.dll") Then
            If File.Exists("d3d11.dll") Then
                APICB.SelectedIndex = 3
            ElseIf File.Exists("wined3d.dll") Then
                APICB.SelectedIndex = 2
            Else
                APICB.SelectedIndex = 1
            End If
        Else
            APICB.SelectedIndex = 0
        End If
        Select Case dgV2Line(41)
            Case "Antialiasing = off" : AACB.SelectedIndex = 0
            Case "Antialiasing = 2x" : AACB.SelectedIndex = 1
            Case "Antialiasing = 4x" : AACB.SelectedIndex = 2
            Case "Antialiasing = 8x" : AACB.SelectedIndex = 3
        End Select
        Select Case dgV2Line(37)
            Case "Filtering = appdriven" : TextureCB.SelectedIndex = 0
            Case "Filtering = pointsampled" : TextureCB.SelectedIndex = 1
            Case "Filtering = Linearmip" : TextureCB.SelectedIndex = 2
            Case "Filtering = 2" : TextureCB.SelectedIndex = 3
            Case "Filtering = 4" : TextureCB.SelectedIndex = 4
            Case "Filtering = 8" : TextureCB.SelectedIndex = 5
            Case "Filtering = 16" : TextureCB.SelectedIndex = 6
        End Select
        MipmapCB.Checked = dgV2Line(39) = "DisableMipmapping = false"
        PhongCB.Checked = dgV2Line(45) = "PhongShadingWhenPossible = true"
    End Sub

    Private Sub ApplyChanges() Handles ApplyB.Click
        Dim Hz As Double = EDSEW.GetRefreshRate()
        Dim Iteration = 0
        For Each S As Size In EDSEW.GetSizes()
            If Iteration = ResolutionCB.SelectedIndex Then
                SelW = S.Width.ToString()
                SelH = S.Height.ToString()
            End If
            Iteration += 1
        Next
        If FullscreenCB.Checked Then
            WriteToF3Ini(28, "fullscreen = 1")
        Else
            WriteToF3Ini(28, "fullscreen = 0")
        End If
        WriteToF3Ini(29, "height = " & SelH) : WriteToF3Ini(35, "width = " & SelW)
        Select Case APICB.SelectedIndex
            Case 0
                File.Delete("d3d8.dll")
                File.Delete("d3d11.dll")
                File.Delete("dxgi.dll")
                File.Delete("wined3d.dll")
            Case 1
                File.Delete("d3d11.dll")
                File.Delete("dxgi.dll")
                File.Delete("wined3d.dll")
                File.WriteAllBytes("d3d8.dll", My.Resources.DXd3d8)
                WriteTodgV2(3, "OutputAPI = d3d11_fl10_1")
            Case 2
                File.Delete("d3d11.dll")
                File.Delete("dxgi.dll")
                File.WriteAllBytes("d3d8.dll", My.Resources.GLd3d8)
                File.WriteAllBytes("wined3d.dll", My.Resources.GLwined3d)
            Case 3
                File.Delete("wined3d.dll")
                File.WriteAllBytes("d3d8.dll", My.Resources.DXd3d8)
                File.WriteAllBytes("d3d11.dll", My.Resources.VKd3d11)
                File.WriteAllBytes("dxgi.dll", My.Resources.VKdxgi)
        End Select
        Select Case AACB.SelectedIndex
            Case 0 : WriteTodgV2(41, "Antialiasing = off")
            Case 1 : WriteTodgV2(41, "Antialiasing = 2x")
            Case 2 : WriteTodgV2(41, "Antialiasing = 4x")
            Case 3 : WriteTodgV2(41, "Antialiasing = 8x")
        End Select
        Select Case TextureCB.SelectedIndex
            Case 0 : WriteTodgV2(37, "Filtering = appdriven")
            Case 1 : WriteTodgV2(37, "Filtering = pointsampled")
            Case 2 : WriteTodgV2(37, "Filtering = Linearmip")
            Case 3 : WriteTodgV2(37, "Filtering = 2")
            Case 4 : WriteTodgV2(37, "Filtering = 4")
            Case 5 : WriteTodgV2(37, "Filtering = 8")
            Case 6 : WriteTodgV2(37, "Filtering = 16")
        End Select
        If MipmapCB.Checked Then WriteTodgV2(39, "DisableMipmapping = false") Else WriteTodgV2(39, "DisableMipmapping = true")
        If PhongCB.Checked Then WriteTodgV2(45, "PhongShadingWhenPossible = true") Else WriteTodgV2(45, "PhongShadingWhenPossible = false")
        WriteTodgV2(29, "FPSLimit = " & Math.Round(Hz))
        WriteToF3Ini(30, "mode32bpp = 1")
        WriteToF3Ini(31, "refresh = " & Hz)
        Hide()
    End Sub

    Private Sub DisableOptions() Handles APICB.SelectedIndexChanged
        Select Case APICB.SelectedIndex
            Case 2, 0
                AACB.Enabled = False
                TextureCB.Enabled = False
                MipmapCB.Enabled = False
                PhongCB.Enabled = False
                AAL.Enabled = False
                TextureL.Enabled = False
            Case Else
                AACB.Enabled = True
                TextureCB.Enabled = True
                MipmapCB.Enabled = True
                PhongCB.Enabled = True
                AAL.Enabled = True
                TextureL.Enabled = True
        End Select
    End Sub

End Class

Public Class EDSEW

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure DevModeW
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)> Private ReadOnly dmDeviceName As String
        Private ReadOnly dmSpecVersion As UShort
        Private ReadOnly dmDriverVersion As UShort
        Public dmSize As UShort
        Private ReadOnly dmDriverExtra As UShort
        Private ReadOnly dmFields As UInteger
        Private ReadOnly Union1 As Struct1
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
        Private ReadOnly Union2 As DFN
        Public ReadOnly dmDisplayFrequency As UInteger
    End Structure

    <StructLayout(LayoutKind.Explicit)>
    Private Structure Struct1
        <FieldOffset(0)> Private ReadOnly Struct1 As PDODFO
    End Structure

    <StructLayout(LayoutKind.Explicit)>
    Private Structure DFN
        <FieldOffset(0)> Private ReadOnly dmDisplayFlags As UInteger
        <FieldOffset(0)> Private ReadOnly dmNup As UInteger
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

    Public Shared Function GetRefreshRate() As Double
        Dim DM As New DevModeW
        Dim Hz = 0
        Dim Index = 0
        DM.dmSize = CUShort(Marshal.SizeOf(GetType(DevModeW)))
        While EnumDisplaySettingsExW(Screen.PrimaryScreen.DeviceName, Index, DM, 0)
            If DM.dmDisplayFrequency > Hz Then Hz = DM.dmDisplayFrequency
            Index += 1
        End While
        Return Hz
    End Function

    Public Shared Function GetSizes() As Size()
        Dim DM As New DevModeW
        Dim Index = 0
        Dim SizeList As New List(Of Size)
        DM.dmSize = CUShort(Marshal.SizeOf(GetType(DevModeW)))
        While EnumDisplaySettingsExW(Screen.PrimaryScreen.DeviceName, Index, DM, 0)
            Dim Size As New Size(CInt(DM.dmPelsWidth), CInt(DM.dmPelsHeight))
            If Not SizeList.Contains(Size) Then SizeList.Add(Size)
            Index += 1
        End While
        Return SizeList.ToArray
    End Function

End Class