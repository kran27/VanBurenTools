Imports System.Data
Imports System.IO
Imports AltUI.Config
Imports VBLauncher.General
Imports VBLauncher.IniManager
Imports VBLauncher.VideoInfo

Public Class Options
    Private ReadOnly MainMenus As MainMenuDef() = {MMD("mainmenu.map", 0, 5.5, 0, 2, 0.75, 59.2), MMD("zz_TestMapsaarontemp2.map", 0, 0, 0, 0, 0, 0), MMD("zz_TestMapsTest_City_Building01.map", 0, 5.5, 0, 45, -2.5, 57), MMD("zz_TestMapsTest_City_Building02.map", 0, 5.5, 0, 43, -2.5, 57), MMD("zz_TestMapsTest_City_Building03.map", 0, 5.5, 0, 43, -5, 57), MMD("zz_TestMapsTest_City_Building04.map", 0, 5.5, 0, 43, -5.5, 57), MMD("98_Canyon_Random_01.map", 50, 5, 10, 61, 0, 45), MMD("98_Canyon_Random_02.map", 55, 5, 10, 36, -2.5, 50), MMD("04_0202_Spelunking.map", 70, 5, 45, 15, 5, 50), MMD("zz_TestMapsTest_City_Fences.map", 0, 40, 0, 42, 35, 50), MMD("zz_TestMapsScottE_Test1.map", 85, 30, 30, 255, 39, 60), MMD("zz_TestMapsScottE_Test2.map", 145, 80, -85, 0.5, 25, 75), MMD("zz_TestMapsScottE_Test4.map", 0, 7.5, 0, 45, 12.5, 50), MMD("zz_TestMapsTest_Junktown_Shacks.map", 0, 50, -10, 42, 39, 50), MMD("Default_StartMap.map", 60, 7.5, 25, 270, 8, 27), MMD("00_03_Tutorial_Junktown.map", 80, 7.5, 50, 5, 10, 68), MMD("00_04_Tutorial_Vault.map", 50, 50.5, 0, 36, 25, 68)}
    Private ReadOnly Maps As String() = {"mainmenu.map", "zz_TestMapsaarontemp2.map", "zz_TestMapsTest_City_Building01.map", "zz_TestMapsTest_City_Building02.map", "zz_TestMapsTest_City_Building03.map", "zz_TestMapsTest_City_Building04.map", "98_Canyon_Random_01.map", "98_Canyon_Random_02.map", "04_0202_Spelunking.map", "zz_TestMapsTest_City_Fences.map", "zz_TestMapsScottE_Test1.map", "zz_TestMapsScottE_Test2.map", "zz_TestMapsScottE_Test4.map", "zz_TestMapsTest_Junktown_Shacks.map", "Default_StartMap.map", "00_03_Tutorial_Junktown.map", "00_04_Tutorial_Vault.map"}
    Private dgV2Conf() As String
    Private ReadOnly AAModes As String() = {"off", "2x", "4x", "8x"}
    Private ReadOnly FModes As String() = {"appdriven", "pointsampled", "Linearmip", "2", "4", "8", "16"}
    Private ReadOnly SSModes As String() = {"unforced", "2x", "3x", "4x"}
    Private Row As Integer
    Private Key As String
    Private Modifier As String
    Private WantKey As Boolean

    Private Shared Sub SetMainMenu(MMD As MainMenuDef)
        SysIni.Ini("Mainmenu", "map name", MMD.MapName)
        SysIni.Ini("Mainmenu", "target x", MMD.TargetX)
        SysIni.Ini("Mainmenu", "target y", MMD.TargetY)
        SysIni.Ini("Mainmenu", "target z", MMD.TargetZ)
        SysIni.Ini("Mainmenu", "azimuth", MMD.Azimuth)
        SysIni.Ini("Mainmenu", "elevation", MMD.Elevation)
        SysIni.Ini("Mainmenu", "fov", MMD.FOV)
    End Sub

    Private Shared Function MMD(MapName As String, TargetX As String, TargetY As String, TargetZ As String, Azimuth As String, Elevation As String, FOV As String) As MainMenuDef
        Return New MainMenuDef With {.MapName = MapName, .TargetX = TargetX, .TargetY = TargetY, .TargetZ = TargetZ, .Azimuth = Azimuth, .Elevation = Elevation, .FOV = FOV}
    End Function

    Private Sub CheckOptions(sender As Object, e As EventArgs) Handles MyBase.Load
        DarkTabControl1.Invalidate()
        IntrosCB.Checked = F3Ini.Ini("Graphics", "enable startup movies") = 1
        MainMenuCB.SelectedIndex = Array.IndexOf(Maps, SysIni.Ini("Mainmenu", "map name"))
        CameraCB.Checked = SysIni.Ini("Camera", "FOV Min") = 0.5D
        AltCamCB.Checked = SysIni.Ini("Camera", "Distance Max") = 70
        Dim Files = SearchForFiles("Override", {"*.map"})
        For Each File In Files
            Dim FI As New FileInfo(File)
            If Not NewGameCB.Items.Contains(FI.Name) Then NewGameCB.Items.Add(FI.Name)
        Next
        NewGameCB.Text = SysIni.Ini("Server", "Start map")
        DarkNumericUpDown1.Value = SysIni.Ini("Server", "Start map entry point")
        'Graphics
        TextureCB.BringToFront()
        SSFCB.BringToFront()
        Try
            dgV2Conf = File.ReadAllLines("dgVoodoo.conf")
        Catch
            File.Exists("dgVoodoo.conf")
            File.WriteAllText("dgVoodoo.conf", My.Resources.dgV2conf)
            dgV2Conf = File.ReadAllLines("dgVoodoo.conf")
        End Try
        ResolutionCB.Items.Clear()
        ResolutionCB.Items.AddRange(GetResAsStrings)
        Dim inires = New Resolution With {
                .Width = F3Ini.Ini("Graphics", "width"),
                .Height = F3Ini.Ini("Graphics", "height"),
                .Hz = F3Ini.Ini("Graphics", "refresh")
                }
        ResolutionCB.SelectedItem = ResToStr(inires)
        SetupSSCB(sender, e)
        FullscreenCB.Checked = F3Ini.Ini("Graphics", "fullscreen") = 1
        If File.Exists("d3d11.dll") Then
            APICB.SelectedIndex = 1
        Else
            APICB.SelectedIndex = 0
        End If
        AACB.SelectedIndex = AAModes.ToList.IndexOf(dgV2Conf.Ini("DirectX", "Antialiasing"))
        TextureCB.SelectedIndex = FModes.ToList.IndexOf(dgV2Conf.Ini("DirectX", "Filtering"))
        SSFCB.SelectedIndex = SSModes.ToList.IndexOf(dgV2Conf.Ini("DirectX", "Resolution"))
        Try : MipmapCB.Checked = Not Boolean.Parse(dgV2Conf.Ini("DirectX", "DisableMipmapping")) : Catch : End Try
        Try : PhongCB.Checked = Boolean.Parse(dgV2Conf.Ini("DirectX", "PhongShadingWhenPossible")) : Catch : End Try
        LoadKeybinds(sender, e)
    End Sub

    Private Sub LoadKeybinds(sender As Object, e As EventArgs) Handles DarkButton2.Click
        DarkLabel1.Hide()
        DarkScrollBar1.BringToFront()
        Dim modifiers As String() = {"Ctrl", "Shift", "Alt"}
        Dim BindString = F3Ini.GetSection("HotKeys")
        Dim Binds As New List(Of Keybind)
        For Each s In BindString
            If s.StartsWith("+") Then
                Dim keys As String() = s.Substring(0, s.IndexOf("=") - 1).Trim.Split("+")
                Dim action As String = s.Substring(s.IndexOf("=") + 1).Trim
                If keys.Length > 2 Then
                    Binds.Add(New Keybind(keys(2), keys(1), action))
                Else
                    Binds.Add(New Keybind(keys(1), "", action))
                End If
            ElseIf s.StartsWith("-") Then
                Dim keys As String() = s.Substring(0, s.IndexOf("=") - 1).Trim.Split("-")
                Dim action As String = s.Substring(s.IndexOf("=") + 1).Trim
                If keys.Length > 2 Then
                    Binds.Add(New Keybind(keys(2), keys(1), action, False))
                Else
                    Binds.Add(New Keybind(keys(1), "", action, False))
                End If
            End If
        Next
        Dim i = 0

        Dim dt As New DataTable

        dt.Columns.Add("Modifier")
        dt.Columns.Add("Key")
        dt.Columns.Add("Action")
        dt.Columns.Add("On Key")
        For Each bind In Binds
            dt.Rows.Add(bind.Modifier, bind.Key, bind.Action, If(bind.OnPress, "Down", "Up"))
            i += 1
        Next
        DataGridView1.GridColor = ThemeProvider.Theme.Colors.GreySelection
        DataGridView1.BackgroundColor = ThemeProvider.Theme.Colors.LightBackground
        DarkScrollBar1.BackColor = ThemeProvider.Theme.Colors.LightBackground
        DataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Dim cellStyle = New DataGridViewCellStyle With {.BackColor = ThemeProvider.Theme.Colors.LightBackground, .ForeColor = ThemeProvider.Theme.Colors.LightText, .SelectionBackColor = ThemeProvider.Theme.Colors.BlueSelection, .SelectionForeColor = ThemeProvider.Theme.Colors.LightText}
        DataGridView1.DefaultCellStyle = cellStyle
        DataGridView1.ColumnHeadersDefaultCellStyle = cellStyle
        DataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.DataSource = dt
        DataGridView1.Columns(0).ReadOnly = True
        DataGridView1.Columns(1).ReadOnly = True
        DataGridView1.Columns(3).ReadOnly = True
    End Sub

    Private Sub ApplyChanges(sender As Object, e As EventArgs) Handles ApplyB.Click
        F3Ini.Ini("Graphics", "enable startup movies", If(IntrosCB.Checked, 1, 0), KeyType.Normal)
        SetMainMenu(MainMenus(MainMenuCB.SelectedIndex))
        SysIni.Ini("Camera", "Distance Max", If(AltCamCB.Checked, 70, 350), KeyType.Normal)
        SysIni.Ini("Camera", "Distance Min", If(AltCamCB.Checked, 70, 350), KeyType.Normal)
        SysIni.Ini("Camera", "FOV Speed", If(CameraCB.Checked, 10, 32.5), KeyType.Normal)
        SysIni.Ini("Camera", "Scroll Speed", If(CameraCB.Checked, 250, 96), KeyType.Normal)
        SysIni.Ini("Camera", "FOV Min", If(CameraCB.Checked, 0.5, If(AltCamCB.Checked, 30, 6)), KeyType.Normal)
        SysIni.Ini("Camera", "FOV Max", If(CameraCB.Checked, 100, If(AltCamCB.Checked, 75, 15)), KeyType.Normal)
        SysIni.Ini("Server", "Start map", NewGameCB.Text, KeyType.Normal)
        SysIni.Ini("Server", "Start map entry point", DarkNumericUpDown1.Value, KeyType.Normal)
        Dim res = StrToRes(ResolutionCB.Text)
        F3Ini.Ini("Graphics", "fullscreen", If(FullscreenCB.Checked, 1, 0), KeyType.Normal)
        F3Ini.Ini("Graphics", "width", res.Width, KeyType.Normal)
        F3Ini.Ini("Graphics", "height", res.Height, KeyType.Normal)
        Select Case APICB.SelectedIndex
            Case 0
                File.Delete("d3d9.dll")
                File.Delete("d3d11.dll")
                File.Delete("dxgi.dll")
                File.Delete("wined3d.dll")
                File.WriteAllBytes("d3d8.dll", My.Resources.D3D8)
            Case 1
                File.Delete("d3d9.dll")
                File.Delete("wined3d.dll")
                File.WriteAllBytes("d3d11.dll", My.Resources.d3d11)
                File.WriteAllBytes("dxgi.dll", My.Resources.dxgi)
        End Select
        dgV2Conf.Ini("DirectX", "Antialiasing", AAModes(AACB.SelectedIndex), KeyType.Normal)
        dgV2Conf.Ini("DirectX", "Filtering", FModes(TextureCB.SelectedIndex), KeyType.Normal)
        dgV2Conf.Ini("DirectX", "Resolution", SSModes(SSFCB.SelectedIndex), KeyType.Normal)
        dgV2Conf.Ini("DirectX", "DisableMipmapping", Not MipmapCB.Checked, KeyType.Normal)
        dgV2Conf.Ini("DirectX", "PhongShadingWhenPossible", PhongCB.Checked, KeyType.Normal)
        dgV2Conf.Ini("GeneralExt", "FPSLimit", res.Hz, KeyType.Normal)
        F3Ini.Ini("Graphics", "refresh", res.Hz, KeyType.Normal)
        Dim NewBinds = (From r As DataGridViewRow In DataGridView1.Rows Select RowToStr(r)).ToList()

        Dim SectionStart = Array.FindIndex(F3Ini, Function(x) x.StartsWith($"[HotKeys]"))
        Dim SectionEnd = Array.FindIndex(F3Ini, SectionStart + 1, Function(x) x.StartsWith("[")) - 1
        If SectionEnd < 0 Then SectionEnd = F3Ini.Length - 1

        For i = SectionStart + 1 To SectionEnd - 1
            F3Ini.RemoveAt(SectionStart + 1)
        Next

        For i = 1 To NewBinds.Count - 1
            F3Ini.InsertAt(SectionStart + i, NewBinds(i - 1))
        Next
        File.WriteAllLines("dgVoodoo.conf", dgV2Conf)
        File.WriteAllLines(SysDir, SysIni)
        File.WriteAllLines(F3Dir, F3Ini)
        Hide()
    End Sub

    Private Sub SetupSSCB(sender As Object, e As EventArgs) Handles ResolutionCB.SelectedIndexChanged
        Dim index = SSFCB.SelectedIndex
        Dim res = StrToRes(ResolutionCB.Text)
        SSFCB.Items.Clear()
        Dim resolutions() As String = {ResToStr(res, False), ResToStr(res, False, 2), ResToStr(res, False, 3), ResToStr(res, False, 4)}
        SSFCB.Items.AddRange(resolutions)
        SSFCB.SelectedIndex = index
    End Sub

    Private Sub NewGameToolTip(sender As Object, e As EventArgs) Handles NewGameCB.SelectedIndexChanged
        ToolTip1.SetToolTip(NewGameCB, NewGameCB.Text)
    End Sub

    Private Shared Function RowToStr(row As DataGridViewRow) As String
        Dim ch As String = If(row.Cells.Item(3).Value = "Up", "-", "+")
        If row.Cells.Item(0).Value = "" Then
            Return $"{ch}{row.Cells.Item(1).Value} = {row.Cells.Item(2).Value}"
        Else
            Return $"{ch}{row.Cells.Item(0).Value}{ch}{row.Cells.Item(1).Value} = {row.Cells.Item(2).Value}"
        End If
    End Function

    Private Function KeybindToStr(kb As Keybind) As String
        Dim ch As String = If(kb.OnPress, "+", "-")
        If kb.Modifier = "" Then
            Return $"{ch}{kb.Key} = {kb.Action}"
        Else
            Return $"{ch}{kb.Modifier}{ch}{kb.Key} = {kb.Action}"
        End If
    End Function

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex < 0 Then Exit Sub
        Select Case e.ColumnIndex
            Case 0, 1
                Row = e.RowIndex
                DarkLabel1.Show()
                WantKey = True
            Case 2
                DarkLabel1.Hide()
                WantKey = False
            Case 3
                If DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value Is DBNull.Value Then DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = "Up"
                DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = If(DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = "Up", "Down", "Up")
                DarkLabel1.Hide()
                WantKey = False
        End Select
    End Sub

    Private Sub DataGridView1_ScrollChanged(sender As Object, e As EventArgs) Handles DataGridView1.Scroll
        DarkScrollBar1.ScrollTo((DataGridView1.FirstDisplayedScrollingRowIndex / (DataGridView1.Rows.Count - DataGridView1.DisplayedRowCount(False))) * DarkScrollBar1.Maximum)
    End Sub

    Private Sub DarkScrollBar1_Click(sender As Object, e As EventArgs) Handles DarkScrollBar1.MouseDown
        Timer1.Start()
    End Sub

    Private Sub DarkScrollBar1_MouseUp(sender As Object, e As EventArgs) Handles DarkScrollBar1.MouseUp
        Timer1.Stop()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        DataGridView1.FirstDisplayedScrollingRowIndex = (DarkScrollBar1.Value / (DarkScrollBar1.Maximum)) * (DataGridView1.Rows.Count - DataGridView1.DisplayedRowCount(False))
    End Sub

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        Select Case e.KeyCode
            Case Keys.ControlKey, Keys.Menu, Keys.ShiftKey
            Case Else
                If WantKey Then
                    Modifier = e.Modifiers.ToString
                    Key = e.KeyCode.ToString
                    DataGridView1.Rows(Row).Cells(0).Value = ProperName(Modifier)
                    DataGridView1.Rows(Row).Cells(1).Value = ProperName(Key)
                End If
        End Select
    End Sub

    Private Shared Function ProperName(key As String) As String
        Select Case key
            Case "D1" : Return "1"
            Case "D2" : Return "2"
            Case "D3" : Return "3"
            Case "D4" : Return "4"
            Case "D5" : Return "5"
            Case "D6" : Return "6"
            Case "D7" : Return "7"
            Case "D8" : Return "8"
            Case "D9" : Return "9"
            Case "D0" : Return "0"
            Case "Control" : Return "Ctrl"
            Case "None" : Return ""
            Case "Oemtilde" : Return "`"
            Case "Oemminus" : Return "-"
            Case "Oemplus" : Return "+"
            Case "OemOpenBrackets" : Return "["
            Case "Oem6" : Return "]"
            Case "Oem5" : Return "\"
            Case "Oem1" : Return ";"
            Case "Oem7" : Return """"
            Case "Oemcomma" : Return ","
            Case "Oemperiod" : Return "."
            Case "OemQuestion" : Return "/"
            Case Else : Return key
        End Select
    End Function

End Class

Public Class MainMenuDef
    Public Property MapName As String
    Public Property TargetX As String
    Public Property TargetY As String
    Public Property TargetZ As String
    Public Property Azimuth As String
    Public Property Elevation As String
    Public Property FOV As String
End Class

Public Class Keybind
    Public Key As String
    Public Modifier As String
    Public Action As String
    Public OnPress As Boolean

    Public Sub New(Key As String, Modifier As String, Action As String, Optional OnPress As Boolean = True)
        Me.Key = Key
        Me.Modifier = Modifier
        Me.Action = Action
        Me.OnPress = OnPress
    End Sub

End Class