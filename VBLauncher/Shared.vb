Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Public Module IniManager
    Public F3Dir As String = $"{My.Computer.FileSystem.SpecialDirectories.MyDocuments}\F3\F3.ini"
    Public SysDir As String = "Override\MenuMap\Engine\sys.ini"
    Public F3Ini() As String
    Public SysIni() As String

    Public Enum KeyType
        Normal
        Multiline
    End Enum

    <Extension>
    Public Function Ini(ByRef IniArray As String(), IniSection As String, IniKey As String, Optional KeyType As KeyType = KeyType.Normal) As String
        ' Find bounds of section
        Dim SectionStart = Array.FindIndex(IniArray, Function(x) x.StartsWith($"[{IniSection}]"))
        Dim SectionEnd = Array.FindIndex(IniArray, SectionStart + 1, Function(x) x.StartsWith("[")) - 1
        If SectionEnd = -1 Then SectionEnd = IniArray.Length - 1

        If KeyType = KeyType.Normal Then
            ' Find key location and value
            Dim KeyIndex = Array.FindIndex(IniArray, SectionStart + 1, Function(x) x.StartsWith(IniKey))
            If KeyIndex > SectionEnd Then Return Nothing
            Dim KeyLine = IniArray(KeyIndex)
            Dim KeyValue = KeyLine.Substring(KeyLine.IndexOf("=", StringComparison.Ordinal) + 1).Trim
            ' Find comment, if any
            Dim Comment = Nothing
            Try : Comment = KeyValue.Substring(KeyValue.IndexOf(";", StringComparison.Ordinal)) : Catch : End Try
            If Comment IsNot Nothing Then KeyValue = KeyValue.Substring(0, KeyValue.LastIndexOf(";", StringComparison.Ordinal)).Trim
            ' Return read value
            Return KeyValue
        Else
            ' Multiline key
            Dim KeyStart = Array.FindIndex(IniArray, SectionStart + 1, Function(x) x.StartsWith($"<{IniKey}>"))
            Dim KeyEnd = Array.FindIndex(IniArray, KeyStart + 1, Function(x) x.StartsWith($"</{IniKey}>"))
            If KeyEnd < 0 Then KeyEnd = IniArray.Length - 1
            ' Get key value
            Dim KeyValue = String.Join(Environment.NewLine, IniArray, KeyStart + 1, KeyEnd - KeyStart - 1)
            ' Return read value
            Return KeyValue
        End If
    End Function

    <Extension>
    Public Sub Ini(ByRef IniArray As String(), IniSection As String, IniKey As String, Value As String, Optional KeyType As KeyType = KeyType.Normal)
        ' Find bounds of section
        Dim SectionStart = Array.FindIndex(IniArray, Function(x) x.StartsWith($"[{IniSection}]"))
        Dim SectionEnd = Array.FindIndex(IniArray, SectionStart + 1, Function(x) x.StartsWith("[")) - 1
        If SectionEnd < 0 Then SectionEnd = IniArray.Length - 1

        If KeyType = KeyType.Normal Then
            ' Find key location and value
            Dim KeyIndex = Array.FindIndex(IniArray, SectionStart + 1, Function(x) x.StartsWith(IniKey))
            If KeyIndex > SectionEnd Then Exit Sub
            Dim KeyLine = IniArray(KeyIndex)
            Dim KeyValue = KeyLine.Substring(KeyLine.IndexOf("=") + 1).Trim
            ' Find comment, if any
            Dim Comment = Nothing
            Try : Comment = KeyValue.Substring(KeyValue.IndexOf(";")) : Catch : End Try
            'Set to new value
            IniArray(KeyIndex) = Trim($"{IniKey} = {Value} {Comment}")
        Else
            Dim startIndex = Array.FindIndex(IniArray, SectionStart + 1, Function(x) x.StartsWith($"<{IniKey}>"))
            Dim endIndex = Array.FindIndex(IniArray, startIndex + 1, Function(x) x.StartsWith($"</{IniKey}>"))

            If startIndex < SectionEnd And endIndex > SectionEnd Then
                Dim keyValueStart = startIndex + 1
                Dim keyValueEnd = endIndex - 1

                Dim newValues As String()
                ReDim newValues(SectionEnd - SectionStart + 1 + keyValueEnd - keyValueStart + 2)
                Array.Copy(IniArray, SectionStart, newValues, 0, SectionEnd - SectionStart + 1)
                newValues(SectionEnd - SectionStart + 1) = $"<{IniKey}>"
                Array.Copy(Value.Split(Environment.NewLine), 0, newValues, SectionEnd - SectionStart + 2,
                           keyValueEnd - keyValueStart + 1)
                newValues(newValues.Length - 1) = $"</{IniKey}>"
                IniArray = newValues
            End If
        End If
    End Sub


    <Extension>
    Public Function GetSection(IniArray As String(), IniSection As String) As String()
        ' Find bounds of section
        Dim SectionStart = Array.FindIndex(IniArray, Function(x) x.StartsWith($"[{IniSection}]"))
        Dim SectionEnd = Array.FindIndex(IniArray, SectionStart + 1, Function(x) x.StartsWith("[")) - 1
        If SectionEnd < 0 Then SectionEnd = IniArray.Length - 1
        ' Return section
        Return IniArray.Skip(SectionStart + 1).Take(SectionEnd - SectionStart).ToArray
    End Function

End Module

Public Module General

    Public Function SearchForFiles(RootFolder As String, FileFilter() As String) As List(Of String)
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
            Catch
            End Try
        Loop
        Return ReturnedData
    End Function

    <Extension()>
    Public Sub RemoveAt(Of T)(ByRef arr As T(), ByVal index As Integer)
        'create an array 1 element less than the input array
        Dim outArr(arr.Length - 1) As T
        'copy the first part of the input array
        Array.Copy(arr, 0, outArr, 0, index)
        'then copy the second part of the input array
        Array.Copy(arr, index + 1, outArr, index, arr.Length - 1 - index)

        arr = outArr
    End Sub

    <Extension()>
    Public Sub InsertAt(Of T)(
          ByRef sourceArray() As T,
          ByVal insertIndex As Integer,
          ByVal newValue As T)

        Dim newPosition As Integer
        Dim counter As Integer

        newPosition = insertIndex
        If (newPosition < 0) Then newPosition = 0
        If (newPosition > sourceArray.Length) Then _
           newPosition = sourceArray.Length

        Array.Resize(sourceArray, sourceArray.Length + 1)

        For counter = sourceArray.Length - 2 To newPosition Step -1
            sourceArray(counter + 1) = sourceArray(counter)
        Next counter

        sourceArray(newPosition) = newValue
    End Sub

    <Extension>
    Function RemoveExtension(fileName As String) As String
        Dim lastDotIndex As Integer = fileName.LastIndexOf(".")
        If lastDotIndex = -1 Then
            Return fileName
        Else
            Return fileName.Substring(0, lastDotIndex)
        End If
    End Function

    <Extension>
    Function GetExtension(fileName As String) As String
        Dim lastDotIndex As Integer = fileName.LastIndexOf(".")
        If lastDotIndex = -1 Then
            Return ""
        Else
            Return fileName.Substring(lastDotIndex + 1)
        End If
    End Function


End Module

Public Class VideoInfo

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure DevModeW
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)> Private ReadOnly dmDeviceName As String
        Private ReadOnly dmSpecVersion As UShort
        Private ReadOnly dmDriverVersion As UShort
        Public dmSize As UShort
        Private ReadOnly dmDriverExtra As UShort
        Private ReadOnly dmFields As UInteger
        Private ReadOnly Union1 As Struct1
        Private ReadOnly dmColor As Short
        Private ReadOnly dmDuplex As Short
        Private ReadOnly dmYResolution As Short
        Private ReadOnly dmTTOption As Short
        Private ReadOnly dmCollate As Short
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)> Private ReadOnly dmFormName As String
        Private ReadOnly dmLogPixels As UShort
        Private ReadOnly dmBitsPerPel As UInteger
        Public ReadOnly dmPelsWidth As UInteger
        Public ReadOnly dmPelsHeight As UInteger
        Private ReadOnly Union2 As DFN
        Public ReadOnly dmDisplayFrequency As UInteger
    End Structure

    <StructLayout(LayoutKind.Explicit)>
    Private Structure Struct1
        <FieldOffset(0)> Private ReadOnly Struct1 As PDODFO
    End Structure

    <StructLayout(LayoutKind.Explicit)>
    Private Structure DFN
        <FieldOffset(0)> Private ReadOnly dmDisplayFlags As UInteger
        <FieldOffset(0)> Private ReadOnly dmNup As UInteger
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure PDODFO
        Private ReadOnly dmPosition As PointL
        Private ReadOnly dmDisplayOrientation As UInteger
        Private ReadOnly dmDisplayFixedOutput As UInteger
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure PointL
        Private ReadOnly x As Integer
        Private ReadOnly y As Integer
    End Structure

    <DllImport("user32.dll", EntryPoint:="EnumDisplaySettingsExW")>
    Private Shared Function EnumDisplaySettingsExW(<MarshalAs(UnmanagedType.LPWStr)> DeviceName As String, ModeNum As Integer, ByRef DevMode As DevModeW, Flags As UInteger) As Boolean
    End Function

    Public Shared Function GetResAsStrings() As String()
        Return (From S In GetResolution() Select $"{S.Width}x{S.Height}@{S.Hz}").ToArray()
    End Function

    Public Shared Function GetResolution() As Resolution()
        Dim DM As New DevModeW
        Dim Index = 0
        Dim SizeList As New List(Of Resolution)
        DM.dmSize = CUShort(Marshal.SizeOf(GetType(DevModeW)))
        While EnumDisplaySettingsExW(Screen.PrimaryScreen.DeviceName, Index, DM, 0)
            Dim Resolution As New Resolution With
            {.Width = DM.dmPelsWidth, .Height = DM.dmPelsHeight, .Hz = DM.dmDisplayFrequency}
            If Not SizeList.Contains(Resolution) Then
                Try : If SizeList.Last.Width = Resolution.Width Then SizeList.Remove(SizeList.Last)
                Catch : End Try
                SizeList.Add(Resolution)
            End If
            Index += 1
        End While
        Return SizeList.ToArray
    End Function

    Public Shared Function StrToRes(str As String) As Resolution
        Try
            Dim a = str.Split(New Char() {"x"})
            Dim b = a(1).Split(New Char() {"@"})
            Return New Resolution() With {
            .Width = a(0),
            .Height = b(0),
            .Hz = b(1)
        }
        Catch
            Return New Resolution() With {
            .Width = 0,
            .Height = 0,
            .Hz = 0
        }
        End Try
    End Function

    Public Shared Function ResToStr(res As Resolution, Optional ShowHz As Boolean = True, Optional mult As Integer = 1) As String
        Return $"{res.Width * mult}x{res.Height * mult}{If(ShowHz, $"@{res.Hz}", "")}"
    End Function

End Class

Public Class Resolution
    Public Property Width As Integer
    Public Property Height As Integer
    Public Property Hz As Integer
End Class