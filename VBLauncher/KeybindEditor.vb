Imports System.Data
Imports System.IO
Imports AltUI.Config
Imports AltUI.Forms

Public Class KeybindEditor
    Private Row As Integer
    Private Key As String
    Private Modifier As String
    Private WantKey As Boolean

    Private Sub Check(sender As Object, e As EventArgs) Handles MyBase.Load, DarkButton1.Click
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
        DataGridView1.BackgroundColor = ThemeProvider.BackgroundColour
        DataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        DataGridView1.DefaultCellStyle = New DataGridViewCellStyle With {.BackColor = ThemeProvider.BackgroundColour, .ForeColor = ThemeProvider.Theme.Colors.LightText, .SelectionBackColor = ThemeProvider.Theme.Colors.BlueSelection, .SelectionForeColor = ThemeProvider.Theme.Colors.LightText}
        DataGridView1.ColumnHeadersDefaultCellStyle = New DataGridViewCellStyle With {.BackColor = ThemeProvider.BackgroundColour, .ForeColor = ThemeProvider.Theme.Colors.LightText, .SelectionBackColor = ThemeProvider.BackgroundColour, .SelectionForeColor = ThemeProvider.Theme.Colors.LightText}
        DataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.DataSource = dt
        DataGridView1.Columns(0).ReadOnly = True
        DataGridView1.Columns(1).ReadOnly = True
        DataGridView1.Columns(3).ReadOnly = True
    End Sub

    Private Sub Apply(sender As Object, e As EventArgs) Handles Button1.Click
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

        File.WriteAllLines(F3Dir, F3Ini)

        DarkMessageBox.ShowMessage("Updated Keybinds", "Done")
        Close()
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

    Private Sub DataGridView1_ScrollChanged() Handles DataGridView1.Scroll
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