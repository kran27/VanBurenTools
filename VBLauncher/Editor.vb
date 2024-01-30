Imports System.Data
Imports System.IO
Imports AltUI.Config
Imports AltUI.Controls
Imports AltUI.Forms

Public Class Editor

#Region "Variables"

    Private stf As List(Of String)
    Private f As String
    Private ext As String
    Private cf 'As ITM ' Current file, ambiguous type (I set the type while coding to avoid errors)
    Private ReadOnly DontWipe As String() = New String() {"Triggertcb", "GCREspcb", "GCREskcb", "GCREtrcb", "GCREtscb", "GCREsoccb"}
    Private HCtrl As Boolean
    Private HN As Boolean : Private H1 As Boolean : Private H2 As Boolean : Private H3 As Boolean : Private H4 As Boolean : Private H5 As Boolean : Private H6 As Boolean : Private H7 As Boolean : Private H8 As Boolean : Private H9 As Boolean : Private H0 As Boolean

#End Region

    Private Sub InitialSetup() Handles MyBase.Load
        Size = New Size(640, 480)
        EnableSTFEdit.Checked = My.Settings.STFEditEnabled
        Mapgb.Hide()
        CRTgb.Hide()
        ITMgb.Hide()
        EPTHnud.Enabled = False
        Triggernud.Enabled = False
        EPTHnud.Minimum = 1
        Triggernud.Minimum = 1
        EMAPslb.CustomColour = True
        Dim CellStyle = New DataGridViewCellStyle With {.BackColor = ThemeProvider.BackgroundColour, .ForeColor = ThemeProvider.Theme.Colors.LightText, .SelectionBackColor = ThemeProvider.Theme.Colors.BlueSelection, .SelectionForeColor = ThemeProvider.Theme.Colors.LightText}
        EME2dgv.GridColor = ThemeProvider.Theme.Colors.GreySelection
        EME2dgv.BackgroundColor = ThemeProvider.BackgroundColour
        EME2dgv.DefaultCellStyle = CellStyle
        EEN2dgv.GridColor = ThemeProvider.Theme.Colors.GreySelection
        EEN2dgv.BackgroundColor = ThemeProvider.BackgroundColour
        EEN2dgv.DefaultCellStyle = CellStyle
        GCREdgv.GridColor = ThemeProvider.Theme.Colors.GreySelection
        GCREdgv.BackgroundColor = ThemeProvider.BackgroundColour
        GCREdgv.DefaultCellStyle = CellStyle
    End Sub

#Region "New Files"

    Private Sub NewAmo() Handles AmoToolStripMenuItem.Click
    End Sub

    Private Sub NewArm() Handles ArmToolStripMenuItem.Click
    End Sub

    Private Sub NewCon() Handles ConToolStripMenuItem.Click
    End Sub

    Private Sub NewCrt() Handles CrtToolStripMenuItem.Click
        If CheckAndLoadStf() Then
            ext = ".crt"
            cf = New CRT()
            CRTSetupUI(cf)
        Else
            DarkMessageBox.ShowError($".STF Not selected, file creation aborted", ".STF Not Selected")
        End If
    End Sub

    Private Sub NewDor() Handles DorToolStripMenuItem.Click
    End Sub

    Private Sub NewInt() Handles IntToolStripMenuItem.Click
    End Sub

    Private Sub NewItm() Handles ItmToolStripMenuItem.Click
        If CheckAndLoadStf() Then
            ext = ".itm"
            cf = New ITM()
            ITMSetupUI(cf)
        Else
            DarkMessageBox.ShowError($".STF Not selected, file creation aborted", ".STF Not Selected")
        End If
    End Sub

    Private Sub NewMap() Handles MapToolStripMenuItem.Click
        ext = ".map"
        cf = New Map()
        MapSetupUI(cf)
    End Sub

    Private Sub NewUse() Handles UseToolStripMenuItem.Click
    End Sub

    Private Sub NewWea() Handles WeaToolStripMenuItem.Click
    End Sub

#End Region

    Private Sub OpenFile() Handles OpenToolStripMenuItem.Click
        ' Loads all regions of a given file into their respective classes, and from there into the UI
        Dim ofd As New OpenFileDialog With {.Filter = "Van Buren Data File|*.amo;*.arm;*.con;*.crt;*.dor;*.int;*.itm;*.map;*.use;*.wea", .Multiselect = False, .ValidateNames = True}
        If ofd.ShowDialog = DialogResult.OK Then
            f = ofd.FileName
            ext = f.Substring(f.LastIndexOf("."), 4).ToLower()
            Dim fb = File.ReadAllBytes(f)
            LoadFile(fb, ext)
        End If
    End Sub

    Private Sub LoadFile(fb As Byte(), ext As String)
        If CheckAndLoadStf() Then
            Select Case ext
                Case ".amo"
                    MsgBox("Not yet implemented")
                Case ".arm"
                    MsgBox("Not yet implemented")
                Case ".con"
                    MsgBox("Not yet implemented")
                Case ".crt"
                    cf = fb.ReadCRT()
                    CRTgb.Text = $".CRT Editor ({f.Substring(f.LastIndexOf("\") + 1)})"
                    CRTSetupUI(cf)
                Case ".dor"
                    MsgBox("Not yet implemented")
                Case ".int"
                    MsgBox("Not yet implemented")
                Case ".itm"
                    cf = fb.ReadITM()
                    ITMgb.Text = $".ITM Editor ({f.Substring(f.LastIndexOf("\") + 1)})"
                    ITMSetupUI(cf)
                Case ".map"
                    cf = fb.ReadMap()
                    Mapgb.Text = $".MAP Editor ({f.Substring(f.LastIndexOf("\") + 1)})"
                    MapSetupUI(cf)
                Case ".use"
                    MsgBox("Not yet implemented")
                Case ".wea"
                    MsgBox("Not yet implemented")
            End Select
        Else
            DarkMessageBox.ShowError($".STF Not selected, loading of {f} aborted", ".STF Not Selected")
        End If
    End Sub

    Private Sub OpenFromgrpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenFromgrpToolStripMenuItem.Click
        Using grpb As New GrpBrowser(New String() {"amo", "arm", "con", "crt", "dor", "int", "itm", "map", "use", "wea"})
            grpb.ShowDialog()
            f = grpb.FileName
            LoadFile(grpb.FileBytes, "." & grpb.Extension)
        End Using
    End Sub

    Private Sub SaveFile() Handles SaveToolStripMenuItem.Click
        Dim sfd As New SaveFileDialog With {.Filter = $"Van Buren Data File|*{ext}", .ValidateNames = True, .DefaultExt = ext}
        If sfd.ShowDialog = DialogResult.OK Then
            If My.Settings.STFEditEnabled AndAlso stf IsNot Nothing Then File.WriteAllBytes(My.Settings.STFDir, TXTToSTF(stf.ToArray()))
            File.WriteAllBytes(sfd.FileName, cf.ToByte.ToArray())
        End If
    End Sub

    Private Sub ResetUI()
        Dim en As Boolean
        For Each c As DarkGroupBox In From con In Controls Where TypeOf con Is DarkGroupBox
            For Each c2 As DarkGroupBox In From con In c.Controls Where TypeOf con Is DarkGroupBox
                For Each c3 In c2.Controls
                    en = c3.Enabled
                    c3.Enabled = False
                    If TypeOf c3 Is DarkTextBox Then
                        Try : c3.Text = "" : Catch : MsgBox(c3.Name) : End Try
                    ElseIf TypeOf c3 Is DarkNumericUpDown Then
                        Try : c3.Value = c3.Minimum : Catch : End Try
                    ElseIf TypeOf c3 Is DarkComboBox AndAlso Not DontWipe.Contains(c3.Name) Then
                        c3.Items.Clear
                        c3.Refresh() ' Refresh control so the text region is not transparent
                    ElseIf TypeOf c3 Is DataGridView Then
                        c3.DataSource = Nothing
                    End If
                    c3.Enabled = en
                Next
            Next
        Next
    End Sub

#Region "File-Specific UI Setup"

    Private Sub MapSetupUI(ByRef cf As Map) ' Only way I found to bypass the issue of functions like .Any() not being found
        Mapgb.Location = New Point(12, 27)
        Size = New Size(Mapgb.Width + Mapgb.Padding.Left * 13, Mapgb.Height + DarkMainMenuStrip.Height + Mapgb.Padding.Top * 17)
        CRTgb.Hide()
        Mapgb.Show()
        ITMgb.Hide()
        ResetUI()
        EMAPToUI()
        For Each EME2 In cf.EME2
            EME2cb.Items.Add(EME2.name)
        Next
        If cf.EME2.Any() Then
            EME2cb.SelectedIndex = 0
            For Each c As Control In EME2gb.Controls
                c.Enabled = True
            Next
            EME2ToUI()
        Else
            For Each c As Control In EME2gb.Controls
                c.Enabled = False
            Next
            EME2cb.Enabled = True : EME2p.Enabled = True
        End If
        If cf.ECAM IsNot Nothing Then ECAMToUI()
        If cf.EMEP.Any() Then
            For i = 1 To cf.EMEP.Count
                EMEPcb.Items.Add(i)
            Next
            EMEPcb.SelectedIndex = 0
            For Each c As Control In EMEPgb.Controls
                c.Enabled = True
            Next
            EMEPToUI()
        Else
            For Each c As Control In EMEPgb.Controls
                c.Enabled = False
            Next
            EMEPcb.Enabled = True : EMEPp.Enabled = True
        End If
        If cf.Triggers.Any() Then
            Dim a = ""
            For i = 1 To cf.Triggers.Count
                If cf.Triggers(i - 1).ExTR.s = "S" Or cf.Triggers(i - 1).ExTR.s = "T" Then a = $" ({cf.Triggers(i - 1).ExTR.s})"
                Triggercb.Items.Add($"{i}{a}")
                a = ""
            Next
            Triggercb.SelectedIndex = 0
            For Each c As Control In Triggergb.Controls
                c.Enabled = True
            Next
            TriggerToUI()
        Else
            For Each c As Control In Triggergb.Controls
                c.Enabled = False
            Next
            Triggercb.Enabled = True : Triggerp.Enabled = True
        End If
        If cf.EPTH.Any() Then
            For i = 1 To cf.EPTH.Count
                EPTHcb.Items.Add($"{i} ({cf.EPTH(i - 1).name})")
            Next
            EPTHcb.SelectedIndex = 0
            For Each c As Control In EPTHGB.Controls
                c.Enabled = True
            Next
            EPTHToUI()
        Else
            For Each c As Control In EPTHGB.Controls
                c.Enabled = False
            Next
            EPTHcb.Enabled = True : EPTHp.Enabled = True
        End If
        If cf.EMSD.Any() Then
            For i = 1 To cf.EMSD.Count
                EMSDcb.Items.Add($"{i} ({cf.EMSD(i - 1).s2.Replace(".psf", "")})")
            Next
            EMSDcb.SelectedIndex = 0
            For Each c As Control In EMSDgb.Controls
                c.Enabled = True
            Next
            EMSDToUI()
        Else
            For Each c As Control In EMSDgb.Controls
                c.Enabled = False
            Next
            EMSDcb.Enabled = True : EMSDp.Enabled = True
        End If
        If cf.EMEF.Any() Then
            For i = 1 To cf.EMEF.Count
                EMEFcb.Items.Add($"{i} ({cf.EMEF(i - 1).s2.Replace(".veg", "")})")
            Next
            EMEFcb.SelectedIndex = 0
            For Each c As Control In EMEFgb.Controls
                c.Enabled = True
            Next
            EMEFToUI()
        Else
            For Each c As Control In EMEFgb.Controls
                c.Enabled = False
            Next
            EMEFcb.Enabled = True : EMEFp.Enabled = True
        End If
        If (cf._2MWT Is Nothing) OrElse Not cf._2MWT.chunks.Any() Then
            For Each c As Control In _2MWTgb.Controls
                c.Enabled = False
            Next
            _2MWTcb.Enabled = True
            _2MWTp.Enabled = True
        Else
            _2MWTToUI()
        End If
    End Sub

    Private Sub CRTSetupUI(ByRef cf As CRT) ' Only way I found to bypass the issue of functions like .Any() not being found
        CRTgb.Controls.Add(EEN2gb)
        CRTgb.Controls.Add(GENTgb)
        GENTgb.Location = New Point(618, 22)
        CRTgb.Location = New Point(12, 27)
        Size = New Size(CRTgb.Width + CRTgb.Padding.Left * 13, CRTgb.Height + DarkMainMenuStrip.Height + CRTgb.Padding.Top * 17)
        CRTgb.Show()
        Mapgb.Hide()
        ITMgb.Hide()
        ResetUI()

        EEN2ToUI()
        GENTToUI()
        GCHRn.Text = cf.GCHR.name

        If cf.GCRE.GWAM.Any() Then
            For i = 1 To cf.GCRE.GWAM.Count
                Dim ind = cf.GCRE.GWAM(i - 1).NameSR - 1
                GWAMcb.Items.Add($"{i} ({If(ind = -1, "null", stf(ind))})")
            Next
            GWAMcb.SelectedIndex = 0
            For Each c As Control In GWAMgb.Controls
                c.Enabled = True
            Next
            GWAMToUI()
        Else
            For Each c As Control In GWAMgb.Controls
                c.Enabled = False
            Next
            GWAMgb.Enabled = True : GWAMp.Enabled = True
        End If

        GCREToUI()
    End Sub

    Private Sub ITMSetupUI(ByRef cf As ITM) ' Only way I found to bypass the issue of functions like .Any() not being found
        ITMgb.Controls.Add(EEN2gb)
        ITMgb.Controls.Add(GENTgb)
        GENTgb.Location = New Point(618, 66)
        ITMgb.Location = New Point(12, 27)
        Size = New Size(ITMgb.Width + ITMgb.Padding.Left * 13, ITMgb.Height + DarkMainMenuStrip.Height + ITMgb.Padding.Top * 17)
        ITMgb.Show()
        CRTgb.Hide()
        Mapgb.Hide()
        ResetUI()

        EEN2ToUI()
        GENTToUI()
        GITMToUI()
    End Sub

#End Region

#Region "Load classes into UI"

    Private Sub _2MWTToUI()
        _2MWTToUI(cf)
    End Sub

    Private Sub _2MWTToUI(ByRef cf As Map)
        If cf._2MWT.chunks.Count = 0 Then
            For Each c As Control In _2MWTgb.Controls
                c.Enabled = False
            Next
            _2MWTcb.Enabled = True
            _2MWTp.Enabled = True
        Else
            _2MWTmpf.Text = cf._2MWT.mpf
            _2MWTfr.Checked = cf._2MWT.frozen
            _2MWTdw.Checked = cf._2MWT.dark
            For i = 1 To cf._2MWT.chunks.Count
                _2MWTcb.Items.Add(i)
            Next
            _2MWTcb.SelectedIndex = 0
            _2MWTx.Text = cf._2MWT.chunks(0).loc.x
            _2MWTy.Text = cf._2MWT.chunks(0).loc.y
            _2MWTz.Text = cf._2MWT.chunks(0).loc.z
            _2MWTlmx.Text = cf._2MWT.chunks(0).texloc.x
            _2MWTlmy.Text = cf._2MWT.chunks(0).texloc.y
            Try : cf._2MWT.chunks(0).tex.Substring(0, cf._2MWT.chunks(0).tex.LastIndexOf(".")) : Catch : _2MWTtex.Text = "" : End Try
        End If
    End Sub

    Private Sub EMAPToUI()
        EMAPslb.BorderColour = cf.EMAP.col
        EMAPs1.Text = cf.EMAP.s1.Replace(".8", "")
        EMAPs2.Text = cf.EMAP.s2.Replace(".rle", "")
        EMAPs3.Text = cf.EMAP.s3.Replace(".dds", "")
        EMAPilcb.Checked = cf.EMAP.il
    End Sub

    Private Sub EME2ToUI() Handles EME2cb.SelectedIndexChanged
        EME2n.Text = cf.EME2(EME2cb.SelectedIndex).name
        EME2s1.Text = cf.EME2(EME2cb.SelectedIndex).EEOV.s1
        EME2s2.Text = cf.EME2(EME2cb.SelectedIndex).EEOV.s2
        EME2s3.Text = cf.EME2(EME2cb.SelectedIndex).EEOV.s3.Replace(".amx", "")
        EME2s4.Text = cf.EME2(EME2cb.SelectedIndex).EEOV.s4.Replace(".dds", "")
        EME2s5.Text = cf.EME2(EME2cb.SelectedIndex).EEOV.s5.Replace(".veg", "")
        EME2x.Text = cf.EME2(EME2cb.SelectedIndex).l.x
        EME2y.Text = cf.EME2(EME2cb.SelectedIndex).l.y
        EME2z.Text = cf.EME2(EME2cb.SelectedIndex).l.z
        EME2r.Text = cf.EME2(EME2cb.SelectedIndex).l.r
        Dim dt As New DataTable
        dt.Columns.Add("item name", GetType(String))
        For Each item In cf.EME2(EME2cb.SelectedIndex).EEOV.inv
            dt.Rows.Add(item)
        Next
        EME2dgv.DataSource = dt
        For Each col As DataGridViewColumn In EME2dgv.Columns
            col.MinimumWidth = EME2dgv.Width - 18
            col.Width = EME2dgv.Width - 18
        Next
    End Sub

    Private Sub EMEPToUI() Handles EMEPcb.SelectedIndexChanged
        EMEPnud.Value = cf.EMEP(EMEPcb.SelectedIndex).index
        EMEPx.Text = cf.EMEP(EMEPcb.SelectedIndex).p.x
        EMEPy.Text = cf.EMEP(EMEPcb.SelectedIndex).p.y
        EMEPz.Text = cf.EMEP(EMEPcb.SelectedIndex).p.z
        EMEPr.Text = cf.EMEP(EMEPcb.SelectedIndex).r
    End Sub

    Private Sub ECAMToUI()
        ECAMx.Text = cf.ECAM.p.x
        ECAMy.Text = cf.ECAM.p.y
        ECAMz.Text = cf.ECAM.p.z
        ECAMr.Text = cf.ECAM.p.r
    End Sub

    Private Sub TriggerToUI() Handles Triggercb.SelectedIndexChanged
        Triggernud.Value = 1
        Triggertcb.SelectedItem = cf.Triggers(Triggercb.SelectedIndex).ExTR.type
        Triggern.Enabled = True
        Triggern.Text = cf.Triggers(Triggercb.SelectedIndex).ExTR.s
        Triggernud_ValueChanged()
    End Sub

    Private Sub Triggernud_ValueChanged() Handles Triggernud.ValueChanged
        If Triggernud.Enabled Then
            If Triggernud.Value > cf.Triggers(Triggercb.SelectedIndex).EMTR.r.Count Then
                cf.Triggers(Triggercb.SelectedIndex).EMTR.r.Add(New Point3(0, 0, 0))
            End If
            Triggerx.Text = cf.Triggers(Triggercb.SelectedIndex).EMTR.r(Triggernud.Value - 1).x
            Triggery.Text = cf.Triggers(Triggercb.SelectedIndex).EMTR.r(Triggernud.Value - 1).y
            Triggerz.Text = cf.Triggers(Triggercb.SelectedIndex).EMTR.r(Triggernud.Value - 1).z
        End If
    End Sub

    Private Sub EPTHToUI() Handles EPTHcb.SelectedIndexChanged
        EPTHnud.Value = 1
        EPTHn.Text = cf.EPTH(EPTHcb.SelectedIndex).name
        EPTHnud_ValueChanged()
    End Sub

    Private Sub EPTHnud_ValueChanged() Handles EPTHnud.ValueChanged
        If EPTHnud.Enabled Then
            If EPTHnud.Value > cf.EPTH(EPTHcb.SelectedIndex).p.Count Then
                cf.EPTH(EPTHcb.SelectedIndex).p.Add(New Point4(0, 0, 0, 0))
            End If
            EPTHx.Text = cf.EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1).x
            EPTHy.Text = cf.EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1).y
            EPTHz.Text = cf.EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1).z
            EPTHr.Text = cf.EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1).r
        End If
    End Sub

    Private Sub Triggerpm_Click() Handles Triggerpm.Click
        Dim i = Triggernud.Value - 1
        If cf.Triggers(Triggercb.SelectedIndex).EMTR.r.Count <> 1 Then
            cf.Triggers(Triggercb.SelectedIndex).EMTR.r.RemoveAt(i)
            Triggernud.Value = i
        End If
    End Sub

    Private Sub EPTHpm_Click() Handles EPTHpm.Click
        Dim i = EPTHnud.Value - 1
        If cf.EPTH(EPTHcb.SelectedIndex).p.Count <> 1 Then
            cf.EPTH(EPTHcb.SelectedIndex).p.RemoveAt(i)
            EPTHnud.Value = i
        End If
    End Sub

    Private Sub EMSDToUI() Handles EMSDcb.SelectedIndexChanged
        EMSDs1.Text = cf.EMSD(EMSDcb.SelectedIndex).s1
        EMSDs2.Text = cf.EMSD(EMSDcb.SelectedIndex).s2.Replace(".psf", "")
        EMSDx.Text = cf.EMSD(EMSDcb.SelectedIndex).l.x
        EMSDy.Text = cf.EMSD(EMSDcb.SelectedIndex).l.y
        EMSDz.Text = cf.EMSD(EMSDcb.SelectedIndex).l.z
    End Sub

    Private Sub EMEFToUI() Handles EMEFcb.SelectedIndexChanged
        EMEFs1.Text = cf.EMEF(EMEFcb.SelectedIndex).s1
        EMEFs2.Text = cf.EMEF(EMEFcb.SelectedIndex).s2.Replace(".veg", "")
        EMEFx.Text = cf.EMEF(EMEFcb.SelectedIndex).l.x
        EMEFy.Text = cf.EMEF(EMEFcb.SelectedIndex).l.y
        EMEFz.Text = cf.EMEF(EMEFcb.SelectedIndex).l.z
        EMEFr.Text = cf.EMEF(EMEFcb.SelectedIndex).l.r
    End Sub

    Private Sub EEN2ToUI()
        EEN2skl.Text = cf.EEN2.skl
        EEN2invt.Text = cf.EEN2.invtex.Replace(".dds", "")
        EEN2actt.Text = cf.EEN2.acttex.Replace(".dds", "")
        EEN2sel.Checked = cf.EEN2.sel
        EEN2s1.Text = cf.EEN2.EEOV.s1
        EEN2s2.Text = cf.EEN2.EEOV.s2
        EEN2s3.Text = cf.EEN2.EEOV.s3.Replace(".amx", "")
        EEN2s4.Text = cf.EEN2.EEOV.s4.Replace(".dds", "")
        EEN2s5.Text = cf.EEN2.EEOV.s5.Replace(".veg", "")
        Dim dt As New DataTable
        dt.Columns.Add("item name", GetType(String))
        For Each item In cf.EEN2.EEOV.inv
            dt.Rows.Add(item)
        Next
        EEN2dgv.DataSource = dt
        For Each col As DataGridViewColumn In EEN2dgv.Columns
            col.MinimumWidth = EEN2dgv.Width - 18
            col.Width = EEN2dgv.Width - 18
        Next
    End Sub

    Private Sub GENTToUI()
        GENThSR.Value = cf.GENT.HoverSR
        Try : GENTh.Text = stf(cf.GENT.HoverSR - 1) : Catch : End Try
        GENTh.Enabled = My.Settings.STFEditEnabled AndAlso cf.GENT.HoverSR <> 0
        GENTlSR.Value = cf.GENT.LookSR
        Try : GENTl.Text = stf(cf.GENT.LookSR - 1) : Catch : End Try
        GENTl.Enabled = My.Settings.STFEditEnabled AndAlso cf.GENT.LookSR <> 0
        GENTnSR.Value = cf.GENT.NameSR
        Try : GENTn.Text = stf(cf.GENT.NameSR - 1) : Catch : End Try
        GENTn.Enabled = My.Settings.STFEditEnabled AndAlso cf.GENT.NameSR <> 0
        GENTuSR.Value = cf.GENT.UnkwnSR
        Try : GENTu.Text = stf(cf.GENT.UnkwnSR - 1) : Catch : End Try
        GENTu.Enabled = My.Settings.STFEditEnabled AndAlso cf.GENT.UnkwnSR <> 0
        GENTmhp.Value = cf.GENT.MaxHealth
        GENTihp.Value = cf.GENT.StartHealth
    End Sub

    Private Sub GWAMToUI()
        GWAMani.Value = cf.GCRE.GWAM(GWAMcb.SelectedIndex).Anim
        GWAMdt.SelectedIndex = cf.GCRE.GWAM(GWAMcb.SelectedIndex).DmgType
        GWAMsf.Value = cf.GCRE.GWAM(GWAMcb.SelectedIndex).ShotsFired
        GWAMr.Value = cf.GCRE.GWAM(GWAMcb.SelectedIndex).Range
        GWAMmin.Value = cf.GCRE.GWAM(GWAMcb.SelectedIndex).MinDmg
        GWAMmax.Value = cf.GCRE.GWAM(GWAMcb.SelectedIndex).MaxDmg
        GWAMap.Value = cf.GCRE.GWAM(GWAMcb.SelectedIndex).AP
        GWAManSR.Value = cf.GCRE.GWAM(GWAMcb.SelectedIndex).NameSR
        Try : GWAMan.Text = stf(cf.GCRE.GWAM(GWAMcb.SelectedIndex).NameSR - 1) : Catch : End Try
        GWAMan.Enabled = My.Settings.STFEditEnabled AndAlso cf.GCRE.GWAM(GWAMcb.SelectedIndex).NameSR <> 0
        GWAMef.Text = cf.GCRE.GWAM(GWAMcb.SelectedIndex).VegName.Replace(".veg", "")
    End Sub

    Private Sub GCREToUI()
        GCREage.Value = cf.GCRE.Age
        GCREspcb.SelectedIndex = 0
        GCREspv.Value = cf.GCRE.Special(0)
        If cf.GCRE.Skills.Count > 0 Then
            GCREskcb.SelectedIndex = cf.GCRE.Skills(0).Index
            GCREskv.Value = cf.GCRE.Skills(0).Value
        Else
            GCREskcb.SelectedIndex = 0
        End If
        GCREtrcb.SelectedIndex = 0
        GCREtrv.Checked = cf.GCRE.Traits.Contains(0)
        GCREtscb.SelectedIndex = 0
        GCREtsv.Checked = cf.GCRE.TagSkills.Contains(0)
        GCREp.Text = cf.GCRE.PortStr.Replace(".dds", "")
        GCREsoccb.SelectedIndex = 0
        GCREsocm.Text = cf.GCRE.Hea.Model
        GCREsoct.Text = cf.GCRE.Hea.Tex.Replace(".dds", "")
        Dim dt = New DataTable
        dt.Columns.Add("item name", GetType(String))
        For Each item In cf.GCRE.Inventory
            dt.Rows.Add(item)
        Next
        GCREdgv.DataSource = dt
        For Each col As DataGridViewColumn In GCREdgv.Columns
            col.MinimumWidth = GCREdgv.Width - 18
            col.Width = GCREdgv.Width - 18
        Next
    End Sub

    Private Sub GITMToUI()
        GITMtype.SelectedIndex = cf.GITM.type
        GITMeq.Checked = cf.GITM.equip
        GITMslot.SelectedIndex = cf.GITM.eqslot
        GITMrl.Text = cf.GITM.reload
        ' GITMml.Checked = cf.GITM.reload =
        GITMhhai.Checked = cf.GITM.hHai
        GITMhbea.Checked = cf.GITM.hBea
        GITMhmus.Checked = cf.GITM.hMus
        GITMheye.Checked = cf.GITM.hEye
        GITMhpon.Checked = cf.GITM.hPon
        GITMhvan.Checked = cf.GITM.hVan
        GITMsoct.Text = cf.GITM.Hea.Tex
        GITMsocm.Text = cf.GITM.Hea.Model
    End Sub

#End Region

#Region "Load UI into classes"

    Private Sub PickLightingColour(sender As Object, e As EventArgs) Handles EMAPslb.Click
        Dim cd As New ColorDialog With {.Color = cf.EMAP.col, .FullOpen = True}
        If cd.ShowDialog() = DialogResult.OK Then
            EMAPslb.BorderColour = cd.Color
            cf.EMAP.col = cd.Color
        End If
    End Sub

    Private Sub EMAPilcb_CheckedChanged(sender As Object, e As EventArgs) Handles EMAPilcb.CheckedChanged
        If sender.Enabled Then cf.EMAP.il = EMAPilcb.Checked
    End Sub

    Private Sub EMAPs1_TextChanged(sender As Object, e As EventArgs) Handles EMAPs1.TextChanged
        If sender.Enabled Then cf.EMAP.s1 = EMAPs1.Text & If(String.IsNullOrWhiteSpace(EMAPs1.Text), "", ".8")
    End Sub

    Private Sub EMAPs2_TextChanged(sender As Object, e As EventArgs) Handles EMAPs2.TextChanged
        If sender.Enabled Then cf.EMAP.s2 = EMAPs2.Text & If(String.IsNullOrWhiteSpace(EMAPs2.Text), "", ".rle")
    End Sub

    Private Sub EMAPs3_TextChanged(sender As Object, e As EventArgs) Handles EMAPs3.TextChanged
        If sender.Enabled Then cf.EMAP.s3 = EMAPs3.Text & If(String.IsNullOrWhiteSpace(EMAPs3.Text), "", ".dds")
    End Sub

    Private Sub ECAMx_TextChanged(sender As Object, e As EventArgs) Handles ECAMx.TextChanged
        If sender.Enabled Then
            If cf.ECAM Is Nothing Then cf.ECAM = New ECAMc()
            cf.ECAM.p.x = If(ECAMx.Text = "", 0, ECAMx.Text)
        End If
    End Sub

    Private Sub ECAMy_TextChanged(sender As Object, e As EventArgs) Handles ECAMy.TextChanged
        If sender.Enabled Then
            If cf.ECAM Is Nothing Then cf.ECAM = New ECAMc()
            cf.ECAM.p.y = If(ECAMy.Text = "", 0, ECAMy.Text)
        End If
    End Sub

    Private Sub ECAMz_TextChanged(sender As Object, e As EventArgs) Handles ECAMz.TextChanged
        If sender.Enabled Then
            If cf.ECAM Is Nothing Then cf.ECAM = New ECAMc()
            cf.ECAM.p.z = If(ECAMz.Text = "", 0, ECAMz.Text)
        End If
    End Sub

    Private Sub ECAMr_TextChanged(sender As Object, e As EventArgs) Handles ECAMr.TextChanged
        If sender.Enabled Then
            If cf.ECAM Is Nothing Then cf.ECAM = New ECAMc()
            cf.ECAM.p.r = If(ECAMr.Text = "", 0, ECAMr.Text)
        End If
    End Sub

    Private Sub EMEFs1_TextChanged(sender As Object, e As EventArgs) Handles EMEFs1.TextChanged
        If sender.Enabled Then cf.EMEF(EMEFcb.SelectedIndex).s1 = EMEFs1.Text
    End Sub

    Private Sub EMEFs2_TextChanged(sender As Object, e As EventArgs) Handles EMEFs2.TextChanged
        If sender.Enabled Then cf.EMEF(EMEFcb.SelectedIndex).s2 = EMEFs2.Text & If(String.IsNullOrWhiteSpace(EMEFs2.Text), "", ".veg")
    End Sub

    Private Sub EMEFx_TextChanged(sender As Object, e As EventArgs) Handles EMEFx.TextChanged
        If sender.Enabled Then cf.EMEF(EMEFcb.SelectedIndex).l.x = If(EMEFx.Text = "", 0, EMEFx.Text)
    End Sub

    Private Sub EMEFy_TextChanged(sender As Object, e As EventArgs) Handles EMEFy.TextChanged
        If sender.Enabled Then cf.EMEF(EMEFcb.SelectedIndex).l.y = If(EMEFy.Text = "", 0, EMEFy.Text)
    End Sub

    Private Sub EMEFz_TextChanged(sender As Object, e As EventArgs) Handles EMEFz.TextChanged
        If sender.Enabled Then cf.EMEF(EMEFcb.SelectedIndex).l.z = If(EMEFz.Text = "", 0, EMEFz.Text)
    End Sub

    Private Sub EMEFr_TextChanged(sender As Object, e As EventArgs) Handles EMEFr.TextChanged
        If sender.Enabled Then cf.EMEF(EMEFcb.SelectedIndex).l.r = If(EMEFr.Text = "", 0, EMEFr.Text)
    End Sub

    Private Sub EMEPnud_ValueChanged(sender As Object, e As EventArgs) Handles EMEPnud.ValueChanged
        If sender.Enabled Then cf.EMEP(EMEPcb.SelectedIndex).index = EMEPnud.Value
    End Sub

    Private Sub EMEPx_TextChanged(sender As Object, e As EventArgs) Handles EMEPx.TextChanged
        If sender.Enabled Then cf.EMEP(EMEPcb.SelectedIndex).p.x = If(EMEPx.Text = "", 0, EMEPx.Text)
    End Sub

    Private Sub EMEPy_TextChanged(sender As Object, e As EventArgs) Handles EMEPy.TextChanged
        If sender.Enabled Then cf.EMEP(EMEPcb.SelectedIndex).p.y = If(EMEPy.Text = "", 0, EMEPy.Text)
    End Sub

    Private Sub EMEPz_TextChanged(sender As Object, e As EventArgs) Handles EMEPz.TextChanged
        If sender.Enabled Then cf.EMEP(EMEPcb.SelectedIndex).p.z = If(EMEPz.Text = "", 0, EMEPz.Text)
    End Sub

    Private Sub EMEPr_TextChanged(sender As Object, e As EventArgs) Handles EMEPr.TextChanged
        If sender.Enabled Then cf.EMEP(EMEPcb.SelectedIndex).r = If(EMEPr.Text = "", 0, EMEPr.Text)
    End Sub

    Private Sub EME2dgv_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles EME2dgv.CellEndEdit
        If sender.Enabled Then cf.EME2(EME2cb.SelectedIndex).EEOV.inv = EME2dgv.GetStringArray()
    End Sub

    Private Sub EME2n_TextChanged(sender As Object, e As EventArgs) Handles EME2n.TextChanged
        If sender.Enabled Then cf.EME2(EME2cb.SelectedIndex).name = EME2n.Text
    End Sub

    Private Sub EME2s1_TextChanged(sender As Object, e As EventArgs) Handles EME2s1.TextChanged
        If sender.Enabled Then cf.EME2(EME2cb.SelectedIndex).EEOV.s1 = EME2s1.Text
    End Sub

    Private Sub EME2s2_TextChanged(sender As Object, e As EventArgs) Handles EME2s2.TextChanged
        If sender.Enabled Then cf.EME2(EME2cb.SelectedIndex).EEOV.s2 = EME2s2.Text
    End Sub

    Private Sub EME2s3_TextChanged(sender As Object, e As EventArgs) Handles EME2s3.TextChanged
        If sender.Enabled Then cf.EME2(EME2cb.SelectedIndex).EEOV.s3 = EME2s3.Text & If(String.IsNullOrWhiteSpace(EME2s3.Text), "", ".amx")
    End Sub

    Private Sub EME2s4_TextChanged(sender As Object, e As EventArgs) Handles EME2s4.TextChanged
        If sender.Enabled Then cf.EME2(EME2cb.SelectedIndex).EEOV.s4 = EME2s4.Text & If(String.IsNullOrWhiteSpace(EME2s4.Text), "", ".dds")
    End Sub

    Private Sub EME2s5_TextChanged(sender As Object, e As EventArgs) Handles EME2s5.TextChanged
        If sender.Enabled Then cf.EME2(EME2cb.SelectedIndex).EEOV.s5 = EME2s5.Text & If(String.IsNullOrWhiteSpace(EME2s5.Text), "", ".veg")
    End Sub

    Private Sub EME2x_TextChanged(sender As Object, e As EventArgs) Handles EME2x.TextChanged
        If sender.Enabled Then cf.EME2(EME2cb.SelectedIndex).l.x = If(EME2x.Text = "", 0, EME2x.Text)
    End Sub

    Private Sub EME2y_TextChanged(sender As Object, e As EventArgs) Handles EME2y.TextChanged
        If sender.Enabled Then cf.EME2(EME2cb.SelectedIndex).l.y = If(EME2y.Text = "", 0, EME2y.Text)
    End Sub

    Private Sub EME2z_TextChanged(sender As Object, e As EventArgs) Handles EME2z.TextChanged
        If sender.Enabled Then cf.EME2(EME2cb.SelectedIndex).l.z = If(EME2z.Text = "", 0, EME2z.Text)
    End Sub

    Private Sub EME2r_TextChanged(sender As Object, e As EventArgs) Handles EME2r.TextChanged
        If sender.Enabled Then cf.EME2(EME2cb.SelectedIndex).l.r = If(EME2r.Text = "", 0, EME2r.Text)
    End Sub

    Private Sub EMSDs1_TextChanged(sender As Object, e As EventArgs) Handles EMSDs1.TextChanged
        If sender.Enabled Then cf.EMSD(EMSDcb.SelectedIndex).s1 = EMSDs1.Text
    End Sub

    Private Sub EMSDs2_TextChanged(sender As Object, e As EventArgs) Handles EMSDs2.TextChanged
        If sender.Enabled Then cf.EMSD(EMSDcb.SelectedIndex).s2 = EMSDs2.Text & If(String.IsNullOrWhiteSpace(EMSDs2.Text), "", ".psf")
    End Sub

    Private Sub EMSDx_TextChanged(sender As Object, e As EventArgs) Handles EMSDx.TextChanged
        If sender.Enabled Then cf.EMSD(EMSDcb.SelectedIndex).l.x = If(EMSDx.Text = "", 0, EMSDx.Text)
    End Sub

    Private Sub EMSDy_TextChanged(sender As Object, e As EventArgs) Handles EMSDy.TextChanged
        If sender.Enabled Then cf.EMSD(EMSDcb.SelectedIndex).l.y = If(EMSDy.Text = "", 0, EMSDy.Text)
    End Sub

    Private Sub EMSDz_TextChanged(sender As Object, e As EventArgs) Handles EMSDz.TextChanged
        If sender.Enabled Then cf.EMSD(EMSDcb.SelectedIndex).l.z = If(EMSDz.Text = "", 0, EMSDz.Text)
    End Sub

    Private Sub EPTHn_TextChanged(sender As Object, e As EventArgs) Handles EPTHn.TextChanged
        If sender.Enabled Then cf.EPTH(EPTHcb.SelectedIndex).name = EPTHn.Text
    End Sub

    Private Sub EPTHx_TextChanged(sender As Object, e As EventArgs) Handles EPTHx.TextChanged
        If sender.Enabled Then cf.EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1).x = If(EPTHx.Text = "", 0, EPTHx.Text)
    End Sub

    Private Sub EPTHy_TextChanged(sender As Object, e As EventArgs) Handles EPTHy.TextChanged
        If sender.Enabled Then cf.EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1).y = If(EPTHy.Text = "", 0, EPTHy.Text)
    End Sub

    Private Sub EPTHz_TextChanged(sender As Object, e As EventArgs) Handles EPTHz.TextChanged
        If sender.Enabled Then cf.EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1).z = If(EPTHz.Text = "", 0, EPTHz.Text)
    End Sub

    Private Sub EPTHr_TextChanged(sender As Object, e As EventArgs) Handles EPTHr.TextChanged
        If sender.Enabled Then cf.EPTH(EPTHcb.SelectedIndex).p(EPTHnud.Value - 1).r = If(EPTHr.Text = "", 0, EPTHr.Text)
    End Sub

    Private Sub Triggern_TextChanged(sender As Object, e As EventArgs) Handles Triggern.TextChanged
        If sender.Enabled Then cf.Triggers(Triggercb.SelectedIndex).ExTR.s = Triggern.Text
    End Sub

    Private Sub Triggertcb_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Triggertcb.SelectedIndexChanged
        If sender.Enabled Then cf.Triggers(Triggercb.SelectedIndex).ExTR.type = Triggertcb.SelectedItem
    End Sub

    Private Sub Triggerx_TextChanged(sender As Object, e As EventArgs) Handles Triggerx.TextChanged
        If sender.Enabled Then cf.Triggers(Triggercb.SelectedIndex).EMTR.r(Triggernud.Value - 1).x = If(Triggerx.Text = "", 0, Triggerx.Text)
    End Sub

    Private Sub Triggery_TextChanged(sender As Object, e As EventArgs) Handles Triggery.TextChanged
        If sender.Enabled Then cf.Triggers(Triggercb.SelectedIndex).EMTR.r(Triggernud.Value - 1).y = If(Triggery.Text = "", 0, Triggery.Text)
    End Sub

    Private Sub Triggerz_TextChanged(sender As Object, e As EventArgs) Handles Triggerz.TextChanged
        If sender.Enabled Then cf.Triggers(Triggercb.SelectedIndex).EMTR.r(Triggernud.Value - 1).z = If(Triggerz.Text = "", 0, Triggerz.Text)
    End Sub

    Private Sub GENThSR_ValueChanged(sender As Object, e As EventArgs) Handles GENThSR.ValueChanged
        If sender.Enabled Then cf.GENT.HoverSR = GENThSR.Value
        If GENThSR.Value = 0 Then
            GENTh.Enabled = False
            GENTh.Text = ""
        Else
            GENTh.Enabled = My.Settings.STFEditEnabled
            If GENThSR.Value - 1 < stf.Count Then stf.Add("")
            GENTh.Text = stf(GENThSR.Value - 1)
        End If
    End Sub

    Private Sub GENTlSR_ValueChanged(sender As Object, e As EventArgs) Handles GENTlSR.ValueChanged
        If sender.Enabled Then cf.GENT.LookSR = GENTlSR.Value
        If GENTlSR.Value = 0 Then
            GENTl.Enabled = False
            GENTl.Text = ""
        Else
            GENTl.Enabled = My.Settings.STFEditEnabled
            If GENTlSR.Value - 1 < stf.Count Then stf.Add("")
            GENTl.Text = stf(GENTlSR.Value - 1)
        End If
    End Sub

    Private Sub GENTnSR_ValueChanged(sender As Object, e As EventArgs) Handles GENTnSR.ValueChanged
        If sender.Enabled Then cf.GENT.NameSR = GENTnSR.Value
        If GENTnSR.Value = 0 Then
            GENTn.Enabled = False
            GENTn.Text = ""
        Else
            GENTn.Enabled = My.Settings.STFEditEnabled
            If GENTnSR.Value - 1 < stf.Count Then stf.Add("")
            GENTn.Text = stf(GENTnSR.Value - 1)
        End If
    End Sub

    Private Sub GENTuSR_ValueChanged(sender As Object, e As EventArgs) Handles GENTuSR.ValueChanged
        If sender.Enabled Then cf.GENT.UnkwnSR = GENTuSR.Value
        If GENTuSR.Value = 0 Then
            GENTu.Enabled = False
            GENTu.Text = ""
        Else
            GENTu.Enabled = My.Settings.STFEditEnabled
            If GENTuSR.Value - 1 < stf.Count Then stf.Add("")
            GENTu.Text = stf(GENTuSR.Value - 1)
        End If
    End Sub

    Private Sub GWAManSR_ValueChanged(sender As Object, e As EventArgs) Handles GWAManSR.ValueChanged
        If sender.Enabled Then cf.GCRE.GWAM(GWAMcb.SelectedIndex).NameSR = GWAManSR.Value
        If GWAManSR.Value = 0 Then
            GWAMan.Enabled = False
            GWAMan.Text = ""
        Else
            GWAMan.Enabled = My.Settings.STFEditEnabled
            If GWAManSR.Value - 1 < stf.Count Then stf.Add("")
            GWAMan.Text = stf(GWAManSR.Value - 1)
        End If
    End Sub

    Private Sub EEN2skl_TextChanged(sender As Object, e As EventArgs) Handles EEN2skl.TextChanged
        If sender.Enabled Then cf.EEN2.skl = EEN2skl.Text
    End Sub

    Private Sub EEN2invt_TextChanged(sender As Object, e As EventArgs) Handles EEN2invt.TextChanged
        If sender.Enabled Then cf.EEN2.invtex = EEN2invt.Text & If(String.IsNullOrWhiteSpace(EEN2invt.Text), "", ".dds")
    End Sub

    Private Sub EEN2actt_TextChanged(sender As Object, e As EventArgs) Handles EEN2actt.TextChanged
        If sender.Enabled Then cf.EEN2.acttex = EEN2actt.Text & If(String.IsNullOrWhiteSpace(EEN2actt.Text), "", ".dds")
    End Sub

    Private Sub EEN2s1_TextChanged(sender As Object, e As EventArgs) Handles EEN2s1.TextChanged
        If sender.Enabled Then cf.EEN2.EEOV.s1 = EEN2s1.Text
    End Sub

    Private Sub EEN2s2_TextChanged(sender As Object, e As EventArgs) Handles EEN2s2.TextChanged
        If sender.Enabled Then cf.EEN2.EEOV.s2 = EEN2s2.Text
    End Sub

    Private Sub EEN2s3_TextChanged(sender As Object, e As EventArgs) Handles EEN2s3.TextChanged
        If sender.Enabled Then cf.EEN2.EEOV.s3 = EEN2s3.Text & If(String.IsNullOrWhiteSpace(EEN2s3.Text), "", ".amx")
    End Sub

    Private Sub EEN2s4_TextChanged(sender As Object, e As EventArgs) Handles EEN2s4.TextChanged
        If sender.Enabled Then cf.EEN2.EEOV.s4 = EEN2s4.Text & If(String.IsNullOrWhiteSpace(EEN2s4.Text), "", ".dds")
    End Sub

    Private Sub EEN2s5_TextChanged(sender As Object, e As EventArgs) Handles EEN2s5.TextChanged
        If sender.Enabled Then cf.EEN2.EEOV.s5 = EEN2s5.Text & If(String.IsNullOrWhiteSpace(EEN2s5.Text), "", ".veg")
    End Sub

    Private Sub EEN2sel_CheckedChanged(sender As Object, e As EventArgs) Handles EEN2sel.CheckedChanged
        If sender.Enabled Then cf.EEN2.sel = EEN2sel.Checked
    End Sub

    Private Sub EEN2dgv_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles EEN2dgv.CellEndEdit
        If sender.Enabled Then cf.EEN2.EEOV.inv = EEN2dgv.GetStringArray()
    End Sub

    Private Sub GENTmhp_ValueChanged(sender As Object, e As EventArgs) Handles GENTmhp.ValueChanged
        If sender.Enabled Then cf.GENT.MaxHealth = GENTmhp.Value
    End Sub

    Private Sub GENTihp_ValueChanged(sender As Object, e As EventArgs) Handles GENTihp.ValueChanged
        If sender.Enabled Then cf.GENT.StartHealth = GENTihp.Value
    End Sub

    Private Sub GENTh_TextChanged(sender As Object, e As EventArgs) Handles GENTh.TextChanged
        If sender.Enabled Then stf(GENThSR.Value - 1) = GENTh.Text
    End Sub

    Private Sub GENTl_TextChanged(sender As Object, e As EventArgs) Handles GENTl.TextChanged
        If sender.Enabled Then stf(GENTlSR.Value - 1) = GENTl.Text
    End Sub

    Private Sub GENTn_TextChanged(sender As Object, e As EventArgs) Handles GENTn.TextChanged
        If sender.Enabled Then stf(GENTnSR.Value - 1) = GENTn.Text
    End Sub

    Private Sub GENTu_TextChanged(sender As Object, e As EventArgs) Handles GENTu.TextChanged
        If sender.Enabled Then stf(GENTuSR.Value - 1) = GENTu.Text
    End Sub

    Private Sub GWAMan_TextChanged(sender As Object, e As EventArgs) Handles GWAMan.TextChanged
        If sender.Enabled Then stf(GWAManSR.Value - 1) = GWAMan.Text
    End Sub

    Private Sub GCREspcb_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GCREspcb.SelectedIndexChanged
        If sender.Enabled Then GCREspv.Value = cf.GCRE.Special(GCREspcb.SelectedIndex)
    End Sub

    Private Sub GCREspv_ValueChanged(sender As Object, e As EventArgs) Handles GCREspv.ValueChanged
        If sender.Enabled Then cf.GCRE.Special(GCREspcb.SelectedIndex) = GCREspv.Value
    End Sub

    Private Sub GCREskcb_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GCREskcb.SelectedIndexChanged
        If sender.Enabled Then
            Dim avoidError As CRT = cf ' this avoids a compiler error caused by the ambiguous type of cf, even though the code *should* still work, thanks MS
            Dim existingSkill = (From sk In avoidError.GCRE.Skills Where sk.Index = GCREskcb.SelectedIndex Select sk)(0) ' Determine whether or not the list of skills
            Dim skillIndex = cf.GCRE.Skills.IndexOf(existingSkill)                                               ' contains a skill with the current index
            If skillIndex <> -1 Then                                                                             '
                GCREskv.Value = cf.GCRE.Skills(skillIndex).Value
            Else
                GCREskv.Value = 0
            End If
        End If
    End Sub

    Private Sub GCREskv_ValueChanged(sender As Object, e As EventArgs) Handles GCREskv.ValueChanged
        If sender.Enabled Then
            Dim avoidError As CRT = cf ' this avoids a compiler error caused by the ambiguous type of cf, even though the code *should* still work, thanks MS
            Dim existingSkill = (From sk In avoidError.GCRE.Skills Where sk.Index = GCREskcb.SelectedIndex Select sk)(0) ' Determine whether or not the list of skills
            Dim skillIndex = cf.GCRE.Skills.IndexOf(existingSkill)                                               ' contains a skill with the current index
            If skillIndex <> -1 Then                                                                             '
                If GCREskv.Value = 0 Then
                    cf.GCRE.Skills.RemoveAt(skillIndex)
                Else
                    cf.GCRE.Skills(skillIndex).Value = GCREskv.Value
                End If
            Else
                If GCREskv.Value <> 0 Then cf.GCRE.Skills.Add(New Skill(GCREskcb.SelectedIndex, GCREskv.Value))
            End If
        End If
    End Sub

    Private Sub GCREtrcb_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GCREtrcb.SelectedIndexChanged
        If sender.Enabled Then GCREtrv.Checked = cf.GCRE.Traits.Contains(GCREtrcb.SelectedIndex)
    End Sub

    Private Sub GCREtrv_CheckedChanged(sender As Object, e As EventArgs) Handles GCREtrv.CheckedChanged
        If sender.Enabled Then
            If GCREtrv.Checked Then
                If Not cf.GCRE.Traits.Contains(GCREtrcb.SelectedIndex) Then cf.GCRE.Traits.Add(GCREtrcb.SelectedIndex)
            Else
                cf.GCRE.Traits.Remove(GCREtrcb.SelectedIndex)
            End If
        End If
    End Sub

    Private Sub GCREtscb_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GCREtscb.SelectedIndexChanged
        If sender.Enabled Then GCREtsv.Checked = cf.GCRE.TagSkills.Contains(GCREtscb.SelectedIndex)
    End Sub

    Private Sub GCREtsv_CheckedChanged(sender As Object, e As EventArgs) Handles GCREtsv.CheckedChanged
        If sender.Enabled Then
            If GCREtsv.Checked Then
                If Not cf.GCRE.TagSkills.Contains(GCREtscb.SelectedIndex) Then cf.GCRE.TagSkills.Add(GCREtscb.SelectedIndex)
            Else
                cf.GCRE.TagSkills.Remove(GCREtscb.SelectedIndex)
            End If
        End If
    End Sub

    Private Sub GCREp_TextChanged(sender As Object, e As EventArgs) Handles GCREp.TextChanged
        If sender.Enabled Then cf.GCRE.PortStr = GCREp.Text & If(String.IsNullOrWhiteSpace(GCREp.Text), "", ".dds")
    End Sub

    Private Sub GCREsoccb_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GCREsoccb.SelectedIndexChanged
        If sender.Enabled Then
            Select Case GCREsoccb.SelectedItem
                Case "Head"
                    GCREsocm.Text = cf.GCRE.Hea.Model
                    GCREsoct.Text = cf.GCRE.Hea.Tex.Replace(".dds", "")
                Case "Hair"
                    GCREsocm.Text = cf.GCRE.Hai.Model
                    GCREsoct.Text = cf.GCRE.Hai.Tex.Replace(".dds", "")
                Case "Ponytail"
                    GCREsocm.Text = cf.GCRE.Pon.Model
                    GCREsoct.Text = cf.GCRE.Pon.Tex.Replace(".dds", "")
                Case "Moustache"
                    GCREsocm.Text = cf.GCRE.Mus.Model
                    GCREsoct.Text = cf.GCRE.Mus.Tex.Replace(".dds", "")
                Case "Beard"
                    GCREsocm.Text = cf.GCRE.Bea.Model
                    GCREsoct.Text = cf.GCRE.Bea.Tex.Replace(".dds", "")
                Case "Eye"
                    GCREsocm.Text = cf.GCRE.Eye.Model
                    GCREsoct.Text = cf.GCRE.Eye.Tex.Replace(".dds", "")
                Case "Body"
                    GCREsocm.Text = cf.GCRE.Bod.Model
                    GCREsoct.Text = cf.GCRE.Bod.Tex.Replace(".dds", "")
                Case "Hand"
                    GCREsocm.Text = cf.GCRE.Han.Model
                    GCREsoct.Text = cf.GCRE.Han.Tex.Replace(".dds", "")
                Case "Feet"
                    GCREsocm.Text = cf.GCRE.Fee.Model
                    GCREsoct.Text = cf.GCRE.Fee.Tex.Replace(".dds", "")
                Case "Back"
                    GCREsocm.Text = cf.GCRE.Bac.Model
                    GCREsoct.Text = cf.GCRE.Bac.Tex.Replace(".dds", "")
                Case "Shoulder"
                    GCREsocm.Text = cf.GCRE.Sho.Model
                    GCREsoct.Text = cf.GCRE.Sho.Tex.Replace(".dds", "")
                Case "Vanity"
                    GCREsocm.Text = cf.GCRE.Van.Model
                    GCREsoct.Text = cf.GCRE.Van.Tex.Replace(".dds", "")
            End Select
        End If
    End Sub

    Private Sub GCREsocm_TextChanged(sender As Object, e As EventArgs) Handles GCREsocm.TextChanged
        If sender.Enabled Then
            Select Case GCREsoccb.SelectedItem
                Case "Head"
                    cf.GCRE.Hea.Model = GCREsocm.Text
                Case "Hair"
                    cf.GCRE.Hai.Model = GCREsocm.Text
                Case "Ponytail"
                    cf.GCRE.Pon.Model = GCREsocm.Text
                Case "Moustache"
                    cf.GCRE.Mus.Model = GCREsocm.Text
                Case "Beard"
                    cf.GCRE.Bea.Model = GCREsocm.Text
                Case "Eye"
                    cf.GCRE.Eye.Model = GCREsocm.Text
                Case "Body"
                    cf.GCRE.Bod.Model = GCREsocm.Text
                Case "Hand"
                    cf.GCRE.Han.Model = GCREsocm.Text
                Case "Feet"
                    cf.GCRE.Fee.Model = GCREsocm.Text
                Case "Back"
                    cf.GCRE.Bac.Model = GCREsocm.Text
                Case "Shoulder"
                    cf.GCRE.Sho.Model = GCREsocm.Text
                Case "Vanity"
                    cf.GCRE.Van.Model = GCREsocm.Text
            End Select
        End If
    End Sub

    Private Sub GCREsoct_TextChanged(sender As Object, e As EventArgs) Handles GCREsoct.TextChanged
        If sender.Enabled Then
            Dim s = GCREsoct.Text & If(String.IsNullOrWhiteSpace(GCREsoct.Text), "", ".dds")
            Select Case GCREsoccb.SelectedItem
                Case "Head"
                    cf.GCRE.Hea.Tex = s
                Case "Hair"
                    cf.GCRE.Hai.Tex = s
                Case "Ponytail"
                    cf.GCRE.Pon.Tex = s
                Case "Moustache"
                    cf.GCRE.Mus.Tex = s
                Case "Beard"
                    cf.GCRE.Bea.Tex = s
                Case "Eye"
                    cf.GCRE.Eye.Tex = s
                Case "Body"
                    cf.GCRE.Bod.Tex = s
                Case "Hand"
                    cf.GCRE.Han.Tex = s
                Case "Feet"
                    cf.GCRE.Fee.Tex = s
                Case "Back"
                    cf.GCRE.Bac.Tex = s
                Case "Shoulder"
                    cf.GCRE.Sho.Tex = s
                Case "Vanity"
                    cf.GCRE.Van.Tex = s
            End Select
        End If
    End Sub

    Private Sub GCREdgv_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles GCREdgv.CellEndEdit
        If sender.Enabled Then cf.GCRE.Inventory = GCREdgv.GetStringArray()
    End Sub

    Private Sub GCHRn_TextChanged(sender As Object, e As EventArgs) Handles GCHRn.TextChanged
        If sender.enabled Then cf.GCHR.name = GCHRn.Text
    End Sub

    Private Sub GWAMani_ValueChanged(sender As Object, e As EventArgs) Handles GWAMani.ValueChanged
        If sender.enabled Then cf.GCRE.GWAM(GWAMcb.SelectedIndex).Anim = GWAMani.Value
    End Sub

    Private Sub GWAMdt_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GWAMdt.SelectedIndexChanged
        If sender.enabled Then cf.GCRE.GWAM(GWAMcb.SelectedIndex).DmgType = GWAMdt.SelectedIndex
    End Sub

    Private Sub GWAMsf_ValueChanged(sender As Object, e As EventArgs) Handles GWAMsf.ValueChanged
        If sender.enabled Then cf.GCRE.GWAM(GWAMcb.SelectedIndex).ShotsFired = GWAMsf.Value
    End Sub

    Private Sub GWAMr_ValueChanged(sender As Object, e As EventArgs) Handles GWAMr.ValueChanged
        If sender.enabled Then cf.GCRE.GWAM(GWAMcb.SelectedIndex).Range = GWAMr.Value
    End Sub

    Private Sub GWAMmin_ValueChanged(sender As Object, e As EventArgs) Handles GWAMmin.ValueChanged
        If sender.enabled Then cf.GCRE.GWAM(GWAMcb.SelectedIndex).MinDmg = GWAMmin.Value
    End Sub

    Private Sub GWAMmax_ValueChanged(sender As Object, e As EventArgs) Handles GWAMmax.ValueChanged
        If sender.enabled Then cf.GCRE.GWAM(GWAMcb.SelectedIndex).MaxDmg = GWAMmax.Value
    End Sub

    Private Sub GWAMap_ValueChanged(sender As Object, e As EventArgs) Handles GWAMap.ValueChanged
        If sender.enabled Then cf.GCRE.GWAM(GWAMcb.SelectedIndex).AP = GWAMap.Value
    End Sub

    Private Sub GWAMef_TextChanged(sender As Object, e As EventArgs) Handles GWAMef.TextChanged
        If sender.enabled Then cf.GCRE.GWAM(GWAMcb.SelectedIndex).VegName = GWAMef.Text
    End Sub

    Private Sub GCREage_ValueChanged(sender As Object, e As EventArgs) Handles GCREage.ValueChanged
        If sender.Enabled Then cf.GCRE.Age = GCREage.Value
    End Sub

    Private Sub _2MWTcb_SelectedIndexChanged(sender As Object, e As EventArgs) Handles _2MWTcb.SelectedIndexChanged
        _2MWTx.Text = cf._2MWT.chunks(_2MWTcb.SelectedIndex).loc.x
        _2MWTy.Text = cf._2MWT.chunks(_2MWTcb.SelectedIndex).loc.y
        _2MWTz.Text = cf._2MWT.chunks(_2MWTcb.SelectedIndex).loc.z
        Try
            _2MWTtex.Text = cf._2MWT.chunks(_2MWTcb.SelectedIndex).tex.Substring(0, cf._2MWT.chunks(_2MWTcb.SelectedIndex).tex.LastIndexOf("."))
        Catch : _2MWTtex.Text = ""
        End Try
        _2MWTlmx.Text = cf._2MWT.chunks(_2MWTcb.SelectedIndex).texloc.x
        _2MWTlmy.Text = cf._2MWT.chunks(_2MWTcb.SelectedIndex).texloc.y
    End Sub

    Private Sub _2MWTmpf_TextChanged(sender As Object, e As EventArgs) Handles _2MWTmpf.TextChanged
        If sender.Enabled Then cf._2MWT.mpf = _2MWTmpf.Text
    End Sub

    Private Sub _2MWTfr_CheckedChanged(sender As Object, e As EventArgs) Handles _2MWTfr.CheckedChanged
        If sender.enabled Then cf._2MWT.frozen = sender.checked
    End Sub

    Private Sub _2MWTdw_CheckedChanged(sender As Object, e As EventArgs) Handles _2MWTdw.CheckedChanged
        If sender.enabled Then cf._2MWT.dark = sender.checked
    End Sub

    Private Sub _2MWTtex_TextChanged(sender As Object, e As EventArgs) Handles _2MWTtex.TextChanged
        If sender.Enabled Then cf._2MWT.chunks(_2MWTcb.SelectedIndex).tex = _2MWTtex.Text & ".dds"
    End Sub

    Private Sub _2MWTx_TextChanged(sender As Object, e As EventArgs) Handles _2MWTx.TextChanged
        If sender.Enabled Then cf._2MWT.chunks(_2MWTcb.SelectedIndex).loc.x = If(_2MWTx.Text = "", 0, _2MWTx.Text)
    End Sub

    Private Sub _2MWTy_TextChanged(sender As Object, e As EventArgs) Handles _2MWTy.TextChanged
        If sender.Enabled Then cf._2MWT.chunks(_2MWTcb.SelectedIndex).loc.y = If(_2MWTy.Text = "", 0, _2MWTy.Text)
    End Sub

    Private Sub _2MWTz_TextChanged(sender As Object, e As EventArgs) Handles _2MWTz.TextChanged
        If sender.Enabled Then cf._2MWT.chunks(_2MWTcb.SelectedIndex).loc.z = If(_2MWTz.Text = "", 0, _2MWTz.Text)
    End Sub

    Private Sub _2MWTlmx_TextChanged(sender As Object, e As EventArgs) Handles _2MWTlmx.TextChanged
        If sender.Enabled Then cf._2MWT.chunks(_2MWTcb.SelectedIndex).texloc.x = _2MWTlmx.Text
    End Sub

    Private Sub _2MWTlmy_TextChanged(sender As Object, e As EventArgs) Handles _2MWTlmy.TextChanged
        If sender.Enabled Then cf._2MWT.chunks(_2MWTcb.SelectedIndex).texloc.y = _2MWTlmy.Text
    End Sub

    ' Prevent invalid floats from being entered into text boxes (Add any float textboxes to the "Handles" section)
    Private Shared Sub FloatsOnly(sender As TextBox, e As KeyPressEventArgs) _
        Handles ECAMx.KeyPress, ECAMy.KeyPress, ECAMz.KeyPress, ECAMr.KeyPress, EMEPx.KeyPress, EMEPy.KeyPress,
                EMEPz.KeyPress, EMEPr.KeyPress, EMEFx.KeyPress, EMEFy.KeyPress, EMEFz.KeyPress, EMEFr.KeyPress,
                EMSDx.KeyPress, EMSDy.KeyPress, EMSDz.KeyPress, EME2x.KeyPress, EME2y.KeyPress, EME2z.KeyPress,
                EME2r.KeyPress, EPTHx.KeyPress, EPTHy.KeyPress, EPTHz.KeyPress, EPTHr.KeyPress, Triggerx.KeyPress,
                Triggery.KeyPress, Triggerz.KeyPress, _2MWTx.KeyPress, _2MWTy.KeyPress, _2MWTz.KeyPress,
                _2MWTlmx.KeyPress, _2MWTlmy.KeyPress
        If Char.IsDigit(e.KeyChar) Or e.KeyChar = "." Or e.KeyChar = "-" Or e.KeyChar = Chr(8) Then
            Dim text As String = sender.Text
            If e.KeyChar = "." Then
                If text.IndexOf(".") > -1 Then e.Handled = True
            ElseIf e.KeyChar = "-" Then
                If text.IndexOf("-") > -1 Or sender.SelectionStart <> 0 Then e.Handled = True
            End If
        Else
            e.Handled = True
        End If
    End Sub

    ' Prevent invalid ints from being entered into text boxes (Add any numeric textboxes to the "Handles" section)
    Private Sub IntsOnly(sender As TextBox, e As KeyPressEventArgs)
        If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

#End Region

#Region "Scroll Bars"

    Private Sub EME2dgv_ScrollChanged() Handles EME2dgv.Scroll
        EME2dsb.ScrollTo(EME2dgv.FirstDisplayedScrollingRowIndex / (EME2dgv.Rows.Count - EME2dgv.DisplayedRowCount(False)) * EME2dsb.Maximum)
    End Sub

    Private Sub EME2dsb_Click(sender As Object, e As EventArgs) Handles EME2dsb.MouseDown
        EME2tmr.Start()
    End Sub

    Private Sub EME2dsb_MouseUp(sender As Object, e As EventArgs) Handles EME2dsb.MouseUp
        EME2tmr.Stop()
    End Sub

    Private Sub EME2tmr_Tick(sender As Object, e As EventArgs) Handles EME2tmr.Tick
        EME2dgv.FirstDisplayedScrollingRowIndex = EME2dsb.Value / EME2dsb.Maximum * (EME2dgv.Rows.Count - EME2dgv.DisplayedRowCount(False))
    End Sub

    Private Sub EEN2dgv_ScrollChanged() Handles EEN2dgv.Scroll
        EEN2dsb.ScrollTo(EEN2dgv.FirstDisplayedScrollingRowIndex / (EEN2dgv.Rows.Count - EEN2dgv.DisplayedRowCount(False)) * EEN2dsb.Maximum)
    End Sub

    Private Sub EEN2dsb_Click(sender As Object, e As EventArgs) Handles EEN2dsb.MouseDown
        EEN2tmr.Start()
    End Sub

    Private Sub EEN2dsb_MouseUp(sender As Object, e As EventArgs) Handles EEN2dsb.MouseUp
        EEN2tmr.Stop()
    End Sub

    Private Sub EEN2tmr_Tick(sender As Object, e As EventArgs) Handles EEN2tmr.Tick
        EEN2dgv.FirstDisplayedScrollingRowIndex = EEN2dsb.Value / EEN2dsb.Maximum * (EEN2dgv.Rows.Count - EEN2dgv.DisplayedRowCount(False))
    End Sub

    Private Sub GCREdgv_ScrollChanged() Handles GCREdgv.Scroll
        GCREdsb.ScrollTo(GCREdgv.FirstDisplayedScrollingRowIndex / (GCREdgv.Rows.Count - GCREdgv.DisplayedRowCount(False)) * GCREdsb.Maximum)
    End Sub

    Private Sub GCREdsb_Click(sender As Object, e As EventArgs) Handles GCREdsb.MouseDown
        GCREtmr.Start()
    End Sub

    Private Sub GCREdsb_MouseUp(sender As Object, e As EventArgs) Handles GCREdsb.MouseUp
        GCREtmr.Stop()
    End Sub

    Private Sub GCREtmr_Tick(sender As Object, e As EventArgs) Handles GCREtmr.Tick
        GCREdgv.FirstDisplayedScrollingRowIndex = GCREdsb.Value / GCREdsb.Maximum * (GCREdgv.Rows.Count - GCREdgv.DisplayedRowCount(False))
    End Sub

#End Region

#Region "Remove/Add Chunks"

    Private Sub EMEPp_Click(sender As Object, e As EventArgs) Handles EMEPp.Click
        For Each c As Control In EMEPgb.Controls
            c.Enabled = True
        Next
        cf.EMEP.Add(New EMEPc())
        EMEPcb.Items.Add(EMEPcb.Items.Count + 1)
        EMEPcb.SelectedIndex = EMEPcb.Items.Count - 1
    End Sub

    Private Sub EMEPm_Click(sender As Object, e As EventArgs) Handles EMEPm.Click
        Dim i = EMEPcb.SelectedIndex
        If cf.EMEP.Count = 1 Then
            cf.EMEP = New List(Of EMEPc)
            MapSetupUI(cf)
            For Each c As Control In EMEPgb.Controls
                c.Enabled = False
            Next
            EMEPcb.Enabled = True : EMEPp.Enabled = True
        Else
            cf.EMEP.RemoveAt(i)
            EMEPcb.Items.RemoveAt(i)
            EMEPcb.SelectedIndex = If(i = 0, 0, i - 1)
        End If
    End Sub

    Private Sub EME2p_Click(sender As Object, e As EventArgs) Handles EME2p.Click
        For Each c As Control In EME2gb.Controls
            c.Enabled = True
        Next
        cf.EME2.Add(New EME2c())
        EME2cb.Items.Add(EME2cb.Items.Count + 1)
        EME2cb.SelectedIndex = EME2cb.Items.Count - 1
    End Sub

    Private Sub EME2m_Click(sender As Object, e As EventArgs) Handles EME2m.Click
        Dim i = EME2cb.SelectedIndex
        If cf.EME2.Count = 1 Then
            cf.EME2 = New List(Of EME2c)
            MapSetupUI(cf)
            For Each c As Control In EME2gb.Controls
                c.Enabled = False
            Next
            EME2cb.Enabled = True : EME2p.Enabled = True
        Else
            cf.EME2.RemoveAt(i)
            EME2cb.Items.RemoveAt(i)
            EME2cb.SelectedIndex = If(i = 0, 0, i - 1)
        End If
    End Sub

    Private Sub EMEFm_Click(sender As Object, e As EventArgs) Handles EMEFm.Click
        Dim i = EMEFcb.SelectedIndex
        If cf.EMEF.Count = 1 Then
            cf.EMEF = New List(Of EMEFc)
            MapSetupUI(cf)
            For Each c As Control In EMEFgb.Controls
                c.Enabled = False
            Next
            EMEFcb.Enabled = True : EMEFp.Enabled = True
        Else
            cf.EMEF.RemoveAt(i)
            EMEFcb.Items.RemoveAt(i)
            EMEFcb.SelectedIndex = If(i = 0, 0, i - 1)
        End If
    End Sub

    Private Sub EMEFp_Click(sender As Object, e As EventArgs) Handles EMEFp.Click
        For Each c As Control In EMEFgb.Controls
            c.Enabled = True
        Next
        cf.EMEF.Add(New EMEFc())
        EMEFcb.Items.Add(EMEFcb.Items.Count + 1)
        EMEFcb.SelectedIndex = EMEFcb.Items.Count - 1
    End Sub

    Private Sub EMSDm_Click(sender As Object, e As EventArgs) Handles EMSDm.Click
        Dim i = EMSDcb.SelectedIndex
        If cf.EMSD.Count = 1 Then
            cf.EMSD = New List(Of EMSDc)
            MapSetupUI(cf)
            For Each c As Control In EMSDgb.Controls
                c.Enabled = False
            Next
            EMSDcb.Enabled = True : EMSDp.Enabled = True
        Else
            cf.EMSD.RemoveAt(i)
            EMSDcb.Items.RemoveAt(i)
            EMSDcb.SelectedIndex = If(i = 0, 0, i - 1)
        End If
    End Sub

    Private Sub EMSDp_Click(sender As Object, e As EventArgs) Handles EMSDp.Click
        For Each c As Control In EMSDgb.Controls
            c.Enabled = True
        Next
        cf.EMSD.Add(New EMSDc())
        EMSDcb.Items.Add(EMSDcb.Items.Count + 1)
        EMSDcb.SelectedIndex = EMSDcb.Items.Count - 1
    End Sub

    Private Sub EPTHm_Click(sender As Object, e As EventArgs) Handles EPTHm.Click
        Dim i = EPTHcb.SelectedIndex
        If cf.EPTH.Count = 1 Then
            cf.EPTH = New List(Of EPTHc)
            MapSetupUI(cf)
            For Each c As Control In EPTHGB.Controls
                c.Enabled = False
            Next
            EPTHcb.Enabled = True : EPTHp.Enabled = True
        Else
            cf.EPTH.RemoveAt(i)
            EPTHcb.Items.RemoveAt(i)
            EPTHcb.SelectedIndex = If(i = 0, 0, i - 1)
        End If
    End Sub

    Private Sub EPTHp_Click(sender As Object, e As EventArgs) Handles EPTHp.Click
        For Each c As Control In EPTHGB.Controls
            c.Enabled = True
        Next
        cf.EPTH.Add(New EPTHc())
        EPTHcb.Items.Add(EPTHcb.Items.Count + 1)
        EPTHcb.SelectedIndex = EPTHcb.Items.Count - 1
    End Sub

    Private Sub Triggerm_Click(sender As Object, e As EventArgs) Handles Triggerm.Click
        Dim i = Triggercb.SelectedIndex
        If cf.Triggers.Count = 1 Then
            cf.Triggers = New List(Of Trigger)
            MapSetupUI(cf)
            For Each c As Control In Triggergb.Controls
                c.Enabled = False
            Next
            Triggercb.Enabled = True : Triggerp.Enabled = True
        Else
            cf.Triggers.RemoveAt(i)
            Triggercb.Items.RemoveAt(i)
            Triggercb.SelectedIndex = If(i = 0, 0, i - 1)
        End If
    End Sub

    Private Sub Triggerp_Click(sender As Object, e As EventArgs) Handles Triggerp.Click
        For Each c As Control In Triggergb.Controls
            c.Enabled = True
        Next
        cf.Triggers.Add(New Trigger())
        Triggercb.Items.Add(Triggercb.Items.Count + 1)
        Triggercb.SelectedIndex = Triggercb.Items.Count - 1
    End Sub

    Private Sub GWAMm_Click(sender As Object, e As EventArgs) Handles GWAMm.Click
        Dim i = GWAMcb.SelectedIndex
        If cf.GCRE.GWAM.Count = 1 Then
            cf.GCRE.GWAM = New List(Of GWAMc)
            CRTSetupUI(cf)
            For Each c As Control In GWAMgb.Controls
                c.Enabled = False
            Next
            GWAMcb.Enabled = True : GWAMp.Enabled = True
        Else
            cf.GCRE.GWAM.RemoveAt(i)
            GWAMcb.Items.RemoveAt(i)
            GWAMcb.SelectedIndex = If(i = 0, 0, i - 1)
        End If
    End Sub

    Private Sub GWAMp_Click(sender As Object, e As EventArgs) Handles GWAMp.Click
        For Each c As Control In GWAMgb.Controls
            c.Enabled = True
        Next
        cf.GCRE.GWAM.Add(New GWAMc())
        GWAMcb.Items.Add(GWAMcb.Items.Count + 1)
        GWAMcb.SelectedIndex = GWAMcb.Items.Count - 1
        GWAMToUI()
    End Sub

    Private Sub _2MWTp_Click(sender As Object, e As EventArgs) Handles _2MWTp.Click
        If cf._2MWT Is Nothing Then cf._2MWT = New _2MWTc
        For Each c As Control In _2MWTgb.Controls
            c.Enabled = True
        Next
        cf._2MWT.chunks.Add(New _2MWTChunk("", New Point3(0, 0, 0), New Point2(0, 0)))
        _2MWTcb.Items.Add(_2MWTcb.Items.Count + 1)
        _2MWTcb.SelectedIndex = _2MWTcb.Items.Count - 1
    End Sub

    Private Sub _2MWTm_Click(sender As Object, e As EventArgs) Handles _2MWTm.Click
        Dim i = _2MWTcb.SelectedIndex
        Dim co As Integer = cf._2MWT.chunks.Count
        If co = 1 Then
            cf._2MWT.chunks = New List(Of _2MWTChunk)
            MapSetupUI(cf)
            For Each c As Control In _2MWTgb.Controls
                c.Enabled = False
            Next
            _2MWTcb.Enabled = True : _2MWTp.Enabled = True
        Else
            cf._2MWT.chunks.RemoveAt(i)
            _2MWTcb.Items.RemoveAt(i)
            _2MWTcb.SelectedIndex = Math.Max(0, i - 1)
        End If
    End Sub

#End Region

#Region ".stf Stuff"

    Private Sub FullSTFToText() Handles StfToolStripMenuItem.Click
        Dim ofd As New OpenFileDialog With {.Filter = "String Table File|*.stf", .Multiselect = False}
        If ofd.ShowDialog() = DialogResult.OK Then
            Dim sfd As New SaveFileDialog With {.Filter = "Text File|*.txt"}
            If sfd.ShowDialog() = DialogResult.OK Then
                File.WriteAllLines(sfd.FileName, STFToTXT(File.ReadAllBytes(ofd.FileName)))
            End If
        End If
    End Sub

    Private Sub FullTextToSTF() Handles TxtTostfToolStripMenuItem.Click
        Dim ofd As New OpenFileDialog With {.Filter = "Text File|*.txt", .Multiselect = False}
        If ofd.ShowDialog() = DialogResult.OK Then
            Dim sfd As New SaveFileDialog With {.Filter = "String Table File|*.stf"}
            If sfd.ShowDialog() = DialogResult.OK Then
                File.WriteAllBytes(sfd.FileName, TXTToSTF(File.ReadAllLines(ofd.FileName)))
            End If
        End If
    End Sub

    Private Function CheckAndLoadStf() As Boolean
        If My.Settings.STFDir = "" Then
            If DarkMessageBox.ShowInformation("English.STF Location not set, please locate it.", "English.stf Not Selected") = DialogResult.OK Then
                SetEngStfLocation()
                Return True
            Else
                Return False
            End If
        ElseIf Not File.Exists(My.Settings.STFDir) Then
            If DarkMessageBox.ShowInformation("Previous English.STF not found, please select a new one.", "English.stf Not Found") = DialogResult.OK Then
                SetEngStfLocation()
                Return True
            Else
                Return False
            End If
        Else
            stf = STFToTXT(File.ReadAllBytes(My.Settings.STFDir))
            Return True
        End If
    End Function

    Private Sub SetEngStfLocation() Handles SetEnglishstfLocationToolStripMenuItem.Click
        Dim ofd As New OpenFileDialog With {.Multiselect = False, .CheckFileExists = True, .Filter = "English.stf|*.stf"}
        If ofd.ShowDialog() = DialogResult.OK Then
            My.Settings.STFDir = ofd.FileName
            stf = STFToTXT(File.ReadAllBytes(ofd.FileName))
        End If
    End Sub

    Private Sub EnableEnglishstfEditingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EnableSTFEdit.CheckedChanged
        My.Settings.STFEditEnabled = EnableSTFEdit.Checked
    End Sub

#End Region

#Region "Custom Accelerators"

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.ControlKey
                HCtrl = True
            Case Keys.N
                HN = True
            Case Keys.D1
                H1 = True
            Case Keys.D2
                H2 = True
            Case Keys.D3
                H3 = True
            Case Keys.D4
                H4 = True
            Case Keys.D5
                H5 = True
            Case Keys.D6
                H6 = True
            Case Keys.D7
                H7 = True
            Case Keys.D8
                H8 = True
            Case Keys.D9
                H9 = True
            Case Keys.D0
                H0 = True
        End Select
        If HCtrl AndAlso HN Then
            If H1 Then
                NewAmo()
            ElseIf H2 Then
                NewArm()
            ElseIf H3 Then
                NewCon()
            ElseIf H4 Then
                NewCrt()
            ElseIf H5 Then
                NewDor()
            ElseIf H6 Then
                NewInt()
            ElseIf H7 Then
                NewItm()
            ElseIf H8 Then
                NewMap()
            ElseIf H9 Then
                NewUse()
            ElseIf H0 Then
                NewWea()
            End If
        End If
        MyBase.OnKeyDown(e)
    End Sub

    Protected Overrides Sub OnKeyUp(e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.ControlKey
                HCtrl = False
            Case Keys.N
                HN = False
            Case Keys.D1
                H1 = False
            Case Keys.D2
                H2 = False
            Case Keys.D3
                H3 = False
            Case Keys.D4
                H4 = False
            Case Keys.D5
                H5 = False
            Case Keys.D6
                H6 = False
            Case Keys.D7
                H7 = False
            Case Keys.D8
                H8 = False
            Case Keys.D9
                H9 = False
            Case Keys.D0
                H0 = False
        End Select
        MyBase.OnKeyUp(e)
    End Sub

    Private Sub GITMtype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GITMtype.SelectedIndexChanged
        If sender.enabled Then cf.GITM.type = sender.selectedindex
    End Sub

    Private Sub GITMeq_CheckedChanged(sender As Object, e As EventArgs) Handles GITMeq.CheckedChanged
        If sender.enabled Then cf.gitm.equip = sender.checked
    End Sub

    Private Sub GITMslot_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GITMslot.SelectedIndexChanged
        If sender.enabled Then cf.GITM.eqslot = sender.selectedindex
    End Sub

    Private Sub GITMrl_TextChanged(sender As Object, e As EventArgs) Handles GITMrl.TextChanged
        If sender.enabled Then cf.GITM.reload = sender.text
    End Sub

    Private Sub GITMml_CheckedChanged(sender As Object, e As EventArgs) Handles GITMml.CheckedChanged
        ' TODO: Figure out Melee
    End Sub

    Private Sub GITMhhai_CheckedChanged(sender As Object, e As EventArgs) Handles GITMhhai.CheckedChanged
        If sender.enabled Then cf.GITM.hHai = sender.checked
    End Sub

    Private Sub GITMhbea_CheckedChanged(sender As Object, e As EventArgs) Handles GITMhbea.CheckedChanged
        If sender.enabled Then cf.GITM.hBea = sender.checked
    End Sub

    Private Sub GITMhmus_CheckedChanged(sender As Object, e As EventArgs) Handles GITMhmus.CheckedChanged
        If sender.enabled Then cf.GITM.hMus = sender.checked
    End Sub

    Private Sub GITMheye_CheckedChanged(sender As Object, e As EventArgs) Handles GITMheye.CheckedChanged
        If sender.enabled Then cf.GITM.hEye = sender.checked
    End Sub

    Private Sub GITMhpon_CheckedChanged(sender As Object, e As EventArgs) Handles GITMhpon.CheckedChanged
        If sender.enabled Then cf.GITM.hPon = sender.checked
    End Sub

    Private Sub GITMhvan_CheckedChanged(sender As Object, e As EventArgs) Handles GITMhvan.CheckedChanged
        If sender.enabled Then cf.GITM.hVan = sender.checked
    End Sub

    Private Sub GITMsocgb_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GITMsoccb.SelectedIndexChanged
        If sender.enabled Then
            Select Case sender.selecteditem
                Case "Head"
                    GITMsoct.Text = cf.GITM.Hea.Tex
                    GITMsocm.Text = cf.GITM.Hea.Model
                Case "Eye"
                    GITMsoct.Text = cf.GITM.Eye.Tex
                    GITMsocm.Text = cf.GITM.Eye.Model
                Case "Body"
                    GITMsoct.Text = cf.GITM.Bod.Tex
                    GITMsocm.Text = cf.GITM.Bod.Model
                Case "Back"
                    GITMsoct.Text = cf.GITM.Bac.Tex
                    GITMsocm.Text = cf.GITM.Bac.Model
                Case "Hands"
                    GITMsoct.Text = cf.GITM.Han.Tex
                    GITMsocm.Text = cf.GITM.Han.Model
                Case "Feet"
                    GITMsoct.Text = cf.GITM.Fee.Tex
                    GITMsocm.Text = cf.GITM.Fee.Model
                Case "Shoes"
                    GITMsoct.Text = cf.GITM.Sho.Tex
                    GITMsocm.Text = cf.GITM.Sho.Model
                Case "Vanity"
                    GITMsoct.Text = cf.GITM.Van.Tex
                    GITMsocm.Text = cf.GITM.Van.Model
                Case "In-Hand"
                    GITMsoct.Text = cf.GITM.IHS.Tex
                    GITMsocm.Text = cf.GITM.IHS.Model
            End Select
        End If
    End Sub

    Private Sub GITMsocm_TextChanged(sender As Object, e As EventArgs) Handles GITMsocm.TextChanged
        If sender.enabled Then
            Select Case GITMsoccb.SelectedItem
                Case "Head"
                    cf.GITM.Hea.Model = GITMsocm.Text
                Case "Eye"
                    cf.GITM.Eye.Model = GITMsocm.Text
                Case "Body"
                    cf.GITM.Bod.Model = GITMsocm.Text
                Case "Back"
                    cf.GITM.Bac.Model = GITMsocm.Text
                Case "Hands"
                    cf.GITM.Han.Model = GITMsocm.Text
                Case "Feet"
                    cf.GITM.Fee.Model = GITMsocm.Text
                Case "Shoes"
                    cf.GITM.Sho.Model = GITMsocm.Text
                Case "Vanity"
                    cf.GITM.Van.Model = GITMsocm.Text
                Case "In-Hand"
                    cf.GITM.IHS.Model = GITMsocm.Text
            End Select
        End If
    End Sub

    Private Sub GITMsoct_TextChanged(sender As Object, e As EventArgs) Handles GITMsoct.TextChanged
        If sender.enabled Then
            Select Case GITMsoccb.SelectedItem
                Case "Head"
                    cf.GITM.Hea.Tex = GITMsoct.Text
                Case "Eye"
                    cf.GITM.Eye.Tex = GITMsoct.Text
                Case "Body"
                    cf.GITM.Bod.Tex = GITMsoct.Text
                Case "Back"
                    cf.GITM.Bac.Tex = GITMsoct.Text
                Case "Hands"
                    cf.GITM.Han.Tex = GITMsoct.Text
                Case "Feet"
                    cf.GITM.Fee.Tex = GITMsoct.Text
                Case "Shoes"
                    cf.GITM.Sho.Tex = GITMsoct.Text
                Case "Vanity"
                    cf.GITM.Van.Tex = GITMsoct.Text
                Case "In-Hand"
                    cf.GITM.IHS.Tex = GITMsoct.Text
            End Select
        End If
    End Sub

#End Region

    Private Sub ExtractgrpFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExtractgrpFilesToolStripMenuItem.Click
        Using grpb As New GrpBrowser(Nothing)
            grpb.ReadRHT()
            grpb.readExtensions()
            grpb.extractFile(-1)
            DarkMessageBox.ShowInformation("Done Extracting", "Finished")
        End Using
    End Sub

    Private Sub ExtractAndConvertToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExtractAndConvertToolStripMenuItem.Click
        Using grpb As New GrpBrowser(Nothing)
            grpb.ReadRHT()
            grpb.readExtensions()
            grpb.extractFile(-1, True)
            DarkMessageBox.ShowInformation("Done Extracting", "Finished")
        End Using
    End Sub

    Private Sub GrpBrowserToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GrpBrowserToolStripMenuItem.Click
        Using grpb As New GrpBrowser(Nothing, True)
            grpb.ShowDialog()
        End Using
    End Sub

End Class