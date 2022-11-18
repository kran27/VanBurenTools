Imports System.IO
Imports VBLauncher.IniManager
Imports VBLauncher.VideoInfo

Public Class VideoOptions
    Private dgV2Conf() As String
    Private ReadOnly AAModes As String() = {"off", "2x", "4x", "8x"}
    Private ReadOnly FModes As String() = {"appdriven", "pointsampled", "Linearmip", "2", "4", "8", "16"}
    Private ReadOnly SSModes As String() = {"unforced", "2x", "3x", "4x"}
    Private Sub CheckOptions() Handles MyBase.Load
        TextureCB.BringToFront()
        SSFCB.BringToFront()
        Try
            dgV2Conf = File.ReadAllLines("dgVoodoo.conf")
        Catch
            File.Exists("dgVoodoo.conf")
            File.WriteAllText("dgVoodoo.conf", My.Resources.dgV2conf)
            dgV2Conf = File.ReadAllLines("dgVoodoo.conf")
        End Try
        F3Ini = File.ReadAllLines(F3Dir)
        ResolutionCB.Items.Clear()
        ResolutionCB.Items.AddRange(GetResAsStrings)
        Dim inires = New Resolution With {
            .Width = F3Ini.Ini("Graphics", "width"),
            .Height = F3Ini.Ini("Graphics", "height"),
            .Hz = F3Ini.Ini("Graphics", "refresh")
        }
        ResolutionCB.SelectedItem = ResToStr(inires)
        SetupSSCB()
        FullscreenCB.Checked = F3Ini.Ini("Graphics", "fullscreen") = 1
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
        AACB.SelectedIndex = AAModes.ToList.IndexOf(dgV2Conf.Ini("DirectX", "Antialiasing"))
        TextureCB.SelectedIndex = FModes.ToList.IndexOf(dgV2Conf.Ini("DirectX", "Filtering"))
        SSFCB.SelectedIndex = SSModes.ToList.IndexOf(dgV2Conf.Ini("DirectX", "Resolution"))
        Try : MipmapCB.Checked = Not Boolean.Parse(dgV2Conf.Ini("DirectX", "DisableMipmapping")) : Catch : End Try
        Try : PhongCB.Checked = Boolean.Parse(dgV2Conf.Ini("DirectX", "PhongShadingWhenPossible")) : Catch : End Try
    End Sub

    Private Sub ApplyChanges() Handles ApplyB.Click
        Dim res = StrToRes(ResolutionCB.Text)
        F3Ini.Ini("Graphics", "fullscreen", If(FullscreenCB.Checked, 1, 0))
        F3Ini.Ini("Graphics", "width", res.Width) : F3Ini.Ini("Graphics", "height", res.Height)
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
        dgV2Conf.Ini("DirectX", "Antialiasing", AAModes(AACB.SelectedIndex))
        dgV2Conf.Ini("DirectX", "Filtering", FModes(TextureCB.SelectedIndex))
        dgV2Conf.Ini("DirectX", "Resolution", SSModes(SSFCB.SelectedIndex))
        dgV2Conf.Ini("DirectX", "DisableMipmapping", Not MipmapCB.Checked)
        dgV2Conf.Ini("DirectX", "PhongShadingWhenPossible", PhongCB.Checked)
        dgV2Conf.Ini("GeneralExt", "FPSLimit", res.Hz)
        F3Ini.Ini("Graphics", "refresh", res.Hz)
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
        Dim res = StrToRes(ResolutionCB.Text)
        SSFCB.Items.Clear()
        Dim resolutions() As String = {ResToStr(res, False), ResToStr(res, False, 2), ResToStr(res, False, 3), ResToStr(res, False, 4)}
        SSFCB.Items.AddRange(resolutions)
        SSFCB.SelectedIndex = index
    End Sub

End Class