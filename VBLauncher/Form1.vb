Public Class Form1
    Public fixdir As String = Application.StartupPath & "\Override\Fixes"

    Public MoveForm As Boolean
    Public MoveForm_MousePosition As Point

    Public Sub MoveForm_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox4.MouseDown, PictureBox5.MouseDown
        If e.Button = MouseButtons.Left Then MoveForm = 1 : MoveForm_MousePosition = e.Location
    End Sub

    Public Sub MoveForm_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox4.MouseMove, PictureBox5.MouseMove
        If MoveForm Then Me.Location = Me.Location + (e.Location - MoveForm_MousePosition)
    End Sub

    Public Sub MoveForm_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox4.MouseUp, PictureBox5.MouseUp
        If e.Button = MouseButtons.Left Then MoveForm = 0
    End Sub

    Private Sub Startup() Handles MyBase.Load
        Dim random As New Random
        Dim rand As String = random.Next(1, 9)
        PictureBox4.Parent = PictureBox5
        Select Case rand
            Case 1
                PictureBox5.Image = My.Resources.BG1
            Case 2
                PictureBox5.Image = My.Resources.BG2
            Case 3
                PictureBox5.Image = My.Resources.BG3
            Case 4
                PictureBox5.Image = My.Resources.BG4
            Case 5
                PictureBox5.Image = My.Resources.BG5
            Case 6
                PictureBox5.Image = My.Resources.BG6
            Case 7
                PictureBox5.Image = My.Resources.BG7
            Case 8
                PictureBox5.Image = My.Resources.BG8
            Case 9
                PictureBox5.Image = My.Resources.BG9
        End Select
        If Not IO.File.Exists(Application.StartupPath & "\dgVoodoo.conf") Then IO.File.WriteAllBytes(Application.StartupPath & "\dgVoodoo.conf", My.Resources.dgV2conf)
        If IO.Directory.Exists(fixdir) Then IO.Directory.Delete(fixdir, 1)
        If Not IO.File.Exists(Application.StartupPath & "\F3.exe") Then
            MsgBox("Please put the launcher in the same directory as the game so you can launch it!", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "Game Executable Not Found!")
        Else
            IO.Directory.CreateDirectory(fixdir)
            IO.File.WriteAllBytes(fixdir & "\Fixes.zip", My.Resources.Fixes)
            IO.Compression.ZipFile.ExtractToDirectory(fixdir & "\Fixes.zip", fixdir)
        End If
        If IO.Directory.Exists(Application.StartupPath & "\Override\UnusedThings") Then IO.Directory.Delete(Application.StartupPath & "\Override\UnusedThings", 1)
        If IO.File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\Characters\None.CRT") Then IO.File.Delete(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\Characters\None.CRT")
        If IO.Directory.Exists(Application.StartupPath & "\Override\FemaleFix") Then IO.Directory.Delete(Application.StartupPath & "\Override\FemaleFix", 1)
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        If Not IO.File.Exists(Application.StartupPath & "\F3.exe") Then
            MsgBox("Please put the launcher in the same directory as the game!", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "Game Executable Not Found!")
        Else
            Process.Start("F3.exe")
            Application.Exit()
        End If
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        If Not IO.File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini") Then IO.File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini", My.Resources.Default_F3)
        Form2.ShowDialog()
    End Sub

    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        Application.Exit()
    End Sub

End Class