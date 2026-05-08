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

    // Top-level token 0x16 — int32 payload. Observation: -1 correlates with mis-rotated
    // static props (DS_carjunk_01_glass etc.); engine semantics still TBD. Captured so
    // the renderer can correlate at placement time. Null means the token wasn't present.
    public int? Token16Value;

    // Top-level token 0x06 — 3 dwords (engine implies a 4th = 1.0f). Engine also flips
    // a flag at struct[144] when this token is present.
    public int[]? Token06Values;
    public bool HasToken06;

    // Per-node transforms parsed from token 0x0D inside ParseNode. Empty for files like
    // carjunk_01_glass that don't emit transforms. The Name is the most recent 0x0C
    // (node name) seen in the same node block.
    public List<NodeTransform> NodeTransforms = [];

    // Header strings/values we previously discarded outright. Captured so the
    // diagnostic dump can show whether a per-model orientation flag lives here.
    public string CoordinateSystem = "";       // token 0x05 (e.g. "LEFT_HANDED")
    public float GlobalScaleFactor;            // token 0x03
    public float CoordinateScale;              // token 0x04

    // First-vertex-node u1/u2 bytes — currently dropped on the floor as "VertNode U1/U2".
    // Captured so we can correlate against rotation behaviour without grepping the log.
    public byte VertNode_U1;
    public byte VertNode_U2;
    public bool VertNode_Captured;

    // Names of the nodes encountered (in tree-walk order). For carjunk that's
    // ["Scene Root", "DS_carjunk_01_glass"]; for many props it's just ["Scene Root"].
    public List<string> NodeNames = [];

    // Hierarchical scene graph built during parsing. SceneRoot is the outermost
    // 0x0A block (typically named "Scene Root"); its children are nested 0x0A
    // blocks. Each B3DSceneNode owns the 0x0D transforms and submesh-keys
    // declared inside its own 0x0A. SceneNodeBySubmesh maps the material-name
    // key (the same key used in SubModels / MatNameToTexture) to the node it
    // was declared in, so the renderer can walk parent chains for per-submesh
    // world matrices. Both are null/empty for files with no scene graph.
    public B3DSceneNode? SceneRoot;
    public Dictionary<string, B3DSceneNode> SceneNodeBySubmesh = new();

    // Token 0x1C value byte. The engine stores this as model[0]; in
    // RecurseChildNode (0x527fd0) it gates a quaternion-conjugate + Z-flip on
    // every node transform during the scene-graph walk:
    //   if (model[0] == 0) { negate quat.xyz; negate translation.z; }
    // We capture it here so the renderer can apply the same flip when present.
    public byte Token1CByte;
    public bool HasToken1C;
    
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

    // True when the file declares the very-old "VT=0x35" header variant (CR_Sheep,
    // CR_Bear, CR_Badger and similar pre-conversion critter meshes). It uses an
    // alternate set of tag IDs (0x0F/0x10/0x18 at top level for material/texture/
    // bone, 0x13 for node-begin, 0x14 for node-end, 0x16 for node name, 0x19 for
    // G3D_Surface) and a smaller 36-byte TVertex44 layout that drops the trailing
    // two ints. None of those IDs collide with the modern (VT=0x1C) tag set, so
    // we can dispatch them unconditionally; the flag exists so the structs that
    // share a parser path (TMaterials, TMaterial_Data44, TVertex_Node) can pick
    // the right wire format.
    public bool IsLegacyV35;

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

        Token16Value = null;
        Token06Values = null;
        HasToken06 = false;
        NodeTransforms = [];
        CoordinateSystem = "";
        GlobalScaleFactor = 0f;
        CoordinateScale = 0f;
        VertNode_U1 = 0;
        VertNode_U2 = 0;
        VertNode_Captured = false;
        NodeNames = [];
        IsLegacyV35 = false;

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
                    GlobalScaleFactor = f.ReadSingle();
                    break;
                case 0x04:
                    CoordinateScale = f.ReadSingle();
                    break;
                case 0x05:
                    CoordinateSystem = new string(StringRead(f));
                    break;
                case 0x06:
                {
                    // 3 dwords; engine implies a 4th component of 1.0f. Engine also flips
                    // a flag at struct[144] when this token is present, so capture both the
                    // values and a "saw it" bool for renderer correlation.
                    Token06Values = [f.ReadInt32(), f.ReadInt32(), f.ReadInt32()];
                    HasToken06 = true;
                    break;
                }
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
                {
                    // First top-level 0x0A is the model's Scene Root; subsequent
                    // top-level 0x0A blocks (rare, but we handle them) become
                    // siblings under the same root so the scene graph stays
                    // single-rooted for the renderer's walk.
                    B3DSceneNode node;
                    if (SceneRoot == null) { node = new B3DSceneNode(); SceneRoot = node; }
                    else
                    {
                        node = new B3DSceneNode { Parent = SceneRoot };
                        SceneRoot.Children.Add(node);
                    }
                    ParseNode(f, b3d, node);
                    break;
                }
                case 0x0E:
                {
                    var bone = new TBone(f);
                    b3d.Bones.Add(bone);
                    break;
                }
                case 0x16:
                    // engine: int32 stored at struct[24]. -1 observed on mis-rotated static
                    // props (e.g. DS_carjunk_01_glass); engine consumer not yet identified.
                    Token16Value = f.ReadInt32();
                    break;
                case 0x2F:
                    // Legacy-V35 equivalent of 0x16 (top-level int32). All three sample
                    // critter meshes carry the same -1 here.
                    Token16Value = f.ReadInt32();
                    break;
                case 0x17:
                    // sub_4D2620: 3 padded strings + 16 + 4 + 4 + 4 bytes
                    StringRead(f);
                    StringRead(f);
                    StringRead(f);
                    f.ReadBytes(28);
                    break;
                case 0x1C:
                    // Engine: this byte is stored at model[0] and gates a coord-handedness
                    // flip during the scene-graph walk (RecurseChildNode at 0x527fd0):
                    //   if (model[0] == 0) negate per-node quat.xyz and translation.z.
                    // Capture both the raw byte and the "saw it" flag so the renderer
                    // can apply the same fix-up. Token 0x1C is also the modern-format
                    // signal (sets FormatVer=0).
                    b3d.Token1CByte = f.ReadByte();
                    b3d.HasToken1C = true;
                    b3d.FormatVer = 0;
                    break;
                case 0x35:
                    // Pre-conversion legacy header (CR_Sheep / CR_Bear / CR_Badger). Same
                    // shape as 0x1C — 1-byte version follows — but it switches the parser
                    // to the legacy tag set + compact TVertex44 layout used by these
                    // older critter meshes.
                    f.ReadByte();
                    b3d.FormatVer = 1;
                    b3d.IsLegacyV35 = true;
                    break;
                case 0x0F:
                {
                    // Legacy-V35 TMaterials (modern equivalent: 0x07). Same Mtl_ID + Name
                    // + 18-float TMater + BLEND_STATE prefix, but no MATERIAL_TYPE / Flag_1
                    // / Flag_2 — five zero-filler bytes instead.
                    var mat = new TMaterials(f, legacy: true);
                    Materials.Add(mat);
                    MaterialDict[new string(mat.Name)] = mat;
                    break;
                }
                case 0x10:
                {
                    // Legacy-V35 TTextures (modern equivalent: 0x08/0x09). Same wire format.
                    _ = new TTextures(f, b3d);
                    break;
                }
                case 0x18:
                {
                    // Legacy-V35 TBone (modern equivalent: 0x0E). Same wire format.
                    var bone = new TBone(f);
                    b3d.Bones.Add(bone);
                    break;
                }
                case 0x13:
                {
                    // Legacy-V35 NODE_BEGIN (modern equivalent: 0x0A). The inner-tag set
                    // is also remapped (0x14 end, 0x16 name, 0x19 surface) so ParseNode
                    // dispatches based on b3d.IsLegacyV35.
                    B3DSceneNode node;
                    if (SceneRoot == null) { node = new B3DSceneNode(); SceneRoot = node; }
                    else
                    {
                        node = new B3DSceneNode { Parent = SceneRoot };
                        SceneRoot.Children.Add(node);
                    }
                    ParseNode(f, b3d, node);
                    break;
                }
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
    private void ParseNode(BinaryReader f, B3DModel b3d, B3DSceneNode currentNode)
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
                case 0x13: // legacy-V35 NODE_BEGIN
                {
                    var child = new B3DSceneNode { Parent = currentNode };
                    currentNode.Children.Add(child);
                    ParseNode(f, b3d, child);
                    break;
                }
                case 0x0B:
                case 0x14: // legacy-V35 NODE_END
                    return;
                case 0x0C:
                case 0x16 when b3d.IsLegacyV35: // legacy-V35 NODE_NAME (re-uses 0x16 inside nodes)
                    // 0x0C names the *current* scene node. NodeNames is kept around for
                    // backwards-compat with the diagnostic dump.
                    currentNode.Name = new string(StringRead(f)); // node Name (sub_4D0670 → sub_4D0450)
                    NodeNames.Add(currentNode.Name);
                    break;
                case 0x0D:
                {
                    var xf = ReadTransform(f, currentNode.Name);
                    currentNode.Transforms.Add(xf);
                    NodeTransforms.Add(xf);
                    break;
                }
                case 0x0F:
                case 0x19 when b3d.IsLegacyV35: // legacy-V35 G3D_Surface
                {
                    var vn = new TVertex_Node(f, b3d);
                    for (var index = 0; index < vn.material_data.Count; index++)
                    {
                        var MD = vn.material_data[index];
                        string key;
                        if (MD is TMaterial_Data44 MD44)
                        {
                            key = new string(MD44.Name);
                            SubModels[key] = vn.face_indices[index];
                            MatNameToTexture[key] = new string(MD44.Zone);
                        }
                        else if (MD is TMaterial_Data MD64)
                        {
                            key = new string(MD64.Name);
                            SubModels[key] = MD64.face_indices;
                            MatNameToTexture[key] = new string(MD64.Mtl_ID);
                        }
                        else
                        {
                            Console.WriteLine("Unknown material data type in Vertex_Node: " + MD.GetType());
                            continue;
                        }
                        currentNode.SubmeshKeys.Add(key);
                        SceneNodeBySubmesh[key] = currentNode;
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

    // Mirrors G3D_Transform::Read at sub_4D0A00. Padded-string Name + 1-byte
    // flag + conditional payloads. Engine wire sizes (verified against
    // sub_4C9400 / sub_4C97A0):
    //   bit 0 → 3 floats translation
    //   bit 1 → 4 floats rotation: (axis_x, axis_y, axis_z, angle_degrees)
    //                              quat = (axis_normalised × sin(deg·π/360),
    //                                      cos(deg·π/360))
    //   bit 2 → 3 floats scale
    // Earlier 6/8/6 reads here were over-shooting and corrupting downstream
    // tokens — that's the parsing bug that was hiding the per-template
    // rotations the engine bakes into Scene Root → child node transforms.
    private static NodeTransform ReadTransform(BinaryReader f, string nodeName)
    {
        var nx = new NodeTransform { OwnerNodeName = nodeName };
        nx.TransformName = new string(StringRead(f));
        nx.Flags = f.ReadByte();
        if ((nx.Flags & 0x01) != 0)
            nx.Translation = new Vector3(f.ReadSingle(), f.ReadSingle(), f.ReadSingle());
        if ((nx.Flags & 0x02) != 0)
            nx.RotationAxisAngle = new Vector4(f.ReadSingle(), f.ReadSingle(), f.ReadSingle(), f.ReadSingle());
        if ((nx.Flags & 0x04) != 0)
            nx.Scale = new Vector3(f.ReadSingle(), f.ReadSingle(), f.ReadSingle());
        return nx;
    }

    // One-line summary for the diagnostic dump in ResourceSystem. Lets us correlate
    // every per-model signal we've parsed against visually-broken vs working props.
    public string DescribeTransformSignals()
    {
        var t16 = Token16Value.HasValue ? Token16Value.Value.ToString() : "-";
        var t06 = HasToken06 ? "yes" : "no ";
        var coordSys = string.IsNullOrEmpty(CoordinateSystem) ? "?" : CoordinateSystem;
        var nodes = NodeNames.Count == 0 ? "-" : string.Join("/", NodeNames);

        // Bone summary — list every bone with flag != 0 so we can spot any baked
        // rotation we're not applying. Compact format: name@flag(t…|r…) per bone,
        // sep by `|`. Bones with flag=0 (identity) are omitted to keep the line short.
        string boneSummary = "noBones";
        if (Bones.Count > 0)
        {
            var parts = new List<string>();
            for (var i = 0; i < Bones.Count; i++)
            {
                var b = Bones[i];
                if (b.Flag == 0) continue;
                var nm = string.IsNullOrEmpty(b.Name) ? $"b{i}" : b.Name;
                var t = $"t({b.Translation.X:F2},{b.Translation.Y:F2},{b.Translation.Z:F2})";
                var r = b.Flag == 3
                    ? $"r({b.Rotation.X:F2},{b.Rotation.Y:F2},{b.Rotation.Z:F2},{b.Rotation.W:F2})"
                    : "r-none";
                parts.Add($"{nm}@{b.Flag}{t}{r}");
            }
            boneSummary = $"bones={Bones.Count}[{string.Join("|", parts)}]";
        }

        // Vertex bbox + the (failed) primary-axis rotation prediction. The prediction
        // matched n=3 perfectly but breaks past that — we keep it in the dump so we
        // can compare predicted-vs-actual for any new sample the user reports as
        // broken or wrongly-corrected, which narrows down what the real encoding
        // depends on.
        string bbox = "noVerts";
        string predRot = "predRot=?";
        if (Model_Vertex_Position.Count > 0)
        {
            float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;
            foreach (var v in Model_Vertex_Position)
            {
                if (v.X < minX) minX = v.X; if (v.X > maxX) maxX = v.X;
                if (v.Y < minY) minY = v.Y; if (v.Y > maxY) maxY = v.Y;
                if (v.Z < minZ) minZ = v.Z; if (v.Z > maxZ) maxZ = v.Z;
            }
            bbox = $"bbox=({minX:F1},{minY:F1},{minZ:F1})..({maxX:F1},{maxY:F1},{maxZ:F1})";
            var sX = maxX - minX; var sY = maxY - minY; var sZ = maxZ - minZ;
            var cX = (minX + maxX) * 0.5f; var cZ = (minZ + maxZ) * 0.5f;
            float r;
            if (sY >= sX && sY >= sZ) r = 0f;
            else if (sX > sZ) r = cX >= 0f ? -90f : 90f;
            else r = cZ < 0f ? 180f : 0f;
            predRot = $"predRot={r,4:F0}";
        }

        // Per-material data we previously discarded. We dump every material's worth of
        // newly-captured ints — the loop-of-16 in TMaterial_Data44 is the most likely
        // place a per-template orientation flag could hide. Show LoopHitIndex (which
        // iteration hit Unknown2==1) and the LoopInts up to that point, plus the
        // standalone PostMatTypeInt.
        string matExtra = "noMat";
        if (Vertex_Nodes.Count > 0 && Vertex_Nodes[0].material_data.Count > 0)
        {
            var parts = new List<string>();
            foreach (var md in Vertex_Nodes[0].material_data)
            {
                if (md is TMaterial_Data44 md44)
                {
                    var loopShown = md44.LoopHitIndex > 0
                        ? string.Join(",", md44.LoopInts.Take(md44.LoopHitIndex))
                        : string.Join(",", md44.LoopInts);
                    parts.Add($"[postMtInt={md44.PostMatTypeInt},hit@{md44.LoopHitIndex},loop=({loopShown})]");
                }
                else if (md is TMaterial_Data md64)
                {
                    parts.Add($"[U3={md64.Unknown3},postStrB={md64.PostStringByte},preKkU={md64.PreKkUint}]");
                }
            }
            matExtra = $"MDExtra={string.Join("", parts)}";
        }

        // Top-level TMaterials list — Mtl_ID / Name / BLEND_STATE / MATERIAL_TYPE / Flag_1 / Flag_2.
        // Captured in the parser but never previously surfaced. The Flag_1/Flag_2 ints
        // are the most likely to encode a per-template orientation since they're
        // explicitly named "flag" and we don't otherwise interpret them.
        string topMats = "noTopMat";
        if (Materials.Count > 0)
        {
            var parts = new List<string>();
            for (var i = 0; i < Materials.Count; i++)
            {
                var m = Materials[i];
                var nm = new string(m.Name);
                var id = new string(m.Mtl_ID);
                var bs = new string(m.BLEND_STATE);
                var mt = new string(m.MATERIAL_TYPE);
                parts.Add($"[{nm}|{id}|bs={bs}|mt={mt}|f1={m.Flag_1}|f2={m.Flag_2}]");
            }
            topMats = $"TopMats({Materials.Count})={string.Join("", parts)}";
        }

        // Scene-graph dump: every node + its transforms + submesh keys, depth-first.
        // This is the fallback diagnostic for investigating per-template rotation when
        // the bbox/Token16/Bones signals all look identical between a working and a
        // mis-rotated prop.
        string sceneTree = "noScene";
        if (SceneRoot != null)
        {
            var sb = new StringBuilder();
            void Walk(B3DSceneNode n, int d)
            {
                sb.Append(' ', d * 2);
                sb.Append('[').Append(string.IsNullOrEmpty(n.Name) ? "?" : n.Name).Append(']');
                foreach (var t in n.Transforms)
                {
                    sb.Append(" T(");
                    if (t.Translation.HasValue) sb.Append("t=").Append(t.Translation.Value);
                    if (t.RotationAxisAngle.HasValue) sb.Append(" rDeg=").Append(t.RotationAxisAngle.Value);
                    if (t.Scale.HasValue) sb.Append(" s=").Append(t.Scale.Value);
                    sb.Append(')');
                }
                if (n.SubmeshKeys.Count > 0)
                    sb.Append(" mesh:").Append(string.Join(",", n.SubmeshKeys));
                sb.Append(" | ");
                foreach (var c in n.Children) Walk(c, d + 1);
            }
            Walk(SceneRoot, 0);
            sceneTree = sb.ToString();
        }

        var flavor = IsLegacyV35 ? "V35" : (FormatVer == 0 ? "modern" : "legacy");
        return $"Flavor={flavor,-6} FmtVer={FormatVer} cs={coordSys,-12} Token16={t16,-4} T06={t06} T1C={(HasToken1C ? Token1CByte.ToString() : "-")} NodeXform={NodeTransforms.Count} {boneSummary} {bbox} {predRot} nodes=[{nodes}] scene={{{sceneTree}}} {matExtra} {topMats}";
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

            Normal.X = -f.ReadSingle();
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
            if (b3d.IsLegacyV35)
            {
                // Legacy-V35 critter meshes ship a 36-byte vertex (no trailing
                // two ints). The G3D_Surface header in those files reports
                // SizeOfVerts == 36, matching this short layout.
            }
            else
            {
                Unknown1[0] = f.ReadInt32();
                Unknown1[1] = f.ReadInt32();
            }
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

        // All 17 floats from the loop that previously kept only the last one. Decoded
        // form is "ambient(4)+diffuse(4)+emissive(4)+specular(4)+shininess+alpha" — but
        // since this format wasn't fully verified, capture all 17 so the diagnostic
        // can spot any non-color-looking value (e.g. a baked rotation/axis).
        public float[] AllFloats17 = new float[17];

        // The `a` int read after MATERIAL_TYPE that was previously printed-and-discarded,
        // and the up-to-16 ints from the search loop before Unknown2 == 1 was found.
        // Capturing both — if any of these vary across broken vs working models, that's
        // the per-material flag we've been missing.
        public int PostMatTypeInt;
        public int[] LoopInts = new int[16];
        public int LoopHitIndex = -1;            // index where Unknown2==1 was hit (or -1 if never)

        public TMaterial_Data44(BinaryReader f, B3DModel b3d)
        {
            //Console.WriteLine("TMaterial_Data44");
            Name = StringRead(f);
            Mtl_ID = StringRead(f);
            for (var i = 0; i < 17; i++)
                AllFloats17[i] = Unknown1 = f.ReadSingle();
            BLEND_STATE = StringRead(f);
            // Legacy-V35 critter meshes drop MATERIAL_TYPE here. The two-byte
            // strlen prefix isn't even present, so we just leave the field empty.
            MATERIAL_TYPE = b3d.IsLegacyV35 ? [] : StringRead(f);
            PostMatTypeInt = f.ReadInt32();
            for (var i = 1; i <= 16; i++)
            {
                Unknown2 = f.ReadInt32();
                LoopInts[i - 1] = Unknown2;
                if (Unknown2 == 1)
                {
                    LoopHitIndex = i;
                    Zone = StringRead(f);
                    b3d.Txs.Add(new string(Name));
                    if (!b3d.IsLegacyV35)
                    {
                        // Modern files follow Zone with `count(int32)` then `count`
                        // small strings (the Name split on spaces). Legacy-V35 stops
                        // right after Zone — the next read belongs to the outer
                        // post-material string list.
                        Unknown2 = f.ReadInt32();
                        for (var j = 1; j <= Unknown2; j++) {
                            StringRead(f);
                        }
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

        // Bytes the parser previously read & dropped on the floor:
        // (1) the byte read between the Mtl_ID strings and the unknown uint32
        // (2) the uint32 read just before the face count `kk`
        // Either could plausibly carry a per-mesh orientation hint we've been ignoring.
        public byte PostStringByte;
        public uint PreKkUint;

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
            PostStringByte = f.ReadByte();
            PreKkUint = f.ReadUInt32();
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
        if (!b3d.VertNode_Captured)
        {
            b3d.VertNode_U1 = (byte)Unknown1;
            b3d.VertNode_U2 = (byte)Unknown2;
            b3d.VertNode_Captured = true;
        }
        Console.WriteLine($"VertNode U1 {Unknown1} U2 {Unknown2}");

        if (b3d.IsLegacyV35)
        {
            // Legacy-V35 G3D_Surface header carries an extra byte between u1/u2
            // and NumberVerts compared to the modern (FormatVer==0) layout: a
            // VertexTypeFlag byte (always 1 in the samples) plus a one-byte pad.
            VertexTypeFlag = f.ReadByte();
            f.ReadByte(); // pad
            b3d.Vertex_Type_Flag = VertexTypeFlag;
        }
        else if (b3d.FormatVer == 0)
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

    public TMaterials(BinaryReader f) : this(f, legacy: false) { }

    public TMaterials(BinaryReader f, bool legacy)
    {
        Mtl_ID = StringRead(f);
        Name = StringRead(f);
        Mater = new TMater(f);
        BLEND_STATE = StringRead(f);
        if (legacy)
        {
            // Legacy-V35 omits MATERIAL_TYPE / Flag_1 / Flag_2 and just leaves
            // five trailing zero bytes before the next tag. Consume them so the
            // outer tag stream stays aligned.
            MATERIAL_TYPE = [];
            f.ReadBytes(5);
        }
        else
        {
            MATERIAL_TYPE = StringRead(f);
            Flag_1 = f.ReadInt32();
            Flag_2 = f.ReadInt32();
        }
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

// Per-node transform parsed from token 0x0D inside ParseNode. Raw float arrays for now —
// engine semantics for the 24/32/24-byte payloads aren't fully decoded, so we keep the
// bytes and let the renderer (or a future pass) interpret once we know what they mean.
public sealed class NodeTransform
{
    public string OwnerNodeName = ""; // most-recent 0x0C name in the same node block
    public string TransformName = ""; // padded-string name read by the transform itself
    public byte Flags;                // bit0 → translation, bit1 → rotation, bit2 → scale

    // Engine sizes (sub_4D0A00 → sub_4C9400 / sub_4C97A0):
    //   translation = 3 floats
    //   rotation    = 4 floats: (axis_x, axis_y, axis_z, angle_degrees)
    //                 Engine converts via: half = deg * π/360, quat = (axis * sin(half), cos(half))
    //                 (dbl_708048 = 2π, file constant 1/720, so dbl_708048 / 720 = π/360)
    //   scale       = 3 floats
    // Earlier 6/8/6 reads here were over-shooting and clobbering subsequent blocks —
    // that's why per-template orientation has been invisible in our parsed data.
    public Vector3? Translation;
    public Vector4? RotationAxisAngle; // xyz = axis, w = degrees
    public Vector3? Scale;

    // Composes the node's local transform in S → R → T order. This matches the
    // engine's apply order in RecurseChildNode (it premultiplies each level's
    // transform onto its child). Returns identity when no flags are set.
    public Matrix4x4 ToLocalMatrix()
    {
        var m = Matrix4x4.Identity;
        if (Scale.HasValue) m *= Matrix4x4.CreateScale(Scale.Value);
        if (RotationAxisAngle.HasValue)
        {
            var v = RotationAxisAngle.Value;
            var axis = new Vector3(v.X, v.Y, v.Z);
            var len = axis.Length();
            // Engine normalises with 1/sqrt before sin/cos. Zero-length axis fall
            // back to identity rather than NaN-out the matrix.
            if (len > 1e-8f) axis /= len;
            else { m *= Matrix4x4.Identity; goto afterRot; }
            var half = v.W * MathF.PI / 360f;
            var s = MathF.Sin(half);
            m *= Matrix4x4.CreateFromQuaternion(
                new Quaternion(axis.X * s, axis.Y * s, axis.Z * s, MathF.Cos(half)));
            afterRot:;
        }
        if (Translation.HasValue) m *= Matrix4x4.CreateTranslation(Translation.Value);
        return m;
    }
}

// Hierarchical scene node parsed from b3d 0x0A blocks. Each node knows its
// parent, its children, the local transforms it carries (token 0x0D), and the
// material-keys of any submeshes it owns (token 0x0F). The renderer composes
// world matrices by walking parent chains from a submesh up to the root —
// without this, all submeshes share the model's outer transform and any
// per-node rotation (the source of the mysterious 180°/-90° offsets) gets
// silently dropped.
public sealed class B3DSceneNode
{
    public string Name = "";
    public B3DSceneNode? Parent;
    public List<B3DSceneNode> Children = new();
    public List<NodeTransform> Transforms = new();
    public List<string> SubmeshKeys = new(); // material-name keys into B3DModel.SubModels

    public Matrix4x4 LocalMatrix
    {
        get
        {
            var m = Matrix4x4.Identity;
            foreach (var t in Transforms) m *= t.ToLocalMatrix();
            return m;
        }
    }

    public Matrix4x4 WorldMatrix
    {
        get
        {
            var m = LocalMatrix;
            for (var p = Parent; p != null; p = p.Parent)
                m *= p.LocalMatrix;
            return m;
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

    // Picks a single representative light for the renderer's existing
    // single-light pipeline. Static map geometry already has lighting baked
    // into the per-vertex DIFU colours, so this is mainly for dynamic
    // entities (critters, props) placed inside the map — they need a
    // direction + colour to shade against, and the hardcoded "noon sun"
    // fallback at (5000, 4000, 10000) tends to look wrong indoors.
    //
    // Heuristic: pick the light with the largest radius * brightness
    // product, weighted by light type (point lights count for less than
    // spotlights since they're usually small fill lights). Returns null when
    // the map has no LGTD chunk or no usable lights.
    public LightRecord? GetDominantLight()
    {
        if (Lights.Count == 0) return null;
        LightRecord? best = null;
        float bestScore = float.NegativeInfinity;
        foreach (var l in Lights)
        {
            if (!float.IsFinite(l.Position.X) || !float.IsFinite(l.Position.Y) || !float.IsFinite(l.Position.Z))
                continue;
            var brightness = l.Color.X + l.Color.Y + l.Color.Z;
            var typeBoost = l.IsSpotlight ? 1.5f : 1.0f;
            var score = brightness * MathF.Max(l.Radius, 0.001f) * MathF.Max(l.Intensity, 0.001f) * typeBoost;
            if (score > bestScore)
            {
                bestScore = score;
                best = l;
            }
        }
        return best;
    }

    // ----- TXTD -----
    // Slot 0 is the engine's reserved "no texture" sentinel; real names start at index 1.
    public List<string> TextureNames = [];

    // First TXTD index whose name is a lightmap label (".ctx"-suffixed). Indices
    // [1..LightmapStart-1] are real base-texture filenames; indices
    // [LightmapStart..count-1] are <MapName>_<n>.ctx labels that the engine
    // dereferences as ordinary textures with the extension swapped to .dds/.tga.
    // Per-face lightmap selection (LMI) is stored as `lmPage` ∈ [0, count-1-LightmapStart];
    // the actual TXTD entry to load is TextureNames[LightmapStart + lmPage].
    // 0 when the file has no baked lightmap section.
    public int LightmapStart;

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
    // Per-face lightmap page index. -1 = no lightmap (face renders without the
    // second texture stage). Otherwise an offset into the .ctx-suffixed segment
    // of TextureNames, i.e. the lightmap atlas to load is
    // TextureNames[LightmapStart + LMI[face]] with extension swapped to .tga.
    public List<int> LMI = [];
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
        LightmapStart = f.ReadInt32();
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
                            var z = f.ReadSingle();
                            var y = f.ReadSingle();
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
                    // FLGS holds one flag per face — but the per-face width isn't
                    // constant across maps. We scan to identify which length is used
                    // by scanning for a known flag after the appropriate number of
                    // flags at both possible lengths.
                    if (f.BaseStream.Position + 8 <= limit)
                    {
                        var probe2 = f.BaseStream.Position;
                        if (ReadTag(f) == "FLGS")
                        {
                            var flgsCount = f.ReadInt32();
                            var bodyStart = f.BaseStream.Position;
                            long pick = ResolveFlgsBodyEnd(f, bodyStart, flgsCount, limit);
                            f.BaseStream.Seek(pick, SeekOrigin.Begin);
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

    private static long ResolveFlgsBodyEnd(BinaryReader f, long bodyStart, int flgsCount, long limit)
    {
        var byteEnd = bodyStart + flgsCount;
        var int4End = bodyStart + flgsCount * 4L;
        bool byteOk = LooksLikeTreeTagAt(f, byteEnd, limit);
        bool int4Ok = LooksLikeTreeTagAt(f, int4End, limit);
        if (byteOk) return byteEnd;
        if (int4Ok) return int4End;
        return int4End;
    }

    private static bool LooksLikeTreeTagAt(BinaryReader f, long pos, long limit)
    {
        if (pos < 0 || pos + 4 > limit || pos + 4 > f.BaseStream.Length) return false;
        var saved = f.BaseStream.Position;
        f.BaseStream.Seek(pos, SeekOrigin.Begin);
        var bytes = f.ReadBytes(4);
        f.BaseStream.Seek(saved, SeekOrigin.Begin);
        if (bytes.Length < 4) return false;
        var s = Encoding.ASCII.GetString(bytes);
        return s == "NODE" || s == "LEAF";
    }

    // IDXS payload is a sequence of groups, each headed by a "material switch":
    //   - 0x80NN  w1  (0x4000+ww2)  w3  m   : verbose form (per-group state)
    //   - 0x40NN  w1                m       : compact form (lmPage update only)
    //   - 0x00NN                            : minimal — tmp itself is m
    //   followed by `count` (u16, =3*tris) and `tris` triangles of u16 indices.
    //
    // Field meanings (reverse-engineered from F3.exe's Octree2Gfx::
    // ConstructRenderData at 0x4bdea0; the per-batch struct uses field +20 for
    // baseTexIdx and field +16 for lmTexIdx, both indexing the same unified
    // texture-handle table at this+72):
    //   m       — base texture index (1-based into the filtered MAT list,
    //             0 = no texture; selects the surface TGA for the group).
    //   matIdx (NN in verbose) — 0-based MaterialRecord index. Picks the
    //             technique (BASE_LM_2X, BASE_IMAGE_ALPHA_2X, etc.) and blend
    //             op; the same texture can appear with multiple matIdx values
    //             so a foliage atlas mixes opaque trunks + alpha-tested cards.
    //   lmPage  — index of the lightmap atlas, encoded differently per form:
    //             verbose: w2 = 0x4000 + (lmPage+1), w2==0x4000 means none;
    //             compact: NN = lmPage+1, NN==0 means none.
    //             Resolves to TextureNames[LightmapStart + lmPage].
    //   w1, w3  — vertex-range bounds within the cnode's vertex buffer (D3D
    //             DrawIndexedPrimitive minIndex/numVertices). Not needed.
    //
    // **Inheritance**: each form only updates the fields it carries, the others
    // carry over from the prior batch. Specifically: compact does NOT update
    // matIdx (despite the historical mis-decoding), and minimal only updates m.
    private void DecodeIDXS(BinaryReader f, long end, int cnodeIdx)
    {
        var baseOffset = cnodeIdx >= 0 && cnodeIdx < CNodeOffsets.Count - 1
            ? CNodeOffsets[cnodeIdx]
            : 0;
        var m = 0;
        var matIdx = 0;
        var lmPage = -1;
        while (f.BaseStream.Position + 12 <= end)
        {
            var tmp = f.ReadUInt16();
            if (tmp == 0) tmp = f.ReadUInt16();
            if ((tmp & 0xFF00) == 0x8000)
            {
                matIdx = tmp & 0xFF;
                f.ReadUInt16();
                var w2 = f.ReadUInt16();
                lmPage = w2 == 0x4000 ? -1 : (w2 - 0x4000) - 1;
                f.ReadUInt16();
                m = f.ReadUInt16();
            }
            else if ((tmp & 0xFF00) == 0x4000)
            {
                var nn = tmp & 0xFF;
                lmPage = nn == 0 ? -1 : nn - 1;
                f.ReadUInt16();
                m = f.ReadUInt16();
            }
            else if ((tmp & 0xFF00) == 0)
            {
                m = tmp;
                // Minimal form has no per-group material or lmPage; the engine
                // inherits both from the previous batch.
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
                LMI.Add(lmPage);
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
    // Light type values found in the Type field (offset 0x04). 0 = spotlight
    // (carries valid Direction + cone angles); 1 = point light variant A;
    // 2 = point light variant B. Variants 1 and 2 differ in their FalloffStart
    // values across the sample set (1 ≈ 1.0, 2 ≈ 0.4–0.5) which suggests one
    // is linear-attenuation and the other inverse-square — engine code that
    // distinguishes them isn't decoded yet, but the value is preserved so a
    // future shader pass can pick the right falloff curve.
    public const int TypeSpot = 0;
    public const int TypePointA = 1;
    public const int TypePointB = 2;

    public byte[] Raw = Array.Empty<byte>();

    // 0x00 — int32. Always 0 in the maps that ship with multiple lights;
    // MarkTest's single test spotlight has value 1. Unknown semantics; could
    // be a "primary / casts shadow" flag or just a serialization id.
    public int Flags;

    // 0x04 — light type enum (see TypeSpot / TypePointA / TypePointB).
    public int Type;

    // 0x08..0x10 — RGB colour as floats in [0, 1].
    public Vector3 Color;

    // 0x14..0x1C — world position. Already remapped into the renderer's
    // coordinate convention (X negated, Y/Z swapped) so it matches the GVP
    // positions produced by VTXD.
    public Vector3 Position;

    // 0x20..0x28 — direction (unit vector). NaN for point-light types; valid
    // for spotlights. Same coordinate remap as Position.
    public Vector3 Direction;

    // 0x2C — attenuation parameter. For point-light type 1 it tends to land
    // near 1.0; for type 2 around 0.4–0.5; for the spotlight it was 2.0 in
    // MarkTest. Stored verbatim — see comment on Type for caveat.
    public float FalloffStart;

    // 0x30 — light radius (max influence range), in world units.
    public float Radius;

    // 0x34 — overall intensity multiplier. Always 1.0 in observed samples,
    // but exposed so future maps can dim/boost lights.
    public float Intensity;

    // 0x38 / 0x3C — spotlight cone angles in radians (inner / outer).
    // MarkTest's spotlight: ~0.4363 (~25°) inner, ~0.5236 (~30°) outer. Zero
    // for point lights — they don't use them.
    public float ConeInnerRad;
    public float ConeOuterRad;

    public bool IsSpotlight => Type == TypeSpot;
    public bool IsPointLight => Type == TypePointA || Type == TypePointB;

    public static LightRecord Parse(byte[] raw)
    {
        var r = new LightRecord { Raw = raw };
        if (raw.Length >= 0x04) r.Flags = BitConverter.ToInt32(raw, 0x00);
        if (raw.Length >= 0x08) r.Type = BitConverter.ToInt32(raw, 0x04);
        if (raw.Length >= 0x14)
            r.Color = new Vector3(
                BitConverter.ToSingle(raw, 0x08),
                BitConverter.ToSingle(raw, 0x0C),
                BitConverter.ToSingle(raw, 0x10));
        if (raw.Length >= 0x20)
        {
            // The renderer's world space is (-fileX, fileY, fileZ): VTXD
            // negates X and swaps Y/Z, then Add8Model un-swaps Y/Z back
            // when building the final vertex buffer. The net transform is
            // just an X negation. Lights have to land in the same frame as
            // the geometry the shader compares them against, so we apply
            // only the X negation here — applying VTXD's Y/Z swap as well
            // would Y/Z-mirror every light relative to its room and make
            // dynamic shading look like normals showing through.
            var x = BitConverter.ToSingle(raw, 0x14);
            var y = BitConverter.ToSingle(raw, 0x18);
            var z = BitConverter.ToSingle(raw, 0x1C);
            r.Position = new Vector3(-x, y, z);
        }
        if (raw.Length >= 0x2C)
        {
            var x = BitConverter.ToSingle(raw, 0x20);
            var y = BitConverter.ToSingle(raw, 0x24);
            var z = BitConverter.ToSingle(raw, 0x28);
            r.Direction = new Vector3(-x, y, z);
        }
        if (raw.Length >= 0x30) r.FalloffStart = BitConverter.ToSingle(raw, 0x2C);
        if (raw.Length >= 0x34) r.Radius = BitConverter.ToSingle(raw, 0x30);
        if (raw.Length >= 0x38) r.Intensity = BitConverter.ToSingle(raw, 0x34);
        if (raw.Length >= 0x3C) r.ConeInnerRad = BitConverter.ToSingle(raw, 0x38);
        if (raw.Length >= 0x40) r.ConeOuterRad = BitConverter.ToSingle(raw, 0x3C);
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