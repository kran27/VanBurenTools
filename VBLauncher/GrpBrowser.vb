Imports System.IO
Imports System.Text
Imports AltUI.Controls

Public Class GrpBrowser

    Public Property FileBytes As Byte()
    Public Property Extension As String
    Public Property FileName As String

    Private filter As String() =
            {"psf", "int", "rle", "itm", "veg", "crt", "map", "wav", "amx", "rtd", "rlz", "ini", "skl", "gr2", "skn",
             "8", "dlg", "str", "sst", "use", "arm", "dor", "wea", "pce", "sco", "gsf", "gls", "fnt", "spl", "pro",
             "enc", "wmp", "amo", "con", "tok", "b3d", "g3d", "tga", "bmp"}

    Public Sub New(filter As String())
        If filter IsNot Nothing AndAlso Not filter.Count = 0 Then Me.filter = filter
    End Sub

    Friend Structure F3RHTHeader
        Public vMajor As Integer
        Public vMinor As Integer
        Public nEntries As Integer 'number of file entries
        Public offsetPacks As Integer 'offset to PACK file data
        Public offsetResources As Integer 'offset to file name strings
    End Structure

    Friend Structure Entry
        Public one As Integer 'always 1 ????
        Public number As Integer 'number of file in PACK file
        Public type As Integer 'type of file as defined in ftype enum
        Public offset As Integer 'string offset (offsetResources+offset)
        Public pack As Integer
    End Structure

    Friend Structure F3GRPHeader
        Public vMajor As Integer
        Public vMinor As Integer
        Public nFiles As Integer 'number of files in PACK
    End Structure

    Friend Structure Lump
        Public offset As Integer
        Public lenght As Integer
    End Structure

    Private head As F3RHTHeader
    Private entries As List(Of Entry)()
    Private grpnames As List(Of String)
    Private filenames As List(Of String)
    Private globalIndexToPackOffset As New Dictionary(Of Integer, Integer)
    Private localIndexToGlobal As Dictionary(Of Integer, Integer)()
    Private globalIndexToLocal As Dictionary(Of Integer, Integer)
    Private extensions As List(Of String)

    'TODO: Automate this for proper support instead of hardcoded to this rht
    Private packOffsetToIndex As New Dictionary(Of Integer, Integer) From {
        {0, 0}, {6, 1}, {16, 2}, {25, 3}, {32, 4}, {42, 5}, {50, 6}, {55, 7}, {63, 8}, {68, 9}, {74, 10}, {81, 11},
        {87, 12}, {92, 13}, {97, 14}, {102, 15}, {107, 16}, {112, 17}, {117, 18}, {122, 19}, {127, 20},
        {132, 21}, {137, 22}, {142, 23}
    }

    Private Function GetExtension(ByVal a1 As Integer) As String
        Dim extensionMap = New Dictionary(Of Integer, String) From {
            {1900, "psf"}, {1600, "int"}, {1700, "rle"}, {1800, "itm"}, {1500, "veg"}, {1400, "crt"}, {1300, "map"},
            {1100, "wav"}, {1200, "amx"}, {1000, "rtd"}, {900, "rlz"}, {800, "ini"}, {600, "skl"}, {700, "gr2"},
            {500, "skn"}, {400, "8"}, {300, "dlg"}, {100, "model"}, {200, "image"}, {2900, "str"}, {2800, "sst"},
            {2700, "use"}, {2500, "arm"}, {2600, "dor"}, {2400, "wea"}, {2300, "pce"}, {2200, "sco"}, {2000, "gsf"},
            {2100, "gls"}, {3400, "fnt"}, {3300, "spl"}, {3200, "pro"}, {3000, "enc"}, {3100, "wmp"}, {3500, "amo"},
            {3600, "con"}, {3700, "tok"}
        }

        If extensionMap.ContainsKey(a1) Then
            Return extensionMap(a1)
        Else
            Return Nothing
        End If
    End Function

    Public Sub extractFile(index As Integer)
        Dim b As Byte()
        If index = -1 then
            For i = 0 To head.nEntries - 1
                b = getFileBytes(i)
                File.WriteAllBytes(grpnames(packOffsetToIndex(globalIndexToPackOffset(i))) & "\" & filenames(i), b)
            Next
        Else
            b = getFileBytes(index)
            File.WriteAllBytes(grpnames(packOffsetToIndex(globalIndexToPackOffset(index))) & "\" & filenames(index), b)
        End if
    End Sub

    Private Function getFileBytes(index As Integer) As Byte()
        Dim packLoc = globalIndexToPackOffset(index)
        Dim localIndex = globalIndexToLocal(index)
        Directory.CreateDirectory(grpnames(packOffsetToIndex(packLoc)))
        Dim grpname = $"data\{grpnames(packOffsetToIndex(packLoc))}.grp"
        Dim extname = $"{grpnames(packOffsetToIndex(packLoc))}\{filenames(index)}"
        Return getFileBytes(grpname, entries(packOffsetToIndex(packLoc))(localIndex).number,
                    entries(packOffsetToIndex(packLoc))(localIndex).type, extname)
    End Function

    Private Function getFileBytes(packname As String, filenumber As Integer, filetype As Integer, filename As String) As Byte()
        Dim fs As FileStream = New FileStream(packname, FileMode.Open, FileAccess.Read)
        Dim br As BinaryReader = New BinaryReader(fs)
        Dim grpHeader As F3GRPHeader
        grpHeader.vMajor = br.ReadInt32()
        grpHeader.vMinor = br.ReadInt32()
        grpHeader.nFiles = br.ReadInt32()

        Dim lumps(grpHeader.nFiles) As Lump
        For i = 0 To grpHeader.nFiles - 1
            lumps(i).offset = br.ReadInt32()
            lumps(i).lenght = br.ReadInt32()
        Next
        Dim buffer(lumps(filenumber).lenght) As Byte
        fs.Seek(lumps(filenumber).offset, SeekOrigin.Begin)
        fs.Read(buffer, 0, lumps(filenumber).lenght)
        fs.Close()

        Dim ext = GetExtension(filetype)
        If ext = "model" Then
            'check if buffer starts with "B3D "
            If buffer(0) = 66 And buffer(1) = 51 And buffer(2) = 68 And buffer(3) = 32 Then
                ext = "b3d"
            Else
                ext = "g3d"
            End If
        ElseIf ext = "image" Then
            'check if buffer starts with "BM"
            If buffer(0) = 66 And buffer(1) = 77 Then
                ext = "bmp"
            Else
                ext = "tga"
            End If
        End If

        Dim b As Byte()
        ReDim b(lumps(filenumber).lenght)
        Array.Copy(buffer, b, lumps(filenumber).lenght)
        Return b
    End Function

    Private Sub readExtensions()
        extensions = New List(Of String)

        For i = 0 To head.nEntries - 1
            Dim packLoc = globalIndexToPackOffset(i)
            Dim localIndex = globalIndexToLocal(i)
            Directory.CreateDirectory(grpnames(packOffsetToIndex(packLoc)))
            Dim grpname = $"data\{grpnames(packOffsetToIndex(packLoc))}.grp"
            Dim extname = $"{grpnames(packOffsetToIndex(packLoc))}\{filenames(i)}"
            readExtensions(grpname, entries(packOffsetToIndex(packLoc))(localIndex).number,
                           entries(packOffsetToIndex(packLoc))(localIndex).type, extname)
        Next
    End Sub

    Private Sub readExtensions(packname As String, filenumber As Integer, filetype As Integer, filename As String)
        Dim fs As FileStream = New FileStream(packname, FileMode.Open, FileAccess.Read)
        Dim br As BinaryReader = New BinaryReader(fs)
        Dim grpHeader As F3GRPHeader
        grpHeader.vMajor = br.ReadInt32()
        grpHeader.vMinor = br.ReadInt32()
        grpHeader.nFiles = br.ReadInt32()

        Dim lumps(grpHeader.nFiles) As Lump
        For i = 0 To grpHeader.nFiles - 1
            lumps(i).offset = br.ReadInt32()
            lumps(i).lenght = br.ReadInt32()
        Next
        Dim buffer(lumps(filenumber).lenght) As Byte
        fs.Seek(lumps(filenumber).offset, SeekOrigin.Begin)
        fs.Read(buffer, 0, lumps(filenumber).lenght)
        fs.Close()

        Dim ext = GetExtension(filetype)
        If ext = "model" Then
            'check if buffer starts with "B3D "
            If buffer(0) = 66 And buffer(1) = 51 And buffer(2) = 68 And buffer(3) = 32 Then
                ext = "b3d"
            Else
                ext = "g3d"
            End If
        ElseIf ext = "image" Then
            'check if buffer starts with "BM"
            If buffer(0) = 66 And buffer(1) = 77 Then
                ext = "bmp"
            Else
                ext = "tga"
            End If
        End If
        extensions.Add(ext)
    End Sub

    Public Sub ReadRHT()
        Dim fs As FileStream = New FileStream("resource.rht", FileMode.Open, FileAccess.Read)
        Dim br As BinaryReader = New BinaryReader(fs)
        head.vMajor = br.ReadInt32()
        head.vMinor = br.ReadInt32()
        head.nEntries = br.ReadInt32()
        head.offsetPacks = br.ReadInt32()
        head.offsetResources = br.ReadInt32()

        entries = Array.CreateInstance(GetType(List(Of Entry)), 24)
        localIndexToGlobal = Array.CreateInstance(GetType(Dictionary(Of Integer, Integer)), 24)
        globalIndexToLocal = New Dictionary(Of Integer, Integer)

        For i = 0 To entries.Length - 1
            entries(i) = New List(Of Entry)()
            localIndexToGlobal(i) = New Dictionary(Of Integer, Integer)
        Next

        For i = 0 To head.nEntries - 1
            Dim entry As Entry
            entry.one = br.ReadInt32()
            entry.number = br.ReadInt32()
            entry.type = br.ReadInt32()
            entry.offset = br.ReadInt32()
            entry.pack = br.ReadInt32()
            globalIndexToPackOffset.Add(i, entry.pack)
            localIndexToGlobal(packOffsetToIndex(entry.pack)).Add(entry.number, i)
            globalIndexToLocal.Add(i, entry.number)
            entries(packOffsetToIndex(entry.pack)).Add(entry)
        Next
        grpnames = New List(Of String)
        filenames = New List(Of String)
        Dim sb As New StringBuilder
        fs.Seek(head.offsetPacks, SeekOrigin.Begin)
        For i = 0 To head.offsetResources - head.offsetPacks
            Dim c = fs.ReadByte()
            If c = 0 Then
                grpnames.Add(sb.ToString())
                sb.Clear()
            Else
                sb.Append(ChrW(c))
            End If
        Next
        For i = 0 To fs.Length - head.offsetResources
            Dim c = fs.ReadByte()
            If c = 0 Then
                filenames.Add(sb.ToString())
                sb.Clear()
            Else
                sb.Append(ChrW(c))
            End If
        Next
    End Sub

    Private Sub DarkButton1_Click(sender As Object, e As EventArgs) Handles DarkButton1.Click
        Dim name = DarkTreeView1.SelectedNodes(0).Text
        name = name.Substring(0, name.Length - 4)
        Dim parentName = DarkTreeView1.SelectedNodes(0).ParentNode.Text
        'find entry with matching name
        Dim packIndex = grpnames.IndexOf(parentName)
        For Each g In From ent In entries(packIndex) Select g1 = localIndexToGlobal(packIndex)(ent.number) Where filenames(g1) = name
            FileBytes = getFileBytes(g)
            Extension = extensions(g)
            Exit For
        Next
        Me.Close()
    End Sub

    Private Sub GrpBrowser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeComponent()
        Refresh()
        ReadRHT()
        readExtensions()
        DarkTreeView1.Nodes.Clear()
        For i = 0 To 23
            DarkTreeView1.Nodes.Add(New DarkTreeNode(grpnames(i)))
            For Each g In From ent In entries(i) Select localIndexToGlobal(i)(ent.number)
                Dim ext = extensions(g)
                If filter.Contains(ext) Then DarkTreeView1.Nodes(i).Nodes.Add(New DarkTreeNode(filenames(g) & "." & ext))
            Next
        Next
        Dim temp As DarkTreeNode() = (From node1 In DarkTreeView1.Nodes Where node1.Nodes.Count = 0).ToArray()
        For Each enode In temp
            DarkTreeView1.Nodes.Remove(enode)
        Next
    End Sub

End Class