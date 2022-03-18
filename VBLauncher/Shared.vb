Imports System.IO
Public Class PShared
    Public Shared F3Dir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public Shared SysDir As String = "Override\MenuMap\Engine\sys.ini"
    Public Shared F3Ini() As String
    Public Shared SysIni() As String
    Public Shared CurrentKey As String
    Public Shared Function Ini(File As String(), IniKey As String, Optional Value As String = Nothing)
        CurrentKey = IniKey
        If Value IsNot Nothing Then
            File(Array.FindIndex(File, AddressOf KeyName)) = IniKey & " = " & Value
            Return Nothing
        Else
            Return File(Array.FindIndex(File, AddressOf KeyName)).Remove(0, IniKey.Length + 3)
        End If
    End Function

    Public Shared Function KeyName(s As String) As Boolean
        Return s.StartsWith(CurrentKey)
    End Function

    Public Shared Function SearchForFiles(RootFolder As String, FileFilter() As String) As List(Of String)
        Dim ReturnedData As New List(Of String)
        Dim FolderStack As New Stack(Of String)
        FolderStack.Push(RootFolder)
        Do While FolderStack.Count > 0
            Dim ThisFolder As String = FolderStack.Pop
            Try
                For Each SubFolder In Directory.GetDirectories(ThisFolder)
                    FolderStack.Push(SubFolder)
                Next
                For Each FileExt In FileFilter
                    ReturnedData.AddRange(Directory.GetFiles(ThisFolder, FileExt))
                Next
            Catch Ex As Exception
            End Try
        Loop
        Return ReturnedData
    End Function

End Class
