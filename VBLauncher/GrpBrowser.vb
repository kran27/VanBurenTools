Imports System.IO
Imports System.Numerics
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
            Parallel.For(0, head.nEntries, Sub(i)
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
                    Dim model = B3DModelToUsableMesh(b3d)
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
                Throw New ArgumentOutOfRangeException()
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
        Dim ms = New MemoryStream()
        b.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
        Dim bitmapImage As New BitmapImage()
        bitmapImage.BeginInit()
        bitmapImage.StreamSource = ms
        bitmapImage.EndInit()
        Return bitmapImage
    End Function

    Private Sub DarkButton1_Click(sender As Object, e As EventArgs) Handles DarkTreeView1.DoubleClick, DarkButton1.Click
        For Each c As Control In TableLayoutPanel1.Controls
            If c IsNot DarkTreeView1 AndAlso c IsNot DarkButton1 Then
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
            Dim g As Integer = localIndexToGlobal(packIndex)(selectedNode.ParentNode.Nodes.IndexOf(selectedNode))
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
                    Dim mesh = B3DModelToUsableMesh(b3d)
                    model = New ModelVisual3D() With {.Content = mesh}
                    viewport.Children.Clear()
                    viewport.Children.Add(model)
                    viewport.Children.Add(New DefaultLights())
                    TableLayoutPanel1.Controls.Add(host, 1, 0)
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

    ' i do NOT want to talk about how this was where i had to end up
    Public Function CropBitmapByUV(bitmapImage As BitmapImage, uvCoordinates As List(Of Vector2)) As BitmapImage
        Dim minX As Double = uvCoordinates.Min(Function(p) p.X)
        Dim minY As Double = uvCoordinates.Min(Function(p) p.Y)
        Dim maxX As Double = uvCoordinates.Max(Function(p) p.X)
        Dim maxY As Double = uvCoordinates.Max(Function(p) p.Y)
        Dim width As Double = maxX - minX
        Dim height As Double = maxY - minY
        Dim croppedBitmap As New CroppedBitmap(bitmapImage,
                                  New Int32Rect(minX * bitmapImage.PixelWidth, minY * bitmapImage.PixelHeight,
                                                width * bitmapImage.PixelWidth, height * bitmapImage.PixelHeight))

        Dim bitmap As New Bitmap(croppedBitmap.PixelWidth, croppedBitmap.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb)
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

            Dim texA = New Point(g3d.Model_Vertex_Texcoords(face(0)).X / cs, g3d.Model_Vertex_Texcoords(face(0)).Y / cs)
            Dim texB = New Point(g3d.Model_Vertex_Texcoords(face(1)).X / cs, g3d.Model_Vertex_Texcoords(face(1)).Y / cs)
            Dim texC = New Point(g3d.Model_Vertex_Texcoords(face(2)).X / cs, g3d.Model_Vertex_Texcoords(face(2)).Y / cs)

            meshBuilder.AddTriangle(a, b, c, texA, texB, texC)
        Next
        Dim mesh = OptimizeMesh(meshBuilder)
        Dim material As Material
        Try
            Dim cropped = CropBitmapByUV(GetImageFromName("CR_Monsta_default_LG.tga"), g3d.Model_Vertex_Texcoords)
            material = New DiffuseMaterial(New ImageBrush(cropped))
        Catch
            material = New DiffuseMaterial(New SolidColorBrush(Colors.LightSlateGray))
        End Try
        Dim model = New GeometryModel3D With {
            .Geometry = mesh,
            .Material = material,
            .BackMaterial = material
        }

        ' rotate model 90 degrees around x axis and 90 degrees around z axis
        Dim rotateTransform = New RotateTransform3D(New AxisAngleRotation3D(New Vector3D(1, 0, 0), 90))
        Dim rotateTransform2 = New RotateTransform3D(New AxisAngleRotation3D(New Vector3D(0, 0, 1), 90))
        Dim transformGroup = New Transform3DGroup()
        transformGroup.Children.Add(rotateTransform)
        transformGroup.Children.Add(rotateTransform2)
        model.Transform = transformGroup

        Dim modelGroup As New Model3DGroup()
        modelGroup.Children.Add(model)

        Return modelGroup
    End Function

    Private Function B3DModelToUsableMesh(b3d As B3DModel) As Model3DGroup
        Dim meshBuilder = New MeshBuilder()
        For i = 0 To b3d.Model_Faces_Index.Count - 1
            Dim face = b3d.Model_Faces_Index(i)
            Dim a = New Point3D(b3d.Model_Vertex_Position(face(0)).X, b3d.Model_Vertex_Position(face(0)).Y, b3d.Model_Vertex_Position(face(0)).Z)
            Dim b = New Point3D(b3d.Model_Vertex_Position(face(1)).X, b3d.Model_Vertex_Position(face(1)).Y, b3d.Model_Vertex_Position(face(1)).Z)
            Dim c = New Point3D(b3d.Model_Vertex_Position(face(2)).X, b3d.Model_Vertex_Position(face(2)).Y, b3d.Model_Vertex_Position(face(2)).Z)
            Dim texA = New Point(b3d.Model_Vertex_Texcoords(face(0)).X, 1 - b3d.Model_Vertex_Texcoords(face(0)).Y)
            Dim texB = New Point(b3d.Model_Vertex_Texcoords(face(1)).X, 1 - b3d.Model_Vertex_Texcoords(face(1)).Y)
            Dim texC = New Point(b3d.Model_Vertex_Texcoords(face(2)).X, 1 - b3d.Model_Vertex_Texcoords(face(2)).Y)
            meshBuilder.AddTriangle(a, b, c, texA, texB, texC)
        Next
        Dim mesh = OptimizeMesh(meshBuilder)
        Dim material As Material
        If b3d.texName <> "" Then
            Try
                Dim cropped = CropBitmapByUV(GetImageFromName(b3d.texName), b3d.Model_Vertex_Texcoords)
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

        Dim rotateTransform = New RotateTransform3D(New AxisAngleRotation3D(New Vector3D(0, 0, 1), -90))
        model.Transform = rotateTransform

        Dim modelGroup As New Model3DGroup()
        modelGroup.Children.Add(model)

        Return modelGroup
    End Function

    ' removes exact duplicate vertices, while keeping texture coordinates and intact
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
        Dim index = 0

        For i = 0 To positions.Count - 1
            Dim pos = positions(i)
            Dim tex = texcoords(i)
            Dim norm = normals(i)
            Dim existingIndex = FindExistingVertexIndex(newPositions, newTexcoords, pos, tex)

            If existingIndex = -1 Then
                newPositions.Add(pos)
                newTexcoords.Add(tex)
                newNormals.Add(norm)
                newIndices.Add(index)
                index += 1
            Else
                newIndices.Add(existingIndex)
            End If
        Next

        meshBuilder.Append(newPositions, newIndices, newNormals, newTexcoords)
        Dim newMesh = meshBuilder.ToMesh()
        Console.WriteLine($"Old Positions: {positions.Count} New Positions: {newPositions.Count}{vbCrLf}Old Indices: {indices.Count} New Indices: {newIndices.Count}{vbCrLf}Old Texcoords: {texcoords.Count} New Texcoords: {newTexcoords.Count}{vbCrLf}Old Normals: {normals.Count} New Normals: {newNormals.Count}")
        Return newMesh
    End Function

    Private Function FindExistingVertexIndex(positions As List(Of Point3D), texcoords As List(Of Point), pos As Point3D, tex As Point) As Integer
        For i = 0 To positions.Count - 1
            If positions(i) = pos AndAlso texcoords(i) = tex Then
                Return i
            End If
        Next
        Return -1
    End Function

    Private viewport As New HelixViewport3D() With {
        .CameraRotationMode = CameraRotationMode.Trackball,
        .IsHeadLightEnabled = True
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
        Dim parentName = selectedNode.ParentNode.Text
        Dim packIndex = grpnames.IndexOf(parentName)
        Dim g As Integer = localIndexToGlobal(packIndex)(selectedNode.ParentNode.Nodes.IndexOf(selectedNode))
        extractFile(g)
    End Sub

End Class