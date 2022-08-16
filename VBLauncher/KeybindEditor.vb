Imports System.Data
Imports VBLauncher.General
Public Class KeybindEditor
    Private Sub Check(sender As Object, e As EventArgs) Handles MyBase.Load
        DataGridView1.Dock = DockStyle.Fill
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

        dt.Columns.Add("Modifier", GetType(String))
        dt.Columns.Add("Key", GetType(String))
        dt.Columns.Add("Action", GetType(String))
        dt.Columns.Add("Key Up/Down", GetType(String))

        For Each bind In Binds
            dt.Rows.Add(bind.Modifier, bind.Key, bind.Action, If(bind.OnPress, "Down", "Up"))
            i += 1
        Next
        DataGridView1.BackgroundColor = AltUI.Config.ThemeProvider.BackgroundColour
        DataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        DataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        DataGridView1.AllowUserToResizeColumns = False
        DataGridView1.AllowUserToResizeRows = False
        DataGridView1.DefaultCellStyle = New DataGridViewCellStyle With {.BackColor = AltUI.Config.ThemeProvider.BackgroundColour, .ForeColor = AltUI.Config.ThemeProvider.Theme.Colors.LightText, .SelectionBackColor = AltUI.Config.ThemeProvider.Theme.Colors.BlueSelection, .SelectionForeColor = AltUI.Config.ThemeProvider.Theme.Colors.LightText}
        DataGridView1.RowHeadersVisible = False
        DataGridView1.DataSource = dt
    End Sub
    Private Sub Apply(sender As Object, e As EventArgs) Handles Button1.Click
        Dim NewBinds = New List(Of String)
        For Each row As DataGridViewRow In DataGridView1.Rows
            NewBinds.Add(RowToStr(row))
        Next

        Dim SectionStart = Array.FindIndex(F3Ini, Function(x) x.StartsWith($"[HotKeys]"))
        Dim SectionEnd = Array.FindIndex(F3Ini, SectionStart + 1, Function(x) x.StartsWith("[")) - 1
        If SectionEnd < 0 Then SectionEnd = F3Ini.Length - 1

        For i = SectionStart + 1 To SectionEnd - 1
            F3Ini.RemoveAt(SectionStart + 1)
        Next

        For i = 1 To NewBinds.Count - 1
            F3Ini.InsertAt(SectionStart + i, NewBinds(i - 1))
        Next

        IO.File.WriteAllLines(F3Dir, F3Ini)

        AltUI.Forms.DarkMessageBox.ShowMessage("Updated Keybinds", "Done")
        Close()
    End Sub
    Private Function RowToStr(row As DataGridViewRow) As String
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