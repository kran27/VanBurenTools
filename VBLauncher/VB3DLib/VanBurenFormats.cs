using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace VBLauncher.VB3DLib;

public class B3DModel
{
    public int FormatVer;
    public List<TMaterials> Materials = [];
    public List<int[]> Model_Faces_Index = [];
    public List<int> Model_Faces_Mats = [];
    public List<Vector3> Model_Vertex_Normal = [];
    public List<Vector3> Model_Vertex_Position = [];
    public List<Vector2> Model_Vertex_Texcoords = [];
    // Per-vertex RGBA color, parallel to Model_Vertex_Position. The alpha channel
    // is what makes BASE_VERTEX_ALPHA_* materials (hair, etc.) actually translucent —
    // the renderer multiplies the texture sample against this and blends on output.
    // Defaults to white when a vertex format doesn't carry a color (TVertex64
    // intermittent paths, etc.) so opaque meshes stay opaque.
    public List<Vector4> Model_Vertex_Color = [];
    
    public Dictionary<string, TMaterials> MaterialDict = new();
    public Dictionary<string, List<int>> SubModels = new();
    public Dictionary<string, string> MatNameToTexture = new();
    
    public List<string> OcclusionZones = [];

    public string texName = "";

    public List<string> Txs = [];
    public List<TVertex_Node> Vertex_Nodes = [];

    // Skinning data. Bones is the file-level bone list (index = position in this list,
    // matching the indices stored in Vert_Bones_Data.Bone). Vertex_Bones is aligned to
    // Model_Vertex_Position: Vertex_Bones[i] holds every (bone, weight) pair referencing
    // global vertex i, with -1 / unused slots filtered out. Vertex nodes that carry no
    // skinning data contribute empty entries so the indexing stays parallel.
    public List<TBone> Bones = [];
    public List<List<TVert_Bones_Data>> Vertex_Bones = [];

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
        Model_Vertex_Position = [];
        Model_Vertex_Texcoords = [];
        Model_Vertex_Normal = [];
        Model_Vertex_Color = [];
        Model_Faces_Index = [];
        Model_Faces_Mats = [];

        Txs = [];
        Materials = [];

        Bones = [];
        Vertex_Bones = [];

        Vertex_Type_Flag = 0;

        // Tag dispatch derived from F3.exe Gfx_G3D_Mesh's B3D loader (sub_4DE680 / sub_4DE120).
        // No fixed-size header — everything after the 8-byte signature is tagged. The earlier
        // C# port had a hand-unrolled "header" plus two divergent switch tables; both diverged
        // from the game and broke door (.dor) meshes with VersionThing=3.
        var sig = f.ReadBytes(8);
        if (sig.Length != 8 || Encoding.ASCII.GetString(sig) != "B3D 1.1 ")
        {
            Console.WriteLine("Not a B3D file (signature mismatch).");
            return;
        }

        // FormatVer gates TVertex_Node between the legacy (VT=3, ~door / pre-conversion) layout
        // and the modern (VT=28, post-conversion) layout — completely different vertex/material
        // wire formats. The old code defaulted to 1 in the VT=3 header path and stayed at 0 in
        // VT=28. Match that: default to 1, and only flip to 0 when we actually see a 0x1C tag.
        b3d.FormatVer = 1;
        var done = false;
        while (!done && f.BaseStream.Position < f.BaseStream.Length)
        {
            var token = f.ReadByte();
            switch (token)
            {
                case 0x01:
                    done = true; // EOF
                    break;
                case 0x03:
                    f.ReadSingle(); // GlobalScaleFactor — not consumed downstream
                    break;
                case 0x04:
                    f.ReadSingle(); // CoordinateScale
                    break;
                case 0x05:
                    StringRead(f); // CoordinateSystem
                    break;
                case 0x06:
                    f.ReadBytes(12); // 3 dwords; 4th component is implicit 1.0f
                    break;
                case 0x07:
                {
                    var mat = new TMaterials(f);
                    Materials.Add(mat);
                    MaterialDict[new string(mat.Name)] = mat;
                    break;
                }
                case 0x08:
                case 0x09:
                {
                    _ = new TTextures(f, b3d);
                    break;
                }
                case 0x0A:
                    ParseNode(f, b3d);
                    break;
                case 0x0E:
                {
                    var bone = new TBone(f);
                    b3d.Bones.Add(bone);
                    break;
                }
                case 0x16:
                    f.ReadInt32();
                    break;
                case 0x17:
                    // sub_4D2620: 3 padded strings + 16 + 4 + 4 + 4 bytes
                    StringRead(f);
                    StringRead(f);
                    StringRead(f);
                    f.ReadBytes(28);
                    break;
                case 0x1C:
                    f.ReadByte(); // version value; presence of this tag is the modern-format signal
                    b3d.FormatVer = 0;
                    break;
                default:
                    Console.WriteLine($"Unknown top-level B3D tag: 0x{token:X2} at position {f.BaseStream.Position}");
                    return;
            }
        }
    }

    // Mirrors sub_4DA760 (G3D_Node::Read). Tag 0x0A spawns a node which has its own tag loop
    // running until 0x0B. 0x0F (G3D_Surface = vertex/face data) lives inside the node, NOT at
    // the top level. 0x0A inside a node spawns a child — recurse. Empty tags like 0x0C/0x0D
    // are consumed for stream alignment but the data isn't currently retained downstream.
    private void ParseNode(BinaryReader f, B3DModel b3d)
    {
        while (f.BaseStream.Position < f.BaseStream.Length)
        {
            var token = f.ReadByte();
            switch (token)
            {
                case 0x01:
                    Console.WriteLine("EOF reached before end of node");
                    return;
                case 0x0A:
                    ParseNode(f, b3d);
                    break;
                case 0x0B:
                    return; // end-of-node
                case 0x0C:
                    StringRead(f); // node Name (sub_4D0670 → sub_4D0450)
                    break;
                case 0x0D:
                    ReadTransform(f);
                    break;
                case 0x0F:
                {
                    var vn = new TVertex_Node(f, b3d);
                    for (var index = 0; index < vn.material_data.Count; index++)
                    {
                        var MD = vn.material_data[index];
                        if (MD is TMaterial_Data44 MD44)
                        {
                            SubModels[new string(MD44.Name)] = vn.face_indices[index];
                            MatNameToTexture[new string(MD44.Name)] = new string(MD44.Zone);
                        }
                        else if (MD is TMaterial_Data MD64)
                        {
                            SubModels[new string(MD64.Name)] = MD64.face_indices;
                            MatNameToTexture[new string(MD64.Name)] = new string(MD64.Mtl_ID);
                        }
                        else
                        {
                            Console.WriteLine("Unknown material data type in Vertex_Node: " + MD.GetType());
                        }
                    }
                    Vertex_Nodes.Add(vn);
                    break;
                }
                case 0x1B:
                {
                    // G3D_OcclusionZones — sub_4D9770: int32 count + N padded strings.
                    // Was previously (incorrectly) handled at the top level; the game only
                    // emits 0x1B inside a node's tag loop.
                    var zoneCount = f.ReadInt32();
                    for (var i = 0; i < zoneCount; i++)
                        OcclusionZones.Add(new string(StringRead(f)));
                    break;
                }
                default:
                    // Inner-node tags 0x10..0x15, 0x17, 0x18, 0x1A, 0x1B exist in the game but
                    // their wire formats aren't decoded here yet. Bail out of the current node
                    // so we don't desync the outer tag stream — the file may still load partially.
                    Console.WriteLine($"Unhandled inner node tag: 0x{token:X2} at position {f.BaseStream.Position}");
                    return;
            }
        }
    }

    // Mirrors G3D_Transform::Read at sub_4D0A00. Padded-string Name + 1-byte flag + conditional
    // 24 / 32 / 24-byte payloads (translation / rotation matrix / scale).
    private static void ReadTransform(BinaryReader f)
    {
        StringRead(f); // Name
        var flags = f.ReadByte();
        if ((flags & 0x01) != 0) f.ReadBytes(24);
        if ((flags & 0x02) != 0) f.ReadBytes(32);
        if ((flags & 0x04) != 0) f.ReadBytes(24);
    }

    public static char[] StringRead(BinaryReader f)
    {
        if (f.BaseStream.Position >= f.BaseStream.Length - 2) return [];
        var s = new List<char>();
        int len = f.ReadInt16();

        if (f.BaseStream.Position + len > f.BaseStream.Length) return [];

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
        public byte Unknown2 = 0; //: byte;
        public byte VersionThing = 0; //: byte;
        public float GlobalScaleFactor = 0.0f; //: single;
        public float CoordinateScale = 0.0f; //: single;
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
            VersionThing = Unknown2;
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
                for (var i = 1; i <= 2; i++) f.ReadByte();
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
                $"\" VersionThing:{VersionThing} GlobalScaleFactor:{GlobalScaleFactor} CoordinateScale:{CoordinateScale} CoordinateSystem:\"");
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
            Coordinate.X = -f.ReadSingle();
            Coordinate.Y = f.ReadSingle();
            Coordinate.Z = f.ReadSingle();
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
            b3d.Model_Vertex_Color.Add(Color);

            //TexCoords
            Unknown1.X = f.ReadInt32();
            for (var i = 0; i < Unknown1.X; i++)
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
            Coordinate.X = -f.ReadSingle();
            Coordinate.Y = f.ReadSingle();
            Coordinate.Z = f.ReadSingle();
            b3d.Model_Vertex_Position.Add(Coordinate);

            Normal.X = f.ReadSingle();
            Normal.Y = f.ReadSingle();
            Normal.Z = f.ReadSingle();
            b3d.Model_Vertex_Normal.Add(Normal);

            Color = new byte[4];
            for (var i = 0; i < 4; i++) Color[i] = f.ReadByte();
            // BGRA on disk (D3DCOLOR layout); store as RGBA floats.
            b3d.Model_Vertex_Color.Add(new Vector4(Color[2] / 255f, Color[1] / 255f, Color[0] / 255f, Color[3] / 255f));

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
        public char[] BLEND_STATE;
        public char[] MATERIAL_TYPE;
        public char[] Mtl_ID;
        public char[] Name;
        public float Unknown1;
        public int Unknown2;
        public int Unknown3;
        public char[] Zone;

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
                    b3d.Txs.Add(new string(Name));
                    Console.WriteLine(new string(Zone));
                    Unknown2 = f.ReadInt32();
                    for (var j = 1; j <= Unknown2; j++) {
                        var unkstr = new string(StringRead(f));
                        Console.WriteLine("SHDJKASHDKJHASKLJHSKDJFHKLSDFF\t\t\t" + unkstr);
                    }
                    break;
                }
            }
        }

        public void print()
        {
            Console.Write("(TMaterial_Data44 Name:\"");
            Console.Write(Name);
            Console.Write($@""" Mtl_ID:""{new string(Mtl_ID)}");
            Console.Write($@""" Unknown1:{Unknown1} BLEND_STATE:""");
            Console.Write(BLEND_STATE);
            Console.Write("\" MATERIAL_TYPE:\"");
            Console.Write(MATERIAL_TYPE);
            Console.Write($@""" Unknown2:{Unknown2} Zone:""");
            Console.Write(Zone);
            Console.WriteLine($@""" Unknown3:{Unknown3})");
        }
    }

    public class TMaterial_Data
    {
        public char[] BLEND_STATE; //: TCharArray;
        public char[] MATERIAL_TYPE; //: TCharArray;
        public char[] Mtl_ID; //: TCharArray;
        public char[] Name; //: TCharArray;

        public float[]
            Unknown1; //: array[0..16]of single; //Most likely Ambient, Diffuse, Emissive, Specular, Shininess, and Alpha

        public byte
            Unknown2; //: array[0..63]of byte; //Flags?? (comment doesn't match usage, but copied from original.)

        public uint Unknown3; //: intword;

        public char[] Zone; //: TCharArray;
        
        public List<int> face_indices = [];

        public TMaterial_Data(BinaryReader f, B3DModel b3d)
        {
            //Console.WriteLine("TMaterial_Data");
            Unknown3 = f.ReadUInt32();
            Name = StringRead(f);
            var a1 = f.ReadUInt32();
            for (uint i = 1; i <= a1; i++) Mtl_ID = StringRead(f);

            var nameStr = new string(Name);
            b3d.Txs.Add(nameStr);

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
                b3d.Model_Faces_Index.Add([b, a, c]);
                b3d.Model_Faces_Mats.Add(b3d.Txs.Count);
                face_indices.Add(a);
                face_indices.Add(b);
                face_indices.Add(c);
            }

            for (uint i = 1; i <= kk; i++) Unknown2 = f.ReadByte();
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

    // Unique non-(-1) bone indices referenced by this node's vertices.
    public List<int> BonesUsed = [];

    public List<dynamic> material_data = new();
    
    public Dictionary<int, List<int>> face_indices = [];

    public TVertex_Node(BinaryReader f, B3DModel b3d)
    {
        Unknown3 = [];
        Vert_Bones_Data = [];
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

            for (var i = 0; i < 6; i++) Unknown3.Add(f.ReadSingle());

            IsBones = f.ReadByte();

            if (IsBones == 1)
            {
                BonesPerVert = f.ReadInt32();
                Console.WriteLine($"BonesPerVert - {BonesPerVert}");
                for (var i = 0; i < NumberVerts * BonesPerVert; i++)
                {
                    var VBD = new TVert_Bones_Data(f);
                    Vert_Bones_Data.Add(VBD);
                    if (VBD.Bone != -1 && !BonesUsed.Contains(VBD.Bone))
                        BonesUsed.Add(VBD.Bone);
                }

                // Source layout is bone-major: Vert_Bones_Data[l*NumberVerts + v].
                // Transpose into per-vertex lists aligned to b3d.Model_Vertex_Position
                // (which TVertex44 already extended by NumberVerts above).
                for (var v = 0; v < NumberVerts; v++)
                {
                    var perVert = new List<TVert_Bones_Data>();
                    for (var l = 0; l < BonesPerVert; l++)
                    {
                        var entry = Vert_Bones_Data[l * NumberVerts + v];
                        if (entry.Bone != -1) perVert.Add(entry);
                    }
                    b3d.Vertex_Bones.Add(perVert);
                }
            }
            else
            {
                for (var v = 0; v < NumberVerts; v++) b3d.Vertex_Bones.Add([]);
            }

            NumberMaterial = f.ReadInt32();
            Console.WriteLine($"NumberMaterial - {NumberMaterial}");
            for (var i = 0; i < NumberMaterial; i++)
            {
                var MD = new TMaterial_Data44(f, b3d);
                material_data.Add(MD);
                MD.print();
            }

            for (var i = 0; i < NumberMaterial; i++) StringRead(f);

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
                for (var i = 0; i < Mat_list[j] / 3; i++)
                {
                    int a = f.ReadUInt16();
                    int b = f.ReadUInt16();
                    int c = f.ReadUInt16();
                    b3d.Model_Faces_Index.Add([c, b, a]);
                    b3d.Model_Faces_Mats.Add(j);
                    if (!face_indices.ContainsKey(j))
                        face_indices[j] = [];
                    face_indices[j].Add(a);
                    face_indices[j].Add(b);
                    face_indices[j].Add(c);
                }
        }
        else
        {
            for (var i = 0; i < NumberVerts; i++)
            {
                var Vertex64 = new TVertex64(f, b3d);
            }
            for (var v = 0; v < NumberVerts; v++) b3d.Vertex_Bones.Add([]);

            Console.WriteLine(f.BaseStream.Position);

            NumberMaterial = f.ReadInt32();
            Console.WriteLine($"NumberMaterial - {NumberMaterial}");
            for (var i = 0; i<NumberMaterial; i++)
            {
                var MD = new TMaterial_Data(f, b3d);
                material_data.Add(MD);
                MD.print();
            }
        }
        }
    }

    public struct TBone
{
    public string Name;
    public ushort Unknown1;
    public byte Flag;
    public Vector3 Translation;
    public Quaternion Rotation;

    public TBone(BinaryReader reader)
    {
        //Console.WriteLine("TBone");
        Name = BoneNames.ProcessName(new string(StringRead(reader)));
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

            if (Flag == 2) reader.ReadSingle();
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
    public float a;
    public float b;
    public float g;
    public float r;

    public TColor(BinaryReader f)
    {
        r = f.ReadSingle();
        g = f.ReadSingle();
        b = f.ReadSingle();
        a = f.ReadSingle();
        Console.WriteLine($"TColor: {r} {g} {b} {a}");
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
        Console.WriteLine("TMater");
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
    public char[] BLEND_STATE;
    public int Flag_1;
    public int Flag_2;
    public TMater Mater;
    public char[] MATERIAL_TYPE;
    public char[] Mtl_ID;
    public char[] Name;

    // The engine (Gfx_G3D_Mesh::LoadMesh) matches this string against the 33-entry
    // technique table at off_6FC800 — BASE / BASE_IMAGE_ALPHA / BASE_VERTEX_ALPHA /
    // NO_TEXTURE / INVISIBLE / etc. Despite the field name, this is the *technique*,
    // not a texture id (matches the existing NO_TEXTURE/INVISIBLE checks in the
    // renderer). Substrings IMAGE_ALPHA / VERTEX_ALPHA gate texture-alpha and
    // per-vertex-alpha sampling, same as .8 MaterialRecord.
    public string TechniqueName => Mtl_ID is { Length: > 0 } ? new string(Mtl_ID) : "";
    public bool UsesImageAlpha => TechniqueName.Contains("IMAGE_ALPHA");
    public bool UsesVertexAlpha => TechniqueName.Contains("VERTEX_ALPHA");

    // Blend mode from the engine's 6-entry off_6FC7C8 table (OPAQUE / ALPHABLEND /
    // MODCOLOR / SUBTRACT / REVSUBTRACT / ADD). Anything other than OPAQUE means the
    // material is doing real per-pixel blending — hair, for example, ships as
    // technique BASE_VERTEX_ALPHA_2X + blend state ALPHABLEND.
    public string BlendStateName => BLEND_STATE is { Length: > 0 } ? new string(BLEND_STATE) : "";
    public bool IsBlended => BlendStateName.Length > 0 && BlendStateName != "OPAQUE";

    // True when the material needs the alpha-tested + alpha-blended pipeline rather
    // than the plain opaque one. Covers all three engine signals: texture-alpha
    // cutout, per-vertex alpha, and any non-OPAQUE blend mode.
    public bool NeedsAlphaPipeline => UsesImageAlpha || UsesVertexAlpha || IsBlended;

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
        if (string.IsNullOrEmpty(b3d.texName)) b3d.texName = new string(Name);
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
    public char[] Name; //: TCharArray;
    public byte Unknown1; //: byte;

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
    public List<int[]> Model_Faces_Index = [];
    public List<Vector3> Model_Vertex_Position = [];
    public List<Vector2> Model_Vertex_Texcoords = [];

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
            Model_Vertex_Texcoords.Add(new Vector2(float.Parse(u) / 1024.0f,
                float.Parse(v) / 1024.0f)); //divide by 1024 to normalize UV
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
                                 where !string.IsNullOrEmpty(ss)
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
                                 where !string.IsNullOrEmpty(ss)
                                 select int.Parse(ss));
            }
        }

        //split tmplist into groups of 3 and add to faces
        for (var i = 0; i < tmpList.Count; i += 3) Model_Faces_Index.Add([tmpList[i], tmpList[i + 1], tmpList[i + 2]]);
    }
}

// Parses the Van Buren OCTREE format ('.8' files) directly off Map::Read8File at
// 0x5b0ad0 in the engine. Top-level chunk order:
//   8TRE
//     HEAD                           4 dwords; flags = HEAD[1]
//     LGTD   (if flags & 4)          INFO + LGTR x N (64 bytes each)
//     LVLD   (if flags & 8)          INFO (16) + LVL0 (lightmap blob)
//     MATD                           INFO + MATR x N (196 bytes each)
//     TXTD                           count1, count2; (count2-1) TXTR sub-chunks
//     VTXD                           node_count; per-CNode VTXB { XYZ?, NRML?, DIFU?, TXUV?, LMUV? }
//     TREE                           NODE/LEAF tree of indices; LEAF { INFO, IDXD/IDXS/FLGS }
public class _8Model
{
    // ----- HEAD -----
    public int Head_Version;
    public int Head_Flags;
    public int Head_Field2;
    public int Head_Field3;
    public bool HasLights => (Head_Flags & 4) != 0;
    public bool HasLightmap => (Head_Flags & 8) != 0;

    // ----- LGTD / LVLD / MATD -----
    public List<LightRecord> Lights = [];
    public LightmapData? Lightmap;
    public List<MaterialRecord> Materials = [];

    // ----- TXTD -----
    // Slot 0 is the engine's reserved "no texture" sentinel; real names start at index 1.
    public List<string> TextureNames = [];

    // ----- VTXD per-CNode buffers, concatenated for backwards compat with consumers -----
    // X is negated and Y/Z are swapped to match the existing renderer convention.
    public List<Vector3> GVP = []; // positions
    public List<Vector3> GVN = []; // normals (engine-style swap)
    public List<Color> GVD = [];   // diffuse vertex colors
    public List<Vector2> GVT = []; // texture coords
    public List<Vector2> GVL = []; // lightmap coords
    // CNodeOffsets[i] = global index where CNode i's vertices start. The list always ends with a sentinel entry equal to GVP.Count.
    public List<int> CNodeOffsets = [];
    public List<int> CNodeVertexCounts = [];

    // ----- TREE-derived face data -----
    public List<int[]> IDX = [];
    public List<int> MDX = [];
    // Per-face material index, sourced from the low byte of each IDXS group's
    // header word (see DecodeIDXS). Parallel to IDX/MDX: MDX gives the bound
    // texture (1-based into the filtered MAT list); MFL picks which
    // MaterialRecord in `Materials` actually applies. The technique on that
    // material is what tells the renderer whether to alpha-test (IMAGE_ALPHA)
    // or alpha-blend per-vertex (VERTEX_ALPHA), so a single texture atlas can
    // host both opaque and alpha-tested cards — e.g. PS_Flora_Desert01_LG:
    // trunks (BASE_2X) and leaf cards (BASE_IMAGE_ALPHA_2X) share one TGA but
    // are split across two MaterialRecords sharing the same texture.
    public List<uint> MFL = [];

    // Stats
    public int NodeCount;
    public int LeafCount;
    public int CNodeCount;

    // Backwards-compat view of TextureNames as BitmapTexture list (skips empty / .ctx)
    public List<BitmapTexture> MAT = [];
    public string MapName = "defaultName";

    public _8Model(string path)
    {
        using var fs = File.Open(path, FileMode.Open, FileAccess.Read);
        Parse(new BinaryReader(fs));
    }

    public _8Model(byte[] file)
    {
        Parse(new BinaryReader(new MemoryStream(file)));
    }

    private void Parse(BinaryReader f)
    {
        if (ReadTag(f) != "8TRE")
            throw new InvalidDataException("Not an 8TRE file");
        f.ReadInt32(); // outer size — engine ignores

        ReadHEAD(f);
        if (HasLights) ReadLGTD(f);
        if (HasLightmap) ReadLVLD(f);
        ReadMATD(f);
        ReadTXTD(f);
        ReadVTXD(f);
        ReadTREE(f);

        // Backwards-compat MAT view: TextureNames in their TXTR order, with .ctx
        // entries skipped. Consumers index this via `MDX[face] - 1`. The MDX `m`
        // values from IDXS are positions in this filtered list, not into Materials.
        for (var i = 1; i < TextureNames.Count; i++)
        {
            var n = TextureNames[i];
            if (string.IsNullOrEmpty(n)) continue;
            if (n.IndexOf("ctx", StringComparison.OrdinalIgnoreCase) >= 0) continue;
            MAT.Add(new BitmapTexture { FileName = n, AlphaSource = 2 });
        }
    }

    private void ReadHEAD(BinaryReader f)
    {
        if (ReadTag(f) != "HEAD") throw new InvalidDataException("Expected HEAD");
        var size = f.ReadInt32();
        var end = f.BaseStream.Position + size;
        if (size >= 16)
        {
            Head_Version = f.ReadInt32();
            Head_Flags = f.ReadInt32();
            Head_Field2 = f.ReadInt32();
            Head_Field3 = f.ReadInt32();
        }
        f.BaseStream.Seek(end, SeekOrigin.Begin);
    }

    private void ReadLGTD(BinaryReader f)
    {
        if (ReadTag(f) != "LGTD") throw new InvalidDataException("Expected LGTD");
        var size = f.ReadInt32();
        var end = f.BaseStream.Position + size;
        if (ReadTag(f) == "INFO")
        {
            f.ReadInt32(); // INFO size, == 4
            var count = f.ReadInt32();
            for (var i = 0; i < count && f.BaseStream.Position + 8 <= end; i++)
            {
                if (ReadTag(f) != "LGTR") break;
                var lgtrSize = f.ReadInt32();
                var n = Math.Min(lgtrSize, 64);
                var raw = f.ReadBytes(n);
                if (lgtrSize > 64) f.BaseStream.Seek(lgtrSize - 64, SeekOrigin.Current);
                Lights.Add(LightRecord.Parse(raw));
            }
        }
        f.BaseStream.Seek(end, SeekOrigin.Begin);
    }

    private void ReadLVLD(BinaryReader f)
    {
        if (ReadTag(f) != "LVLD") throw new InvalidDataException("Expected LVLD");
        var size = f.ReadInt32();
        var end = f.BaseStream.Position + size;
        if (ReadTag(f) == "INFO")
        {
            f.ReadInt32(); // INFO size, == 16
            var lm = new LightmapData
            {
                Width = f.ReadInt32(),
                Height = f.ReadInt32(),
                UnknownF0 = f.ReadSingle(),
                UnknownF1 = f.ReadSingle()
            };
            if (f.BaseStream.Position + 8 <= end && ReadTag(f) == "LVL0")
            {
                var lvl0Size = f.ReadInt32();
                lm.Pixels = f.ReadBytes(lvl0Size);
            }
            Lightmap = lm;
        }
        f.BaseStream.Seek(end, SeekOrigin.Begin);
    }

    private void ReadMATD(BinaryReader f)
    {
        if (ReadTag(f) != "MATD") throw new InvalidDataException("Expected MATD");
        var size = f.ReadInt32();
        var end = f.BaseStream.Position + size;
        if (ReadTag(f) == "INFO")
        {
            f.ReadInt32(); // INFO size, == 4
            var count = f.ReadInt32();
            for (var i = 0; i < count && f.BaseStream.Position + 8 <= end; i++)
            {
                if (ReadTag(f) != "MATR") break;
                var matrSize = f.ReadInt32();
                var n = Math.Min(matrSize, 196);
                var raw = f.ReadBytes(n);
                if (matrSize > 196) f.BaseStream.Seek(matrSize - 196, SeekOrigin.Current);
                Materials.Add(MaterialRecord.Parse(raw));
            }
        }
        f.BaseStream.Seek(end, SeekOrigin.Begin);
    }

    private void ReadTXTD(BinaryReader f)
    {
        if (ReadTag(f) != "TXTD") throw new InvalidDataException("Expected TXTD");
        var size = f.ReadInt32();
        var end = f.BaseStream.Position + size;
        f.ReadInt32(); // count1 — material count, currently unused here
        var count2 = f.ReadInt32();
        TextureNames.Add(""); // engine's [0] sentinel
        for (var i = 1; i < count2 && f.BaseStream.Position + 8 <= end; i++)
        {
            ReadTag(f); // expected "TXTR"
            var subSize = f.ReadInt32();
            var bytes = f.ReadBytes(subSize);
            var nz = Array.IndexOf(bytes, (byte)0);
            TextureNames.Add(Encoding.ASCII.GetString(bytes, 0, nz < 0 ? bytes.Length : nz));
        }
        f.BaseStream.Seek(end, SeekOrigin.Begin);
    }

    private void ReadVTXD(BinaryReader f)
    {
        if (ReadTag(f) != "VTXD") throw new InvalidDataException("Expected VTXD");
        var size = f.ReadInt32();
        var end = f.BaseStream.Position + size;
        var nodeCount = f.ReadInt32();
        CNodeCount = nodeCount;
        const int wantMask = 0x1F00; // engine hard-codes XYZ|NRML|DIFU|TXUV|LMUV in Map::GetOctree
        for (var node = 0; node < nodeCount && f.BaseStream.Position + 8 <= end; node++)
        {
            if (ReadTag(f) != "VTXB") break;
            var vtxbSize = f.ReadInt32();
            var vtxbEnd = f.BaseStream.Position + vtxbSize;
            var vc = f.ReadInt32();
            var baseIdx = GVP.Count;
            CNodeOffsets.Add(baseIdx);
            CNodeVertexCounts.Add(vc);
            // Pre-grow per-attribute arrays so they stay index-aligned regardless of which sub-chunks are present.
            for (var v = 0; v < vc; v++)
            {
                GVP.Add(Vector3.Zero);
                GVN.Add(Vector3.Zero);
                GVD.Add(Color.White);
                GVT.Add(Vector2.Zero);
                GVL.Add(Vector2.Zero);
            }
            var wanted = wantMask;
            while (wanted != 0 && f.BaseStream.Position + 8 <= vtxbEnd)
            {
                var subTag = ReadTag(f);
                var subSize = f.ReadInt32();
                var subEnd = f.BaseStream.Position + subSize;
                switch (subTag)
                {
                    case "XYZ " when (wanted & 0x100) != 0:
                        for (var v = 0; v < vc; v++)
                        {
                            var x = -f.ReadSingle();
                            var y = f.ReadSingle();
                            var z = f.ReadSingle();
                            GVP[baseIdx + v] = new Vector3(x, z, y);
                        }
                        wanted &= ~0x100;
                        break;
                    case "NRML" when (wanted & 0x200) != 0:
                        for (var v = 0; v < vc; v++)
                        {
                            var x = -f.ReadSingle();
                            var y = f.ReadSingle();
                            var z = f.ReadSingle();
                            GVN[baseIdx + v] = new Vector3(x, z, y);
                        }
                        wanted &= ~0x200;
                        break;
                    case "DIFU" when (wanted & 0x400) != 0:
                        // Stored as half-brightness BGRA: each channel is the displayed
                        // value / 2 so the engine can encode overbright (>1.0) lighting
                        // by multiplying by 2 at sample time. We decode + clamp here.
                        for (var v = 0; v < vc; v++)
                        {
                            byte bl = f.ReadByte(), gr = f.ReadByte(), rd = f.ReadByte(), al = f.ReadByte();
                            int r = Math.Min(rd * 2, 255);
                            int g = Math.Min(gr * 2, 255);
                            int b = Math.Min(bl * 2, 255);
                            GVD[baseIdx + v] = Color.FromArgb(al, r, g, b);
                        }
                        wanted &= ~0x400;
                        break;
                    case "TXUV" when (wanted & 0x800) != 0:
                        for (var v = 0; v < vc; v++)
                        {
                            var u = f.ReadSingle();
                            var vv = f.ReadSingle();
                            GVT[baseIdx + v] = new Vector2(u, vv);
                        }
                        wanted &= ~0x800;
                        break;
                    case "LMUV" when (wanted & 0x1000) != 0:
                        for (var v = 0; v < vc; v++)
                        {
                            var u = f.ReadSingle();
                            var vv = f.ReadSingle();
                            GVL[baseIdx + v] = new Vector2(u, vv);
                        }
                        wanted &= ~0x1000;
                        break;
                }
                f.BaseStream.Seek(subEnd, SeekOrigin.Begin);
            }
            f.BaseStream.Seek(vtxbEnd, SeekOrigin.Begin);
        }
        CNodeOffsets.Add(GVP.Count); // sentinel for "past the end"
        f.BaseStream.Seek(end, SeekOrigin.Begin);
    }

    private void ReadTREE(BinaryReader f)
    {
        if (ReadTag(f) != "TREE") throw new InvalidDataException("Expected TREE");
        var size = f.ReadInt32();
        var declaredEnd = f.BaseStream.Position + size;
        // TREE's declared size is unreliable — at least 03_Tutorial_Junktown
        // has a final LEAF whose body runs past `declaredEnd` into trailing file
        // bytes (the engine never uses this size as a bound). Walk against the
        // stream length instead so we don't drop the last subtree.
        WalkTreeNode(f, f.BaseStream.Length);
        // Seek to the larger of declaredEnd or current position so any caller
        // logic that expects to be past the TREE chunk still works.
        var resumeAt = Math.Max(declaredEnd, f.BaseStream.Position);
        if (resumeAt <= f.BaseStream.Length)
            f.BaseStream.Seek(resumeAt, SeekOrigin.Begin);
    }

    // Tag-driven recursive walk that mirrors OCTREE::ReadTree. Both NODE and LEAF
    // can carry an inline IDXD/IDXS [/FLGS] block after their INFO chunk; the
    // engine's post-INFO loop accepts NODE/LEAF/IDXD/FLGS interchangeably and
    // skips DBUG. NODE additionally has an 8-dword child-presence mask at INFO
    // offset 32; non-zero children follow inline as further NODE/LEAF subtrees.
    // The size fields on NODE/LEAF/IDXD are read but not used to bound — the
    // engine ignores them too.
    private void WalkTreeNode(BinaryReader f, long limit)
    {
        if (f.BaseStream.Position + 8 > limit) return;
        var tag = ReadTag(f);
        f.ReadInt32(); // size (unused, tag-driven)
        var isNode = tag == "NODE";
        var isLeaf = tag == "LEAF";
        if (!isNode && !isLeaf) return;
        if (isNode) NodeCount++; else LeafCount++;

        if (f.BaseStream.Position + 8 > limit) return;
        if (ReadTag(f) != "INFO") return;
        var infoSize = f.ReadInt32();
        var infoStart = f.BaseStream.Position;
        // INFO data layout (lifted from the engine's `v7+3` read):
        //   offset 0..23 — bbox / metadata
        //   offset 24..27 — cnode_idx (used when this node has its own IDXS)
        //   offset 32..63 — 8-dword child presence mask (NODE only)
        var cnodeIdx = 0;
        if (infoSize >= 28)
        {
            f.BaseStream.Seek(infoStart + 24, SeekOrigin.Begin);
            cnodeIdx = f.ReadInt32();
        }
        var presence = new int[8];
        if (isNode && infoSize >= 64)
        {
            f.BaseStream.Seek(infoStart + 32, SeekOrigin.Begin);
            for (var i = 0; i < 8; i++) presence[i] = f.ReadInt32();
        }
        f.BaseStream.Seek(infoStart + infoSize, SeekOrigin.Begin);

        // Optionally consume an inline IDXD/IDXS [/FLGS] block. NODEs may or may
        // not carry one; LEAFs typically do but a malformed leaf can omit it.
        if (f.BaseStream.Position + 8 <= limit)
        {
            var probe = f.BaseStream.Position;
            var t = ReadTag(f);
            if (t == "IDXD")
            {
                f.ReadInt32(); // IDXD size — engine ignores
                if (ReadTag(f) == "IDXS")
                {
                    var idxsSize = f.ReadInt32();
                    var idxsEnd = f.BaseStream.Position + idxsSize;
                    DecodeIDXS(f, idxsEnd, cnodeIdx);
                    f.BaseStream.Seek(idxsEnd, SeekOrigin.Begin);
                    // FLGS is per-face state read by the engine but not the
                    // material assignment (that lives in IDXS group headers,
                    // see DecodeIDXS). Its contents look like coarse face-
                    // category flags that we don't need for rendering.
                    if (f.BaseStream.Position + 8 <= limit)
                    {
                        var probe2 = f.BaseStream.Position;
                        if (ReadTag(f) == "FLGS")
                        {
                            var flgsCount = f.ReadInt32();
                            f.BaseStream.Seek(flgsCount * 4L, SeekOrigin.Current);
                        }
                        else
                        {
                            f.BaseStream.Seek(probe2, SeekOrigin.Begin);
                        }
                    }
                }
            }
            else
            {
                f.BaseStream.Seek(probe, SeekOrigin.Begin);
            }
        }

        if (isNode)
        {
            for (var i = 0; i < 8; i++)
            {
                if (presence[i] == 0) continue;
                WalkTreeNode(f, limit);
            }
        }
    }

    // IDXS payload is a sequence of groups, each headed by a "material switch":
    //   - 0x80NN  w1  (0x4000+ww2)  w3  m   : verbose form (per-group state)
    //   - 0x40NN  w1                m       : compact form
    //   - 0x00NN                            : minimal — tmp itself is m
    //   followed by `count` (u16, =3*tris) and `tris` triangles of u16 indices.
    //
    // What each header field means (reverse-engineered from F3.exe's
    // Octree2Gfx::ConstructRenderData, which builds per-batch render structs at
    // sub_4BDEA0:0x4bf252 — `material = materials[batch[3]]`):
    //   m       — texture index (1-based into the filtered MAT list, 0 = no
    //             texture; selects which TGA gets bound for the group).
    //   tmp&0xFF — the 0-based MaterialRecord index for the group. This is what
    //             picks the technique (BASE, BASE_IMAGE_ALPHA_2X, etc.) and the
    //             blend op. The same texture (m) can appear with multiple
    //             tmp_low values, so a foliage atlas can mix opaque trunks
    //             (tmp_low → BASE_2X) with alpha-tested cards (tmp_low →
    //             BASE_IMAGE_ALPHA_2X).
    //   w1, w3   — vertex-range bounds within the cnode's vertex buffer (used
    //             for D3D's DrawIndexedPrimitive minIndex/numVertices). Not
    //             needed for our renderer — we draw the whole index buffer.
    //   w2 - 0x4000 — secondary state bit (FX/lighting variant); engine reads
    //                it but the visible effect is small and we ignore it.
    private void DecodeIDXS(BinaryReader f, long end, int cnodeIdx)
    {
        var baseOffset = cnodeIdx >= 0 && cnodeIdx < CNodeOffsets.Count - 1
            ? CNodeOffsets[cnodeIdx]
            : 0;
        var m = 0;
        var matIdx = 0;
        while (f.BaseStream.Position + 12 <= end)
        {
            var tmp = f.ReadUInt16();
            if (tmp == 0) tmp = f.ReadUInt16();
            if ((tmp & 0xFF00) == 0x8000)
            {
                matIdx = tmp & 0xFF;
                f.ReadUInt16();
                _ = f.ReadUInt16() - 0x4000;
                f.ReadUInt16();
                m = f.ReadUInt16();
            }
            else if ((tmp & 0xFF00) == 0x4000)
            {
                matIdx = tmp & 0xFF;
                f.ReadUInt16();
                m = f.ReadUInt16();
            }
            else if ((tmp & 0xFF00) == 0)
            {
                m = tmp;
                // Minimal form has no per-group material; the engine inherits
                // the previously-set one. Keep matIdx as-is across iterations.
            }
            var sz = f.ReadUInt16();
            for (var i = 0; i < sz / 3; i++)
            {
                if (f.BaseStream.Position + 6 > end) break;
                var a = baseOffset + f.ReadUInt16();
                var b = baseOffset + f.ReadUInt16();
                var c = baseOffset + f.ReadUInt16();
                IDX.Add([a, c, b]);
                MDX.Add(m);
                MFL.Add((uint)matIdx);
            }
        }
    }

    private static string ReadTag(BinaryReader f)
    {
        var bytes = f.ReadBytes(4);
        return bytes.Length < 4 ? "" : Encoding.ASCII.GetString(bytes);
    }

    public struct BitmapTexture
    {
        public string FileName;
        public int AlphaSource;
    }
}

public sealed class LightRecord
{
    public byte[] Raw = Array.Empty<byte>();
    // Layout inferred from MarkTest LGTR (64 bytes); fields beyond what's named here
    // are still under analysis and remain accessible via Raw.
    public Vector3 Color;
    public Vector3 Position;
    public Vector3 Direction;
    public float Radius;
    public float Intensity;

    public static LightRecord Parse(byte[] raw)
    {
        var r = new LightRecord { Raw = raw };
        if (raw.Length >= 0x14)
            r.Color = new Vector3(
                BitConverter.ToSingle(raw, 0x08),
                BitConverter.ToSingle(raw, 0x0C),
                BitConverter.ToSingle(raw, 0x10));
        if (raw.Length >= 0x20)
            r.Position = new Vector3(
                BitConverter.ToSingle(raw, 0x14),
                BitConverter.ToSingle(raw, 0x18),
                BitConverter.ToSingle(raw, 0x1C));
        if (raw.Length >= 0x2C)
            r.Direction = new Vector3(
                BitConverter.ToSingle(raw, 0x20),
                BitConverter.ToSingle(raw, 0x24),
                BitConverter.ToSingle(raw, 0x28));
        if (raw.Length >= 0x30) r.Radius = BitConverter.ToSingle(raw, 0x2C);
        if (raw.Length >= 0x34) r.Intensity = BitConverter.ToSingle(raw, 0x30);
        return r;
    }
}

public sealed class LightmapData
{
    public int Width;
    public int Height;
    public float UnknownF0;
    public float UnknownF1;
    public byte[] Pixels = Array.Empty<byte>();
}

public sealed class MaterialRecord
{
    public byte[] Raw = Array.Empty<byte>();
    // MATR layout (196 bytes), reconstructed from F3.exe's
    // Octree2Gfx::ConstructRenderData (sub_4BDEA0):
    //   0x00 u32 (name length / id)
    //   0x04 64-byte technique name — strcmp'd against a fixed 33-entry table
    //        (off_6FC800: BASE, BASE_LM, BASE_IMAGE_ALPHA, BASE_VERTEX_ALPHA, ...).
    //        Substrings IMAGE_ALPHA / VERTEX_ALPHA in the name gate texture-alpha
    //        and per-vertex-alpha sampling respectively. This is how a single
    //        foliage atlas can have alpha-tested cards next to opaque trunks.
    //   0x58 16-byte blend-state name — strcmp'd against a 6-entry table
    //        (off_6FC7C8: OPAQUE, ALPHABLEND, MODCOLOR, SUBTRACT, REVSUBTRACT, ADD).
    //   0x74 16-byte sound type (e.g. "SILENT")
    //   0x88 u32 flag word OR'd into the per-mesh render flags
    //   0xB0..0xBC four u32 TCFX params (passed to Gfx_TCFX_Param::SetParam)
    //   0xC0 u32 packed flags + texture-stage indices: bits 0x10/0x08/0x04 enable
    //        the base/lightmap/third stages; (value >> 5) & 0xF is the 4-bit slot
    //        used to look up which actual texture is bound for each enabled stage.
    public string Name = "";
    public string BlendState = "";
    public string SoundType = "";
    public uint StageFlags;

    public bool UsesImageAlpha => Name.Contains("IMAGE_ALPHA");
    public bool UsesVertexAlpha => Name.Contains("VERTEX_ALPHA");

    public static MaterialRecord Parse(byte[] raw)
    {
        var m = new MaterialRecord { Raw = raw };
        if (raw.Length >= 0x44) m.Name = ReadAsciiZ(raw, 0x04, 64);
        if (raw.Length >= 0x68) m.BlendState = ReadAsciiZ(raw, 0x58, 16);
        if (raw.Length >= 0x84) m.SoundType = ReadAsciiZ(raw, 0x74, 16);
        if (raw.Length >= 0xC4) m.StageFlags = BitConverter.ToUInt32(raw, 0xC0);
        return m;
    }

    private static string ReadAsciiZ(byte[] data, int offset, int max)
    {
        var n = 0;
        while (n < max && offset + n < data.Length && data[offset + n] != 0) n++;
        return Encoding.ASCII.GetString(data, offset, n);
    }
}

// Normalizes bone names so the same bone produces the same string regardless of
// whether it came from a B3D mesh or a GR2 skeleton/animation. Mirrors
// processBoneName() from the reference VB3dConvSrcEng/G3D.h.
public static class BoneNames
{
    public static string ProcessName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        // 1. Spaces -> underscores.
        name = name.Replace(' ', '_');

        // 2. If the name leads with a digit, move it to the end. When the second
        //    char is also '_', insert an extra '_' so "0_Foo" -> "Foo_0" (vs.
        //    "5Bones" -> "Bones5"). Matches the in-place pointer juggling in the
        //    reference's processBoneName.
        if (char.IsDigit(name[0]))
        {
            var leading = name[0];
            if (name.Length > 1 && name[1] == '_')
                name = name.Substring(2) + "_" + leading;
            else
                name = name.Substring(1) + leading;
        }

        // 3. Strip trailing "_1" from "*_Clavicle_1" / "*_Fingers_1" — the
        //    reference treats these as the canonical bone, with "_1" being a
        //    spurious suffix from the exporter.
        if ((name.Contains("_Clavicle") || name.Contains("_Fingers")) && name.EndsWith("_1"))
            name = name.Substring(0, name.Length - 2);

        return name;
    }
}