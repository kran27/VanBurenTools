Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows
Imports System.Windows.Forms.Integration
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports AltUI.Controls
Imports Be.Windows.Forms
Imports HelixToolkit.Wpf
Imports VB3DLib

Public Class GrpBrowser
    Public Property FileBytes As Byte()
    Public Property Extension As String
    Public Property FileName As String
    Private Property previewMode As Boolean

#Region ".rht/.grp Reading"

    Private filter As String() =
                {"psf", "int", "rle", "itm", "veg", "crt", "map", "wav", "amx", "rtd", "rlz", "ini", "skl", "gr2", "skn",
                 "8", "dlg", "str", "sst", "use", "arm", "dor", "wea", "pce", "sco", "gsf", "gls", "fnt", "spl", "pro",
                 "enc", "wmp", "amo", "con", "tok", "b3d", "g3d", "tga", "bmp"}

    Public Sub New(filter As String(), Optional previewMode As Boolean = False)
        Me.previewMode = previewMode
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
    Private fullNameToGlobalIndex As New Dictionary(Of String, Integer)
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

    Public Sub extractFile(index As Integer, Optional convert As Boolean = False)
        Dim b As Byte()
        If index = -1 Then
            Parallel.For(0, head.nEntries,
            Sub(i)
                Dim ext = extensions(i)
                Console.WriteLine(filenames(i) & "." & ext)
                If convert Then
                    If ext = "tga" Then ext = "png"
                    If ext = "b3d" OrElse ext = "g3d" Then ext = "obj"
                End If
                b = getFileBytes(i, convert)
                File.WriteAllBytes(
                    grpnames(packOffsetToIndex(globalIndexToPackOffset(i))) & "\" & filenames(i) & "." & ext,
                    b)
                GC.Collect() ' auto GC is too slow and will run out of memory otherwise
            End Sub)
        Else
            Dim ext = extensions(index)
            Console.WriteLine(filenames(index) & "." & ext)
            If convert Then
                If ext = "tga" Then ext = "png"
                If ext = "b3d" OrElse ext = "g3d" Then ext = "obj"
            End If
            b = getFileBytes(index, convert)
            File.WriteAllBytes(
                grpnames(packOffsetToIndex(globalIndexToPackOffset(index))) & "\" & filenames(index) & "." &
                ext, b)
        End If
        GC.Collect()
    End Sub

    Private Function getFileBytes(index As Integer, Optional convert As Boolean = False) As Byte()
        Dim packLoc = globalIndexToPackOffset(index)
        Dim localIndex = globalIndexToLocal(index)
        Directory.CreateDirectory(grpnames(packOffsetToIndex(packLoc)))
        Dim grpname = $"data\{grpnames(packOffsetToIndex(packLoc))}.grp"
        Dim extname = $"{grpnames(packOffsetToIndex(packLoc))}\{filenames(index)}"
        Return getFileBytes(grpname, entries(packOffsetToIndex(packLoc))(localIndex).number,
                            entries(packOffsetToIndex(packLoc))(localIndex).type, extname, convert)
    End Function

    Private Function getFileBytes(packname As String, filenumber As Integer, filetype As Integer, filename As String, Optional convert As Boolean = False) As Byte()
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

        If convert Then
            If ext = "tga" Then
                Using stream = New MemoryStream()
                    TargaToBitmap(b).Save(stream, System.Drawing.Imaging.ImageFormat.Png)
                    Return stream.ToArray()
                End Using
            ElseIf ext = "b3d" Then
                Try
                    Dim b3d = New B3DModel(b)
                    Dim model = B3DModelToUsableMesh(b3d, filename)
                    ' Create an instance of ObjExporter
                    Dim exporter As New ObjExporter()
                    exporter.MaterialsFile = filename & ".mtl"
                    ' Create a MemoryStream
                    Using memoryStream As New MemoryStream()
                        ' Export the Model3DGroup to the MemoryStream
                        exporter.Export(model, memoryStream)
                        ' Convert the MemoryStream to a byte array
                        Dim byteArray() As Byte = memoryStream.ToArray()
                        Return byteArray
                    End Using
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            ElseIf ext = "g3d" Then
                Dim g3d = New G3DModel(b)
                Dim model = G3DModelToUsableMesh(g3d)
                ' Create an instance of ObjExporter
                Dim exporter As New ObjExporter()
                exporter.MaterialsFile = filename & ".mtl"
                ' Create a MemoryStream
                Using memoryStream As New MemoryStream()
                    ' Export the Model3DGroup to the MemoryStream
                    exporter.Export(model, memoryStream)
                    ' Convert the MemoryStream to a byte array
                    Dim byteArray() As Byte = memoryStream.ToArray()
                    Return byteArray
                End Using
            End If
        End If

        Return b
    End Function

    Public Sub readExtensions()
        extensions = New List(Of String)

        For i = 0 To head.nEntries - 1 ' can't use Parallel.For to speed up
            Dim packLoc = globalIndexToPackOffset(i)
            Dim localIndex = globalIndexToLocal(i)
            Directory.CreateDirectory(grpnames(packOffsetToIndex(packLoc)))
            Dim grpname = $"data\{grpnames(packOffsetToIndex(packLoc))}.grp"
            Dim extname = $"{grpnames(packOffsetToIndex(packLoc))}\{filenames(i)}"
            readExtensions(grpname, entries(packOffsetToIndex(packLoc))(localIndex).number,
                           entries(packOffsetToIndex(packLoc))(localIndex).type, extname)
            fullNameToGlobalIndex.Add((filenames(i) & "." & extensions(i)).ToLower(), i)
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

#End Region

    Private Function TargaToBitmap(b As Byte()) As Bitmap
        Dim ms = New MemoryStream(b)
        Dim image = Pfim.Pfimage.FromStream(ms)
        Dim format As System.Drawing.Imaging.PixelFormat
        Select Case image.Format
            Case Pfim.ImageFormat.Rgb8
                format = System.Drawing.Imaging.PixelFormat.Format8bppIndexed
            Case Pfim.ImageFormat.R5g5b5
                format = System.Drawing.Imaging.PixelFormat.Format16bppRgb555
            Case Pfim.ImageFormat.R5g6b5
                format = System.Drawing.Imaging.PixelFormat.Format16bppRgb565
            Case Pfim.ImageFormat.R5g5b5a1
                format = System.Drawing.Imaging.PixelFormat.Format16bppArgb1555
            Case Pfim.ImageFormat.Rgba16
                ' TODO this is probably wrong
                format = System.Drawing.Imaging.PixelFormat.Format16bppGrayScale
            Case Pfim.ImageFormat.Rgb24
                format = System.Drawing.Imaging.PixelFormat.Format24bppRgb
            Case Pfim.ImageFormat.Rgba32
                format = System.Drawing.Imaging.PixelFormat.Format32bppArgb
            Case Else
                MsgBox(image.Format.ToString())
        End Select
        ' pin image data as the picture box can outlive the pfim image object
        ' which, unless pinned, will garbage collect the data array causing image corruption
        Dim handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned)
        Dim ptr = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0)
        Return New Bitmap(image.Width, image.Height, image.Stride, format, ptr)
    End Function

    Private Function TargaToBitmapImage(b As Byte()) As BitmapImage
        Try
            Dim ms = New MemoryStream(b)
            Dim image = Pfim.Pfimage.FromStream(ms)
            Dim bmpSource = BitmapSource.Create(image.Width, image.Height, 96.0, 96.0, PixelFormats.Bgra32, Nothing, image.Data, image.Stride)
            ' Convert the BitmapSource to a BitmapImage
            Dim bitmapImage As New BitmapImage()
            Dim memoryStream As New MemoryStream()
            Dim encoder As New PngBitmapEncoder() ' Use PNG encoder as BitmapImage does not support the TGA format
            encoder.Frames.Add(BitmapFrame.Create(bmpSource))
            encoder.Save(memoryStream)
            memoryStream.Position = 0
            bitmapImage.BeginInit()
            bitmapImage.StreamSource = memoryStream
            bitmapImage.EndInit()

            Return bitmapImage
        Catch
            Return BitmapToBitmapImage(TargaToBitmap(b))
        End Try
    End Function

    Private Function BitmapToBitmapImage(b As Bitmap) As BitmapImage
        Using memory = New MemoryStream()
            b.Save(memory, ImageFormat.Png)
            memory.Position = 0
            Dim bitmapImage = New BitmapImage()
            bitmapImage.BeginInit()
            bitmapImage.StreamSource = memory
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad
            bitmapImage.EndInit()
            bitmapImage.Freeze()
            Return bitmapImage
        End Using
    End Function

    Private Sub DarkButton1_Click(sender As Object, e As EventArgs) Handles DarkTreeView1.SelectedNodesChanged, DarkButton1.Click
        If Not DarkTreeView1.SelectedNodes.Any() Then Return
        For Each c As Control In TableLayoutPanel1.Controls
            If c IsNot DarkTreeView1 AndAlso c IsNot DarkButton1 AndAlso c IsNot DarkButton2 Then
                TableLayoutPanel1.Controls.Remove(c)
            End If
        Next
        Dim selectedNode = DarkTreeView1.SelectedNodes(0)
        If selectedNode.Nodes.Count <> 0 Then Return
        Dim name = selectedNode.Text
        Dim ext = name.Split(".")(1)
        name = name.Split(".")(0)
        Dim parentName = selectedNode.ParentNode.Text
        'find entry with matching name
        Dim packIndex = grpnames.IndexOf(parentName)
        If previewMode Then
            Dim b As Byte()
            Dim g As Integer = fullNameToGlobalIndex(selectedNode.Text.ToLower())
            b = getFileBytes(g)
            Select Case ext
                Case "bmp"
                    TableLayoutPanel1.Controls.Add(PictureBox1, 1, 0)
                    Dim ms = New MemoryStream(b)
                    PictureBox1.BackgroundImage = New Bitmap(ms)
                Case "tga"
                    TableLayoutPanel1.Controls.Add(PictureBox1, 1, 0)
                    PictureBox1.BackgroundImage = TargaToBitmap(b)
                Case "psf", "ini", "pce", "sco", "gsf", "gls", "tok"
                    TableLayoutPanel1.Controls.Add(DarkRichTextBox1, 1, 0)
                    DarkRichTextBox1.Text = Encoding.ASCII.GetString(b)
                Case "g3d"
                    Dim g3d = New G3DModel(b)
                    Dim mesh = G3DModelToUsableMesh(g3d)
                    model = New ModelVisual3D() With {.Content = mesh}
                    viewport.Children.Clear()
                    viewport.Children.Add(model)
                    viewport.Children.Add(New DefaultLights())
                    TableLayoutPanel1.Controls.Add(host, 1, 0)
                Case "b3d"
                    Dim b3d = New B3DModel(b)
                    Dim mesh = B3DModelToUsableMesh(b3d, selectedNode.Text)
                    model = New ModelVisual3D() With {.Content = mesh}
                    viewport.Children.Clear()
                    viewport.Children.Add(model)
                    viewport.Children.Add(New DefaultLights())
                    TableLayoutPanel1.Controls.Add(host, 1, 0)
                Case "8"
                    Dim _8 = New _8Model(b, MsgBox("FLGS Size?", MsgBoxStyle.YesNo, "Option") = DialogResult.Yes)
                    Dim mesh = _8ModelToUsableMesh(_8)
                    model = New ModelVisual3D() With {.Content = mesh}
                    viewport.Children.Clear()
                    viewport.Children.Add(model)
                    viewport.Children.Add(New DefaultLights())
                    TableLayoutPanel1.Controls.Add(host, 1, 0)
                Case "map"
                    viewport.Children.Clear()
                    Dim map As Map
                    Try
                        map = b.ReadMap()
                    Catch
                        TableLayoutPanel1.Controls.Add(HexBox1, 1, 0)
                        HexBox1.ByteProvider = New DynamicByteProvider(b)
                        Return
                    End Try

                    Dim mmn = map.EMAP.s1.ToLower()
                    Try
                        Dim mgi = fullNameToGlobalIndex(mmn)
                        Dim mmb = getFileBytes(mgi)

                        Dim _8 = New _8Model(mmb, MsgBox("FLGS Size?", MsgBoxStyle.YesNo, "Option") = DialogResult.Yes)
                        Dim mapMesh = _8ModelToUsableMesh(_8)
                        Dim mapModel = New ModelVisual3D() With {.Content = mapMesh}

                        viewport.Children.Add(mapModel)
                    Catch
                    End Try
                    For Each ent In map.EME2
                        Dim gInd = 0
                        Try
                            gInd = fullNameToGlobalIndex(ent.name.ToLower())
                        Catch
                            Continue For
                        End Try
                        If ent.name.ToLower.EndsWith(".crt") Then
                            Dim crt = CRTToUsableMesh(getFileBytes(gInd))

                            Dim t = New Transform3DGroup()

                            Dim rot = ent.l.r * 180 / Math.PI '+ 90 ' adding 90 fixes some rotations, breaks others

                            t.Children.Add(New RotateTransform3D(New AxisAngleRotation3D(New Vector3D(0, 0, 1), rot)))
                            t.Children.Add(New TranslateTransform3D(ent.l.x, ent.l.y, ent.l.z))

                            crt.Transform = t

                            Dim mv3 = New ModelVisual3D() With {.Content = crt}

                            viewport.Children.Add(mv3)
                        Else
                            Dim c = getFileBytes(gInd).ToEEN2c()
                            Dim gInd2 = fullNameToGlobalIndex(c.skl.ToLower() & ".b3d")
                            Dim b3d = New B3DModel(getFileBytes(gInd2))
                            Dim m = B3DModelToUsableMesh(b3d, selectedNode.Text)

                            Dim t = New Transform3DGroup()

                            Dim rot = ent.l.r * 180 / Math.PI '+ 90 ' adding 90 fixes some rotations, breaks others

                            t.Children.Add(New RotateTransform3D(New AxisAngleRotation3D(New Vector3D(0, 0, 1), rot)))
                            t.Children.Add(New TranslateTransform3D(ent.l.x, ent.l.y, ent.l.z))

                            m.Transform = t

                            Dim mv3 = New ModelVisual3D() With {.Content = m}

                            viewport.Children.Add(mv3)
                        End If
                    Next

                    For Each ep In map.EMEP
                        Dim gInd = fullNameToGlobalIndex("default_pc.crt")
                        Dim crt = CRTToUsableMesh(getFileBytes(gInd))

                        Dim t = New Transform3DGroup()

                        Dim rot = ep.r * 180 / Math.PI '+ 90 ' adding 90 fixes some rotations, breaks others

                        t.Children.Add(New RotateTransform3D(New AxisAngleRotation3D(New Vector3D(0, 0, 1), rot)))
                        t.Children.Add(New TranslateTransform3D(ep.p.x, ep.p.y, ep.p.z))

                        crt.Transform = t

                        Dim mv3 = New ModelVisual3D() With {.Content = crt}

                        viewport.Children.Add(mv3)
                    Next

                    viewport.Children.Add(New DefaultLights())
                    TableLayoutPanel1.Controls.Add(host, 1, 0)
                Case "crt"
                    'Try
                    Dim crt = CRTToUsableMesh(b)
                    model = New ModelVisual3D() With {.Content = crt}
                    viewport.Children.Clear()
                    viewport.Children.Add(model)
                    viewport.Children.Add(New DefaultLights())
                    TableLayoutPanel1.Controls.Add(host, 1, 0)
                    'Catch
                    '    TableLayoutPanel1.Controls.Add(HexBox1, 1, 0)
                    '    HexBox1.ByteProvider = New DynamicByteProvider(b)
                    'End Try
                Case Else
                    TableLayoutPanel1.Controls.Add(HexBox1, 1, 0)
                    HexBox1.ByteProvider = New DynamicByteProvider(b)
            End Select
        Else
            For Each g In
                From ent In entries(packIndex) Select g1 = localIndexToGlobal(packIndex)(ent.number)
                Where filenames(g1) = name
                FileBytes = getFileBytes(g)
                Extension = extensions(g)
                FileName = filenames(g)
                Exit For
            Next
            Me.Close()
        End If
    End Sub

    Private Function GetImageFromName(name As String) As BitmapImage
        Dim nne = name.Split(".")(0)
        For i = 0 To entries.Length - 1
            For Each g In
                From e In entries(i) Let g1 = localIndexToGlobal(i)(e.number)
                Where filenames(g1) = nne AndAlso GetExtension(e.type) = "image" Select g1
                Return TargaToBitmapImage(getFileBytes(g))
            Next
        Next
    End Function

    Private Function CRTToUsableMesh(b As Byte()) As Model3DGroup
        Dim crt = b.ReadCRT()
        If crt.EEN2.skl.StartsWith("Hum") Then
            Dim baseModel = (crt.EEN2.skl & ".b3d").ToLower()
            Dim heaModel = (crt.EEN2.skl & "_" & crt.GCRE.Hea.Model & ".b3d").ToLower()
            Dim haiModel = (crt.EEN2.skl & "_" & crt.GCRE.Hai.Model & ".b3d").ToLower()
            Dim ponModel = (crt.EEN2.skl & "_" & crt.GCRE.Pon.Model & ".b3d").ToLower()
            Dim musModel = (crt.EEN2.skl & "_" & crt.GCRE.Mus.Model & ".b3d").ToLower()
            Dim beaModel = (crt.EEN2.skl & "_" & crt.GCRE.Bea.Model & ".b3d").ToLower()
            Dim eyeModel = (crt.EEN2.skl & "_" & crt.GCRE.Eye.Model & ".b3d").ToLower()
            Dim bodModel = (crt.EEN2.skl & "_" & crt.GCRE.Bod.Model & ".b3d").ToLower()
            Dim hanModel = (crt.EEN2.skl & "_" & crt.GCRE.Han.Model & ".b3d").ToLower()
            Dim feeModel = (crt.EEN2.skl & "_" & crt.GCRE.Fee.Model & ".b3d").ToLower()
            Dim bacModel = (crt.EEN2.skl & "_" & crt.GCRE.Bac.Model & ".b3d").ToLower()
            Dim shoModel = (crt.EEN2.skl & "_" & crt.GCRE.Sho.Model & ".b3d").ToLower()
            Dim vanModel = (crt.EEN2.skl & "_" & crt.GCRE.Van.Model & ".b3d").ToLower()

            Dim heaTex As String
            Dim eyeTex As String
            Dim bodTex As String
            Dim hanTex As String
            Dim feeTex As String
            Dim bacTex As String
            Dim shoTex As String
            Dim vanTex As String

            For Each itm In From i In crt.GCRE.Inventory Where i.ToLower.EndsWith(".arm") Select getFileBytes(fullNameToGlobalIndex(i.ToLower)).ReadITM()
                If itm.GITM.Bac.Model <> "" Then bacModel = (crt.EEN2.skl & "_" & itm.GITM.Bac.Model & ".b3d").ToLower()
                If itm.GITM.Bod.Model <> "" Then bodModel = (crt.EEN2.skl & "_" & itm.GITM.Bod.Model & ".b3d").ToLower()
                If itm.GITM.Fee.Model <> "" Then feeModel = (crt.EEN2.skl & "_" & itm.GITM.Fee.Model & ".b3d").ToLower()
                If itm.GITM.Han.Model <> "" Then hanModel = (crt.EEN2.skl & "_" & itm.GITM.Han.Model & ".b3d").ToLower()
                If itm.GITM.Hea.Model <> "" Then heaModel = (crt.EEN2.skl & "_" & itm.GITM.Hea.Model & ".b3d").ToLower()
                If itm.GITM.Sho.Model <> "" Then shoModel = (crt.EEN2.skl & "_" & itm.GITM.Sho.Model & ".b3d").ToLower()
                If itm.GITM.Van.Model <> "" Then vanModel = (crt.EEN2.skl & "_" & itm.GITM.Van.Model & ".b3d").ToLower()
                heaTex = itm.GITM.Hea.Tex.Replace(".dds", ".tga").ToLower
                eyeTex = itm.GITM.Eye.Tex.Replace(".dds", ".tga").ToLower
                bodTex = itm.GITM.Bod.Tex.Replace(".dds", ".tga").ToLower
                hanTex = itm.GITM.Han.Tex.Replace(".dds", ".tga").ToLower
                feeTex = itm.GITM.Fee.Tex.Replace(".dds", ".tga").ToLower
                bacTex = itm.GITM.Bac.Tex.Replace(".dds", ".tga").ToLower
                shoTex = itm.GITM.Sho.Tex.Replace(".dds", ".tga").ToLower
                vanTex = itm.GITM.Van.Tex.Replace(".dds", ".tga").ToLower
            Next

            Dim baseB3D = New B3DModel(getFileBytes(fullNameToGlobalIndex(baseModel)))
            Dim baseMesh = B3DToNoMatMesh(baseB3D)

            Dim heaB3D As B3DModel
            Dim haiB3D As B3DModel
            Dim ponB3D As B3DModel
            Dim musB3D As B3DModel
            Dim beaB3D As B3DModel
            Dim eyeB3D As B3DModel
            Dim bodB3D As B3DModel
            Dim hanB3D As B3DModel
            Dim feeB3D As B3DModel
            Dim bacB3D As B3DModel
            Dim shoB3D As B3DModel
            Dim vanB3D As B3DModel

            Dim heaMesh As MeshGeometry3D = Nothing
            Dim haiMesh As MeshGeometry3D = Nothing
            Dim ponMesh As MeshGeometry3D = Nothing
            Dim musMesh As MeshGeometry3D = Nothing
            Dim beaMesh As MeshGeometry3D = Nothing
            Dim eyeMesh As MeshGeometry3D = Nothing
            Dim bodMesh As MeshGeometry3D = Nothing
            Dim hanMesh As MeshGeometry3D = Nothing
            Dim feeMesh As MeshGeometry3D = Nothing
            Dim bacMesh As MeshGeometry3D = Nothing
            Dim shoMesh As MeshGeometry3D = Nothing
            Dim vanMesh As MeshGeometry3D = Nothing

            If heaModel <> crt.EEN2.skl.ToLower() & "_.b3d" Then
                heaB3D = New B3DModel(getFileBytes(fullNameToGlobalIndex(heaModel)))
                heaMesh = B3DToNoMatMesh(heaB3D)
            End If
            If haiModel <> crt.EEN2.skl.ToLower() & "_.b3d" Then
                haiB3D = New B3DModel(getFileBytes(fullNameToGlobalIndex(haiModel)))
                haiMesh = B3DToNoMatMesh(haiB3D)
            End If
            If ponModel <> crt.EEN2.skl.ToLower() & "_.b3d" Then
                ponB3D = New B3DModel(getFileBytes(fullNameToGlobalIndex(ponModel)))
                ponMesh = B3DToNoMatMesh(ponB3D)
            End If
            If musModel <> crt.EEN2.skl.ToLower() & "_.b3d" Then
                musB3D = New B3DModel(getFileBytes(fullNameToGlobalIndex(musModel)))
                musMesh = B3DToNoMatMesh(musB3D)
            End If
            If beaModel <> crt.EEN2.skl.ToLower() & "_.b3d" Then
                beaB3D = New B3DModel(getFileBytes(fullNameToGlobalIndex(beaModel)))
                beaMesh = B3DToNoMatMesh(beaB3D)
            End If
            If eyeModel <> crt.EEN2.skl.ToLower() & "_.b3d" Then
                eyeB3D = New B3DModel(getFileBytes(fullNameToGlobalIndex(eyeModel)))
                eyeMesh = B3DToNoMatMesh(eyeB3D)
            End If
            If bodModel <> crt.EEN2.skl.ToLower() & "_.b3d" Then
                bodB3D = New B3DModel(getFileBytes(fullNameToGlobalIndex(bodModel)))
                bodMesh = B3DToNoMatMesh(bodB3D)
            End If
            If hanModel <> crt.EEN2.skl.ToLower() & "_.b3d" Then
                hanB3D = New B3DModel(getFileBytes(fullNameToGlobalIndex(hanModel)))
                hanMesh = B3DToNoMatMesh(hanB3D)
            End If
            If feeModel <> crt.EEN2.skl.ToLower() & "_.b3d" Then
                feeB3D = New B3DModel(getFileBytes(fullNameToGlobalIndex(feeModel)))
                feeMesh = B3DToNoMatMesh(feeB3D)
            End If
            If bacModel <> crt.EEN2.skl.ToLower() & "_.b3d" Then
                bacB3D = New B3DModel(getFileBytes(fullNameToGlobalIndex(bacModel)))
                bacMesh = B3DToNoMatMesh(bacB3D)
            End If
            If shoModel <> crt.EEN2.skl.ToLower() & "_.b3d" Then
                shoB3D = New B3DModel(getFileBytes(fullNameToGlobalIndex(shoModel)))
                shoMesh = B3DToNoMatMesh(shoB3D)
            End If
            If vanModel <> crt.EEN2.skl.ToLower() & "_.b3d" Then
                vanB3D = New B3DModel(getFileBytes(fullNameToGlobalIndex(vanModel)))
                vanMesh = B3DToNoMatMesh(vanB3D)
            End If

            Dim x As New Bitmap(512, 512)
            Using gr = Graphics.FromImage(x)
                Dim baseTex = If(crt.EEN2.EEOV.s4 <> "", crt.EEN2.EEOV.s4.Replace(".dds", ".tga").ToLower(), baseB3D.texName.ToLower())
                Dim baseImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex(baseTex)))
                gr.DrawImageUnscaled(baseImage, 0, 0)
                If heaMesh IsNot Nothing Then
                    If heaTex = "" Then heaTex = If(crt.GCRE.Hea.Tex <> "", crt.GCRE.Hea.Tex.Replace(".dds", ".tga").ToLower(), heaB3D.texName.ToLower())
                    Dim heaImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex(heaTex)))
                    gr.DrawImageUnscaled(heaImage, 384, 0)
                End If
                If haiMesh IsNot Nothing Then
                    Dim haiTex = If(crt.GCRE.Hai.Tex <> "", crt.GCRE.Hai.Tex.Replace(".dds", ".tga").ToLower(), haiB3D.texName.ToLower())
                    Dim haiImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex(haiTex)))
                    gr.DrawImageUnscaled(haiImage, 256, 128)
                End If
                If ponMesh IsNot Nothing Then
                    Dim ponTex = If(crt.GCRE.Pon.Tex <> "", crt.GCRE.Pon.Tex.Replace(".dds", ".tga").ToLower(), ponB3D.texName.ToLower())
                    Dim ponImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex(ponTex)))
                    gr.DrawImageUnscaled(ponImage, 288, 224)
                End If
                If musMesh IsNot Nothing Then
                    Dim musTex = If(crt.GCRE.Mus.Tex <> "", crt.GCRE.Mus.Tex.Replace(".dds", ".tga").ToLower(), musB3D.texName.ToLower())
                    Dim musImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex(musTex)))
                    gr.DrawImageUnscaled(musImage, 256, 224)
                End If
                If beaMesh IsNot Nothing Then
                    Dim beaTex = If(crt.GCRE.Bea.Tex <> "", crt.GCRE.Bea.Tex.Replace(".dds", ".tga").ToLower(), beaB3D.texName.ToLower())
                    Dim beaImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex(beaTex)))
                    gr.DrawImageUnscaled(beaImage, 256, 192)
                End If
                If eyeMesh IsNot Nothing Then
                    If eyeTex = "" Then eyeTex = If(crt.GCRE.Eye.Tex <> "", crt.GCRE.Eye.Tex.Replace(".dds", ".tga").ToLower(), eyeB3D.texName.ToLower())
                    Dim eyeImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex(eyeTex)))
                    gr.DrawImageUnscaled(eyeImage, 320, 192) ' guess, i know of nothing that uses the eye socket
                End If
                If bodMesh IsNot Nothing Then
                    If bodTex = "" Then bodTex = If(crt.GCRE.Bod.Tex <> "", crt.GCRE.Bod.Tex.Replace(".dds", ".tga").ToLower(), bodB3D.texName.ToLower())
                    Dim bodImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex(bodTex)))
                    gr.DrawImageUnscaled(bodImage, 0, 256)
                End If
                If hanMesh IsNot Nothing Then
                    If hanTex = "" Then hanTex = If(crt.GCRE.Han.Tex <> "", crt.GCRE.Han.Tex.Replace(".dds", ".tga").ToLower(), hanB3D.texName.ToLower())
                    Dim hanImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex(hanTex)))
                    gr.DrawImageUnscaled(hanImage, 384, 128)
                End If
                If feeMesh IsNot Nothing Then
                    If feeTex = "" Then feeTex = If(crt.GCRE.Fee.Tex <> "", crt.GCRE.Fee.Tex.Replace(".dds", ".tga").ToLower(), feeB3D.texName.ToLower())
                    Dim feeImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex(feeTex)))
                    gr.DrawImageUnscaled(feeImage, 384, 256)
                End If
                If bacMesh IsNot Nothing Then
                    If bacTex = "" Then bacTex = If(crt.GCRE.Bac.Tex <> "", crt.GCRE.Bac.Tex.Replace(".dds", ".tga").ToLower(), bacB3D.texName.ToLower())
                    Dim bacImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex(bacTex)))
                    gr.DrawImageUnscaled(bacImage, 256, 256)
                End If
                If shoMesh IsNot Nothing Then
                    If shoTex = "" Then shoTex = If(crt.GCRE.Sho.Tex <> "", crt.GCRE.Sho.Tex.Replace(".dds", ".tga").ToLower(), shoB3D.texName.ToLower())
                    Dim shoImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex(shoTex)))
                    gr.DrawImageUnscaled(shoImage, 256, 0)
                End If
                If vanMesh IsNot Nothing Then
                    If vanTex = "" Then vanTex = If(crt.GCRE.Van.Tex <> "", crt.GCRE.Van.Tex.Replace(".dds", ".tga").ToLower(), vanB3D.texName.ToLower())
                    Dim vanImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex(vanTex)))
                    gr.DrawImageUnscaled(vanImage, 320, 128)
                End If
            End Using

            Dim xbi = BitmapToBitmapImage(x)

            Dim baseMat = New DiffuseMaterial(New ImageBrush(CropBitmapByUV(xbi, baseMesh.TextureCoordinates)))
            Dim modelGroup = New Model3DGroup()

            Dim baseGM3 = New GeometryModel3D With {
                .Geometry = baseMesh,
                .Material = baseMat,
                .BackMaterial = baseMat
            }
            modelGroup.Children.Add(baseGM3)

            If heaMesh IsNot Nothing Then
                Dim heaMat = New DiffuseMaterial(New ImageBrush(CropBitmapByUV(xbi, heaMesh.TextureCoordinates)))
                Dim heaGM3 = New GeometryModel3D With {
                    .Geometry = heaMesh,
                    .Material = heaMat,
                    .BackMaterial = heaMat
                }
                modelGroup.Children.Add(heaGM3)
            End If
            If haiMesh IsNot Nothing Then
                Dim haiMat = New DiffuseMaterial(New ImageBrush(CropBitmapByUV(xbi, haiMesh.TextureCoordinates, True)))
                Dim haiGM3 = New GeometryModel3D With {
                    .Geometry = haiMesh,
                    .Material = haiMat,
                    .BackMaterial = haiMat
                }
                modelGroup.Children.Add(haiGM3)
            End If
            If ponMesh IsNot Nothing Then
                Dim ponMat = New DiffuseMaterial(New ImageBrush(CropBitmapByUV(xbi, ponMesh.TextureCoordinates)))
                Dim ponGM3 = New GeometryModel3D With {
                    .Geometry = ponMesh,
                    .Material = ponMat,
                    .BackMaterial = ponMat
                }
                modelGroup.Children.Add(ponGM3)
            End If
            If musMesh IsNot Nothing Then
                Dim musMat = New DiffuseMaterial(New ImageBrush(CropBitmapByUV(xbi, musMesh.TextureCoordinates, True)))
                Dim musGM3 = New GeometryModel3D With {
                    .Geometry = musMesh,
                    .Material = musMat,
                    .BackMaterial = musMat
                }
                modelGroup.Children.Add(musGM3)
            End If
            If beaMesh IsNot Nothing Then
                Dim beaMat = New DiffuseMaterial(New ImageBrush(CropBitmapByUV(xbi, beaMesh.TextureCoordinates, True)))
                Dim beaGM3 = New GeometryModel3D With {
                    .Geometry = beaMesh,
                    .Material = beaMat,
                    .BackMaterial = beaMat
                }
                modelGroup.Children.Add(beaGM3)
            End If
            If eyeMesh IsNot Nothing Then
                Dim eyeMat = New DiffuseMaterial(New ImageBrush(CropBitmapByUV(xbi, eyeMesh.TextureCoordinates)))
                Dim eyeGM3 = New GeometryModel3D With {
                    .Geometry = eyeMesh,
                    .Material = eyeMat,
                    .BackMaterial = eyeMat
                }
                modelGroup.Children.Add(eyeGM3)
            End If
            If bodMesh IsNot Nothing Then
                Dim bodMat = New DiffuseMaterial(New ImageBrush(CropBitmapByUV(xbi, bodMesh.TextureCoordinates, True)))
                Dim bodGM3 = New GeometryModel3D With {
                    .Geometry = bodMesh,
                    .Material = bodMat,
                    .BackMaterial = bodMat
                }
                modelGroup.Children.Add(bodGM3)
            End If
            If hanMesh IsNot Nothing Then
                Dim hanMat = New DiffuseMaterial(New ImageBrush(CropBitmapByUV(xbi, hanMesh.TextureCoordinates)))
                Dim hanGM3 = New GeometryModel3D With {
                    .Geometry = hanMesh,
                    .Material = hanMat,
                    .BackMaterial = hanMat
                }
                modelGroup.Children.Add(hanGM3)
            End If
            If feeMesh IsNot Nothing Then
                Dim feeMat = New DiffuseMaterial(New ImageBrush(CropBitmapByUV(xbi, feeMesh.TextureCoordinates)))
                Dim feeGM3 = New GeometryModel3D With {
                    .Geometry = feeMesh,
                    .Material = feeMat,
                    .BackMaterial = feeMat
                }
                modelGroup.Children.Add(feeGM3)
            End If
            If bacMesh IsNot Nothing Then
                Dim bacMat = New DiffuseMaterial(New ImageBrush(CropBitmapByUV(xbi, bacMesh.TextureCoordinates, True)))
                Dim bacGM3 = New GeometryModel3D With {
                    .Geometry = bacMesh,
                    .Material = bacMat,
                    .BackMaterial = bacMat
                }
                modelGroup.Children.Add(bacGM3)
            End If
            If shoMesh IsNot Nothing Then
                Dim shoMat = New DiffuseMaterial(New ImageBrush(CropBitmapByUV(xbi, shoMesh.TextureCoordinates)))
                Dim shoGM3 = New GeometryModel3D With {
                    .Geometry = shoMesh,
                    .Material = shoMat,
                    .BackMaterial = shoMat
                }
                modelGroup.Children.Add(shoGM3)
            End If
            If vanMesh IsNot Nothing Then
                Dim vanMat = New DiffuseMaterial(New ImageBrush(CropBitmapByUV(xbi, vanMesh.TextureCoordinates)))
                Dim vanGM3 = New GeometryModel3D With {
                    .Geometry = vanMesh,
                    .Material = vanMat,
                    .BackMaterial = vanMat
                }
                modelGroup.Children.Add(vanGM3)
            End If

            Return modelGroup
        Else
            Dim baseModel = (crt.EEN2.skl & ".b3d").ToLower()
            Dim baseB3D = New B3DModel(getFileBytes(fullNameToGlobalIndex(baseModel)))
            Dim baseMesh = B3DToNoMatMesh(baseB3D)
            Dim baseTex = If(crt.EEN2.EEOV.s4 <> "", crt.EEN2.EEOV.s4.Replace(".dds", ".tga").ToLower(), baseB3D.texName.ToLower())
            Dim baseMat As DiffuseMaterial
            Try
                Dim baseImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex(baseTex)))
                baseMat = New DiffuseMaterial(New ImageBrush(CropBitmapByUV(BitmapToBitmapImage(baseImage), baseMesh.TextureCoordinates, True)))
            Catch
                baseMat = New DiffuseMaterial(New SolidColorBrush(Colors.LightSlateGray))
            End Try
            Dim modelGroup = New Model3DGroup()

            Dim baseGM3 = New GeometryModel3D With {
                    .Geometry = baseMesh,
                    .Material = baseMat,
                    .BackMaterial = baseMat
                    }
            modelGroup.Children.Add(baseGM3)
            Return modelGroup
        End If
    End Function

    ' crops texture to the UV bounds for previewing
    ' necessary due to a weird quirk of HelixToolkit
    ' transparency also disabled by default due to rendering issue
    Public Function CropBitmapByUV(bitmapImage As BitmapImage, uvCoordinates As PointCollection, Optional trans As Boolean = False) As BitmapImage
        Dim minX As Double = uvCoordinates.Min(Function(p) p.X)
        Dim minY As Double = uvCoordinates.Min(Function(p) p.Y)
        Dim maxX As Double = uvCoordinates.Max(Function(p) p.X)
        Dim maxY As Double = uvCoordinates.Max(Function(p) p.Y)

        Dim x = minX * bitmapImage.PixelWidth
        Dim y = minY * bitmapImage.PixelHeight
        Dim width = (maxX - minX) * bitmapImage.PixelWidth
        Dim height = (maxY - minY) * bitmapImage.PixelHeight
        Console.WriteLine($"x: {x} y: {y}{vbCrLf}w: {width} h: {height}")

        Dim croppedBitmap As New CroppedBitmap(bitmapImage, New Int32Rect(x, y, width, height))

        Dim bitmap As New Bitmap(croppedBitmap.PixelWidth, croppedBitmap.PixelHeight, If(trans, System.Drawing.Imaging.PixelFormat.Format32bppArgb, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
        Dim bitmapData As System.Drawing.Imaging.BitmapData = bitmap.LockBits(New System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.[WriteOnly], bitmap.PixelFormat)
        croppedBitmap.CopyPixels(System.Windows.Int32Rect.Empty, bitmapData.Scan0, bitmapData.Height * bitmapData.Stride, bitmapData.Stride)
        bitmap.UnlockBits(bitmapData)
        Return BitmapToBitmapImage(bitmap)
    End Function

    Private Function G3DModelToUsableMesh(g3d As G3DModel) As Model3DGroup
        Dim meshBuilder = New MeshBuilder()
        Dim cs = 96.0F ' coordinate scale, to roughly match the size of b3d models (not the same as CoordinateScale in models, it's purpose is unknown)
        For i = 0 To g3d.Model_Faces_Index.Count - 1
            Dim face = g3d.Model_Faces_Index(i)
            Dim a = New Point3D(g3d.Model_Vertex_Position(face(0)).X / cs, g3d.Model_Vertex_Position(face(0)).Y / cs, g3d.Model_Vertex_Position(face(0)).Z / cs)
            Dim b = New Point3D(g3d.Model_Vertex_Position(face(1)).X / cs, g3d.Model_Vertex_Position(face(1)).Y / cs, g3d.Model_Vertex_Position(face(1)).Z / cs)
            Dim c = New Point3D(g3d.Model_Vertex_Position(face(2)).X / cs, g3d.Model_Vertex_Position(face(2)).Y / cs, g3d.Model_Vertex_Position(face(2)).Z / cs)

            Dim texA = New Point(g3d.Model_Vertex_Texcoords(face(0)).X, g3d.Model_Vertex_Texcoords(face(0)).Y)
            Dim texB = New Point(g3d.Model_Vertex_Texcoords(face(1)).X, g3d.Model_Vertex_Texcoords(face(1)).Y)
            Dim texC = New Point(g3d.Model_Vertex_Texcoords(face(2)).X, g3d.Model_Vertex_Texcoords(face(2)).Y)

            meshBuilder.AddTriangle(a, b, c, texA, texB, texC)
        Next
        Dim mesh = OptimizeMesh(meshBuilder)
        Dim material As Material
        Try
            Dim cropped = CropBitmapByUV(GetImageFromName("CR_Monsta_default_LG.tga"), mesh.TextureCoordinates)
            material = New DiffuseMaterial(New ImageBrush(cropped))
        Catch
            material = New DiffuseMaterial(New SolidColorBrush(Colors.LightSlateGray))
        End Try
        Dim model = New GeometryModel3D With {
            .Geometry = mesh,
            .Material = material,
            .BackMaterial = material
        }

        ' rotate model to match b3ds
        Dim rotateTransform = New RotateTransform3D(New AxisAngleRotation3D(New Vector3D(1, 0, 0), 90))
        Dim rotateTransform2 = New RotateTransform3D(New AxisAngleRotation3D(New Vector3D(0, 0, 1), -90))
        Dim transformGroup = New Transform3DGroup()
        transformGroup.Children.Add(rotateTransform)
        transformGroup.Children.Add(rotateTransform2)
        model.Transform = transformGroup

        Dim modelGroup As New Model3DGroup()
        modelGroup.Children.Add(model)

        Return modelGroup
    End Function

    Private Function B3DToNoMatMesh(b3d As B3DModel) As MeshGeometry3D
        Dim meshBuilder = New MeshBuilder()
        For i = 0 To b3d.Model_Faces_Index.Count - 1
            Dim face = b3d.Model_Faces_Index(i)
            Dim a = New Point3D(b3d.Model_Vertex_Position(face(0)).X, b3d.Model_Vertex_Position(face(0)).Y, b3d.Model_Vertex_Position(face(0)).Z)
            Dim b = New Point3D(b3d.Model_Vertex_Position(face(1)).X, b3d.Model_Vertex_Position(face(1)).Y, b3d.Model_Vertex_Position(face(1)).Z)
            Dim c = New Point3D(b3d.Model_Vertex_Position(face(2)).X, b3d.Model_Vertex_Position(face(2)).Y, b3d.Model_Vertex_Position(face(2)).Z)
            Dim texA = New Point(b3d.Model_Vertex_Texcoords(face(0)).X, b3d.Model_Vertex_Texcoords(face(0)).Y)
            Dim texB = New Point(b3d.Model_Vertex_Texcoords(face(1)).X, b3d.Model_Vertex_Texcoords(face(1)).Y)
            Dim texC = New Point(b3d.Model_Vertex_Texcoords(face(2)).X, b3d.Model_Vertex_Texcoords(face(2)).Y)
            meshBuilder.AddTriangle(a, b, c, texA, texB, texC)
        Next
        Return OptimizeMesh(meshBuilder)
    End Function

    Private Function B3DModelToUsableMesh(b3d As B3DModel, fn As String) As Model3DGroup
        Dim mesh = B3DToNoMatMesh(b3d)
        Dim material As Material
        If b3d.texName <> "" Then
            Try
                Dim cropped = CropBitmapByUV(GetImageFromName(b3d.texName), mesh.TextureCoordinates)
                material = New DiffuseMaterial(New ImageBrush(cropped))
            Catch
                material = New DiffuseMaterial(New SolidColorBrush(Colors.LightSlateGray))
            End Try
        Else
            material = New DiffuseMaterial(New SolidColorBrush(Colors.LightSlateGray))
        End If
        Dim model = New GeometryModel3D With {
            .Geometry = mesh,
            .Material = material,
            .BackMaterial = material
        }

        'Dim rotateTransform = New RotateTransform3D(New AxisAngleRotation3D(New Vector3D(0, 0, 1), -90))
        'model.Transform = rotateTransform

        If b3d.FormatVer = 1 Then
            Dim scaleTransform = New ScaleTransform3D(New Vector3D(10, 10, 10))
            model.Transform = scaleTransform
        End If

        Dim modelGroup As New Model3DGroup()
        modelGroup.Children.Add(model)

        Return modelGroup
    End Function

    Private Function _8ModelToUsableMesh(_8 As _8Model) As Model3DGroup
        Dim mbldrs = New List(Of MeshBuilder)

        For i = 0 To _8.MAT.Count - 1
            mbldrs.Add(New MeshBuilder())
        Next

        For i = 0 To _8.IDX.Count - 1
            Dim face = _8.IDX(i)
            Dim a = New Point3D(_8.GVP(face(0)).X, _8.GVP(face(0)).Y, _8.GVP(face(0)).Z)
            Dim b = New Point3D(_8.GVP(face(1)).X, _8.GVP(face(1)).Y, _8.GVP(face(1)).Z)
            Dim c = New Point3D(_8.GVP(face(2)).X, _8.GVP(face(2)).Y, _8.GVP(face(2)).Z)
            Dim texA = New Point(_8.GVT(face(0)).X, _8.GVT(face(0)).Y)
            Dim texB = New Point(_8.GVT(face(1)).X, _8.GVT(face(1)).Y)
            Dim texC = New Point(_8.GVT(face(2)).X, _8.GVT(face(2)).Y)
            mbldrs(Math.Clamp(_8.MDX(i) - 1, 0, Integer.MaxValue)).AddTriangle(a, b, c, texA, texB, texC)
        Next

        Dim modelGroup As New Model3DGroup()

        For i = 0 To mbldrs.Count - 1
            Dim mesh = OptimizeMesh(mbldrs(i))
            Dim material As Material

            Try
                Dim cropped = CropBitmapByUV(GetImageFromName(_8.MAT(i).FileName), mesh.TextureCoordinates, If(_8.MAT(i).FileName.Contains("Flora"), True, False))
                material = New DiffuseMaterial(New ImageBrush(cropped))
            Catch
                material = New DiffuseMaterial(New SolidColorBrush(Colors.LightSlateGray))
            End Try

            Dim model = New GeometryModel3D With {
                    .Geometry = mesh,
                    .Material = material,
                    .BackMaterial = material
                    }

            'Dim rotateTransform = New RotateTransform3D(New AxisAngleRotation3D(New Vector3D(0, 0, 1), -90))
            'model.Transform = rotateTransform

            modelGroup.Children.Add(model)
        Next

        Dim exp = New ObjExporter
        exp.MaterialsFile = "temp.mtl"
        Dim tmp = New ModelVisual3D() With {.Content = modelGroup}
        Dim fs = New FileStream("test.obj", FileMode.Create)
        exp.Export(tmp, fs)
        fs.Close()

        Return modelGroup
    End Function

    ' removes exact duplicate vertices, while keeping texture coordinates and faces intact
    Private Function OptimizeMesh(mesh As MeshBuilder) As MeshGeometry3D
        Dim meshBuilder = New MeshBuilder()
        Dim positions = mesh.Positions
        Dim texcoords = mesh.TextureCoordinates
        Dim indices = mesh.TriangleIndices
        Dim normals = mesh.Normals
        Dim newIndices = New List(Of Integer)()
        Dim newPositions = New List(Of Point3D)()
        Dim newTexcoords = New List(Of Point)()
        Dim newNormals = New List(Of Vector3D)()
        Dim vertexIndexMap = New Dictionary(Of (Point3D, Point), Integer)()
        Dim index = 0

        For i = 0 To positions.Count - 1
            Dim pos = positions(i)
            Dim tex = texcoords(i)
            Dim norm = normals(i)
            Dim vertex = (pos, tex)

            If Not vertexIndexMap.ContainsKey(vertex) Then
                newPositions.Add(pos)
                newTexcoords.Add(tex)
                newNormals.Add(norm)
                vertexIndexMap(vertex) = index
                index += 1
            End If

            newIndices.Add(vertexIndexMap(vertex))
        Next

        meshBuilder.Append(newPositions, newIndices, newNormals, newTexcoords)
        Dim newMesh = meshBuilder.ToMesh()
        Console.WriteLine($"Old Positions: {positions.Count} New Positions: {newPositions.Count}{vbCrLf}Old Indices: {indices.Count} New Indices: {newIndices.Count}{vbCrLf}Old Texcoords: {texcoords.Count} New Texcoords: {newTexcoords.Count}{vbCrLf}Old Normals: {normals.Count} New Normals: {newNormals.Count}")
        Return newMesh
    End Function

    Private viewport As New HelixViewport3D() With {
        .CameraRotationMode = CameraRotationMode.Turntable,
        .RotateAroundMouseDownPoint = True
    }

    Private host As New ElementHost With {
        .Dock = DockStyle.Fill,
        .Child = viewport
    }

    Private model As New ModelVisual3D()

    Private Sub LoadModel(objFileLocation As String)
        Dim reader = New ObjReader()
        model = New ModelVisual3D() With {.Content = reader.Read(objFileLocation)}
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
                If filter.Contains(ext) Then _
                    DarkTreeView1.Nodes(i).Nodes.Add(New DarkTreeNode(filenames(g) & "." & ext))
            Next
        Next
        Dim temp As DarkTreeNode() = (From node1 In DarkTreeView1.Nodes Where node1.Nodes.Count = 0).ToArray()
        For Each enode In temp
            DarkTreeView1.Nodes.Remove(enode)
        Next

        If Not previewMode Then
            TableLayoutPanel1.SetColumnSpan(DarkTreeView1, 2)
            DarkButton2.Hide()
        Else
            HexBox1.BackColor = AltUI.Config.ThemeProvider.Theme.Colors.OpaqueBackground
            HexBox1.ForeColor = AltUI.Config.ThemeProvider.Theme.Colors.LightText
            HexBox1.SelectionBackColor = AltUI.Config.ThemeProvider.GetAccentColor(0)
            HexBox1.SelectionForeColor = AltUI.Config.ThemeProvider.Theme.Colors.LightText
            Dim t = AltUI.Config.ThemeProvider.GetAccentColor(50)
            HexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(100, t.R, t.G, t.B)
        End If
    End Sub

    Private Sub DarkButton2_Click(sender As Object, e As EventArgs) Handles DarkButton2.Click
        Dim selectedNode = DarkTreeView1.SelectedNodes(0)
        extractFile(fullNameToGlobalIndex(selectedNode.Text.ToLower()), True)
    End Sub

End Class