Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.IO.Compression
Imports AltUI.Config
Imports AltUI.Config.ThemeProvider

Public Class ModLoader
    Private modList As New List(Of ModInfo)
    Private bArray As String()

    Public Sub LoadMods()
        Dim directory As New DirectoryInfo("Mods")
        For Each file In directory.GetFiles("*.zip")
            Dim zip As ZipArchive = ZipFile.OpenRead(file.FullName)
            For Each entry As ZipArchiveEntry In zip.Entries
                If entry.Name = "mod.info" Then
                    Using reader As New StreamReader(entry.Open)
                        Dim iniData = reader.ReadToEnd().Split(vbCrLf)
                        Dim mname = iniData.Ini("Info", "Name")
                        Dim description = iniData.Ini("Info", "Description", KeyType.Multiline)
                        Dim version = iniData.Ini("Info", "Version")
                        Dim entries = (From ent In zip.Entries
                                Where ent.Name.ToLower() <> "mod.info"
                                Select ent.Name).ToList()
                        modList.Add(New ModInfo(mname, description, version, New FileInfo(file.FullName), entries))
                    End Using
                    Exit For
                End If
            Next
        Next

        For Each m In modList
            ListView1.Items.Add(m.ToListViewItem())
        Next
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DarkRichTextBox1.SelectionBackColor = BackgroundColour
        ListView1.Scrollable = False
        ListView1.Items.Clear()
        DarkRichTextBox1.BackColor = BackgroundColour
        Timer1.Start()
        ListView1.BackColor = BackgroundColour
        ListView1.ForeColor = Theme.Colors.LightText
        ListView1.AllowColumnReorder = false

        Dim files As String() = Directory.GetFiles("Mods")
        Dim directories As String() = Directory.GetDirectories("Mods")

        LoadMods()
    End Sub

    Private Class ModInfo
        Property Name As String
        Property Conflict As ConflictStatus
        Property Priority As Integer
        Property Description As String
        Property Version As String
        Property Zip As FileInfo
        Property Entries As New List(Of String)

        Sub New(Name As String, Description As String, Version As String, Zip As FileInfo, Entries As List(Of String))
            Me.Name = Name
            Conflict = ConflictStatus.Clear
            Priority = 0
            Me.Zip = Zip
            Me.Description = Description
            Me.Version = Version
            Me.Entries = Entries
        End Sub

        Function ToListViewItem() As ListViewItem
            Dim out = New ListViewItem(Name)
            out.SubItems.Add(Conflict.ToString())
            out.SubItems.Add(Priority)
            Return out
        End Function
    End Class

    Private Enum ConflictStatus
        Clear
        Overwrite
        Overwritten
        Mixed
        Redundant
    End Enum

#Region "Reordering"

    Private Sub ListView1_ItemDrag(sender As Object, e As ItemDragEventArgs) Handles ListView1.ItemDrag
        ListView1.DoDragDrop(e.Item, DragDropEffects.Move)
    End Sub

    Private Shared Sub ListView1_DragEnter(sender As Object, e As DragEventArgs) Handles ListView1.DragEnter
        e.Effect = e.AllowedEffect
    End Sub

    Private Sub ListView1_DragLeave(sender As Object, e As EventArgs) Handles ListView1.DragLeave
        ListView1.InsertionMark.Index = - 1
    End Sub

    Private Sub ListView1_DragOver(sender As Object, e As DragEventArgs) Handles ListView1.DragOver
        Dim targetPoint As Point = ListView1.PointToClient(New Point(e.X, e.Y))
        Dim targetIndex As Integer = ListView1.InsertionMark.NearestIndex(targetPoint)
        If targetIndex > - 1 Then
            Dim itemBounds As Rectangle = ListView1.GetItemRect(targetIndex)
            If targetPoint.Y > itemBounds.Top + (itemBounds.Height\2) Then
                ListView1.InsertionMark.AppearsAfterItem = True
            Else
                ListView1.InsertionMark.AppearsAfterItem = False
            End If
        End If
        ListView1.InsertionMark.Index = targetIndex
    End Sub

    Private Sub ListView1_DragDrop(sender As Object, e As DragEventArgs) Handles ListView1.DragDrop
        Dim targetIndex As Integer = ListView1.InsertionMark.Index
        If targetIndex = - 1 Then
            Return
        End If
        If ListView1.InsertionMark.AppearsAfterItem Then
            targetIndex += 1
        End If
        Dim draggedItem = DirectCast(e.Data.GetData(GetType(ListViewItem)), ListViewItem)

        Dim tmp = modList(ListView1.Items.IndexOf(draggedItem))
        modList.RemoveAt(ListView1.Items.IndexOf(draggedItem))
        If targetIndex > modList.Count - 1 Then
            modList.Add(tmp)
        Else
            modList.Insert(targetIndex, tmp)
        End If
        ListView1.Items.Insert(targetIndex, DirectCast(draggedItem.Clone(), ListViewItem))
        ListView1.Items.Remove(draggedItem)
        UpdateStatus(sender, e)
    End Sub

#End Region

#Region "Paint"

    Private Sub ListView1_DrawColumnHeader(sender As Object, e As DrawListViewColumnHeaderEventArgs) _
        Handles ListView1.DrawColumnHeader
        Using txtbrsh As New SolidBrush(Theme.Colors.LightText)
            e.Graphics.FillRectangle(New SolidBrush(BackgroundColour), e.Bounds)
            e.Graphics.DrawString(e.Header.Text, e.Font, txtbrsh,
                                  CInt(
                                      e.Bounds.Left + (e.Bounds.Width/2) -
                                      (e.Graphics.MeasureString(e.Header.Text, e.Font).Width/2)),
                                  e.Bounds.Top + 5)

            e.Graphics.DrawLine(New Pen(Theme.Colors.GreySelection, 1), e.Bounds.Left, e.Bounds.Top, e.Bounds.Right,
                                e.Bounds.Top)

            If e.Header.DisplayIndex = 1 Then
                e.Graphics.DrawLine(New Pen(Theme.Colors.GreySelection, 1), e.Bounds.Left, e.Bounds.Top + 3,
                                    e.Bounds.Left,
                                    e.Bounds.Bottom - 3)
                e.Graphics.DrawLine(New Pen(Theme.Colors.GreySelection, 1), e.Bounds.Right - 1, e.Bounds.Top + 3,
                                    e.Bounds.Right - 1, e.Bounds.Bottom - 3)
            End If
        End Using
    End Sub

    Private Sub ListView1_DrawSubItem(ByVal sender As Object, e As DrawListViewSubItemEventArgs) _
        Handles ListView1.DrawSubItem
        Using txtbrsh As New SolidBrush(Theme.Colors.LightText)
            If ListView1.SelectedIndices.Contains(e.ItemIndex) And ListView1.Focused Then
                e.Graphics.FillRectangle(New SolidBrush(Theme.Colors.BlueHighlight), e.Bounds)
            Else
                e.Graphics.FillRectangle(New SolidBrush(BackgroundColour), e.Bounds)
            End If
            If e.Item.SubItems(0) Is e.SubItem Then
                Dim g = e.Graphics
                g.Clip = New Region(New Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height))
                g.SmoothingMode = SmoothingMode.AntiAlias
                Dim rect = e.Bounds

                Dim size = Theme.Sizes.CheckBoxSize

                Dim textColor = Theme.Colors.LightText
                Dim borderColor = Theme.Colors.GreySelection
                Dim fillColor = If(e.Item.Checked, Theme.Colors.LightestBackground, Theme.Colors.GreyBackground)

                Using b = New SolidBrush(Theme.Colors.LightBackground)
                    Dim boxRect = New Rectangle(e.Bounds.Left + 2, e.Bounds.Top + (rect.Height\2) - (size/2), size, size)
                    g.FillRoundedRectangle(b, boxRect, 2)
                End Using

                Using p = New Pen(borderColor)
                    Dim boxRect = New Rectangle(e.Bounds.Left + 2, e.Bounds.Top + (rect.Height\2) - (size/2), size, size)
                    g.DrawRoundedRectangle(p, boxRect, 2)
                End Using

                If e.Item.Checked Then
                    Using p = New Pen(fillColor, 1)
                        g.DrawLine(p, e.Bounds.Left + 5, e.Bounds.Top + 9, e.Bounds.Left + 7, e.Bounds.Top + 12)
                        g.DrawLine(p, e.Bounds.Left + 7, e.Bounds.Top + 12, e.Bounds.Left + 11, e.Bounds.Top + 6)
                    End Using
                End If

                Using b = New SolidBrush(textColor)
                    g.DrawString(e.Item.Text, e.Item.Font, b, e.Bounds.Left + size + 4, e.Bounds.Top + 1)
                End Using
            ElseIf e.Item.SubItems(1) Is e.SubItem Then
                If e.Item.Checked Then
                    Using sf As New StringFormat _
                        With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Center,
                            .FormatFlags = StringFormatFlags.NoWrap, .Trimming = StringTrimming.EllipsisCharacter}
                        e.Graphics.DrawImage(
                            My.Resources.ResourceManager.GetObject($"conflict_{e.SubItem.Text.ToLower()}"),
                            e.Bounds.Left + (e.Bounds.Width/2.0F) - 8, e.Bounds.Top, 16, 16)
                    End Using
                End If
            ElseIf e.Item.SubItems(2) Is e.SubItem Then
                If e.Item.Checked Then
                    e.Graphics.DrawString(e.SubItem.Text, e.Item.Font, txtbrsh,
                                          CInt(e.Bounds.Left + (e.Bounds.Width/2)) -
                                          (e.Graphics.MeasureString(e.SubItem.Text, e.SubItem.Font).Width/2),
                                          e.Bounds.Top + 2)
                End If
            Else
                e.DrawDefault = True
            End If

        End Using
    End Sub

    Private Shared Sub ToolStripLabel_MouseEnter(sender As Object, e As EventArgs) _
        Handles ToolStripLabel2.MouseEnter, ToolStripLabel3.MouseEnter
        sender.ForeColor = Theme.Colors.BlueHighlight
    End Sub

    Private Shared Sub ToolStripLabel_MouseLeave(sender As Object, e As EventArgs) _
        Handles ToolStripLabel2.MouseLeave, ToolStripLabel3.MouseLeave
        sender.ForeColor = Theme.Colors.LightText
    End Sub

    Private Shared Sub TableLayoutPanel1_Paint(sender As Object, e As PaintEventArgs) Handles TableLayoutPanel1.Paint
        e.Graphics.DrawLine(New Pen(Theme.Colors.GreySelection, 1), 0, 0, sender.Width, 0)
        e.Graphics.DrawLine(New Pen(Theme.Colors.GreySelection, 1), 0, 0, 0, sender.Height)
    End Sub

    Private Shared Sub DarkToolStrip2_Paint(sender As Object, e As PaintEventArgs) Handles DarkToolStrip2.Paint
        e.Graphics.DrawLine(New Pen(Theme.Colors.GreySelection, 1), 0, 0, sender.Width, 0)
    End Sub

    Public Class DoubleBufferedListView
        Inherits ListView

        Public Sub New()
            Me.DoubleBuffered = True
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.EnableNotifyMessage, True)
        End Sub

        Protected Overrides Sub OnNotifyMessage(m As Message)
            If m.Msg <> &H14 Then
                MyBase.OnNotifyMessage(m)
            End If
        End Sub

        Protected Overrides Sub OnHandleCreated(e As EventArgs)
            MyBase.OnHandleCreated(e)
            Application.VisualStyleState =
                System.Windows.Forms.VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled
        End Sub
    End Class

#End Region

    Private Sub SizeAdjust()
        Dim sz As Integer = ListView1.Width/4
        ListView1.Columns(0).Width = sz*2
        ListView1.Columns(1).Width = sz
        ListView1.Columns(2).Width = sz + (ListView1.Width - sz*4)
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ScrollControlIntoView(ListView1)
        SizeAdjust()
    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        If ListView1.SelectedItems.Count = 0 Then
            Return
        End If

        Dim selectedMod As ModInfo = modList(ListView1.SelectedItems(0).Index)
        DarkRichTextBox1.Text = selectedMod.Description
        DarkLabel1.Text = selectedMod.Name
        DarkLabel3.Text = "Version: " & selectedMod.Version
    End Sub

    Private Sub UpdateStatus(sender As Object, e As EventArgs) Handles ListView1.ItemChecked
        For Each i As Integer In ListView1.CheckedIndices
            Dim lpe As New List(Of String)
            Dim hpe As New List(Of String)
            ' get all entries of checked mods
            Try
                For Each j In ListView1.CheckedIndices
                    If j < i Then
                        lpe.AddRange(modList(j).Entries)
                    ElseIf j > i Then
                        hpe.AddRange(modList(j).Entries)
                    End If
                Next
            Catch
                Exit Sub
            End Try

            Dim cfs
            Dim overwrites =
                    modList(i).Entries.Any(
                        Function(s) s.Equals("English.stf", StringComparison.OrdinalIgnoreCase) AndAlso lpe.Contains(s))
            Dim overwritten =
                    modList(i).Entries.Any(
                        Function(s) s.Equals("English.stf", StringComparison.OrdinalIgnoreCase) AndAlso hpe.Contains(s))
            If overwrites AndAlso overwritten Then
                cfs = ConflictStatus.Mixed
                modList(i).Conflict = cfs
            ElseIf overwrites Then
                cfs = ConflictStatus.Overwrite
                modList(i).Conflict = cfs
            ElseIf overwritten Then
                cfs = ConflictStatus.Overwritten
                modList(i).Conflict = cfs
            Else
                cfs = ConflictStatus.Clear
                modList(i).Conflict = cfs
            End If
            Dim redundant =
                    modList(i).Entries.Where(Function(s) s.Equals("English.stf", StringComparison.OrdinalIgnoreCase)).
                    All(Function(s) hpe.Contains(s))
            If redundant Then
                cfs = ConflictStatus.Redundant
                modList(i).Conflict = cfs
            End If
            ListView1.Items(i).SubItems(1).Text = cfs.ToString()
            modList(i).Priority = ListView1.CheckedItems.IndexOf(ListView1.Items(i))
            ListView1.Items(i).SubItems(2).Text = ListView1.CheckedItems.IndexOf(ListView1.Items(i))
        Next
        ToolStripLabel1.Text = $"Active: {ListView1.CheckedItems.Count}"
    End Sub

    Private Sub EnableAllToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles EnableAllToolStripMenuItem.Click
        For Each itm As ListViewItem In ListView1.Items
            itm.Checked = True
        Next
    End Sub

    Private Sub DisableAllToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles DisableAllToolStripMenuItem.Click
        For Each itm As ListViewItem In ListView1.Items
            itm.Checked = False
        Next
    End Sub

    Private Sub ToolStripLabel3_Click(sender As Object, e As EventArgs) Handles ToolStripLabel3.Click
        Close()
    End Sub

    Public Function MergeSTF(lArray As String(), hArray As String()) As ValueTuple(Of String(), Integer)
        Dim oArray = New String(3281) {}
        Dim additionalStrings = 0

        ' Copy low-priority array's Vanilla string section into output, to preserve any changes.
        Array.Copy(lArray, oArray, 3281)

        ' If high-priority array has modified Vanilla strings, overwrite that string.
        ' this will effectively merge changes from both low and high, prioritizing high.
        For i = 0 To 3281
            If hArray(i) <> bArray(i) Then
                oArray(i) = hArray(i)
            End If
        Next

        ' if low priority array has additional strings, append those to the Vanilla segment, and save that value
        ' this value will be used to shift string references to non-vanilla strings.
        If lArray.Length > 3281 Then
            additionalStrings = lArray.Length - 3281
            ReDim Preserve oArray(oArray.Length + additionalStrings)
            Array.Copy(lArray, 3281, oArray, 3281, additionalStrings)
        End If

        ' if high-priority array has additional strings, append those to the end. this allows both high and low priority arrays to add to the base.
        ' no value needs to be stored. this comparison will happen again for the next array using this output as the low priority.
        ' this way, it can go through the list and update the files for each mod consecutively.
        If hArray.Length > 3281 Then
            ReDim Preserve oArray(oArray.Length + (hArray.Length - 3281))
            Array.Copy(hArray, 3281, oArray, 3281 + additionalStrings, hArray.Length - 3281)
        End If

        ' returns the string array for future use, and the # of strings between the vanilla and high-priority new.
        ' this value is used to shift string references in the high priority mod's files.
        Return (oArray, additionalStrings)
    End Function

    Private Sub IncSTFRefs(fp As String, incVal As Integer)

        Dim b = IO.File.ReadAllBytes(fp)

        Dim file As Object

        Select Case fp.GetExtension().ToLower()
            Case ".amo"
                'file = b.ReadAMO()
            Case ".arm"
                'file = b.ReadARM()
            Case ".con"
                'file = b.ReadCON()
            Case ".crt"
                file = b.ReadCRT()
            Case ".dor"
                'file = b.ReadDOR()
            Case ".itm"
                file = b.ReadITM()
            Case ".use"
                'file = b.ReadUSE()
            Case ".wea"
                'file = b.ReadWEA()
        End Select

        If file Is Nothing Then Exit Sub

        If file.GENT.HoverSR > 3281 Then file.GENT.HoverSR += incVal
        If file.GENT.LookSR > 3281 Then file.GENT.LookSR += incVal
        If file.GENT.NameSR > 3281 Then file.GENT.NameSR += incVal
        If file.GENT.UnkwnSR > 3281 Then file.GENT.UnkwnSR += incVal

        IO.File.WriteAllBytes(fp, file.ToByte())
    End Sub

    Private Sub ToolStripLabel2_Click(sender As Object, e As EventArgs) Handles ToolStripLabel2.Click
        bArray = STFToTXT(IO.File.ReadAllBytes(My.Settings.STFDir))
        Dim tmpArray = bArray
        For Each i In ListView1.CheckedIndices
            If modList(i).Entries.Any(Function(x) x.Equals("English.stf", StringComparison.OrdinalIgnoreCase)) Then
                Dim zip As ZipArchive = ZipFile.OpenRead(modList(i).Zip.FullName)
                For Each entry As ZipArchiveEntry In zip.Entries
                    If entry.Name.ToLower() = "english.stf" Then
                        Using memoryStream As New MemoryStream()
                            entry.Open().CopyTo(memoryStream)
                            Dim stfData = memoryStream.ToArray()
                            Dim stfArray = STFToTXT(stfData)

                            Dim tmp = MergeSTF(tmpArray, stfArray)
                            tmpArray = tmp.Item1

                            Dim tmpDir = "Mods\tmp"
                            Directory.CreateDirectory(tmpDir)
                            For Each entry2 As ZipArchiveEntry In zip.Entries
                                If _
                                    Not entry2.Name.ToLower() = "english.stf" AndAlso
                                    Not entry2.Name.ToLower() = "mod.info" Then
                                    entry2.ExtractToFile(Path.Combine(tmpDir, entry2.Name), True)
                                End If
                            Next
                            For Each f In Directory.GetFiles(tmpDir)
                                Dim fi = New FileInfo(f)
                                IncSTFRefs(f, tmp.Item2)
                                File.Move(f, $"Overrides\Deployed\{fi.Name}", True)
                            Next
                            Exit For
                        End Using
                    End If
                Next
            End If
        Next
    End Sub
End Class