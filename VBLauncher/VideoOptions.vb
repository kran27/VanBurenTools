Imports System.IO
Imports VBLauncher.IniManager
Imports VBLauncher.VideoInfo

Public Class VideoOptions
    Private dgV2Conf() As String
    Private SelH As String
    Private SelW As String

    Private Sub CheckOptions() Handles MyBase.Load
        TextureCB.BringToFront()
        SSFCB.BringToFront()
        Icon = My.Resources.F3
        Try
            dgV2Conf = File.ReadAllLines("dgVoodoo.conf")
        Catch
            File.Exists("dgVoodoo.conf")
            File.WriteAllText("dgVoodoo.conf", My.Resources.dgV2conf)
            dgV2Conf = File.ReadAllLines("dgVoodoo.conf")
        End Try
        F3Ini = File.ReadAllLines(F3Dir)
        ResolutionCB.Items.Clear()
        ResolutionCB.Items.AddRange(GetSizesAsStrings)
        Dim inires = New Size(Ini(F3Ini, "Graphics", "width"), Ini(F3Ini, "Graphics", "height"))
        ResolutionCB.SelectedItem = SizeToStr(inires)
        SetupSSCB()
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
        Select Case Ini(dgV2Conf, "DirectX", "Resolution")
            Case "unforced" : SSFCB.SelectedIndex = 0
            Case "2x" : SSFCB.SelectedIndex = 1
            Case "3x" : SSFCB.SelectedIndex = 2
            Case "4x" : SSFCB.SelectedIndex = 3
        End Select
        MipmapCB.Checked = Not Boolean.Parse(Ini(dgV2Conf, "DirectX", "DisableMipmapping"))
        PhongCB.Checked = Boolean.Parse(Ini(dgV2Conf, "DirectX", "PhongShadingWhenPossible"))
    End Sub

    Private Sub ApplyChanges() Handles ApplyB.Click
        Dim Hz As Integer = GetRefreshRate()
        Dim Iteration = 0
        Dim res = StrToSize(ResolutionCB.Text)
        Ini(F3Ini, "Graphics", "fullscreen", If(FullscreenCB.Checked, 1, 0))
        Ini(F3Ini, "Graphics", "width", res.Width) : Ini(F3Ini, "Graphics", "height", res.Height)
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
            Case 3 : Ini(dgV2Conf, "DirectX", "Filtering", 2)
            Case 4 : Ini(dgV2Conf, "DirectX", "Filtering", 4)
            Case 5 : Ini(dgV2Conf, "DirectX", "Filtering", 8)
            Case 6 : Ini(dgV2Conf, "DirectX", "Filtering", 16)
        End Select
        Select Case SSFCB.SelectedIndex
            Case 0 : Ini(dgV2Conf, "DirectX", "Resolution", "unforced")
            Case 1 : Ini(dgV2Conf, "DirectX", "Resolution", "2x")
            Case 2 : Ini(dgV2Conf, "DirectX", "Resolution", "3x")
            Case 3 : Ini(dgV2Conf, "DirectX", "Resolution", "4x")
        End Select
        Ini(dgV2Conf, "DirectX", "DisableMipmapping", Not MipmapCB.Checked)
        Ini(dgV2Conf, "DirectX", "PhongShadingWhenPossible", PhongCB.Checked)
        Ini(dgV2Conf, "GeneralExt", "FPSLimit", Hz)
        Ini(F3Ini, "Graphics", "refresh", Hz)
        File.WriteAllLines("dgVoodoo.conf", dgV2Conf)
        File.WriteAllLines(F3Dir, F3Ini)
        Hide()
    End Sub

    Private Sub DisableOptions() Handles APICB.SelectedIndexChanged
        Select Case APICB.SelectedIndex
            Case 2, 0
                AACB.Enabled = False
                SSFCB.Enabled = False
                TextureCB.Enabled = False
                MipmapCB.Enabled = False
                PhongCB.Enabled = False
                AAL.Enabled = False
                SSFL.Enabled = False
                TextureL.Enabled = False
            Case Else
                AACB.Enabled = True
                SSFCB.Enabled = True
                TextureCB.Enabled = True
                MipmapCB.Enabled = True
                PhongCB.Enabled = True
                AAL.Enabled = True
                SSFL.Enabled = True
                TextureL.Enabled = True
        End Select
    End Sub

    Private Sub SetupSSCB() Handles ResolutionCB.SelectedIndexChanged
        Dim index = SSFCB.SelectedIndex
        Dim res = StrToSize(ResolutionCB.Text)
        SSFCB.Items.Clear()
        Dim resolutions() As String = {SizeToStr(res), SizeToStr(res, 2), SizeToStr(res, 3), SizeToStr(res, 4)}
        SSFCB.Items.AddRange(resolutions)
        SSFCB.SelectedIndex = index
    End Sub

End Class