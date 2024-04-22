using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace VB3DLib
{
    public class B3DModel
    {
        public int FormatVer;
        public List<Vector3> Model_Vertex_Position = new();
        public List<Vector2> Model_Vertex_Texcoords = new();
        public List<Vector3> Model_Vertex_Normal = new();
        public List<int[]> Model_Faces_Index = new();
        public List<int> Model_Faces_Mats = new();
        public List<TVertex_Node> Vertex_Nodes = new();

        public string texName = "";
        
        public List<string> Txs = new();

        public int Vertex_Type_Flag;

        public B3DModel(string path)
        {
            var f = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read));
            ReadB3D(f, this);
        }

        public B3DModel(byte[] file)
        {
            var f = new BinaryReader(new MemoryStream(file));
            ReadB3D(f, this);
        }

        private void ReadB3D(BinaryReader f, B3DModel b3d)
        {
            texName = "";
            FormatVer = 0;
            Model_Vertex_Position = new();
            Model_Vertex_Texcoords = new();
            Model_Vertex_Normal = new();
            Model_Faces_Index = new();
            Model_Faces_Mats = new();

            Txs = new();

            Vertex_Type_Flag = 0;

            var header = new b3d_header(f, b3d);
            Console.WriteLine("head");
            header.print();
            Console.WriteLine("head_end");

            //var Material_Node = new TMaterial_Node(f);
            while (f.BaseStream.Position < f.BaseStream.Length)
            {
                var b = f.ReadByte();
                switch (b)
                {
                    case 0x07:
                        Console.WriteLine("Materials");
                        var Materials = new TMaterials(f);
                        Materials.print();
                        Console.WriteLine(f.BaseStream.Position);
                        break;

                    case 0x08:
                        Console.WriteLine("Textures");
                        var Textures = new TTextures(f, b3d);
                        Textures.print();
                        Console.WriteLine(f.BaseStream.Position);
                        break;

                    case 0x0A:
                        Console.WriteLine("Nodes");
                        var Nodes = new TNodes(f);
                        Nodes.print();
                        Console.WriteLine(f.BaseStream.Position);
                        break;

                    case 0x0E:
                        Console.WriteLine("Bones");
                        var Bones_Flag = 1;
                        var Bone1 = new TBone(f);
                        Bone1.print();
                        Console.WriteLine(f.BaseStream.Position);
                        break;

                    case 0x0F:
                        Console.WriteLine("VertexNodes");
                        var Vertex_Node = new TVertex_Node(f, b3d);
                        Vertex_Nodes.Add(Vertex_Node);
                        Console.WriteLine(f.BaseStream.Position);
                        break;

                    default:
                        Console.WriteLine(f.BaseStream.Position);
                        return;
                }
            }
        }

        public static char[] StringRead(BinaryReader f)
        {
            if (f.BaseStream.Position >= f.BaseStream.Length - 2) { return new char[0]; }
            var s = new List<char>();
            int len = f.ReadInt16();

            if (f.BaseStream.Position + len > f.BaseStream.Length) { return new char[0]; }

            for (var i = 0; i < len; i++)
            {
                var n = f.ReadByte();
                s.Add((char)n);
            }

            //Console.WriteLine(s.ToArray());
            //Console.WriteLine(s.Count);

            return s.ToArray();
        }

        public struct b3d_header
        {
            public char[] Sign = new char[8]; //: array[0..7]of char; // "B3D 1.1 "
            public byte Unknown1 = 0; //: byte;
            public byte Unknown2 = 0; //: byte;
            public byte Unknown3 = 0; //: byte; //Header ID??
            public float GlobalScaleFactor = 0.0f; //: single;
            public byte Unknown4 = 0; //: byte; //Header ID??
            public float CoordinateScale = 0.0f; //: single;
            public byte Unknown5 = 0; //: byte; //Header ID??
            public char[] CoordinateSystem = new char[4]; //: TCharArray;
            public byte Unknown6 = 0; //: byte;
            public int Unknown7 = 0; //: intword;

            public b3d_header(BinaryReader f, B3DModel b3d)
            {
                //Console.WriteLine("b3d_header");
                for (var i = 0; i < 8; i++)
                {
                    var n = f.ReadByte();
                    Sign[i] = (char)n;
                }

                Unknown2 = f.ReadByte();
                if (Unknown2 == 3)
                {
                    b3d.FormatVer = 1;
                    GlobalScaleFactor = f.ReadSingle();
                    f.ReadByte();
                    CoordinateScale = f.ReadSingle();
                    f.ReadByte();
                    CoordinateSystem = StringRead(f);
                    Unknown6 = f.ReadByte();
                    Unknown7 = f.ReadInt32();
                }
                else
                {
                    for (var i = 1; i <= 2; i++)
                    {
                        f.ReadByte();
                    }
                    GlobalScaleFactor = f.ReadSingle();
                    f.ReadByte();
                    CoordinateScale = f.ReadSingle();
                    f.ReadByte();
                    CoordinateSystem = StringRead(f);
                    Unknown6 = f.ReadByte();
                    Unknown7 = f.ReadInt32();
                }
            }

            public void print()
            {
                Console.Write("(b3d_header Sign:\"");
                Console.Write(Sign);
                Console.Write(
                    $"\" Unknown1:{Unknown1} Unknown2:{Unknown2} Unknown3:{Unknown3} GlobalScaleFactor:{GlobalScaleFactor} Unknown4:{Unknown4} CoordinateScale:{CoordinateScale} Unknown5:{Unknown5} CoordinateSystem:\"");
                Console.Write(CoordinateSystem);
                Console.WriteLine($"\" Unknown6:{Unknown6} Unknown7:{Unknown7})");
            }
        }

        public struct TVertex64
        {
            public Vector3 Coordinate; //: array[0..2]of single;
            public Vector3 Normal; //: array[0..2]of single;
            public Vector4 Color; //: array[0..3]of single;
            public Vector2 Unknown1; //: array[0..1]of single;
            public Vector2 Unknown2; //: array[0..1]of single;
            public Vector2 Unknown3; //: array[0..1]of single;

            public TVertex64(BinaryReader f, B3DModel b3d)
            {
                //Console.WriteLine("TVertex64");
                Coordinate.X = f.ReadSingle();
                Coordinate.Z = f.ReadSingle();
                Coordinate.Y = f.ReadSingle();
                b3d.Model_Vertex_Position.Add(Coordinate);
                b3d.Model_Vertex_Texcoords.Add(Unknown2);

                Normal.X = f.ReadSingle();
                Normal.Y = f.ReadSingle();
                Normal.Z = f.ReadSingle();
                b3d.Model_Vertex_Normal.Add(Normal);

                Color.X = f.ReadSingle();
                Color.Y = f.ReadSingle();
                Color.Z = f.ReadSingle();
                Color.W = f.ReadSingle();

                //TexCoords
                Unknown1.X = f.ReadInt32();
                for (var i = 0; i < Unknown1.X; i++)
                {
                    if (i == 0)
                    {
                        Unknown2.X = f.ReadSingle();
                        Unknown2.Y = f.ReadSingle();
                        b3d.Model_Vertex_Texcoords[b3d.Model_Vertex_Position.Count - 1] = Unknown2;
                    }
                    else
                    {
                        f.ReadSingle();
                        f.ReadSingle();
                    }
                }

                Unknown1.X = f.ReadInt32();
                for (var i = 0; i < Unknown1.X; i++)
                {
                    f.ReadSingle();
                    f.ReadSingle();
                }
            }
        }

        public struct TVertex44
        {
            public Vector3 Coordinate; // Eii?aeiaoa oi?ee (x, y, z: single)
            public Vector3 Normal; // Ii?iaeu oi?ee (x, y, z: single)
            public byte[] Color; // Oaao oi?ee (r, g, b, a: byte)
            public Vector2 TextureCoord; // Oaenoo?iua eii?aeiaou (s, t: single);
            public int[] Unknown1; // Iaecaanoii

            public TVertex44(BinaryReader f, B3DModel b3d)
            {
                //Console.WriteLine("TVertex44");
                Coordinate.X = f.ReadSingle();
                Coordinate.Z = f.ReadSingle();
                Coordinate.Y = f.ReadSingle();
                b3d.Model_Vertex_Position.Add(Coordinate);

                Normal.X = f.ReadSingle();
                Normal.Y = f.ReadSingle();
                Normal.Z = f.ReadSingle();
                b3d.Model_Vertex_Normal.Add(Normal);

                Color = new byte[4];
                for (var i = 0; i < 4; i++) Color[i] = f.ReadByte();

                TextureCoord.X = f.ReadSingle();
                TextureCoord.Y = f.ReadSingle();
                b3d.Model_Vertex_Texcoords.Add(TextureCoord);

                Unknown1 = new int[2];
                Unknown1[0] = f.ReadInt32();
                Unknown1[1] = f.ReadInt32();
            }
        }

        public class TVert_Bones_Data
        {
            public int Bone;
            public float Weight;

            public TVert_Bones_Data(BinaryReader f)
            {
                //Console.WriteLine("TVert_Bones_Data");
                Bone = f.ReadInt32();
                Weight = f.ReadSingle();
            }
        }

        public class TMaterial_Data44
        {
            public char[] Name;
            public char[] Mtl_ID;
            public float Unknown1;
            public char[] BLEND_STATE;
            public char[] MATERIAL_TYPE;
            public int Unknown2;
            public char[] Zone;
            public int Unknown3;

            public TMaterial_Data44(BinaryReader f, B3DModel b3d)
            {
                //Console.WriteLine("TMaterial_Data44");
                Name = StringRead(f);
                Mtl_ID = StringRead(f);
                for (var i = 0; i < 17; i++)
                    Unknown1 = f.ReadSingle();
                BLEND_STATE = StringRead(f);
                MATERIAL_TYPE = StringRead(f);
                var a = f.ReadInt32();
                Console.WriteLine(a);
                for (var i = 1; i <= 16; i++)
                {
                    Unknown2 = f.ReadInt32();
                    if (Unknown2 == 1)
                    {
                        Zone = StringRead(f);
                        b3d.Txs.Add(new string(Zone));
                        Unknown2 = f.ReadInt32();
                        for (var j = 1; j <= Unknown2; j++)
                            StringRead(f);
                        break;
                    }
                }
            }

            public void print()
            {
                Console.Write("(TMaterial_Data44 Name:\"");
                Console.Write(Name);
                Console.Write("\" Mtl_ID:\"{Mtl_ID}");
                Console.Write("\" Unknown1:{Unknown1} BLEND_STATE:\"");
                Console.Write(BLEND_STATE);
                Console.Write("\" MATERIAL_TYPE:\"");
                Console.Write(MATERIAL_TYPE);
                Console.Write($"\" Unknown2:{Unknown2} Zone:\"");
                Console.Write(Zone);
                Console.WriteLine($"\" Unknown3:{Unknown3})");
            }
        }

        public class TMaterial_Data
        {
            public char[] Name; //: TCharArray;
            public char[] Mtl_ID; //: TCharArray;

            public float[]
                Unknown1; //: array[0..16]of single; //Most likely Ambient, Diffuse, Emissive, Specular, Shininess, and Alpha

            public char[] BLEND_STATE; //: TCharArray;
            public char[] MATERIAL_TYPE; //: TCharArray;

            public byte
                Unknown2; //: array[0..63]of byte; //Flags?? (comment doesn't match usage, but copied from original.)

            public char[] Zone; //: TCharArray;
            public uint Unknown3; //: intword;

            public TMaterial_Data(BinaryReader f, B3DModel b3d)
            {
                //Console.WriteLine("TMaterial_Data");
                Unknown3 = f.ReadUInt32();
                Name = StringRead(f);
                var a1 = f.ReadUInt32();
                for (uint i = 1; i <= a1; i++)
                {
                    Mtl_ID = StringRead(f);
                }

                b3d.Txs.Add(new string(Mtl_ID));

                uint kk;
                f.ReadByte();
                f.ReadUInt32();
                kk = f.ReadUInt32();
                Console.WriteLine(kk);
                for (uint i = 1; i <= kk / 3; i++)
                {
                    var a = f.ReadInt32();
                    var b = f.ReadInt32();
                    var c = f.ReadInt32();
                    b3d.Model_Faces_Index.Add(new[] { b, a, c });
                    b3d.Model_Faces_Mats.Add(b3d.Txs.Count);
                }

                for (uint i = 1; i <= kk; i++)
                {
                    Unknown2 = f.ReadByte();
                }
            }

            public void print()
            {
                Console.Write("(TMaterial_Data Name:\"");
                Console.Write(Name);
                Console.Write("\" Mtl_ID:\"");
                Console.Write(Mtl_ID);
                Console.Write($"\" Unknown1:{Unknown1} BLEND_STATE:\"");
                Console.Write(BLEND_STATE);
                Console.Write("\" MATERIAL_TYPE:\"");
                Console.Write(MATERIAL_TYPE);
                Console.Write($"\" Unknown2:{Unknown2} Zone:\"");
                Console.Write(Zone);
                Console.WriteLine($"\" Unknown3:{Unknown3})");
            }
        }

        public struct TVertex_Node
        {
            public int Unknown1;
            public int Unknown2;
            public int VertexTypeFlag;
            public int NumberVerts;
            public int SizeOfVerts;
            public List<float> Unknown3;
            public int IsBones;
            public int BonesPerVert;
            public int NumberMaterial;
            public List<TVert_Bones_Data> Vert_Bones_Data;

            public TVertex_Node(BinaryReader f, B3DModel b3d)
            {
                Unknown3 = new List<float>();
                Vert_Bones_Data = new List<TVert_Bones_Data>();
                //Console.WriteLine("TVertex_Node");
                Unknown1 = f.ReadByte();
                Unknown2 = f.ReadByte();
                Console.WriteLine($"VertNode U1 {Unknown1} U2 {Unknown2}");

                if (b3d.FormatVer == 0)
                {
                    VertexTypeFlag = f.ReadByte();
                    b3d.Vertex_Type_Flag = VertexTypeFlag;
                }
                else
                {
                    b3d.Vertex_Type_Flag = 0;
                }

                NumberVerts = f.ReadInt32();
                Console.WriteLine($"Vertex count - {NumberVerts}");
                Console.WriteLine($"Vertex_Type_Flag - {VertexTypeFlag}");
                if (b3d.Vertex_Type_Flag == 1)
                {
                    SizeOfVerts = f.ReadInt32();
                    Console.WriteLine($"SizeOfVerts - {SizeOfVerts}");
                    for (var i = 0; i < NumberVerts; i++)
                    {
                        var Vertex44 = new TVertex44(f, b3d);
                    }

                    for (var i = 0; i < 6; i++)
                    {
                        Unknown3.Add(f.ReadSingle());
                    }

                    IsBones = f.ReadByte();

                    if (IsBones == 1)
                    {
                        BonesPerVert = f.ReadInt32();
                        Console.WriteLine($"BonesPerVert - {BonesPerVert}");
                        for (var i = 0; i < NumberVerts * BonesPerVert; i++)
                        {
                            var VBD = new TVert_Bones_Data(f);
                            Vert_Bones_Data.Add(VBD);
                        }
                    }

                    NumberMaterial = f.ReadInt32();
                    Console.WriteLine($"NumberMaterial - {NumberMaterial}");
                    for (var i = 0; i < NumberMaterial; i++)
                    {
                        var MD = new TMaterial_Data44(f, b3d);
                        MD.print();
                    }

                    for (var i = 0; i < NumberMaterial; i++)
                    {
                        StringRead(f);
                    }

                    var Mat_list = new List<int>();

                    var NumVert = 0;
                    var NumTri = 0;

                    for (var i = 0; i < NumberMaterial; i++)
                    {
                        var a = f.ReadInt32();
                        Mat_list.Add(a);
                    }

                    for (var i = 0; i < NumberMaterial; i++)
                    {
                        var a = f.ReadInt32();
                        NumVert += a;
                    }

                    Console.WriteLine("NumVert");
                    Console.WriteLine(NumVert);
                    Console.WriteLine("NumTri");
                    Console.WriteLine(NumTri);

                    for (var j = 0; j < NumberMaterial; j++)
                    {
                        for (var i = 0; i < (Mat_list[j] / 3); i++)
                        {
                            int a = f.ReadUInt16();
                            int b = f.ReadUInt16();
                            int c = f.ReadUInt16();
                            b3d.Model_Faces_Index.Add(new[] { c, b, a });
                            b3d.Model_Faces_Mats.Add(j);
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < NumberVerts; i++)
                    {
                        var Vertex64 = new TVertex64(f, b3d);
                    }

                    Console.WriteLine(f.BaseStream.Position);

                    NumberMaterial = f.ReadInt32();
                    Console.WriteLine($"NumberMaterial - {NumberMaterial}");
                    for (var i = 0; i < NumberMaterial; i++)
                    {
                        var MD = new TMaterial_Data(f, b3d);
                        MD.print();
                    }
                }
            }
        }

        public struct TBone
        {
            public char[] Name;
            public ushort Unknown1;
            public byte Flag;
            public Vector3 Translation;
            public Quaternion Rotation;

            public TBone(BinaryReader reader)
            {
                //Console.WriteLine("TBone");
                Name = StringRead(reader);
                Unknown1 = reader.ReadUInt16();
                Flag = reader.ReadByte();

                if (Flag != 0)
                {
                    Translation.X = reader.ReadSingle();
                    Translation.Z = reader.ReadSingle();
                    Translation.Y = reader.ReadSingle();

                    if (Flag == 3)
                    {
                        Rotation.X = reader.ReadSingle();
                        Rotation.Z = reader.ReadSingle();
                        Rotation.Y = reader.ReadSingle();
                        Rotation.W = reader.ReadSingle();
                    }

                    if (Flag == 2)
                    {
                        reader.ReadSingle();
                    }
                }
            }

            public void print()
            {
                Console.Write("(TBone Name:\"");
                Console.Write(Name);
                Console.WriteLine(
                    $"\" Unknown1:{Unknown1} Flag:{Flag} Translation:[{Translation.X} {Translation.Y} {Translation.Z}] Rotation:(quat {Rotation.X} {Rotation.Y} {Rotation.Z} {Rotation.W}))");
            }
        }

        public class TColor
        {
            public float r;
            public float g;
            public float b;
            public float a;

            public TColor(BinaryReader f)
            {
                //Console.WriteLine("TColor");
                r = f.ReadSingle();
                g = f.ReadSingle();
                b = f.ReadSingle();
                a = f.ReadSingle();
            }
        }

        public struct TMater
        {
            public TColor Ambient;
            public TColor Diffuse;
            public TColor Emissive;
            public TColor Specular;
            public float Shininess;
            public float Alpha;

            public TMater(BinaryReader f)
            {
                //Console.WriteLine("TMater");
                Ambient = new TColor(f);
                Diffuse = new TColor(f);
                Emissive = new TColor(f);
                Specular = new TColor(f);
                Shininess = f.ReadSingle();
                Alpha = f.ReadSingle();
            }
        }

        public class TMaterials
        {
            public char[] Mtl_ID;
            public char[] Name;
            public TMater Mater;
            public char[] BLEND_STATE;
            public char[] MATERIAL_TYPE;
            public int Flag_1;
            public int Flag_2;

            public TMaterials(BinaryReader f)
            {
                //Console.WriteLine("TMaterials");
                Mtl_ID = StringRead(f);
                Name = StringRead(f);
                Mater = new TMater(f);
                BLEND_STATE = StringRead(f);
                MATERIAL_TYPE = StringRead(f);
                Flag_1 = f.ReadInt32();
                Flag_2 = f.ReadInt32();
            }

            public void print()
            {
                Console.Write("(TMaterials Mtl_ID:\"");
                Console.Write(Mtl_ID);
                Console.Write("\" Name:\"");
                Console.Write(Name);
                Console.Write("\" BLEND_STATE:\"");
                Console.Write(BLEND_STATE);
                Console.Write("\" MATERIAL_TYPE:\"");
                Console.Write(MATERIAL_TYPE);
                Console.WriteLine($"\" Flag_1:{Flag_1} Flag_2:{Flag_2})");
            }
        }

        public struct TTextures
        {
            public byte Unknown1;
            public char[] Name;
            public char[] FileName;
            public uint Width;
            public uint Height;

            public TTextures(BinaryReader f, B3DModel b3d)
            {
                //Console.WriteLine("TTextures");
                Unknown1 = f.ReadByte();
                Name = StringRead(f);
                if (b3d.texName == "") {b3d.texName = new string(Name);}
                FileName = StringRead(f);
                Width = f.ReadUInt32();
                Height = f.ReadUInt32();
            }

            public void print()
            {
                Console.Write("(TTextures Unknown1:{Unknown1} Name:\"");
                Console.Write(Name);
                Console.Write("\" FileName:\"");
                Console.Write(FileName);
                Console.WriteLine($"\" Width:{Width} Height:{Height})");
            }
        }

        public class TNodes
        {
            public byte Unknown1; //: byte;
            public char[] Name; //: TCharArray;

            public TNodes(BinaryReader f)
            {
                //Console.WriteLine("TNodes");
                Unknown1 = f.ReadByte();
                Name = StringRead(f);
            }

            public void print()
            {
                Console.Write($"(TNodes Unknown1:{Unknown1} Name:\"");
                Console.Write(Name);
                Console.WriteLine("\")");
            }
        }

        public struct TMat_Data
        {
            public byte[] Unknown1; // array[0..2]of byte;
            public char[] Name; // TCharArray;
            public uint TextureOn; // intword;
            public char[] Texture; // TCharArray;
            public uint Unknown3; // intword;
            public byte Unknown4; // byte;
            public uint NumOfPoints; // intword;

            public TMat_Data(BinaryReader f)
            {
                //Console.WriteLine("TMat_Data");
                Unknown1 = f.ReadBytes(3);
                Name = StringRead(f);
                Console.WriteLine(Name);
                TextureOn = f.ReadUInt32();
                if (TextureOn != 0) Texture = StringRead(f);
                Unknown3 = f.ReadUInt32();
                Unknown4 = f.ReadByte();
                NumOfPoints = f.ReadUInt32();
                Console.WriteLine(NumOfPoints);
            }
        }

        public struct TMaterial_Node
        {
            public uint NumberMaterialNodes; //: intword;
            public byte Unknown1; //: byte;

            public TMaterial_Node(BinaryReader f)
            {
                //Console.WriteLine("TMaterial_Node");
                NumberMaterialNodes = f.ReadUInt32();
                Unknown1 = f.ReadByte(); //Unknown4 in maxscript? somehow? idfk
                Console.WriteLine($"MaterialNode - NumberMaterialNodes = {NumberMaterialNodes}");
            }
        }
    }

    public class G3DModel
    {
        public List<Vector3> Model_Vertex_Position = new();
        public List<int[]> Model_Faces_Index = new();
        public List<Vector2> Model_Vertex_Texcoords = new();

        public G3DModel(string path)
        {
            var s = File.ReadAllLines(path);
            ReadG3D(s);
        }

        public G3DModel(byte[] file)
        {
            var s = Encoding.UTF8.GetString(file);
            // split into array at every CrLf
            s = s.Replace("\r\n", "\n");
            var sa = s.Split('\n');
            ReadG3D(sa);
        }

        // there's only one so a lot of this will be hardcoded
        // if you want to fix it, be my guest.
        // if not, it works so i don't care.
        private void ReadG3D(string[] f)
        {
            for (var i = 73; i < 3645; i++)
            {
                //example line
                //      Vertex{ Coordinate(0 565 36) Normal(0 -264 989) Color(586 80 80 512) TexCoord(156 141) Bone(7 1.000000) }
                var x = f[i].Split("      Vertex{ Coordinate(")[1].Split(' ')[0];
                var y = f[i].Split("      Vertex{ Coordinate(")[1].Split(' ')[1];
                var z = f[i].Split("      Vertex{ Coordinate(")[1].Split(' ')[2].Split(')')[0];
                Model_Vertex_Position.Add(new Vector3(float.Parse(x), float.Parse(y), float.Parse(z)));
                var u = f[i].Split("TexCoord(")[1].Split(' ')[0];
                var v = f[i].Split("TexCoord(")[1].Split(' ')[1].Split(')')[0];
                Model_Vertex_Texcoords.Add(new Vector2(float.Parse(u) / 1024.0f, float.Parse(v) / 1024.0f)); //divide by 1024 to normalize UV
            }

            var tmpList = new List<int>();
            var skipLine = false;

            for (var i = 3648; i < 4123; i++)
            {
                //example lines
                //      Triangles{Index( 2976 2977 2978 2979 2976 2978 2980 2981 2982 2983 2981 2980 2984 2985 2986 2987 2988 2989 2990 2987 2989 2991 2992 2993 2993 2994 2995 2996 2997 2998 2999 3000
                //        3001 3002 3003 3004 3005 3006 3007 3008 3009 3010 3011 3009 3008 3012 2999 3013 2987 2995 2988 3014 3015 3016 3015 3017 3016 3018 3019 3020 2994 3019 3018 3021
                //        3449 3446 ) Edge( 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1
                var s = f[i];
                if (s.StartsWith("      Triangles{Index( "))
                {
                    skipLine = false;
                    tmpList.AddRange(from ss in s.Split("      Triangles{Index( ")[1].Split(" ")
                                     where ss != ""
                                     select int.Parse(ss));
                }
                else if (!skipLine)
                {
                    if (s.Contains("Edge"))
                    {
                        skipLine = true;
                        s = s.Split(" ) Edge( ")[0];
                    }
                    tmpList.AddRange(from ss in s.Split(" ")
                                     where ss != ""
                                     select int.Parse(ss));
                }
            }
            //split tmplist into groups of 3 and add to faces
            for (var i = 0; i < tmpList.Count; i += 3)
            {
                Model_Faces_Index.Add(new[] { tmpList[i], tmpList[i + 1], tmpList[i + 2] });
            }
        }
    }

    public class _8Model
    {
        public string MapName = "defaultName";

        public List<Vector3> GVP = new(); // vertex position
        public List<Vector3> GVN = new(); // vertex normal
        public List<Color> GVD = new(); // vertex diffuse color
        public List<Vector2> GVT = new(); // texture coords
        public List<Vector2> GVL = new(); // lightmap coords

        public List<int[]> IDX = new(); // face indexes
        public List<int> MDX = new(); // mat index
        public List<BitmapTexture> MAT = new(); // materials

        public int buffer_index;
        public List<int> buffer_offset = new();

        public int Flag = 1;

        public _8Model(string path, bool flgs)
        {
            Flag = flgs ? 4 : 1;

            var f = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read));
            ReadTREE_Data(f);
        }

        public _8Model(byte[] file, bool flgs)
        {
            Flag = flgs ? 4 : 1;
            GVP = new();

            GVN = new();
            GVD = new();
            GVT = new();
            GVL = new();

            IDX = new();
            MDX = new();
            MAT = new();

            buffer_index = 0;
            buffer_offset = new();

            var f = new BinaryReader(new MemoryStream(file));
            ReadTREE_Data(f);
        }

        public static string String4Read(BinaryReader f)
        {
            var s = new byte[4];
            for (var i = 0; i < 4; ++i)
            {
                var t = f.BaseStream.Position;
                s[i] = f.ReadByte();
            }

            try
            {
                return Encoding.ASCII.GetString(s);
            }
            catch
            {
                Console.WriteLine("ERRO");
                return "    ";
            }
        }

        public struct TSectionHeader
        {
            public string id; // name
            public int size;

            public TSectionHeader(BinaryReader f)
            {
                id = String4Read(f);
                size = f.ReadInt32();
            }
        }

        public struct BitmapTexture
        {
            public string FileName;
            public int AlphaSource;
        }

        public static bool IsHeader(string s)
        {
            return s switch
            {
                "NODE" or "LEAF" or "INFO" or "IDXD" or "IDXS" or "FLGS" or "TREE" or "LVL0" or "XYZ " or "MATR"
                    or "MATD" or "TXTD" or "VTXD" or "VTXB" or "TXUV" or "LMUV" or "DIFU" or "NRML" or "LVLD"
                    or "HEAD" or "8TRE" or "LGTD" or "LGTR" => true,
                _ => false,
            };
        }

        public void ReadTREE_Data(BinaryReader f)
        {
            ReadBlock(f, 0);
        }

        public int ReadBlock(BinaryReader f, int size)
        {
            var header = new TSectionHeader(f);

            Console.WriteLine(header.id);

            switch (header.id)
            {
                case "FLGS":
                    f.BaseStream.Seek(header.size * Flag, SeekOrigin.Current);
                    return 0;

                case "IDXD":
                    f.BaseStream.Seek(-16, SeekOrigin.Current);
                    var t = f.ReadInt32();
                    if (t >= 0 && t <= buffer_offset.Count - 1) // maybe if (t >= 1 && t <= buffer_offset.Count)
                        buffer_index = t;
                    f.BaseStream.Seek(12, SeekOrigin.Current);
                    break;

                case "TXTD":
                    Console.WriteLine(f.ReadInt32());
                    Console.WriteLine(f.ReadInt32());
                    break;

                case "TXUV":
                    {
                        for (var i = 1; i <= header.size / 8; i++)
                        {
                            var u = f.ReadSingle();
                            var v = f.ReadSingle();
                            GVT.Add(new Vector2(u, v));
                        }

                        break;
                    }
                //case "LMUV":
                //{
                //    for (int i = 1; i <= header.size / 8; i++)
                //    {
                //        float u = f.ReadSingle();
                //        float v = f.ReadSingle();
                //        GVL.Add(new Vector2(u, v));
                //    }
                //
                //    break;
                //}
                case "TXTR":
                    {
                        var s = "";
                        for (var i = 1; i <= header.size; i++)
                            s += f.ReadChar();

                        if (s.IndexOf("ctx", StringComparison.Ordinal) != -1) return 0;

                        var tm = new BitmapTexture
                        {
                            AlphaSource = 2,
                            FileName = s
                        };
                        Console.WriteLine(s);
                        MAT.Add(tm);

                        return 0;
                    }
                case "IDXS":
                    {
                        var end = (f.BaseStream.Position) + header.size;
                        var m = 0;
                        var mp = 0;

                        while ((f.BaseStream.Position + 12) < end)
                        {
                            var tmp = f.ReadUInt16();
                            if (tmp == 0)
                            {
                                tmp = f.ReadUInt16();
                            }

                            if ((tmp & 0xFF00) == 0x8000)
                            {
                                f.ReadUInt16();
                                var t1 = (f.ReadUInt16()) - 0x4000;
                                f.ReadUInt16();
                                var t2 = f.ReadUInt16();
                                m = t2;
                            }

                            if ((tmp & 0xFF00) == 0x4000)
                            {
                                f.ReadUInt16();
                                var t1 = f.ReadUInt16();
                                m = t1;
                            }

                            if ((tmp & 0xFF00) == 0) m = tmp;

                            var sz = f.ReadUInt16();

                            for (var i = 1; i <= (sz / 3); i++)
                            {
                                var a = buffer_offset[buffer_index] + f.ReadUInt16();
                                var b = buffer_offset[buffer_index] + f.ReadUInt16();
                                var c = buffer_offset[buffer_index] + f.ReadUInt16();

                                IDX.Add(new[] { a, c, b });
                                MDX.Add(m == 0 ? 0 : m);
                            }
                        }

                        f.BaseStream.Seek(end, SeekOrigin.Begin);
                        return 0;
                    }
                case "VTXB":
                    f.ReadInt32();
                    header.size -= 4;
                    buffer_offset.Add(GVP.Count);
                    break;

                case "VTXD":
                    f.ReadInt32();
                    header.size -= 4;
                    break;

                case "XYZ ":
                    {
                        for (var i = 1; i <= header.size / 12; i++)
                        {
                            var x = f.ReadSingle();
                            var y = f.ReadSingle();
                            var z = f.ReadSingle();

                            GVP.Add(new Vector3(x, z, y));
                        }

                        return 0;
                    }
            }

            if (IsHeader(header.id))
            {
                var end = (f.BaseStream.Position + header.size);
                while (f.BaseStream.Position < end)
                {
                    ReadBlock(f, header.size);
                }
            }
            else
            {
                f.BaseStream.Seek(-8, SeekOrigin.Current);
                if ((size < 4 && (size != 0)))
                {
                    size = 4;
                }

                f.BaseStream.Seek(size, SeekOrigin.Current);
            }

            return 0;
        }
    }
}