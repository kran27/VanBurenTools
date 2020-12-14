Public Class Form1
    Public undir As String = Application.StartupPath & "\Override"
    Private Sub LoadExtras() Handles MyBase.VisibleChanged
        If Not IO.Directory.Exists(undir) Then
            IO.Directory.CreateDirectory(undir & "\_CRT")
            IO.Directory.CreateDirectory(undir & "\_WEA")
            IO.Directory.CreateDirectory(undir & "\Critters")
            IO.Directory.CreateDirectory(undir & "\Interface")
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersAnt.CRT", My.Resources.CrittersAnt)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersAntQ.CRT", My.Resources.CrittersAntQ)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersBat.CRT", My.Resources.CrittersBat)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersBeetle.CRT", My.Resources.CrittersBeetle)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersCentipede.CRT", My.Resources.CrittersCentipede)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersDeathclaw.CRT", My.Resources.CrittersDeathclaw)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersDesertStalker.CRT", My.Resources.CrittersDesertStalker)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersGila.CRT", My.Resources.CrittersGila)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersManTrap.CRT", My.Resources.CrittersManTrap)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersRadToad.CRT", My.Resources.CrittersRadToad)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersRat.CRT", My.Resources.CrittersRat)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersThornSlinger.CRT", My.Resources.CrittersThornSlinger)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersTiger.CRT", My.Resources.CrittersTiger)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersWaspGiant.CRT", My.Resources.CrittersWaspGiant)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersWeedling.CRT", My.Resources.CrittersWeedling)
            IO.File.WriteAllBytes(undir & "\_CRT\CrittersWolf.CRT", My.Resources.CrittersWolf)
            IO.File.WriteAllBytes(undir & "\_WEA\Weapons2mmGaussPistol.WEA", My.Resources.Weapons2mmGaussPistol)
            IO.File.WriteAllBytes(undir & "\_WEA\WeaponsLaserSaw.WEA", My.Resources.WeaponsLaserSaw)
            IO.File.WriteAllBytes(undir & "\_WEA\WeaponsSawedOffShotgun.WEA", My.Resources.WeaponsSawedOffShotgun)
            IO.File.WriteAllBytes(undir & "\Critters\CR_DesertStalk.B3D", My.Resources.CR_DesertStalk)
            IO.File.WriteAllBytes(undir & "\Interface\WP_2mmGaussPistol_ACT.TGA", My.Resources.WP_2mmGaussPistol_ACT)
            IO.File.WriteAllBytes(undir & "\Interface\WP_2mmGaussPistol_INV.TGA", My.Resources.WP_2mmGaussPistol_INV)
        End If
    End Sub
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
            MsgBox("Please put the launcher in the same directory as the game so you can launch it!", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "Game Executable Not Found!")
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
