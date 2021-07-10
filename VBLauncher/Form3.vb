Public Class Form3
    Public ifdir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public line() As String
    Public content As String
    Private Sub ReadSettings() Handles MyBase.VisibleChanged
        line = IO.File.ReadAllLines(ifdir)
        TextBox1.Text = line(38)
    End Sub
    Private Sub ApplyChanges() Handles Button1.Click
        line = IO.File.ReadAllLines(ifdir)
        content = IO.File.ReadAllText(ifdir)
        content = content.Replace(line(38), TextBox1.Text)
        IO.File.WriteAllText(ifdir, content)
    End Sub
End Class