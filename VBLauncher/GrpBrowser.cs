using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using AltUI.Controls;
using Be.Windows.Forms;
using HelixToolkit.Wpf;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using VB3DLib;

namespace VBLauncher
{

    public partial class GrpBrowser
    {
        public byte[] FileBytes { get; set; }
        public string Extension { get; set; }
        public string FileName { get; set; }
        private bool previewMode { get; set; }

        #region .rht/.grp Reading

        private string[] filter = new[] { "psf", "int", "rle", "itm", "veg", "crt", "map", "wav", "amx", "rtd", "rlz", "ini", "skl", "gr2", "skn", "8", "dlg", "str", "sst", "use", "arm", "dor", "wea", "pce", "sco", "gsf", "gls", "fnt", "spl", "pro", "enc", "wmp", "amo", "con", "tok", "b3d", "g3d", "tga", "bmp" };

        public GrpBrowser(string[] filter, bool previewMode = false)
        {
            host = new ElementHost()
            {
                Name = "Model Viewer",
                Dock = DockStyle.Fill,
                Child = viewport
            };
            this.previewMode = previewMode;
            if (filter is not null && !(filter.Count() == 0))
                this.filter = filter;
            InitializeComponent();
        }

        internal struct F3RHTHeader
        {
            public int vMajor;
            public int vMinor;
            public int nEntries; // number of file entries
            public int offsetPacks; // offset to PACK file data
            public int offsetResources; // offset to file name strings
        }

        internal struct Entry
        {
            public int one; // always 1 ????
            public int number; // number of file in PACK file
            public int @type; // type of file as defined in ftype enum
            public int offset; // string offset (offsetResources+offset)
            public int pack;
        }

        internal struct F3GRPHeader
        {
            public int vMajor;
            public int vMinor;
            public int nFiles; // number of files in PACK
        }

        internal struct Lump
        {
            public int offset;
            public int length;
        }

        private F3RHTHeader head;
        private List<Entry>[] entries;
        private List<string> grpnames;
        private List<string> filenames;
        private Dictionary<int, int> globalIndexToPackOffset = new Dictionary<int, int>();
        private Dictionary<int, int>[] localIndexToGlobal;
        private Dictionary<int, int> globalIndexToLocal;
        private Dictionary<string, int> fullNameToGlobalIndex = new Dictionary<string, int>();
        private List<string> extensions;

        // TODO: Automate this for proper support instead of hardcoded to this rht
        private Dictionary<int, int> packOffsetToIndex = new Dictionary<int, int>() { { 0, 0 }, { 6, 1 }, { 16, 2 }, { 25, 3 }, { 32, 4 }, { 42, 5 }, { 50, 6 }, { 55, 7 }, { 63, 8 }, { 68, 9 }, { 74, 10 }, { 81, 11 }, { 87, 12 }, { 92, 13 }, { 97, 14 }, { 102, 15 }, { 107, 16 }, { 112, 17 }, { 117, 18 }, { 122, 19 }, { 127, 20 }, { 132, 21 }, { 137, 22 }, { 142, 23 } };

        private string GetExtension(int a1)
        {
            var extensionMap = new Dictionary<int, string>() { { 1900, "psf" }, { 1600, "int" }, { 1700, "rle" }, { 1800, "itm" }, { 1500, "veg" }, { 1400, "crt" }, { 1300, "map" }, { 1100, "wav" }, { 1200, "amx" }, { 1000, "rtd" }, { 900, "rlz" }, { 800, "ini" }, { 600, "skl" }, { 700, "gr2" }, { 500, "skn" }, { 400, "8" }, { 300, "dlg" }, { 100, "model" }, { 200, "image" }, { 2900, "str" }, { 2800, "sst" }, { 2700, "use" }, { 2500, "arm" }, { 2600, "dor" }, { 2400, "wea" }, { 2300, "pce" }, { 2200, "sco" }, { 2000, "gsf" }, { 2100, "gls" }, { 3400, "fnt" }, { 3300, "spl" }, { 3200, "pro" }, { 3000, "enc" }, { 3100, "wmp" }, { 3500, "amo" }, { 3600, "con" }, { 3700, "tok" } };

            string value = null;
            if (extensionMap.TryGetValue(a1, out value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public void extractFile(int index, bool convert = false)
        {
            byte[] b;
            if (index == -1)
            {
                for (int i = 0, loopTo = head.nEntries - 1; i <= loopTo; i++)
                {
                    // Parallel.For(0, head.nEntries,
                    // Sub[i]
                    string ext = extensions[i];
                    Console.WriteLine(filenames[i] + "." + ext);
                    if (convert)
                    {
                        if (ext == "tga")
                            ext = "png";
                        if (ext == "b3d" || ext == "g3d")
                            ext = "obj";
                    }
                    b = getFileBytes(i, convert);
                    File.WriteAllBytes(grpnames[packOffsetToIndex[globalIndexToPackOffset[i]]] + @"\" + filenames[i] + "." + ext, b);
                    GC.Collect(); // auto GC is too slow and will run out of memory otherwise
                                  // End Sub)
                }
            }
            else
            {
                string ext = extensions[index];
                Console.WriteLine(filenames[index] + "." + ext);
                if (convert)
                {
                    if (ext == "tga")
                        ext = "png";
                    if (ext == "b3d" || ext == "g3d")
                        ext = "obj";
                }
                b = getFileBytes(index, convert);
                File.WriteAllBytes(grpnames[packOffsetToIndex[globalIndexToPackOffset[index]]] + @"\" + filenames[index] + "." + ext, b);
            }
            GC.Collect();
        }

        private byte[] getFileBytes(int index, bool convert = false)
        {
            int packLoc = globalIndexToPackOffset[index];
            int localIndex = globalIndexToLocal[index];
            Directory.CreateDirectory(grpnames[packOffsetToIndex[packLoc]]);
            string grpname = $@"data\{grpnames[packOffsetToIndex[packLoc]]}.grp";
            string extname = $@"{grpnames[packOffsetToIndex[packLoc]]}\{filenames[index]}";
            return getFileBytes(grpname, entries[packOffsetToIndex[packLoc]][localIndex].number, entries[packOffsetToIndex[packLoc]][localIndex].type, extname, convert);
        }

        private byte[] getFileBytes(string packname, int filenumber, int filetype, string filename, bool convert = false)
        {
            var fs = new FileStream(packname, FileMode.Open, FileAccess.Read);
            var br = new BinaryReader(fs);
            F3GRPHeader grpHeader;
            grpHeader.vMajor = br.ReadInt32();
            grpHeader.vMinor = br.ReadInt32();
            grpHeader.nFiles = br.ReadInt32();

            var lumps = new Lump[grpHeader.nFiles + 1];
            for (int i = 0, loopTo = grpHeader.nFiles - 1; i <= loopTo; i++)
            {
                lumps[i].offset = br.ReadInt32();
                lumps[i].length = br.ReadInt32();
            }
            var buffer = new byte[lumps[filenumber].length + 1];
            fs.Seek(lumps[filenumber].offset, SeekOrigin.Begin);
            fs.Read(buffer, 0, lumps[filenumber].length);
            fs.Close();

            string ext = GetExtension(filetype);
            if (ext == "model")
            {
                // check if buffer starts with "B3D "
                if (buffer[0] == 66 & buffer[1] == 51 & buffer[2] == 68 & buffer[3] == 32)
                {
                    ext = "b3d";
                }
                else
                {
                    ext = "g3d";
                }
            }
            else if (ext == "image")
            {
                // check if buffer starts with "BM"
                if (buffer[0] == 66 & buffer[1] == 77)
                {
                    ext = "bmp";
                }
                else
                {
                    ext = "tga";
                }
            }

            byte[] b;
            b = new byte[lumps[filenumber].length];
            Array.Copy(buffer, b, lumps[filenumber].length);

            if (convert)
            {
                if (ext == "tga")
                {
                    using (var stream = new MemoryStream())
                    {
                        TargaToBitmap(b).Save(stream, ImageFormat.Png);
                        return stream.ToArray();
                    }
                }
                else if (ext == "b3d")
                {
                    try
                    {
                        var b3d = new B3DModel(b);
                        var model = B3DModelToUsableMesh(b3d, filename);
                        // Create an instance of ObjExporter
                        var exporter = new ObjExporter();
                        exporter.MaterialsFile = filename + ".mtl";
                        // Create a MemoryStream
                        using (var memoryStream = new MemoryStream())
                        {
                            // Export the Model3DGroup to the MemoryStream
                            exporter.Export(model, memoryStream);
                            // Convert the MemoryStream to a byte array
                            byte[] byteArray = memoryStream.ToArray();
                            return byteArray;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else if (ext == "g3d")
                {
                    var g3d = new G3DModel(b);
                    var model = G3DModelToUsableMesh(g3d);
                    // Create an instance of ObjExporter
                    var exporter = new ObjExporter();
                    exporter.MaterialsFile = filename + ".mtl";
                    // Create a MemoryStream
                    using (var memoryStream = new MemoryStream())
                    {
                        // Export the Model3DGroup to the MemoryStream
                        exporter.Export(model, memoryStream);
                        // Convert the MemoryStream to a byte array
                        byte[] byteArray = memoryStream.ToArray();
                        return byteArray;
                    }
                }
            }

            return b;
        }

        public void readExtensions()
        {
            extensions = new List<string>();

            for (int i = 0, loopTo = head.nEntries - 1; i <= loopTo; i++) // can't use Parallel.For to speed up
            {
                int packLoc = globalIndexToPackOffset[i];
                int localIndex = globalIndexToLocal[i];
                Directory.CreateDirectory(grpnames[packOffsetToIndex[packLoc]]);
                string grpname = $@"data\{grpnames[packOffsetToIndex[packLoc]]}.grp";
                string extname = $@"{grpnames[packOffsetToIndex[packLoc]]}\{filenames[i]}";
                readExtensions(grpname, entries[packOffsetToIndex[packLoc]][localIndex].number, entries[packOffsetToIndex[packLoc]][localIndex].type, extname);
                fullNameToGlobalIndex.Add((filenames[i] + "." + extensions[i]).ToLower(), i);
            }
        }

        private void readExtensions(string packname, int filenumber, int filetype, string filename)
        {
            var fs = new FileStream(packname, FileMode.Open, FileAccess.Read);
            var br = new BinaryReader(fs);
            F3GRPHeader grpHeader;
            grpHeader.vMajor = br.ReadInt32();
            grpHeader.vMinor = br.ReadInt32();
            grpHeader.nFiles = br.ReadInt32();

            var lumps = new Lump[grpHeader.nFiles + 1];
            for (int i = 0, loopTo = grpHeader.nFiles - 1; i <= loopTo; i++)
            {
                lumps[i].offset = br.ReadInt32();
                lumps[i].length = br.ReadInt32();
            }
            var buffer = new byte[lumps[filenumber].length + 1];
            fs.Seek(lumps[filenumber].offset, SeekOrigin.Begin);
            fs.Read(buffer, 0, lumps[filenumber].length);
            fs.Close();

            string ext = GetExtension(filetype);
            if (ext == "model")
            {
                // check if buffer starts with "B3D "
                if (buffer[0] == 66 & buffer[1] == 51 & buffer[2] == 68 & buffer[3] == 32)
                {
                    ext = "b3d";
                }
                else
                {
                    ext = "g3d";
                }
            }
            else if (ext == "image")
            {
                // check if buffer starts with "BM"
                if (buffer[0] == 66 & buffer[1] == 77)
                {
                    ext = "bmp";
                }
                else
                {
                    ext = "tga";
                }
            }
            extensions.Add(ext);
        }

        public void ReadRHT()
        {
            var fs = new FileStream("resource.rht", FileMode.Open, FileAccess.Read);
            var br = new BinaryReader(fs);
            head.vMajor = br.ReadInt32();
            head.vMinor = br.ReadInt32();
            head.nEntries = br.ReadInt32();
            head.offsetPacks = br.ReadInt32();
            head.offsetResources = br.ReadInt32();

            entries = (List<Entry>[])Array.CreateInstance(typeof(List<Entry>), 24);
            localIndexToGlobal = (Dictionary<int, int>[])Array.CreateInstance(typeof(Dictionary<int, int>), 24);
            globalIndexToLocal = new Dictionary<int, int>();

            for (int i = 0, loopTo = entries.Length - 1; i <= loopTo; i++)
            {
                entries[i] = new List<Entry>();
                localIndexToGlobal[i] = new Dictionary<int, int>();
            }

            for (int i = 0, loopTo1 = head.nEntries - 1; i <= loopTo1; i++)
            {
                Entry entry;
                entry.one = br.ReadInt32();
                entry.number = br.ReadInt32();
                entry.type = br.ReadInt32();
                entry.offset = br.ReadInt32();
                entry.pack = br.ReadInt32();
                globalIndexToPackOffset.Add(i, entry.pack);
                localIndexToGlobal[packOffsetToIndex[entry.pack]].Add(entry.number, i);
                globalIndexToLocal.Add(i, entry.number);
                entries[packOffsetToIndex[entry.pack]].Add(entry);
            }
            grpnames = new List<string>();
            filenames = new List<string>();
            var sb = new StringBuilder();
            fs.Seek(head.offsetPacks, SeekOrigin.Begin);
            for (int i = 0, loopTo2 = head.offsetResources - head.offsetPacks; i <= loopTo2; i++)
            {
                int c = fs.ReadByte();
                if (c == 0)
                {
                    grpnames.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(Strings.ChrW(c));
                }
            }
            for (long i = 0L, loopTo3 = fs.Length - head.offsetResources; i <= loopTo3; i++)
            {
                int c = fs.ReadByte();
                if (c == 0)
                {
                    filenames.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(Strings.ChrW(c));
                }
            }
        }

        #endregion

        private Bitmap TargaToBitmap(byte[] b)
        {
            var ms = new MemoryStream(b);
            var image = Pfim.Pfimage.FromStream(ms);
            var format = default(System.Drawing.Imaging.PixelFormat);
            switch (image.Format)
            {
                case Pfim.ImageFormat.Rgb8:
                    {
                        format = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
                        break;
                    }
                case Pfim.ImageFormat.R5g5b5:
                    {
                        format = System.Drawing.Imaging.PixelFormat.Format16bppRgb555;
                        break;
                    }
                case Pfim.ImageFormat.R5g6b5:
                    {
                        format = System.Drawing.Imaging.PixelFormat.Format16bppRgb565;
                        break;
                    }
                case Pfim.ImageFormat.R5g5b5a1:
                    {
                        format = System.Drawing.Imaging.PixelFormat.Format16bppArgb1555;
                        break;
                    }
                case Pfim.ImageFormat.Rgba16:
                    {
                        // TODO this is probably wrong
                        format = System.Drawing.Imaging.PixelFormat.Format16bppGrayScale;
                        break;
                    }
                case Pfim.ImageFormat.Rgb24:
                    {
                        format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                        break;
                    }
                case Pfim.ImageFormat.Rgba32:
                    {
                        format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                        break;
                    }

                default:
                    {
                        Interaction.MsgBox(image.Format.ToString());
                        break;
                    }
            }
            // pin image data as the picture box can outlive the pfim image object
            // which, unless pinned, will garbage collect the data array causing image corruption
            var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
            var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
            return new Bitmap(image.Width, image.Height, image.Stride, format, ptr);
        }

        private BitmapImage TargaToBitmapImage(byte[] b)
        {
            try
            {
                var ms = new MemoryStream(b);
                var image = Pfim.Pfimage.FromStream(ms);
                var bmpSource = BitmapSource.Create(image.Width, image.Height, 96.0d, 96.0d, PixelFormats.Bgra32, null, image.Data, image.Stride);
                // Convert the BitmapSource to a BitmapImage
                var bitmapImage = new BitmapImage();
                var memoryStream = new MemoryStream();
                var encoder = new PngBitmapEncoder(); // Use PNG encoder as BitmapImage does not support the TGA format
                encoder.Frames.Add(BitmapFrame.Create(bmpSource));
                encoder.Save(memoryStream);
                memoryStream.Position = 0L;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();

                return bitmapImage;
            }
            catch
            {
                return BitmapToBitmapImage(TargaToBitmap(b));
            }
        }

        private BitmapImage BitmapToBitmapImage(Bitmap b)
        {
            using (var memory = new MemoryStream())
            {
                b.Save(memory, ImageFormat.Png);
                memory.Position = 0L;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        private object lastIndex = -1;

        private void DarkButton1_Click(object sender, EventArgs e)
        {
            if (!DarkTreeView1.SelectedNodes.Any())
                return;
            var selectedNode = DarkTreeView1.SelectedNodes[0];
            if (selectedNode.Nodes.Count != 0)
                return;
            int g = fullNameToGlobalIndex[selectedNode.Text.ToLower()];
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(lastIndex, g, false)))
                return;
            lastIndex = g;
            DarkSectionPanel2.Controls.Clear();
            string name = selectedNode.Text;
            string ext = name.Split(".")[1];
            name = name.Split(".")[0];
            string parentName = selectedNode.ParentNode.Text;
            // find entry with matching name
            int packIndex = grpnames.IndexOf(parentName);
            if (previewMode)
            {
                byte[] b;
                b = getFileBytes(g);
                switch (ext ?? "")
                {
                    case "bmp":
                        {
                            DarkSectionPanel2.Controls.Add(PictureBox1);
                            var ms = new MemoryStream(b);
                            PictureBox1.BackgroundImage = new Bitmap(ms);
                            break;
                        }
                    case "tga":
                        {
                            DarkSectionPanel2.Controls.Add(PictureBox1);
                            PictureBox1.BackgroundImage = TargaToBitmap(b);
                            break;
                        }
                    case "psf":
                    case "ini":
                    case "pce":
                    case "sco":
                    case "gsf":
                    case "gls":
                    case "tok":
                        {
                            DarkSectionPanel2.Controls.Add(DarkRichTextBox1);
                            DarkRichTextBox1.Text = Encoding.ASCII.GetString(b);
                            break;
                        }
                    case "g3d":
                        {
                            var g3d = new G3DModel(b);
                            var mesh = G3DModelToUsableMesh(g3d);
                            model = new ModelVisual3D() { Content = mesh };
                            viewport.Children.Clear();
                            viewport.Children.Add(model);
                            viewport.Children.Add(new DefaultLights());
                            DarkSectionPanel2.Controls.Add(host);
                            break;
                        }
                    case "b3d":
                        {
                            var b3d = new B3DModel(b);
                            Console.WriteLine(b3d.Model_Vertex_Position.Count);
                            Console.WriteLine(b3d.Vertex_Nodes.Count);
                            var mesh = B3DModelToUsableMesh(b3d, selectedNode.Text);
                            model = new ModelVisual3D() { Content = mesh };
                            viewport.Children.Clear();
                            viewport.Children.Add(model);
                            try
                            {
                                byte[] gr2b = getFileBytes(fullNameToGlobalIndex[name.ToLower() + ".skl"]);
                                var gr2 = GrannyFormats.ReadFileFromMemory(gr2b);
                                if (gr2.Models.Any())
                                {
                                    var skl = GetSkeletonModel(gr2.Models[0]);
                                    var model2 = new ModelVisual3D() { Content = skl };
                                    viewport.Children.Add(model2);
                                }
                            }
                            catch
                            {
                            }
                            viewport.Children.Add(new DefaultLights());
                            DarkSectionPanel2.Controls.Add(host);
                            break;
                        }
                    case "8":
                        {
                            var _8 = new _8Model(b, (int)Interaction.MsgBox("FLGS Size?", MsgBoxStyle.YesNo, "Option") == (int)DialogResult.Yes);
                            var mesh = _8ModelToUsableMesh(_8);
                            model = new ModelVisual3D() { Content = mesh };
                            viewport.Children.Clear();
                            viewport.Children.Add(model);
                            viewport.Children.Add(new DefaultLights());
                            DarkSectionPanel2.Controls.Add(host);
                            break;
                        }
                    case "map":
                        {
                            viewport.Children.Clear();
                            VBLauncher.Map map;
                            try
                            {
                                map = b.ReadMap();
                            }
                            catch
                            {
                                DarkSectionPanel2.Controls.Add(HexBox1);
                                HexBox1.ByteProvider = new DynamicByteProvider(b);
                                return;
                            }

                            string mmn = map.EMAP.s1.ToLower();
                            try
                            {
                                int mgi = fullNameToGlobalIndex[mmn];
                                byte[] mmb = getFileBytes(mgi);

                                var _8 = new _8Model(mmb, (int)Interaction.MsgBox("FLGS Size?", MsgBoxStyle.YesNo, "Option") == (int)DialogResult.Yes);
                                var mapMesh = _8ModelToUsableMesh(_8);
                                var mapModel = new ModelVisual3D() { Content = mapMesh };

                                viewport.Children.Add(mapModel);
                            }
                            catch
                            {
                            }
                            foreach (var ent in map.EME2)
                            {
                                int gInd = 0;
                                try
                                {
                                    gInd = fullNameToGlobalIndex[ent.name.ToLower()];
                                }
                                catch
                                {
                                    continue;
                                }

                                Model3DGroup m;

                                if (ent.name.ToLower().EndsWith(".crt"))
                                {
                                    m = CRTToUsableMesh(getFileBytes(gInd));
                                }
                                else
                                {
                                    var c = VBLauncher.Extensions.ToEEN2c(getFileBytes(gInd));
                                    // EEN2 is the first chunk for all entities, only .CRT requires special processing.
                                    int gInd2 = fullNameToGlobalIndex[c.skl.ToLower() + ".b3d"];
                                    var b3d = new B3DModel(getFileBytes(gInd2));
                                    m = B3DModelToUsableMesh(b3d, selectedNode.Text);
                                }

                                var t = new Transform3DGroup();

                                double rot = (double)(ent.l.r * 180f) / Math.PI; // convert to degrees

                                t.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0d, 0d, 1d), rot)));
                                t.Children.Add(new TranslateTransform3D((double)ent.l.x, (double)ent.l.y, (double)ent.l.z));

                                m.Transform = t;

                                var mv3 = new ModelVisual3D() { Content = m };

                                viewport.Children.Add(mv3);
                            }

                            foreach (var ep in map.EMEP) // adds default player character to map entry points
                            {
                                int gInd = fullNameToGlobalIndex["default_pc.crt"];
                                var crt = CRTToUsableMesh(getFileBytes(gInd));

                                var t = new Transform3DGroup();

                                double rot = (double)(ep.r * 180f) / Math.PI;

                                t.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0d, 0d, 1d), rot)));
                                t.Children.Add(new TranslateTransform3D((double)ep.p.x, (double)ep.p.y, (double)ep.p.z));

                                crt.Transform = t;

                                var mv3 = new ModelVisual3D() { Content = crt };

                                viewport.Children.Add(mv3);
                            }

                            viewport.Children.Add(new DefaultLights());
                            DarkSectionPanel2.Controls.Add(host);
                            break;
                        }
                    case "crt":
                        {
                            // Try
                            var crt = CRTToUsableMesh(b);
                            model = new ModelVisual3D() { Content = crt };
                            viewport.Children.Clear();
                            viewport.Children.Add(model);
                            viewport.Children.Add(new DefaultLights());
                            DarkSectionPanel2.Controls.Add(host);
                            break;
                        }
                    // Catch
                    // DarkSectionPanel2.Controls.Add(HexBox1)
                    // HexBox1.ByteProvider = New DynamicByteProvider(b)
                    // End Try
                    case "gr2":
                    case "skl":
                        {
                            var gr2 = GrannyFormats.ReadFileFromMemory(b);
                            Console.WriteLine(gr2.FromFileName);
                            if (gr2.Models.Any())
                            {
                                var skl = GetSkeletonModel(gr2.Models[0]);
                                model = new ModelVisual3D() { Content = skl };
                                viewport.Children.Clear();
                                viewport.Children.Add(model);
                                viewport.Children.Add(new DefaultLights());
                                DarkSectionPanel2.Controls.Add(host);
                            }

                            break;
                        }

                    default:
                        {
                            DarkSectionPanel2.Controls.Add(HexBox1);
                            HexBox1.ByteProvider = new DynamicByteProvider(b);
                            break;
                        }
                }
            }
            else
            {
                foreach (var curG in entries[packIndex].Select(ent => localIndexToGlobal[packIndex][ent.number]).Where(g1 => filenames[g1] == name))
                {
                    FileBytes = getFileBytes(curG);
                    Extension = extensions[curG];
                    FileName = filenames[curG];
                    break;
                }
                Close();
            }
        }

        private BitmapImage GetImageFromName(string name)
        {
            string nne = name.Split(".")[0];
            for (int i = 0, loopTo = entries.Length - 1; i <= loopTo; i++)
            {
                foreach (var g in from e in entries[i]
                                  let g1 = localIndexToGlobal[i][e.number]
                                  where filenames[g1] == nne && GetExtension(e.type) == "image"
                                  select g1)
                    return TargaToBitmapImage(getFileBytes(g));
            }

            return default;
        }

        private Model3DGroup GetSkeletonModel(GrannyFormats.model m)
        {
            var mb = new MeshBuilder();
            var baseTranslate = Matrix4x4.CreateTranslation(m.InitialPlacement.Position);
            foreach (GrannyFormats.bone b in m.Skeleton.Bones)
            {
                var p1 = b.ActualPosition;
                var p2 = new Vector3(0f, 0f, 0f);
                if (b.ParentIndex != -1)
                {
                    p2 = m.Skeleton.Bones[b.ParentIndex].ActualPosition;
                }
                p1 *= 10; // match scale of b3d
                p2 *= 10f; // 
                mb.AddArrow(new Point3D(p2.X, p2.Y, p2.Z), new Point3D(p1.X, p1.Y, p1.Z), 0.01d);
                Console.WriteLine(b.Name);
                Console.WriteLine($"X: {p1.X} Y: {p1.Y} Z: {p1.Z}");
            }

            var mesh = mb.ToMesh();
            var material = new EmissiveMaterial(new SolidColorBrush(Colors.White));
            var model = new GeometryModel3D()
            {
                Geometry = mesh,
                Material = material,
                BackMaterial = material
            };

            var rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1d, 0d, 0d), 90d));
            var rotateTransform2 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0d, 0d, 1d), 180d));
            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(rotateTransform);
            transformGroup.Children.Add(rotateTransform2);
            model.Transform = transformGroup;

            var modelGroup = new Model3DGroup();
            modelGroup.Children.Add(model);

            return modelGroup;
        }

        private Model3DGroup CRTToUsableMesh(byte[] b)
        {
            var crt = b.ReadCRT();
            if (crt.EEN2.skl.StartsWith("Hum"))
            {
                string baseModel = (crt.EEN2.skl + ".b3d").ToLower();
                string heaModel = (crt.EEN2.skl + "_" + crt.GCRE.Hea.Model + ".b3d").ToLower();
                string haiModel = (crt.EEN2.skl + "_" + crt.GCRE.Hai.Model + ".b3d").ToLower();
                string ponModel = (crt.EEN2.skl + "_" + crt.GCRE.Pon.Model + ".b3d").ToLower();
                string musModel = (crt.EEN2.skl + "_" + crt.GCRE.Mus.Model + ".b3d").ToLower();
                string beaModel = (crt.EEN2.skl + "_" + crt.GCRE.Bea.Model + ".b3d").ToLower();
                string eyeModel = (crt.EEN2.skl + "_" + crt.GCRE.Eye.Model + ".b3d").ToLower();
                string bodModel = (crt.EEN2.skl + "_" + crt.GCRE.Bod.Model + ".b3d").ToLower();
                string hanModel = (crt.EEN2.skl + "_" + crt.GCRE.Han.Model + ".b3d").ToLower();
                string feeModel = (crt.EEN2.skl + "_" + crt.GCRE.Fee.Model + ".b3d").ToLower();
                string bacModel = (crt.EEN2.skl + "_" + crt.GCRE.Bac.Model + ".b3d").ToLower();
                string shoModel = (crt.EEN2.skl + "_" + crt.GCRE.Sho.Model + ".b3d").ToLower();
                string vanModel = (crt.EEN2.skl + "_" + crt.GCRE.Van.Model + ".b3d").ToLower();

                var heaTex = default(string);
                var eyeTex = default(string);
                var bodTex = default(string);
                var hanTex = default(string);
                var feeTex = default(string);
                var bacTex = default(string);
                var shoTex = default(string);
                var vanTex = default(string);

                foreach (var itm in from i in crt.GCRE.Inventory
                                    where i.ToLower().EndsWith(".arm")
                                    select getFileBytes(fullNameToGlobalIndex[i.ToLower()]).ReadITM())
                {
                    if (!string.IsNullOrEmpty(itm.GITM.Bac.Model))
                        bacModel = (crt.EEN2.skl + "_" + itm.GITM.Bac.Model + ".b3d").ToLower();
                    if (!string.IsNullOrEmpty(itm.GITM.Bod.Model))
                        bodModel = (crt.EEN2.skl + "_" + itm.GITM.Bod.Model + ".b3d").ToLower();
                    if (!string.IsNullOrEmpty(itm.GITM.Fee.Model))
                        feeModel = (crt.EEN2.skl + "_" + itm.GITM.Fee.Model + ".b3d").ToLower();
                    if (!string.IsNullOrEmpty(itm.GITM.Han.Model))
                        hanModel = (crt.EEN2.skl + "_" + itm.GITM.Han.Model + ".b3d").ToLower();
                    if (!string.IsNullOrEmpty(itm.GITM.Hea.Model))
                        heaModel = (crt.EEN2.skl + "_" + itm.GITM.Hea.Model + ".b3d").ToLower();
                    if (!string.IsNullOrEmpty(itm.GITM.Sho.Model))
                        shoModel = (crt.EEN2.skl + "_" + itm.GITM.Sho.Model + ".b3d").ToLower();
                    if (!string.IsNullOrEmpty(itm.GITM.Van.Model))
                        vanModel = (crt.EEN2.skl + "_" + itm.GITM.Van.Model + ".b3d").ToLower();
                    heaTex = itm.GITM.Hea.Tex.Replace(".dds", ".tga").ToLower();
                    eyeTex = itm.GITM.Eye.Tex.Replace(".dds", ".tga").ToLower();
                    bodTex = itm.GITM.Bod.Tex.Replace(".dds", ".tga").ToLower();
                    hanTex = itm.GITM.Han.Tex.Replace(".dds", ".tga").ToLower();
                    feeTex = itm.GITM.Fee.Tex.Replace(".dds", ".tga").ToLower();
                    bacTex = itm.GITM.Bac.Tex.Replace(".dds", ".tga").ToLower();
                    shoTex = itm.GITM.Sho.Tex.Replace(".dds", ".tga").ToLower();
                    vanTex = itm.GITM.Van.Tex.Replace(".dds", ".tga").ToLower();
                }

                var baseB3D = new B3DModel(getFileBytes(fullNameToGlobalIndex[baseModel]));
                var baseMesh = B3DToNoMatMesh(baseB3D);

                var heaB3D = default(B3DModel);
                var haiB3D = default(B3DModel);
                var ponB3D = default(B3DModel);
                var musB3D = default(B3DModel);
                var beaB3D = default(B3DModel);
                var eyeB3D = default(B3DModel);
                var bodB3D = default(B3DModel);
                var hanB3D = default(B3DModel);
                var feeB3D = default(B3DModel);
                var bacB3D = default(B3DModel);
                var shoB3D = default(B3DModel);
                var vanB3D = default(B3DModel);

                MeshGeometry3D heaMesh = null;
                MeshGeometry3D haiMesh = null;
                MeshGeometry3D ponMesh = null;
                MeshGeometry3D musMesh = null;
                MeshGeometry3D beaMesh = null;
                MeshGeometry3D eyeMesh = null;
                MeshGeometry3D bodMesh = null;
                MeshGeometry3D hanMesh = null;
                MeshGeometry3D feeMesh = null;
                MeshGeometry3D bacMesh = null;
                MeshGeometry3D shoMesh = null;
                MeshGeometry3D vanMesh = null;

                if ((heaModel ?? "") != (crt.EEN2.skl.ToLower() + "_.b3d" ?? ""))
                {
                    heaB3D = new B3DModel(getFileBytes(fullNameToGlobalIndex[heaModel]));
                    heaMesh = B3DToNoMatMesh(heaB3D);
                }
                if ((haiModel ?? "") != (crt.EEN2.skl.ToLower() + "_.b3d" ?? ""))
                {
                    haiB3D = new B3DModel(getFileBytes(fullNameToGlobalIndex[haiModel]));
                    haiMesh = B3DToNoMatMesh(haiB3D);
                }
                if ((ponModel ?? "") != (crt.EEN2.skl.ToLower() + "_.b3d" ?? ""))
                {
                    ponB3D = new B3DModel(getFileBytes(fullNameToGlobalIndex[ponModel]));
                    ponMesh = B3DToNoMatMesh(ponB3D);
                }
                if ((musModel ?? "") != (crt.EEN2.skl.ToLower() + "_.b3d" ?? ""))
                {
                    musB3D = new B3DModel(getFileBytes(fullNameToGlobalIndex[musModel]));
                    musMesh = B3DToNoMatMesh(musB3D);
                }
                if ((beaModel ?? "") != (crt.EEN2.skl.ToLower() + "_.b3d" ?? ""))
                {
                    beaB3D = new B3DModel(getFileBytes(fullNameToGlobalIndex[beaModel]));
                    beaMesh = B3DToNoMatMesh(beaB3D);
                }
                if ((eyeModel ?? "") != (crt.EEN2.skl.ToLower() + "_.b3d" ?? ""))
                {
                    eyeB3D = new B3DModel(getFileBytes(fullNameToGlobalIndex[eyeModel]));
                    eyeMesh = B3DToNoMatMesh(eyeB3D);
                }
                if ((bodModel ?? "") != (crt.EEN2.skl.ToLower() + "_.b3d" ?? ""))
                {
                    bodB3D = new B3DModel(getFileBytes(fullNameToGlobalIndex[bodModel]));
                    bodMesh = B3DToNoMatMesh(bodB3D);
                }
                if ((hanModel ?? "") != (crt.EEN2.skl.ToLower() + "_.b3d" ?? ""))
                {
                    hanB3D = new B3DModel(getFileBytes(fullNameToGlobalIndex[hanModel]));
                    hanMesh = B3DToNoMatMesh(hanB3D);
                }
                if ((feeModel ?? "") != (crt.EEN2.skl.ToLower() + "_.b3d" ?? ""))
                {
                    feeB3D = new B3DModel(getFileBytes(fullNameToGlobalIndex[feeModel]));
                    feeMesh = B3DToNoMatMesh(feeB3D);
                }
                if ((bacModel ?? "") != (crt.EEN2.skl.ToLower() + "_.b3d" ?? ""))
                {
                    bacB3D = new B3DModel(getFileBytes(fullNameToGlobalIndex[bacModel]));
                    bacMesh = B3DToNoMatMesh(bacB3D);
                }
                if ((shoModel ?? "") != (crt.EEN2.skl.ToLower() + "_.b3d" ?? ""))
                {
                    shoB3D = new B3DModel(getFileBytes(fullNameToGlobalIndex[shoModel]));
                    shoMesh = B3DToNoMatMesh(shoB3D);
                }
                if ((vanModel ?? "") != (crt.EEN2.skl.ToLower() + "_.b3d" ?? ""))
                {
                    vanB3D = new B3DModel(getFileBytes(fullNameToGlobalIndex[vanModel]));
                    vanMesh = B3DToNoMatMesh(vanB3D);
                }

                var x = new Bitmap(512, 512);
                using (var gr = Graphics.FromImage(x))
                {
                    string baseTex = !string.IsNullOrEmpty(crt.EEN2.EEOV.s4) ? crt.EEN2.EEOV.s4.Replace(".dds", ".tga").ToLower() : baseB3D.texName.ToLower();
                    var baseImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex[baseTex]));
                    gr.DrawImageUnscaled(baseImage, 0, 0);
                    if (heaMesh is not null)
                    {
                        if (string.IsNullOrEmpty(heaTex))
                            heaTex = !string.IsNullOrEmpty(crt.GCRE.Hea.Tex) ? crt.GCRE.Hea.Tex.Replace(".dds", ".tga").ToLower() : heaB3D.texName.ToLower();
                        var heaImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex[heaTex]));
                        gr.DrawImageUnscaled(heaImage, 384, 0);
                    }
                    if (haiMesh is not null)
                    {
                        string haiTex = !string.IsNullOrEmpty(crt.GCRE.Hai.Tex) ? crt.GCRE.Hai.Tex.Replace(".dds", ".tga").ToLower() : haiB3D.texName.ToLower();
                        var haiImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex[haiTex]));
                        gr.DrawImageUnscaled(haiImage, 256, 128);
                    }
                    if (ponMesh is not null)
                    {
                        string ponTex = !string.IsNullOrEmpty(crt.GCRE.Pon.Tex) ? crt.GCRE.Pon.Tex.Replace(".dds", ".tga").ToLower() : ponB3D.texName.ToLower();
                        var ponImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex[ponTex]));
                        gr.DrawImageUnscaled(ponImage, 288, 224);
                    }
                    if (musMesh is not null)
                    {
                        string musTex = !string.IsNullOrEmpty(crt.GCRE.Mus.Tex) ? crt.GCRE.Mus.Tex.Replace(".dds", ".tga").ToLower() : musB3D.texName.ToLower();
                        var musImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex[musTex]));
                        gr.DrawImageUnscaled(musImage, 256, 224);
                    }
                    if (beaMesh is not null)
                    {
                        string beaTex = !string.IsNullOrEmpty(crt.GCRE.Bea.Tex) ? crt.GCRE.Bea.Tex.Replace(".dds", ".tga").ToLower() : beaB3D.texName.ToLower();
                        var beaImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex[beaTex]));
                        gr.DrawImageUnscaled(beaImage, 256, 192);
                    }
                    if (eyeMesh is not null)
                    {
                        if (string.IsNullOrEmpty(eyeTex))
                            eyeTex = !string.IsNullOrEmpty(crt.GCRE.Eye.Tex) ? crt.GCRE.Eye.Tex.Replace(".dds", ".tga").ToLower() : eyeB3D.texName.ToLower();
                        var eyeImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex[eyeTex]));
                        gr.DrawImageUnscaled(eyeImage, 320, 192); // guess, i know of nothing that uses the eye socket
                    }
                    if (bodMesh is not null)
                    {
                        if (string.IsNullOrEmpty(bodTex))
                            bodTex = !string.IsNullOrEmpty(crt.GCRE.Bod.Tex) ? crt.GCRE.Bod.Tex.Replace(".dds", ".tga").ToLower() : bodB3D.texName.ToLower();
                        var bodImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex[bodTex]));
                        gr.DrawImageUnscaled(bodImage, 0, 256);
                    }
                    if (hanMesh is not null)
                    {
                        if (string.IsNullOrEmpty(hanTex))
                            hanTex = !string.IsNullOrEmpty(crt.GCRE.Han.Tex) ? crt.GCRE.Han.Tex.Replace(".dds", ".tga").ToLower() : hanB3D.texName.ToLower();
                        var hanImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex[hanTex]));
                        gr.DrawImageUnscaled(hanImage, 384, 128);
                    }
                    if (feeMesh is not null)
                    {
                        if (string.IsNullOrEmpty(feeTex))
                            feeTex = !string.IsNullOrEmpty(crt.GCRE.Fee.Tex) ? crt.GCRE.Fee.Tex.Replace(".dds", ".tga").ToLower() : feeB3D.texName.ToLower();
                        var feeImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex[feeTex]));
                        gr.DrawImageUnscaled(feeImage, 384, 256);
                    }
                    if (bacMesh is not null)
                    {
                        if (string.IsNullOrEmpty(bacTex))
                            bacTex = !string.IsNullOrEmpty(crt.GCRE.Bac.Tex) ? crt.GCRE.Bac.Tex.Replace(".dds", ".tga").ToLower() : bacB3D.texName.ToLower();
                        var bacImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex[bacTex]));
                        gr.DrawImageUnscaled(bacImage, 256, 256);
                    }
                    if (shoMesh is not null)
                    {
                        if (string.IsNullOrEmpty(shoTex))
                            shoTex = !string.IsNullOrEmpty(crt.GCRE.Sho.Tex) ? crt.GCRE.Sho.Tex.Replace(".dds", ".tga").ToLower() : shoB3D.texName.ToLower();
                        var shoImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex[shoTex]));
                        gr.DrawImageUnscaled(shoImage, 256, 0);
                    }
                    if (vanMesh is not null)
                    {
                        if (string.IsNullOrEmpty(vanTex))
                            vanTex = !string.IsNullOrEmpty(crt.GCRE.Van.Tex) ? crt.GCRE.Van.Tex.Replace(".dds", ".tga").ToLower() : vanB3D.texName.ToLower();
                        var vanImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex[vanTex]));
                        gr.DrawImageUnscaled(vanImage, 320, 128);
                    }
                }

                var xbi = BitmapToBitmapImage(x);

                var baseMat = new DiffuseMaterial(new ImageBrush(CropBitmapByUV(xbi, baseMesh.TextureCoordinates)));
                var modelGroup = new Model3DGroup();

                var baseGM3 = new GeometryModel3D()
                {
                    Geometry = baseMesh,
                    Material = baseMat,
                    BackMaterial = baseMat
                };
                modelGroup.Children.Add(baseGM3);

                if (heaMesh is not null)
                {
                    var heaMat = new DiffuseMaterial(new ImageBrush(CropBitmapByUV(xbi, heaMesh.TextureCoordinates)));
                    var heaGM3 = new GeometryModel3D()
                    {
                        Geometry = heaMesh,
                        Material = heaMat,
                        BackMaterial = heaMat
                    };
                    modelGroup.Children.Add(heaGM3);
                }
                if (haiMesh is not null)
                {
                    var haiMat = new DiffuseMaterial(new ImageBrush(CropBitmapByUV(xbi, haiMesh.TextureCoordinates, true)));
                    var haiGM3 = new GeometryModel3D()
                    {
                        Geometry = haiMesh,
                        Material = haiMat,
                        BackMaterial = haiMat
                    };
                    modelGroup.Children.Add(haiGM3);
                }
                if (ponMesh is not null)
                {
                    var ponMat = new DiffuseMaterial(new ImageBrush(CropBitmapByUV(xbi, ponMesh.TextureCoordinates)));
                    var ponGM3 = new GeometryModel3D()
                    {
                        Geometry = ponMesh,
                        Material = ponMat,
                        BackMaterial = ponMat
                    };
                    modelGroup.Children.Add(ponGM3);
                }
                if (musMesh is not null)
                {
                    var musMat = new DiffuseMaterial(new ImageBrush(CropBitmapByUV(xbi, musMesh.TextureCoordinates, true)));
                    var musGM3 = new GeometryModel3D()
                    {
                        Geometry = musMesh,
                        Material = musMat,
                        BackMaterial = musMat
                    };
                    modelGroup.Children.Add(musGM3);
                }
                if (beaMesh is not null)
                {
                    var beaMat = new DiffuseMaterial(new ImageBrush(CropBitmapByUV(xbi, beaMesh.TextureCoordinates, true)));
                    var beaGM3 = new GeometryModel3D()
                    {
                        Geometry = beaMesh,
                        Material = beaMat,
                        BackMaterial = beaMat
                    };
                    modelGroup.Children.Add(beaGM3);
                }
                if (eyeMesh is not null)
                {
                    var eyeMat = new DiffuseMaterial(new ImageBrush(CropBitmapByUV(xbi, eyeMesh.TextureCoordinates)));
                    var eyeGM3 = new GeometryModel3D()
                    {
                        Geometry = eyeMesh,
                        Material = eyeMat,
                        BackMaterial = eyeMat
                    };
                    modelGroup.Children.Add(eyeGM3);
                }
                if (bodMesh is not null)
                {
                    var bodMat = new DiffuseMaterial(new ImageBrush(CropBitmapByUV(xbi, bodMesh.TextureCoordinates, true)));
                    var bodGM3 = new GeometryModel3D()
                    {
                        Geometry = bodMesh,
                        Material = bodMat,
                        BackMaterial = bodMat
                    };
                    modelGroup.Children.Add(bodGM3);
                }
                if (hanMesh is not null)
                {
                    var hanMat = new DiffuseMaterial(new ImageBrush(CropBitmapByUV(xbi, hanMesh.TextureCoordinates)));
                    var hanGM3 = new GeometryModel3D()
                    {
                        Geometry = hanMesh,
                        Material = hanMat,
                        BackMaterial = hanMat
                    };
                    modelGroup.Children.Add(hanGM3);
                }
                if (feeMesh is not null)
                {
                    var feeMat = new DiffuseMaterial(new ImageBrush(CropBitmapByUV(xbi, feeMesh.TextureCoordinates)));
                    var feeGM3 = new GeometryModel3D()
                    {
                        Geometry = feeMesh,
                        Material = feeMat,
                        BackMaterial = feeMat
                    };
                    modelGroup.Children.Add(feeGM3);
                }
                if (bacMesh is not null)
                {
                    var bacMat = new DiffuseMaterial(new ImageBrush(CropBitmapByUV(xbi, bacMesh.TextureCoordinates, true)));
                    var bacGM3 = new GeometryModel3D()
                    {
                        Geometry = bacMesh,
                        Material = bacMat,
                        BackMaterial = bacMat
                    };
                    modelGroup.Children.Add(bacGM3);
                }
                if (shoMesh is not null)
                {
                    var shoMat = new DiffuseMaterial(new ImageBrush(CropBitmapByUV(xbi, shoMesh.TextureCoordinates)));
                    var shoGM3 = new GeometryModel3D()
                    {
                        Geometry = shoMesh,
                        Material = shoMat,
                        BackMaterial = shoMat
                    };
                    modelGroup.Children.Add(shoGM3);
                }
                if (vanMesh is not null)
                {
                    var vanMat = new DiffuseMaterial(new ImageBrush(CropBitmapByUV(xbi, vanMesh.TextureCoordinates)));
                    var vanGM3 = new GeometryModel3D()
                    {
                        Geometry = vanMesh,
                        Material = vanMat,
                        BackMaterial = vanMat
                    };
                    modelGroup.Children.Add(vanGM3);
                }

                return modelGroup;
            }
            else
            {
                string baseModel = (crt.EEN2.skl + ".b3d").ToLower();
                var baseB3D = new B3DModel(getFileBytes(fullNameToGlobalIndex[baseModel]));
                var baseMesh = B3DToNoMatMesh(baseB3D);
                string baseTex = !string.IsNullOrEmpty(crt.EEN2.EEOV.s4) ? crt.EEN2.EEOV.s4.Replace(".dds", ".tga").ToLower() : baseB3D.texName.ToLower();
                DiffuseMaterial baseMat;
                try
                {
                    var baseImage = TargaToBitmap(getFileBytes(fullNameToGlobalIndex[baseTex]));
                    baseMat = new DiffuseMaterial(new ImageBrush(CropBitmapByUV(BitmapToBitmapImage(baseImage), baseMesh.TextureCoordinates, true)));
                }
                catch
                {
                    baseMat = new DiffuseMaterial(new SolidColorBrush(Colors.LightSlateGray));
                }
                var modelGroup = new Model3DGroup();

                var baseGM3 = new GeometryModel3D()
                {
                    Geometry = baseMesh,
                    Material = baseMat,
                    BackMaterial = baseMat
                };
                modelGroup.Children.Add(baseGM3);
                return modelGroup;
            }
        }

        // crops texture to the UV bounds for previewing
        // necessary due to a weird quirk of HelixToolkit
        // transparency also disabled by default due to rendering issue
        public BitmapImage CropBitmapByUV(BitmapImage bitmapImage, PointCollection uvCoordinates, bool trans = false)
        {
            double minX = uvCoordinates.Min(p => p.X);
            double minY = uvCoordinates.Min(p => p.Y);
            double maxX = uvCoordinates.Max(p => p.X);
            double maxY = uvCoordinates.Max(p => p.Y);

            double x = minX * bitmapImage.PixelWidth;
            double y = minY * bitmapImage.PixelHeight;
            double width = (maxX - minX) * bitmapImage.PixelWidth;
            double height = (maxY - minY) * bitmapImage.PixelHeight;
            Console.WriteLine($"x: {x} y: {y}{Constants.vbCrLf}w: {width} h: {height}");

            var croppedBitmap = new CroppedBitmap(bitmapImage, new Int32Rect((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(width), (int)Math.Round(height)));

            var bitmap = new Bitmap(croppedBitmap.PixelWidth, croppedBitmap.PixelHeight, trans ? System.Drawing.Imaging.PixelFormat.Format32bppArgb : System.Drawing.Imaging.PixelFormat.Format32bppRgb);


            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            croppedBitmap.CopyPixels(Int32Rect.Empty, bitmapData.Scan0, bitmapData.Height * bitmapData.Stride, bitmapData.Stride);
            bitmap.UnlockBits(bitmapData);
            return BitmapToBitmapImage(bitmap);
        }

        private Model3DGroup G3DModelToUsableMesh(G3DModel g3d)
        {
            var meshBuilder = new MeshBuilder();
            float cs = 96.0f;
            // coordinate scale, to roughly match the size of b3d models (not the same as CoordinateScale in models, it's purpose is unknown)
            for (int i = 0, loopTo = g3d.Model_Faces_Index.Count - 1; i <= loopTo; i++)
            {
                var face = g3d.Model_Faces_Index[i];
                var a = new Point3D(g3d.Model_Vertex_Position[face[0]].X / cs, g3d.Model_Vertex_Position[face[0]].Y / cs, g3d.Model_Vertex_Position[face[0]].Z / cs);
                var b = new Point3D(g3d.Model_Vertex_Position[face[1]].X / cs, g3d.Model_Vertex_Position[face[1]].Y / cs, g3d.Model_Vertex_Position[face[1]].Z / cs);
                var c = new Point3D(g3d.Model_Vertex_Position[face[2]].X / cs, g3d.Model_Vertex_Position[face[2]].Y / cs, g3d.Model_Vertex_Position[face[2]].Z / cs);

                var texA = new System.Windows.Point(g3d.Model_Vertex_Texcoords[face[0]].X, g3d.Model_Vertex_Texcoords[face[0]].Y);
                var texB = new System.Windows.Point(g3d.Model_Vertex_Texcoords[face[1]].X, g3d.Model_Vertex_Texcoords[face[1]].Y);
                var texC = new System.Windows.Point(g3d.Model_Vertex_Texcoords[face[2]].X, g3d.Model_Vertex_Texcoords[face[2]].Y);

                meshBuilder.AddTriangle(a, b, c, texA, texB, texC);
            }
            var mesh = OptimizeMesh(meshBuilder);
            Material material;
            try
            {
                var cropped = CropBitmapByUV(GetImageFromName("CR_Monsta_default_LG.tga"), mesh.TextureCoordinates);
                material = new DiffuseMaterial(new ImageBrush(cropped));
            }
            catch
            {
                material = new DiffuseMaterial(new SolidColorBrush(Colors.LightSlateGray));
            }
            var model = new GeometryModel3D()
            {
                Geometry = mesh,
                Material = material,
                BackMaterial = material
            };

            // rotate model to match b3ds
            var rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1d, 0d, 0d), 90d));
            var rotateTransform2 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0d, 0d, 1d), -90));
            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(rotateTransform);
            transformGroup.Children.Add(rotateTransform2);
            model.Transform = transformGroup;

            var modelGroup = new Model3DGroup();
            modelGroup.Children.Add(model);

            return modelGroup;
        }

        private MeshGeometry3D B3DToNoMatMesh(B3DModel b3d)
        {
            var meshBuilder = new MeshBuilder();
            for (int i = 0, loopTo = b3d.Model_Faces_Index.Count - 1; i <= loopTo; i++)
            {
                var face = b3d.Model_Faces_Index[i];
                var a = new Point3D(b3d.Model_Vertex_Position[face[0]].X, b3d.Model_Vertex_Position[face[0]].Y, b3d.Model_Vertex_Position[face[0]].Z);
                var b = new Point3D(b3d.Model_Vertex_Position[face[1]].X, b3d.Model_Vertex_Position[face[1]].Y, b3d.Model_Vertex_Position[face[1]].Z);
                var c = new Point3D(b3d.Model_Vertex_Position[face[2]].X, b3d.Model_Vertex_Position[face[2]].Y, b3d.Model_Vertex_Position[face[2]].Z);
                var texA = new System.Windows.Point(b3d.Model_Vertex_Texcoords[face[0]].X, b3d.Model_Vertex_Texcoords[face[0]].Y);
                var texB = new System.Windows.Point(b3d.Model_Vertex_Texcoords[face[1]].X, b3d.Model_Vertex_Texcoords[face[1]].Y);
                var texC = new System.Windows.Point(b3d.Model_Vertex_Texcoords[face[2]].X, b3d.Model_Vertex_Texcoords[face[2]].Y);
                meshBuilder.AddTriangle(a, b, c, texA, texB, texC);
            }
            return OptimizeMesh(meshBuilder);
        }

        private Model3DGroup B3DModelToUsableMesh(B3DModel b3d, string fn)
        {
            var mesh = B3DToNoMatMesh(b3d);
            Material material;
            if (b3d.texName != "")
            {
                try
                {
                    var cropped = CropBitmapByUV(GetImageFromName(b3d.texName), mesh.TextureCoordinates);
                    material = new DiffuseMaterial(new ImageBrush(cropped));
                }
                catch
                {
                    material = new DiffuseMaterial(new SolidColorBrush(Colors.LightSlateGray));
                }
            }
            else
            {
                material = new DiffuseMaterial(new SolidColorBrush(Colors.LightGreen));
            }
            var model = new GeometryModel3D()
            {
                Geometry = mesh,
                Material = material,
                BackMaterial = material
            };

            // Dim rotateTransform = New RotateTransform3D(New AxisAngleRotation3D(New Vector3D(0, 0, 1), -90))
            // model.Transform = rotateTransform

            if (b3d.FormatVer == 1)
            {
                var scaleTransform = new ScaleTransform3D(new Vector3D(10d, 10d, 10d));
                model.Transform = scaleTransform;
            }

            var modelGroup = new Model3DGroup();
            modelGroup.Children.Add(model);

            return modelGroup;
        }

        private Model3DGroup _8ModelToUsableMesh(_8Model _8)
        {
            var mbldrs = new List<MeshBuilder>();

            for (int i = 0, loopTo = _8.MAT.Count - 1; i <= loopTo; i++)
                mbldrs.Add(new MeshBuilder());

            for (int i = 0, loopTo1 = _8.IDX.Count - 1; i <= loopTo1; i++)
            {
                var face = _8.IDX[i];
                var a = new Point3D(_8.GVP[face[0]].X, _8.GVP[face[0]].Y, _8.GVP[face[0]].Z);
                var b = new Point3D(_8.GVP[face[1]].X, _8.GVP[face[1]].Y, _8.GVP[face[1]].Z);
                var c = new Point3D(_8.GVP[face[2]].X, _8.GVP[face[2]].Y, _8.GVP[face[2]].Z);
                var texA = new System.Windows.Point(_8.GVT[face[0]].X, _8.GVT[face[0]].Y);
                var texB = new System.Windows.Point(_8.GVT[face[1]].X, _8.GVT[face[1]].Y);
                var texC = new System.Windows.Point(_8.GVT[face[2]].X, _8.GVT[face[2]].Y);
                mbldrs[Math.Clamp(_8.MDX[i] - 1, 0, int.MaxValue)].AddTriangle(a, b, c, texA, texB, texC);
            }

            var modelGroup = new Model3DGroup();

            for (int i = 0, loopTo2 = mbldrs.Count - 1; i <= loopTo2; i++)
            {
                var mesh = OptimizeMesh(mbldrs[i]);
                Material material;

                try
                {
                    var cropped = CropBitmapByUV(GetImageFromName(_8.MAT[i].FileName), mesh.TextureCoordinates, _8.MAT[i].FileName.Contains("Flora") ? true : false);
                    material = new DiffuseMaterial(new ImageBrush(cropped));
                }
                catch
                {
                    material = new DiffuseMaterial(new SolidColorBrush(Colors.LightSlateGray));
                }

                var model = new GeometryModel3D()
                {
                    Geometry = mesh,
                    Material = material,
                    BackMaterial = material
                };

                // Dim rotateTransform = New RotateTransform3D(New AxisAngleRotation3D(New Vector3D(0, 0, 1), -90))
                // model.Transform = rotateTransform

                modelGroup.Children.Add(model);
            }

            var exp = new ObjExporter();
            exp.MaterialsFile = "temp.mtl";
            var tmp = new ModelVisual3D() { Content = modelGroup };
            var fs = new FileStream("test.obj", FileMode.Create);
            exp.Export(tmp, fs);
            fs.Close();

            return modelGroup;
        }

        // removes exact duplicate vertices, while keeping texture coordinates and faces intact
        private MeshGeometry3D OptimizeMesh(MeshBuilder mesh)
        {
            var meshBuilder = new MeshBuilder();
            var positions = mesh.Positions;
            var texcoords = mesh.TextureCoordinates;
            var indices = mesh.TriangleIndices;
            var normals = mesh.Normals;
            var newIndices = new List<int>();
            var newPositions = new List<Point3D>();
            var newTexcoords = new List<System.Windows.Point>();
            var newNormals = new List<Vector3D>();
            var vertexIndexMap = new Dictionary<(Point3D, System.Windows.Point), int>();
            int index = 0;

            for (int i = 0, loopTo = positions.Count - 1; i <= loopTo; i++)
            {
                var pos = positions[i];
                var tex = texcoords[i];
                var norm = normals[i];
                var vertex = (pos, tex);

                if (!vertexIndexMap.ContainsKey(vertex))
                {
                    newPositions.Add(pos);
                    newTexcoords.Add(tex);
                    newNormals.Add(norm);
                    vertexIndexMap[vertex] = index;
                    index += 1;
                }

                newIndices.Add(vertexIndexMap[vertex]);
            }

            meshBuilder.Append(newPositions, newIndices, newNormals, newTexcoords);
            var newMesh = meshBuilder.ToMesh();
            Console.WriteLine($"Old Positions: {positions.Count} New Positions: {newPositions.Count}{Constants.vbCrLf}Old Indices: {indices.Count} New Indices: {newIndices.Count}{Constants.vbCrLf}Old Texcoords: {texcoords.Count} New Texcoords: {newTexcoords.Count}{Constants.vbCrLf}Old Normals: {normals.Count} New Normals: {newNormals.Count}");

            return newMesh;
        }

        private HelixViewport3D viewport = new HelixViewport3D()
        {
            CameraRotationMode = CameraRotationMode.Turntable,
            RotateAroundMouseDownPoint = true
        };

        private ElementHost host;

        private ModelVisual3D model = new ModelVisual3D();

        private void LoadModel(string objFileLocation)
        {
            var reader = new ObjReader();
            model = new ModelVisual3D() { Content = reader.Read(objFileLocation) };
        }

        private void GrpBrowser_Load(object sender, EventArgs e)
        {
            Refresh();
            ReadRHT();
            readExtensions();
            DarkTreeView1.Nodes.Clear();
            for (int i = 0; i <= 23; i++)
            {
                DarkTreeView1.Nodes.Add(new DarkTreeNode(grpnames[i]));
                foreach (var g in from ent in entries[i]
                                  select localIndexToGlobal[i][ent.number])
                {
                    string ext = extensions[g];
                    if (filter.Contains(ext))
                        DarkTreeView1.Nodes[i].Nodes.Add(new DarkTreeNode(filenames[g] + "." + ext));
                }
            }
            DarkTreeNode[] temp = (from node1 in DarkTreeView1.Nodes
                                   where node1.Nodes.Count == 0
                                   select node1).ToArray();
            foreach (var enode in temp)
                DarkTreeView1.Nodes.Remove(enode);

            if (!previewMode)
            {
                TableLayoutPanel1.SetColumnSpan(DarkTreeView1, 2);
                DarkButton2.Hide();
            }
            else
            {
                HexBox1.BackColor = AltUI.Config.ThemeProvider.Theme.Colors.OpaqueBackground;
                HexBox1.ForeColor = AltUI.Config.ThemeProvider.Theme.Colors.LightText;
                HexBox1.SelectionBackColor = AltUI.Config.ThemeProvider.GetAccentColor(0);
                HexBox1.SelectionForeColor = AltUI.Config.ThemeProvider.Theme.Colors.LightText;
                var t = AltUI.Config.ThemeProvider.GetAccentColor(50);
                HexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(100, t.R, t.G, t.B);
            }
        }

        private void DarkButton2_Click(object sender, EventArgs e)
        {
            var selectedNode = DarkTreeView1.SelectedNodes[0];
            extractFile(fullNameToGlobalIndex[selectedNode.Text.ToLower()], true);
        }

        private void DarkSectionPanel2_ControlAdded(object sender, EventArgs e)
        {
            DarkSectionPanel2.SectionHeader = DarkSectionPanel2.Controls[0].Name;
        }
    }
}