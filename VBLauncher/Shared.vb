Imports System.IO
Public Class PShared
    Public Shared F3Dir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public Shared SysDir As String = "Override\MenuMap\Engine\sys.ini"
    Public Shared F3Ini() As String
    Public Shared SysIni() As String
    Private Shared CheckFor As String
    Public Shared Function Ini(IniArray As String(), IniSection As String, IniKey As String, Optional Value As String = Nothing)
        CheckFor = "[" & IniSection & "]"
        Dim SectionStart = Array.FindIndex(IniArray, AddressOf StartsWith)
        CheckFor = "["
        Dim SectionEnd = Array.FindIndex(IniArray, SectionStart + 1, AddressOf StartsWith) - 1
        If SectionEnd = -1 Then
            SectionEnd = IniArray.Length - 1
        End If
        CheckFor = IniKey
        Dim KeyIndex = Array.FindIndex(IniArray, SectionStart + 1, AddressOf StartsWith)
        If KeyIndex > SectionEnd Then
            Return Nothing
        End If
        If Value IsNot Nothing Then
            IniArray(Array.FindIndex(IniArray, SectionStart + 1, AddressOf StartsWith)) = IniKey & " = " & Value
            Return Nothing
        Else
            Return IniArray(Array.FindIndex(IniArray, SectionStart + 1, AddressOf StartsWith)).Remove(0, IniKey.Substring(IniKey.IndexOf("=") + 1).Length + 3)
        End If
    End Function

    Private Shared Function StartsWith(Item As String)
        Return Item.StartsWith(CheckFor)
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
