Public Class Form1
#Region " Move Form "
    Public MoveForm As Boolean
    Public MoveForm_MousePosition As Point
    Public Sub MoveForm_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown, PictureBox1.MouseDown, PictureBox2.MouseDown, PictureBox3.MouseDown
        If e.Button = MouseButtons.Left Then
            MoveForm = True
            MoveForm_MousePosition = e.Location
        End If
    End Sub
    Public Sub MoveForm_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove, PictureBox1.MouseMove, PictureBox2.MouseMove, PictureBox3.MouseMove
        If MoveForm Then
            Me.Location = Me.Location + (e.Location - MoveForm_MousePosition)
        End If
    End Sub
    Public Sub MoveForm_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp, PictureBox1.MouseUp, PictureBox2.MouseUp, PictureBox3.MouseUp
        If e.Button = MouseButtons.Left Then
            MoveForm = False
        End If
    End Sub
#End Region
    Private Sub Warning() Handles MyBase.VisibleChanged
        If Not IO.File.Exists(Application.StartupPath & "\F3.exe") Then
            MsgBox("Please put the launcher in the same directory as the game. Some things wont work!", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "Game Executable Not Found!")
        End If
    End Sub
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        If Not IO.File.Exists(Application.StartupPath & "\F3.exe") Then
            MsgBox("Please put the launcher in the same directory as the game!", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "Game Executable Not Found!")
        Else
            MoveForm = False
            Process.Start("F3.exe")
            Application.Exit()
        End If
    End Sub
    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        If Not IO.File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini") Then
            MsgBox("Please launch the game before trying to change options!", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "Game Settings .ini Not Found!")
        Else
            MoveForm = False
            Form2.ShowDialog()
        End If
    End Sub
    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        MoveForm = False
        Application.Exit()
    End Sub
End Class
