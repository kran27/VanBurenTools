Imports System.IO
Imports System.Runtime.InteropServices
Imports VBLauncher.PShared

Public Class VideoOptions
    Private dgV2Conf() As String
    Private SelH As String
    Private SelW As String

    Private Sub CheckOptions() Handles MyBase.Load
        Icon = My.Resources.F3
        If Not File.Exists("dgVoodoo.conf") Then _
            File.WriteAllBytes("dgVoodoo.conf", My.Resources.dgV2conf)
        dgV2Conf = File.ReadAllLines("dgVoodoo.conf")
        F3Ini = File.ReadAllLines(F3Dir)
        ResolutionCB.Items.Clear()
        ResolutionCB.Items.AddRange(EDSEW.GetSizesAsStrings)
        ResolutionCB.SelectedItem = Ini(F3Ini, "Graphics", "width") & "x" & Ini(F3Ini, "Graphics", "height")
        FullscreenCB.Checked = Ini(F3Ini, "Graphics", "fullscreen") = 1
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
        Select Case Ini(dgV2Conf, "DirectX", "Antialiasing")
            Case "off" : AACB.SelectedIndex = 0
            Case "2x" : AACB.SelectedIndex = 1
            Case "4x" : AACB.SelectedIndex = 2
            Case "8x" : AACB.SelectedIndex = 3
        End Select
        Select Case Ini(dgV2Conf, "DirectX", "Filtering")
            Case "appdriven" : TextureCB.SelectedIndex = 0
            Case "pointsampled" : TextureCB.SelectedIndex = 1
            Case "Linearmip" : TextureCB.SelectedIndex = 2
            Case "2" : TextureCB.SelectedIndex = 3
            Case "4" : TextureCB.SelectedIndex = 4
            Case "8" : TextureCB.SelectedIndex = 5
            Case "16" : TextureCB.SelectedIndex = 6
        End Select
        MipmapCB.Checked = Not Boolean.Parse(Ini(dgV2Conf, "DirectX", "DisableMipmapping"))
        PhongCB.Checked = Boolean.Parse(Ini(dgV2Conf, "DirectX", "PhongShadingWhenPossible"))
    End Sub

    Private Sub ApplyChanges() Handles ApplyB.Click
        Dim Hz As Integer = EDSEW.GetRefreshRate()
        Dim Iteration = 0
        For Each S As Size In EDSEW.GetSizes()
            If Iteration = ResolutionCB.SelectedIndex Then
                SelW = S.Width.ToString()
                SelH = S.Height.ToString()
            End If
            Iteration += 1
        Next
        Ini(F3Ini, "Graphics", "fullscreen", If(FullscreenCB.Checked, 1, 0))
        Ini(F3Ini, "Graphics", "width", SelW) : Ini(F3Ini, "Graphics", "height", SelH)
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
            Case 0 : Ini(dgV2Conf, "DirectX", "Antialiasing", "off")
            Case 1 : Ini(dgV2Conf, "DirectX", "Antialiasing", "2x")
            Case 2 : Ini(dgV2Conf, "DirectX", "Antialiasing", "4x")
            Case 3 : Ini(dgV2Conf, "DirectX", "Antialiasing", "8x")
        End Select
        Select Case TextureCB.SelectedIndex
            Case 0 : Ini(dgV2Conf, "DirectX", "Filtering", "appdriven")
            Case 1 : Ini(dgV2Conf, "DirectX", "Filtering", "pointsampled")
            Case 2 : Ini(dgV2Conf, "DirectX", "Filtering", "Linearmip")
            Case 3 : Ini(dgV2Conf, "DirectX", "Filtering", "2")
            Case 4 : Ini(dgV2Conf, "DirectX", "Filtering", "4")
            Case 5 : Ini(dgV2Conf, "DirectX", "Filtering", "8")
            Case 6 : Ini(dgV2Conf, "DirectX", "Filtering", "16")
        End Select
        Ini(dgV2Conf, "DirectX", "DisableMipmapping", Not MipmapCB.Checked)
        Ini(dgV2Conf, "DirectX", "PhongShadingWhenPossible", PhongCB.Checked)
        Ini(dgV2Conf, "DirectX", "FPSLimit", Hz)
        Ini(F3Ini, "Graphics", "refresh", Hz)
        File.WriteAllLines("dgVoodoo.conf", dgV2Conf)
        File.WriteAllLines(F3Dir, F3Ini)
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

#Region "GPU/Display Information"
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
#End Region