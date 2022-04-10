Imports System.IO
Imports System.Runtime.InteropServices

Public Class IniManager
    Public Shared F3Dir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\F3\F3.ini"
    Public Shared SysDir As String = "Override\MenuMap\Engine\sys.ini"
    Public Shared F3Ini() As String
    Public Shared SysIni() As String
    Private Shared CheckFor As String
    Public Shared Function Ini(IniArray As String(), IniSection As String, IniKey As String, Optional Value As String = Nothing)
        ' Find bounds of section
        CheckFor = "[" & IniSection & "]"
        Dim SectionStart = Array.FindIndex(IniArray, AddressOf StartsWith)
        CheckFor = "["
        Dim SectionEnd = Array.FindIndex(IniArray, SectionStart + 1, AddressOf StartsWith) - 1
        If SectionEnd < 0 Then
            SectionEnd = IniArray.Length - 1
        End If
        ' Find Key
        CheckFor = IniKey
        Dim KeyIndex = Array.FindIndex(IniArray, SectionStart + 1, AddressOf StartsWith)
        Dim KeyLine = IniArray(KeyIndex)
        Dim KeyValue = KeyLine.Substring(KeyLine.IndexOf("=") + 1).Trim
        If KeyIndex > SectionEnd Then Return Nothing
        ' Find comment, remove comment from value if present
        Dim Comment = Nothing
        Try : Comment = KeyValue.Substring(KeyValue.IndexOf(";")) : Catch : End Try
        If Comment IsNot Nothing Then KeyValue = KeyValue.Substring(0, KeyValue.LastIndexOf(";")).Trim
        ' Either apply changes or return value
        If Value IsNot Nothing Then
            IniArray(KeyIndex) = Trim(IniKey & " = " & Value & " " & Comment)
            Return Nothing
        Else
            Return KeyValue
        End If
    End Function

    Private Shared Function StartsWith(Item As String)
        Return Item.StartsWith(CheckFor)
    End Function

End Class

Public Class General
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

    Public Shared Function GetSizesAsStrings() As String()
        Dim SizeList As List(Of String) = (From S In GetSizes() Select S.Width.ToString & "x" & S.Height.ToString).ToList()
        Return SizeList.ToArray
    End Function

    Public Shared Function GetRefreshRate() As Double
        Dim DM As New DevModeW
        Dim Hz = 0
        Dim Index = 0
        DM.dmSize = CUShort(Marshal.SizeOf(GetType(DevModeW)))
        While EnumDisplaySettingsExW(Screen.PrimaryScreen.DeviceName, Index, DM, 0)
            If DM.dmDisplayFrequency > Hz Then Hz = DM.dmDisplayFrequency
            Index += 1
        End While
        Return Hz
    End Function

    Public Shared Function GetSizes() As Size()
        Dim DM As New DevModeW
        Dim Index = 0
        Dim SizeList As New List(Of Size)
        DM.dmSize = CUShort(Marshal.SizeOf(GetType(DevModeW)))
        While EnumDisplaySettingsExW(Screen.PrimaryScreen.DeviceName, Index, DM, 0)
            Dim Size As New Size(CInt(DM.dmPelsWidth), CInt(DM.dmPelsHeight))
            If Not SizeList.Contains(Size) Then SizeList.Add(Size)
            Index += 1
        End While
        Return SizeList.ToArray
    End Function

    Public Shared Function StrToSize(str As String) As Size
        Try
            Dim a = str.Split(New Char() {"x"})
            Return New Size() With {
            .Width = Integer.Parse(a(0)),
            .Height = Integer.Parse(a(1))
        }
        Catch
            Return New Size() With {
            .Width = 0,
            .Height = 0
        }
        End Try
    End Function

    Public Shared Function SizeToStr(size As Size, Optional mult As Integer = 1) As String
        Return size.Width * mult & "x" & size.Height * mult
    End Function

End Class
