using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using VBLauncher.VB3DLib;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.Sdl2;
using Veldrid.SPIRV;

namespace VBLauncher;

public class SceneRenderer// : IDisposable
{
    private const string VertexCode = @"#version 450

layout(set = 0, binding = 0) uniform ProjectionBuffer { mat4 Projection; };
layout(set = 0, binding = 1) uniform ViewBuffer       { mat4 View;       };
layout(set = 1, binding = 0) uniform WorldBuffer      { mat4 World;      };

layout(location = 0) in vec3 Position;
layout(location = 1) in vec2 TexCoords;  // keep UVs at loc 1
layout(location = 2) in vec3 Normal;     // normals now at loc 2

layout(location = 0) out vec2  fsin_texCoords;
layout(location = 1) out vec3  fsin_normal;
layout(location = 2) out vec3  fsin_worldPos;

void main() {
    // world‐space position
    vec4 worldPos4 = World * vec4(Position, 1.0);
    fsin_worldPos = worldPos4.xyz;

    // transform & normalize normal into world‐space
    fsin_normal   = normalize(mat3(World) * Normal);

    // pass UVs
    fsin_texCoords = TexCoords;

    // MVP
    gl_Position = Projection * View * worldPos4;
}";

    private const string FragmentCode = @"#version 450

layout(location = 0) in vec2  fsin_texCoords;
layout(location = 1) in vec3  fsin_normal;
layout(location = 2) in vec3  fsin_worldPos;

layout(location = 0) out vec4 fsout_color;

layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler  SurfaceSampler;

layout(set = 0, binding = 3) uniform LightInfo {
    vec3 LightPosition;
    vec3 LightColor;
    vec3 CameraPosition;
} uLight;

void main() {
    // Sample surface color + alpha
    vec4 tex    = texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords);
    vec3 albedo = tex.rgb;
    float alpha = tex.a;

    // lighting
    vec3 N      = normalize(fsin_normal);
    vec3 L      = normalize(uLight.LightPosition - fsin_worldPos);
    float NdotL = max(dot(N, L), 0.0);

    vec3 diffuse = albedo * uLight.LightColor * NdotL;
    vec3 ambient = 0.3 * albedo;

    // output with texture’s alpha
    fsout_color = vec4(diffuse + ambient, alpha);
}";
    
    private static readonly string SkinnedVertexCode = @"#version 450

layout(set = 0, binding = 0) uniform ProjectionBuffer { mat4 Projection; };
layout(set = 0, binding = 1) uniform ViewBuffer       { mat4 View;       };
layout(set = 1, binding = 0) uniform WorldBuffer      { mat4 World;      };
layout(set = 2, binding = 0) uniform BoneBuffer       { mat4 Bones[" + MaxBones + @"]; };

layout(location = 0) in vec3  Position;
layout(location = 1) in vec2  TexCoords;
layout(location = 2) in vec3  Normal;
layout(location = 3) in vec4  BoneWeights;
layout(location = 4) in uvec4 BoneIndices;

layout(location = 0) out vec2 fsin_texCoords;
layout(location = 1) out vec3 fsin_normal;
layout(location = 2) out vec3 fsin_worldPos;

void main() {
    // Bone-weighted skinning matrix. Indices are pre-bridged to B3D space at upload,
    // so the input vertex (B3D space) → skinned vertex (B3D space).
    mat4 skin = Bones[BoneIndices.x] * BoneWeights.x
              + Bones[BoneIndices.y] * BoneWeights.y
              + Bones[BoneIndices.z] * BoneWeights.z
              + Bones[BoneIndices.w] * BoneWeights.w;

    vec4 skinnedPos    = skin * vec4(Position, 1.0);
    vec3 skinnedNormal = mat3(skin) * Normal;

    vec4 worldPos4 = World * skinnedPos;
    fsin_worldPos  = worldPos4.xyz;
    fsin_normal    = normalize(mat3(World) * skinnedNormal);
    fsin_texCoords = TexCoords;
    gl_Position    = Projection * View * worldPos4;
}";

    // Minimal flat-color line shader for the skeleton overlay. Uses set 0 = projView
    // (matches the existing projViewLayout — LightBuffer is bound but unreferenced)
    // and set 1 = WorldBuffer only (its own layout, no texture/sampler).
    private const string LineVertexCode = @"#version 450
layout(set = 0, binding = 0) uniform ProjectionBuffer { mat4 Projection; };
layout(set = 0, binding = 1) uniform ViewBuffer       { mat4 View;       };
layout(set = 1, binding = 0) uniform WorldBuffer      { mat4 World;      };

layout(location = 0) in vec3 Position;
void main() {
    gl_Position = Projection * View * World * vec4(Position, 1.0);
}";

    private const string LineFragmentCode = @"#version 450
layout(location = 0) out vec4 fsout_color;
void main() {
    fsout_color = vec4(0.2, 1.0, 0.2, 1.0);
}";

    // Hard cap on bone palette size. 128 mat4 = 8 KB uniform buffer, well within the
    // typical 16 KB minimum guarantee. AddSkinnedB3DModel throws if a skeleton exceeds.
    private const int MaxBones = 128;

    // Coordinate-space bridge between bones (engine space, post swap+×3 from
    // CreateAdjustedTransform / AdjustTransformTrackCurves) and B3D mesh vertices
    // (raw B3D space). For a vertex v in B3D space and a skin matrix S in engine
    // space, the renderable matrix is B3dToEngine * S * EngineToB3d so that
    //   v_b3d * (B3dToEngine * S * EngineToB3d) = v_b3d_skinned.
    // Recipe is the inverse of the reference's TVertex44 transform:
    //   engine = (b3d.x * 0.3, -b3d.z * 0.3, b3d.y * 0.3).
    private static readonly Matrix4x4 B3dToEngine = new Matrix4x4(
        0.3f,  0f,    0f,   0f,
        0f,    0f,    0.3f, 0f,
        0f,   -0.3f,  0f,   0f,
        0f,    0f,    0f,   1f);
    private static readonly Matrix4x4 EngineToB3d = new Matrix4x4(
        1f / 0.3f, 0f,         0f,         0f,
        0f,        0f,        -1f / 0.3f,  0f,
        0f,        1f / 0.3f,  0f,         0f,
        0f,        0f,         0f,         1f);

    public bool AllowInput = false;
    public bool Initialized = false;
    public bool DrawSkeletonOverlay = false;
    public bool AnimateSkinned = true;

    public uint Height;
    public uint Width;

    public IntPtr ImGuiBinding;

    private List<ModelData> _models = new List<ModelData>();
    private List<SkinnedModelData> _skinnedModels = new List<SkinnedModelData>();
    private Camera _camera;

    private DeviceBuffer _lightBuffer;
    private Pipeline _pipeline;
    private Pipeline _skinnedPipeline;
    private Pipeline _linePipeline;
    private ResourceLayout _lineProjViewLayout;
    private ResourceLayout _lineWorldLayout;
    private ResourceSet _lineProjViewSet;
    private ResourceSet _lineWorldSet;
    private DeviceBuffer _skeletonLineBuffer;
    private Vector3[] _skeletonLineScratch;

    private GraphicsDevice _graphicsDevice = null!;
    private ImGuiRenderer _imguiRenderer;
    private Sdl2Window _window;
    private CommandList _commandList = null!;

    private DeviceBuffer _projectionBuffer;
    private ResourceSet _projViewSet;
    private float _rotation;
    private Texture _sceneColorTarget;
    private Texture _sceneDepthTexture;
    private Texture _msaaColorTarget;
    private Texture _msaaDepthTexture;
    private Framebuffer _sceneFramebuffer;
    private DeviceBuffer _viewBuffer;
    private DeviceBuffer _worldBuffer;
    private ResourceLayout _worldTextureLayout;
    private ResourceLayout _boneLayout;

    private HashSet<string> _ignoredZones = new();
    
    public SceneRenderer(GraphicsDevice graphicsDevice, ImGuiRenderer imguiRenderer, Sdl2Window window, CommandList commandList, int width = 800, int height = 600)
    {
        _graphicsDevice = graphicsDevice;
        _imguiRenderer = imguiRenderer;
        _window = window;
        _commandList = commandList;
        Width = (uint)width;
        Height = (uint)height;

        CreateSceneFramebuffer();
        InitializeSharedResources();
    }
    
    public void AddB3DModel(B3DModel model, Matrix4x4? worldMatrix = null, List<Vector2>? uvs = null)
    {
        for (var i = 0; i < model.SubModels.Keys.Count; i++)
        {
            AddModel(model, i, null, worldMatrix, uvs);
        }
        foreach (var zone in model.OcclusionZones)
            _ignoredZones.Add(zone);
    }
    
    public void AddB3DModel(B3DModel model, string textureOverride, Matrix4x4? worldMatrix = null, List<Vector2>? uvs = null)
    {
        for (var i = 0; i < model.SubModels.Keys.Count; i++)
        {
            AddModel(model, i, textureOverride, worldMatrix, uvs);
        }
        foreach (var zone in model.OcclusionZones)
            _ignoredZones.Add(zone);
    }

    void AddModelIfValid(string modelName, string textureFileName, Func<Vector2, Vector2> uvMapper, Matrix4x4? worldMatrix = null,
                         GrannyFormats.model? gr2Skeleton = null, GrannyFormats.animation? gr2Anim = null)
    {
        if (modelName.EndsWith('_')) return;
        var model = (B3DModel?)ResourceSystem.FindResource(modelName, "b3d");
        if (model == null)
        {
            Console.WriteLine($"Model {modelName}.b3d not found, skipping.");
            return;
        }
        List<Vector2> uvs = model.Model_Vertex_Texcoords.Select(uvMapper).ToList();
        if (gr2Skeleton.HasValue && gr2Anim.HasValue)
            AddSkinnedB3DModel(model, gr2Skeleton.Value, gr2Anim.Value, worldMatrix,
                string.IsNullOrEmpty(textureFileName) ? null : textureFileName, uvs);
        else
            AddB3DModel(model, string.IsNullOrEmpty(textureFileName) ? null : textureFileName, worldMatrix, uvs);
    }

    public void AddARM(ARM arm, string modelPrefix = "HumMNor", Matrix4x4? worldMatrix = null,
                       GrannyFormats.model? gr2Skeleton = null, GrannyFormats.animation? gr2Anim = null)
    {
        AddModelIfValid(modelPrefix + '_' + arm.GITM.Bac.Model, arm.GITM.Bac.Tex, UVAtlasMapper.ToBacUV, worldMatrix, gr2Skeleton, gr2Anim);
        AddModelIfValid(modelPrefix + '_' + arm.GITM.Bod.Model, arm.GITM.Bod.Tex, UVAtlasMapper.ToBodUV, worldMatrix, gr2Skeleton, gr2Anim);
        AddModelIfValid(modelPrefix + '_' + arm.GITM.Eye.Model, arm.GITM.Eye.Tex, UVAtlasMapper.ToEyeUV, worldMatrix, gr2Skeleton, gr2Anim);
        AddModelIfValid(modelPrefix + '_' + arm.GITM.Fee.Model, arm.GITM.Fee.Tex, UVAtlasMapper.ToFeeUV, worldMatrix, gr2Skeleton, gr2Anim);
        AddModelIfValid(modelPrefix + '_' + arm.GITM.Han.Model, arm.GITM.Han.Tex, UVAtlasMapper.ToHanUV, worldMatrix, gr2Skeleton, gr2Anim);
        AddModelIfValid(modelPrefix + '_' + arm.GITM.Hea.Model, arm.GITM.Hea.Tex, UVAtlasMapper.ToHeaUV, worldMatrix, gr2Skeleton, gr2Anim);
        AddModelIfValid(modelPrefix + '_' + arm.GITM.Sho.Model, arm.GITM.Sho.Tex, UVAtlasMapper.ToShoUV, worldMatrix, gr2Skeleton, gr2Anim);
    }
    
    public void AddCRT(CRT crt, Matrix4x4? worldMatrix = null)
    {
        var baseModelName = crt.EEN2.skl;
        if (string.IsNullOrEmpty(baseModelName))
        {
            Console.WriteLine("CRT has no base model, skipping.");
            return;
        }
        B3DModel baseModel = ResourceSystem.FindResource(baseModelName, "b3d");
        
        // Skeleton lives next to the b3d under the same prefix (HumMNor.skl). The
        // animation set is shared across body sub-types — HumMNor / HumMMus / etc.
        // all idle from HumM_Def_Una_Idle_01.gr2 — so the lookup uses the first 4
        // chars as the animation prefix.
        GrannyFormats.model? gr2Skel = null;
        GrannyFormats.animation? gr2Anim = null;
        // FindResource returns dynamic; file_info is a struct, so we pattern-match
        // through object to avoid the runtime binder choking on `!= null`.
        if ((object)ResourceSystem.FindResource(baseModelName, "skl") is GrannyFormats.file_info sklFile
            && sklFile.Models is { Count: > 0 } sklModels)
            gr2Skel = sklModels[0];
        if (gr2Skel.HasValue)
        {
            //TODO: is there a flag for human models?
            var animName = (baseModelName.StartsWith("Hum") ? baseModelName.Substring(0, 4) : baseModelName) + "_Def_Una_Idle_01";
            if ((object)ResourceSystem.FindResource(animName, "gr2") is GrannyFormats.file_info animFile
                && animFile.Animations is { Count: > 0 } anims)
                gr2Anim = anims[0];
            else
                Console.WriteLine($"CRT animation {animName}.gr2 not found, falling back to static pose.");
        }
        if (!gr2Anim.HasValue) gr2Skel = null; // need both for the skinned path
        
        if (baseModelName.StartsWith("Hum"))
        {
            var hideHair = false;
            var hideBeard = false;
            var hideMoustache = false;
            var hideEye = false;
            var hidePonytail = false;
            var hideVanity = false;

            foreach (var item in crt.GCRE.Inventory)
            {
                var itm = ResourceSystem.FindResource(item);
                if (itm is ARM arm) {
                    if (arm.GITM.hHai) hideHair = true;
                    if (arm.GITM.hBea) hideBeard = true;
                    if (arm.GITM.hMus) hideMoustache = true;
                    if (arm.GITM.hEye) hideEye = true;
                    if (arm.GITM.hPon) hidePonytail = true;
                    if (arm.GITM.hVan) hideVanity = true;
                    AddARM(arm, baseModelName, worldMatrix, gr2Skel, gr2Anim);
                }
                else if (itm is WEA wea && false) // flagged false for now, unsure how animations are meant to be applied to in hand items.
                {
                    GrannyFormats.model? weaSkel = null;
                    if ((object)ResourceSystem.FindResource(wea.GITM.IHS.Model, "skl") is GrannyFormats.file_info { Models.Count: > 0 } weaSkl) {
                        weaSkel = weaSkl.Models[0];
                        Console.WriteLine($"Found skeleton {wea.GITM.IHS.Model}.skl for weapon {item}");
                        Console.WriteLine(weaSkel.Value.Name);
                        foreach (var bone in weaSkel.Value.Skeleton.Bones)
                        {
                            Console.WriteLine($"Bone: {bone.Name}, Parent: {bone.ParentIndex}");
                        }

                        foreach (var track in gr2Anim.Value.TrackGroups.SelectMany(tg => tg.TransformTracks))
                        {
                            Console.WriteLine($"Anim Track: {track.Name}");
                        }
                    }
                    AddModelIfValid(wea.GITM.IHS.Model, wea.GITM.IHS.Tex, UVAtlasMapper.Default, worldMatrix, weaSkel, gr2Anim);
                }
            }

            AddModelIfValid(baseModelName + '_' + crt.GCRE.Hea.Model, crt.GCRE.Hea.Tex, UVAtlasMapper.ToHeaUV, worldMatrix, gr2Skel, gr2Anim);
            if (!hideEye)
                AddModelIfValid(baseModelName + '_' + crt.GCRE.Eye.Model, crt.GCRE.Eye.Tex, UVAtlasMapper.ToEyeUV, worldMatrix, gr2Skel, gr2Anim);
            if (!hideHair)
                AddModelIfValid(baseModelName + '_' + crt.GCRE.Hai.Model, crt.GCRE.Hai.Tex, UVAtlasMapper.ToHaiUV, worldMatrix, gr2Skel, gr2Anim);
            if (!hidePonytail)
                AddModelIfValid(baseModelName + '_' + crt.GCRE.Pon.Model, crt.GCRE.Pon.Tex, UVAtlasMapper.ToPonUV, worldMatrix, gr2Skel, gr2Anim);
            if (!hideMoustache)
                AddModelIfValid(baseModelName + '_' + crt.GCRE.Mus.Model, crt.GCRE.Mus.Tex, UVAtlasMapper.ToMusUV, worldMatrix, gr2Skel, gr2Anim);
            if (!hideBeard)
                AddModelIfValid(baseModelName + '_' + crt.GCRE.Bea.Model, crt.GCRE.Bea.Tex, UVAtlasMapper.ToBeaUV, worldMatrix, gr2Skel, gr2Anim);
            AddModelIfValid(baseModelName + '_' + crt.GCRE.Han.Model, crt.GCRE.Han.Tex, UVAtlasMapper.ToHanUV, worldMatrix, gr2Skel, gr2Anim);
            if (!hideVanity)
                AddModelIfValid(baseModelName + '_' + crt.GCRE.Van.Model, crt.GCRE.Van.Tex, UVAtlasMapper.ToVanUV, worldMatrix, gr2Skel, gr2Anim);
            AddModelIfValid(baseModelName + '_' + crt.GCRE.Bod.Model, crt.GCRE.Bod.Tex, UVAtlasMapper.ToBodUV, worldMatrix, gr2Skel, gr2Anim);
            AddModelIfValid(baseModelName + '_' + crt.GCRE.Sho.Model, crt.GCRE.Sho.Tex, UVAtlasMapper.ToShoUV, worldMatrix, gr2Skel, gr2Anim);
            AddModelIfValid(baseModelName + '_' + crt.GCRE.Fee.Model, crt.GCRE.Fee.Tex, UVAtlasMapper.ToFeeUV, worldMatrix, gr2Skel, gr2Anim);
            AddModelIfValid(baseModelName + '_' + crt.GCRE.Bac.Model, crt.GCRE.Bac.Tex, UVAtlasMapper.ToBacUV, worldMatrix, gr2Skel, gr2Anim);

            var baseUVs = baseModel.Model_Vertex_Texcoords.Select(UVAtlasMapper.ToBaseUV).ToList();
            if (gr2Skel.HasValue && gr2Anim.HasValue)
                AddSkinnedB3DModel(baseModel, gr2Skel.Value, gr2Anim.Value, worldMatrix, crt.EEN2.EEOV.s4, baseUVs);
            else
                AddB3DModel(baseModel, crt.EEN2.EEOV.s4, worldMatrix, baseUVs);
            
            return;
        }
        if (gr2Skel.HasValue && gr2Anim.HasValue)
            AddSkinnedB3DModel(baseModel, gr2Skel.Value, gr2Anim.Value, worldMatrix, crt.EEN2.EEOV.s4);
        else
            AddB3DModel(baseModel, crt.EEN2.EEOV.s4, worldMatrix);
    }

    public void AddMap(Map map)
    {
        // Add Map Geometry
        if (!string.IsNullOrEmpty(map.EMAP.s1))
        {
            try
            {
                var searchName = Path.GetFileNameWithoutExtension(map.EMAP.s1);
                
                var _8 = ResourceSystem.FindResource(searchName, "8");
                if (_8 != null)
                {
                    Console.WriteLine($"Loading map geometry from 8Model: {searchName}");
                    Add8Model(_8);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading map geometry: {ex.Message}");
            }
        }

        // Add Entities
        foreach (var ent in map.EME2)
        {
            try
            {
                var transform = Matrix4x4.CreateRotationY(ent.l.W) * 
                                Matrix4x4.CreateTranslation(-ent.l.X, ent.l.Y, ent.l.Z);

                if (ent.name.ToLower().EndsWith(".crt"))
                {
                    var crt = ResourceSystem.FindResource(ent.name);
                    if (crt != null) AddCRT(crt, transform);
                }
                else
                {
                     var een2 = ResourceSystem.FindResource(ent.name).EEN2;
                     if (een2 != null && !string.IsNullOrEmpty(een2.skl))
                     {
                         var b3d = ResourceSystem.FindResource(een2.skl, "b3d");
                         if (b3d != null)
                         {
                             AddB3DModel(b3d, transform);
                         }
                     }
                }
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Error adding entity {ent.name} to map: {ex.Message}");
            }
            // Clear ignored zones after each entity so they don't affect the next one
            _ignoredZones.Clear();
        }

        // Add Default PC at Entry Points
        foreach (var ep in map.EMEP)
        {
            try {
                var crt = ResourceSystem.FindResource("default_pc.crt");
                if (crt != null)
                {
                    var transform = Matrix4x4.CreateRotationY(ep.r) * 
                                    Matrix4x4.CreateTranslation(-ep.p.X, ep.p.Y, ep.p.Z);
                    AddCRT(crt, transform);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding default PC to map: {ex.Message}");
            }
            // Clear ignored zones here too
            _ignoredZones.Clear();
        }
        
        FocusCamera();
    }
    
    public void Add8Model(_8Model model)
    {
        if (!Initialized)
        {
            throw new InvalidOperationException("SceneRenderer must be initialized before adding models");
        }

        // Prepare factory for buffer creation
        var factory = _graphicsDevice.ResourceFactory;
        
        // Group indices by material index
        var materialIndices = new Dictionary<int, List<ushort>>();
        
        // Pre-allocate lists for each material
        for (int i = 0; i <= model.MAT.Count; i++)
        {
             materialIndices[i] = new List<ushort>();
        }
        
        // Populate indices (collect faces)
        // Using a dictionary that creates lists on demand to handle any MDX value
        var materialFaces = new Dictionary<int, List<int>>();

        for (int i = 0; i < model.IDX.Count; i++)
        {
            var mdxindex = model.MDX[i];
            if (mdxindex == 0) mdxindex = model.MAT.Count;
            
            if (!materialFaces.ContainsKey(mdxindex))
            {
                materialFaces[mdxindex] = new List<int>();
            }
            materialFaces[mdxindex].Add(i);
        }

        Console.WriteLine($"[SceneRenderer] Add8Model: GVP Count {model.GVP.Count}, IDX Count {model.IDX.Count}, MAT Count {model.MAT.Count}");
        Console.WriteLine($"[SceneRenderer] Add8Model: Material Faces Breakdown: {string.Join(", ", materialFaces.Select(k => $"Mat {k.Key}: {k.Value.Count} faces"))}");

        foreach (var kvp in materialFaces)
        {
            var matIndex = kvp.Key;
            var faceIndices = kvp.Value;
            if (faceIndices.Count == 0) continue;

            string textureName = null;
            if (matIndex > 0 && matIndex <= model.MAT.Count)
            {
                 textureName = Path.GetFileNameWithoutExtension(model.MAT[matIndex - 1].FileName);
            }
            
            var modelData = new ModelData
            {
                Name = $"8Model_Mat{matIndex}",
                WorldMatrix = Matrix4x4.Identity,
                Pipeline = textureName != null && textureName.Contains("Flora") ? null : _opaquePipeline
            };
            
            // Process Texture
            if (string.IsNullOrEmpty(textureName))
            {
                 modelData.SurfaceTexture = Create1x1TextureFromColor(_graphicsDevice, factory, 0.5f, 0.5f, 0.5f); // Grey default
                 modelData.SurfaceTextureView = factory.CreateTextureView(modelData.SurfaceTexture);
            }
            else
            {
                ImageSharpTexture tx = ResourceSystem.FindResource(textureName, "tga");
                if (tx != null)
                {
                    modelData.SurfaceTextureView = factory.CreateTextureView(tx.CreateDeviceTexture(_graphicsDevice, factory));
                }
                else
                {
                     // Red fallback for missing texture to make it obvious
                    Console.WriteLine($"[SceneRenderer] Add8Model: Missing texture '{textureName}' for material {matIndex}");
                    modelData.SurfaceTexture = Create1x1TextureFromColor(_graphicsDevice, factory, 1f, 0f, 0f);
                    modelData.SurfaceTextureView = factory.CreateTextureView(modelData.SurfaceTexture);
                }
            }
             
            modelData.WorldTextureSet = factory.CreateResourceSet(new ResourceSetDescription(
                _worldTextureLayout, _worldBuffer, modelData.SurfaceTextureView, _graphicsDevice.Aniso4xSampler));

            // Triangles are "exploded" into 3 fresh vertices each, so each chunk
            // tops out at ushort.MaxValue / 3 = 21845 faces (65535 / 3). Past that,
            // finalize the current chunk and start a new one sharing the same
            // material. Without this, baseIdx wraps and the geometry past ~21k
            // faces per material disappears (e.g. ~half of 03_Tutorial_Junktown).
            const int FacesPerChunk = ushort.MaxValue / 3;
            var totalChunks = (faceIndices.Count + FacesPerChunk - 1) / FacesPerChunk;
            var chunkIndex = 0;
            for (var faceCursor = 0; faceCursor < faceIndices.Count; faceCursor += FacesPerChunk, chunkIndex++)
            {
                var chunkEnd = Math.Min(faceCursor + FacesPerChunk, faceIndices.Count);
                var uniqueVertices = new List<VertexPositionTextureNormal>((chunkEnd - faceCursor) * 3);
                var newIndices = new List<ushort>((chunkEnd - faceCursor) * 3);

                for (var fi = faceCursor; fi < chunkEnd; fi++)
                {
                    var face = model.IDX[faceIndices[fi]];
                    int idx0 = face[0];
                    int idx1 = face[1];
                    int idx2 = face[2];

                    // Get Positions and Un-Swap Y/Z (Undo VanBurenFormats swap)
                    Vector3 p0 = new Vector3(model.GVP[idx0].X, model.GVP[idx0].Z, model.GVP[idx0].Y);
                    Vector3 p1 = new Vector3(model.GVP[idx1].X, model.GVP[idx1].Z, model.GVP[idx1].Y);
                    Vector3 p2 = new Vector3(model.GVP[idx2].X, model.GVP[idx2].Z, model.GVP[idx2].Y);

                    // Calculate Flat Normal
                    Vector3 v1 = p1 - p0;
                    Vector3 v2 = p2 - p0;
                    Vector3 normal = Vector3.Normalize(Vector3.Cross(v2, v1));

                    if (float.IsNaN(normal.X) || float.IsNaN(normal.Y) || float.IsNaN(normal.Z))
                        normal = Vector3.UnitY;

                    // Get UVs
                    Vector2 uv0 = model.GVT.Count > idx0 ? model.GVT[idx0] : Vector2.Zero;
                    Vector2 uv1 = model.GVT.Count > idx1 ? model.GVT[idx1] : Vector2.Zero;
                    Vector2 uv2 = model.GVT.Count > idx2 ? model.GVT[idx2] : Vector2.Zero;

                    uniqueVertices.Add(new VertexPositionTextureNormal(p0, uv0, normal));
                    uniqueVertices.Add(new VertexPositionTextureNormal(p1, uv1, normal));
                    uniqueVertices.Add(new VertexPositionTextureNormal(p2, uv2, normal));

                    ushort baseIdx = (ushort)(uniqueVertices.Count - 3);
                    newIndices.Add(baseIdx);
                    newIndices.Add((ushort)(baseIdx + 1));
                    newIndices.Add((ushort)(baseIdx + 2));
                }

                var chunkData = totalChunks == 1
                    ? modelData
                    : new ModelData
                    {
                        Name = $"{modelData.Name}_chunk{chunkIndex}",
                        WorldMatrix = modelData.WorldMatrix,
                        Pipeline = modelData.Pipeline,
                        SurfaceTexture = modelData.SurfaceTexture,
                        SurfaceTextureView = modelData.SurfaceTextureView,
                        WorldTextureSet = modelData.WorldTextureSet
                    };

                chunkData.Vertices = uniqueVertices;
                chunkData.Indices = newIndices;

                chunkData.VertexBuffer = factory.CreateBuffer(new BufferDescription(
                    (uint)(chunkData.Vertices.Count * VertexPositionTextureNormal.SizeInBytes),
                    BufferUsage.VertexBuffer));
                _graphicsDevice.UpdateBuffer(chunkData.VertexBuffer, 0, chunkData.Vertices.ToArray());

                chunkData.IndexBuffer = factory.CreateBuffer(new BufferDescription(
                    (uint)(chunkData.Indices.Count * sizeof(ushort)),
                    BufferUsage.IndexBuffer));
                _graphicsDevice.UpdateBuffer(chunkData.IndexBuffer, 0, chunkData.Indices.ToArray());

                _models.Add(chunkData);
                Console.WriteLine($"Added 8Model mesh part {matIndex} chunk {chunkIndex + 1}/{totalChunks}: {uniqueVertices.Count} vertices, {newIndices.Count} indices. Texture: {textureName ?? "None"}");
            }
        }
        Console.WriteLine($"Finished adding 8Model. Total model parts: {_models.Count}");
    }
    
    public void HandleWindowResize(uint width, uint height)
    {
        Width = width;
        Height = height;
        _camera.WindowResized(width, height);
    }

    private void CreateSceneFramebuffer()
    {
        var factory = _graphicsDevice.ResourceFactory;

        _sceneColorTarget = factory.CreateTexture(TextureDescription.Texture2D(
            Width, Height,
            1,
            1,
            PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.RenderTarget | TextureUsage.Sampled));

        _sceneDepthTexture = factory.CreateTexture(TextureDescription.Texture2D(
            Width, Height,
            1,
            1,
            PixelFormat.D24_UNorm_S8_UInt,
            TextureUsage.DepthStencil));
        
        _msaaColorTarget = factory.CreateTexture(TextureDescription.Texture2D(
            Width, Height, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.RenderTarget, TextureSampleCount.Count8));

        _msaaDepthTexture = factory.CreateTexture(TextureDescription.Texture2D(
            Width, Height, 1, 1, PixelFormat.D24_UNorm_S8_UInt,
            TextureUsage.DepthStencil, TextureSampleCount.Count8));

        _sceneFramebuffer = factory.CreateFramebuffer(new FramebufferDescription(
            _msaaDepthTexture, _msaaColorTarget));

        ImGuiBinding = _imguiRenderer.GetOrCreateImGuiBinding(
            _graphicsDevice.ResourceFactory, _sceneColorTarget);
    }
    
    private void InitializeSharedResources()
    {
        var factory = _graphicsDevice.ResourceFactory;

        _camera = new Camera(Width, Height)
        {
            Position = new Vector3(0, 0, 3)
        };

        _projectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
        _viewBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
        _worldBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
        _lightBuffer = factory.CreateBuffer(new BufferDescription((uint)Marshal.SizeOf<LightInfo>(), BufferUsage.UniformBuffer));

        var vert = new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(VertexCode), "main");
        var frag = new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(FragmentCode), "main");
        var shaders = factory.CreateFromSpirv(vert, frag);

        var projViewLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("LightBuffer", ResourceKind.UniformBuffer, ShaderStages.Fragment)));

        _worldTextureLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
            new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

        _pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
            BlendStateDescription.SingleAlphaBlend, 
            DepthStencilStateDescription.DepthOnlyLessEqual,
            new RasterizerStateDescription(
                FaceCullMode.None, // Disable backface culling
                PolygonFillMode.Solid,
                FrontFace.CounterClockwise,
                true, false),
            PrimitiveTopology.TriangleList,
            new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float3),
                        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float2),
                        new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float3))
                },
                shaders),
            new[] { projViewLayout, _worldTextureLayout },
            _sceneFramebuffer.OutputDescription));
            
        // Create Opaque Pipeline (skips blending, effectively ignoring alpha channel for visibility)
        _opaquePipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
            BlendStateDescription.SingleOverrideBlend, 
            DepthStencilStateDescription.DepthOnlyLessEqual,
            new RasterizerStateDescription(
                FaceCullMode.None,
                PolygonFillMode.Solid,
                FrontFace.CounterClockwise,
                true, false),
            PrimitiveTopology.TriangleList,
            new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float3),
                        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float2),
                        new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float3))
                },
                shaders),
            new[] { projViewLayout, _worldTextureLayout },
            _sceneFramebuffer.OutputDescription));

        _projViewSet = factory.CreateResourceSet(new ResourceSetDescription(
            projViewLayout, _projectionBuffer, _viewBuffer, _lightBuffer));

        // Skinned pipeline. Reuses the existing fragment shader so lighting/texturing
        // stays consistent. Resource sets: 0=projView/light, 1=world+texture, 2=bones.
        _boneLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("BoneBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

        var skinnedVert = new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(SkinnedVertexCode), "main");
        var skinnedShaders = factory.CreateFromSpirv(skinnedVert, frag);

        _skinnedPipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
            BlendStateDescription.SingleAlphaBlend,
            DepthStencilStateDescription.DepthOnlyLessEqual,
            new RasterizerStateDescription(
                FaceCullMode.None,
                PolygonFillMode.Solid,
                FrontFace.CounterClockwise,
                true, false),
            PrimitiveTopology.TriangleList,
            new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float3),
                        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float2),
                        new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float3),
                        new VertexElementDescription("BoneWeights", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float4),
                        new VertexElementDescription("BoneIndices", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.UInt4))
                },
                skinnedShaders),
            new[] { projViewLayout, _worldTextureLayout, _boneLayout },
            _sceneFramebuffer.OutputDescription));

        // Skeleton overlay pipeline. Depth disabled so bones stay visible through the
        // skinned mesh — the whole point is debug viz. Uses dedicated minimal layouts
        // so the pipeline doesn't carry along the textured pipeline's LightBuffer (the
        // D3D11 backend rejects the bind otherwise — shader doesn't reference it).
        _lineProjViewLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
        _lineProjViewSet = factory.CreateResourceSet(new ResourceSetDescription(
            _lineProjViewLayout, _projectionBuffer, _viewBuffer));

        _lineWorldLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
        _lineWorldSet = factory.CreateResourceSet(new ResourceSetDescription(_lineWorldLayout, _worldBuffer));

        var lineVert = new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(LineVertexCode), "main");
        var lineFrag = new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(LineFragmentCode), "main");
        var lineShaders = factory.CreateFromSpirv(lineVert, lineFrag);

        _linePipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
            BlendStateDescription.SingleOverrideBlend,
            DepthStencilStateDescription.Disabled,
            new RasterizerStateDescription(FaceCullMode.None, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, false),
            PrimitiveTopology.LineList,
            new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3))
                },
                lineShaders),
            new[] { _lineProjViewLayout, _lineWorldLayout },
            _sceneFramebuffer.OutputDescription));

        // Two endpoints per bone (child + parent). Reused across every skinned model
        // every frame — only the first MaxBones*2 slots are ever touched.
        _skeletonLineScratch = new Vector3[MaxBones * 2];
        _skeletonLineBuffer = factory.CreateBuffer(new BufferDescription(
            (uint)(_skeletonLineScratch.Length * 12), BufferUsage.VertexBuffer));

        Initialized = true;
    }

    private Pipeline _opaquePipeline;

    public void AddModel(B3DModel model, int materialIndex, List<Vector2>? uvs = null)
    {
        AddModel(model, materialIndex, null, null, uvs);
    }
    
    // Helper to keep similar signature but with matrix
    public void AddModel(B3DModel model, int materialIndex, Matrix4x4? worldMatrix, List<Vector2>? uvs = null)
    {
        AddModel(model, materialIndex, null, worldMatrix, uvs);
    }
    
    private Texture Create1x1TextureFromColor(GraphicsDevice graphicsDevice, ResourceFactory factory, float r, float g, float b, float a = 1.0f)
    {
        // Create a 1x1 texture
        var texture = factory.CreateTexture(TextureDescription.Texture2D(
            1, 1, 1, 1, PixelFormat.B8_G8_R8_A8_UNorm, TextureUsage.Sampled));

        // Convert the color components to bytes
        var colorData = new byte[]
        {
            (byte)(b * 255), // Blue
            (byte)(g * 255), // Green
            (byte)(r * 255), // Red
            (byte)(a * 255)  // Alpha
        };

        // Update the texture with the color data
        graphicsDevice.UpdateTexture(texture, colorData, 0, 0, 0, 1, 1, 1, 0, 0);

        return texture;
    }

    public void AddModel(B3DModel model, int materialIndex, string? textureFileName, Matrix4x4? worldMatrix = null, List<Vector2>? uvs = null)
    {
        if (!Initialized)
        {
            throw new InvalidOperationException("SceneRenderer must be initialized before adding models");
        }
        
        var texId = model.SubModels.Keys.ElementAt(materialIndex);
        if (_ignoredZones.Contains(texId))
        {
            Console.WriteLine($"Skipping zone {texId}.");
            return;
        }

        var factory = _graphicsDevice.ResourceFactory;
        var modelData = new ModelData
        {
            Name = texId,
            WorldMatrix = worldMatrix ?? Matrix4x4.Identity // Use provided matrix or Identity
        };
        
        var mtl_id = new string(model.MaterialDict[texId].Mtl_ID);
        
        if (mtl_id is "NO_TEXTURE")
        {
            var color = model.MaterialDict[texId].Mater.Diffuse;
            modelData.SurfaceTexture = Create1x1TextureFromColor(_graphicsDevice, factory, color.r, color.g, color.b, color.a);
            modelData.SurfaceTextureView = factory.CreateTextureView(modelData.SurfaceTexture);
            modelData.WorldTextureSet = factory.CreateResourceSet(new ResourceSetDescription(
                _worldTextureLayout, _worldBuffer, modelData.SurfaceTextureView, _graphicsDevice.Aniso4xSampler));
        }
        else if (mtl_id is "INVISIBLE")
        {
            return;
        }
        else
        {
            // Read texture from file name
            if (string.IsNullOrEmpty(textureFileName)) textureFileName = model.MatNameToTexture[texId];
            
            // instead of stitching together entity textures, we will process UV coordinates based on model type.

            ImageSharpTexture tx = ResourceSystem.FindResource(textureFileName);
            if (tx is not null)
            {
                modelData.SurfaceTextureView =
                    factory.CreateTextureView(tx.CreateDeviceTexture(_graphicsDevice, factory));
            }
            else // default to black texture if not found
            {
                Console.WriteLine($"Texture {textureFileName} not found, using default black texture.");
                modelData.SurfaceTexture = Create1x1TextureFromColor(_graphicsDevice, factory, 0.1f, 0.1f, 0.1f);
                modelData.SurfaceTextureView = factory.CreateTextureView(modelData.SurfaceTexture);
            }

            modelData.WorldTextureSet = factory.CreateResourceSet(new ResourceSetDescription(
                _worldTextureLayout, _worldBuffer, modelData.SurfaceTextureView, _graphicsDevice.Aniso4xSampler));
        }

        var modelFaces = model.SubModels[texId];
        
        Console.WriteLine($"{modelFaces.Count/3} faces for material {texId}");
        
        uvs ??= model.Model_Vertex_Texcoords; // Use UVs if provided, otherwise default to model's UVs
        
        // Load model vertices and indices
        for (var i = 0; i < model.Model_Vertex_Position.Count; i++)
        {
            modelData.Vertices.Add(
                new VertexPositionTextureNormal(
                    model.Model_Vertex_Position[i], 
                    uvs[i], 
                    model.Model_Vertex_Normal[i]));
        }
        
        foreach (var face in model.SubModels[texId])
        {
            modelData.Indices.Add((ushort)(face));
        }

        // Create vertex buffer
        modelData.VertexBuffer = factory.CreateBuffer(new BufferDescription(
            (uint)(modelData.Vertices.Count * VertexPositionTextureNormal.SizeInBytes),
            BufferUsage.VertexBuffer));
        _graphicsDevice.UpdateBuffer(modelData.VertexBuffer, 0, modelData.Vertices.ToArray());

        // Create index buffer
        modelData.IndexBuffer = factory.CreateBuffer(new BufferDescription(
            (uint)(modelData.Indices.Count * sizeof(ushort)),
            BufferUsage.IndexBuffer));
        _graphicsDevice.UpdateBuffer(modelData.IndexBuffer, 0, modelData.Indices.ToArray());

        _models.Add(modelData);
    }
    
    public bool IsCleared()
    {
        return _models.Count == 0 && _skinnedModels.Count == 0;
    }

    public void ClearModels()
    {
        foreach (var model in _models)
        {
            model.VertexBuffer?.Dispose();
            model.IndexBuffer?.Dispose();
            model.SurfaceTexture?.Dispose();
            model.SurfaceTextureView?.Dispose();
            model.WorldTextureSet?.Dispose();
        }
        _models.Clear();

        foreach (var model in _skinnedModels)
        {
            model.VertexBuffer?.Dispose();
            model.IndexBuffer?.Dispose();
            model.SurfaceTexture?.Dispose();
            model.SurfaceTextureView?.Dispose();
            model.WorldTextureSet?.Dispose();
            model.BoneBuffer?.Dispose();
            model.BoneSet?.Dispose();
        }
        _skinnedModels.Clear();
        _ignoredZones.Clear();
    }

    public int GetModelCount()
    {
        return _models.Count + _skinnedModels.Count;
    }

    public void SetModelWorldMatrix(int index, Matrix4x4 worldMatrix)
    {
        if (index >= 0 && index < _models.Count)
        {
            _models[index].WorldMatrix = worldMatrix;
        }
    }

    public void RemoveModel(int index)
    {
        if (index >= 0 && index < _models.Count)
        {
            var model = _models[index];
            model.VertexBuffer?.Dispose();
            model.IndexBuffer?.Dispose();
            model.SurfaceTexture?.Dispose();
            model.SurfaceTextureView?.Dispose();
            model.WorldTextureSet?.Dispose();
            _models.RemoveAt(index);
        }
    }

    public string GetModelName(int index)
    {
        if (index >= 0 && index < _models.Count)
        {
            return _models[index].Name;
        }
        return "";
    }

    public Matrix4x4 GetModelWorldMatrix(int index)
    {
        if (index >= 0 && index < _models.Count)
        {
            return _models[index].WorldMatrix;
        }
        return Matrix4x4.Identity;
    }
    
    public void DrawScene(float deltaTime)
    {
        if (!Initialized || (_models.Count == 0 && _skinnedModels.Count == 0))
            return;

        var cl = _commandList;

        InputTracker.UpdateFrameInput(_window.PumpEvents());
        if (AllowInput) _camera.Update(deltaTime);

        LightInfo light = new()
        {
            LightPosition = _camera.Position,
            LightColor = new Vector3(1, 1, 1),
            CameraPosition = _camera.Position,
        };

        cl.Begin();
        cl.SetFramebuffer(_sceneFramebuffer);
        cl.ClearColorTarget(0, RgbaFloat.Black);
        cl.ClearDepthStencil(1f);

        cl.UpdateBuffer(_projectionBuffer, 0, _camera.ProjectionMatrix);
        cl.UpdateBuffer(_viewBuffer, 0, _camera.ViewMatrix);
        cl.UpdateBuffer(_lightBuffer, 0, light);

        // Render each model
        foreach (var model in _models)
        {
            cl.SetPipeline(model.Pipeline ?? _pipeline);
            cl.SetGraphicsResourceSet(0, _projViewSet);
            cl.UpdateBuffer(_worldBuffer, 0, model.WorldMatrix);
            cl.SetGraphicsResourceSet(1, model.WorldTextureSet);
            cl.SetVertexBuffer(0, model.VertexBuffer);
            cl.SetIndexBuffer(model.IndexBuffer, IndexFormat.UInt16);
            cl.DrawIndexed((uint)model.Indices.Count, 1, 0, 0, 0);
        }

        // Skinned models: advance time, sample animation, bridge into B3D space, upload
        // bone palette, draw. Each model owns its own time accumulator + cached pose so
        // characters tick independently.
        // For the skeleton overlay we'd otherwise re-draw identical lines for every
        // sub-model on the same character (head/body/hair/etc. all share a skeleton).
        // Tracking poses we've already drawn this frame dedups them by reference.
        var drawnPoses = DrawSkeletonOverlay ? new HashSet<GrannyFormats.SkeletonPose>() : null;
        foreach (var model in _skinnedModels)
        {
            AdvanceAndUpload(cl, model, deltaTime);

            cl.SetPipeline(_skinnedPipeline);
            cl.SetGraphicsResourceSet(0, _projViewSet);
            cl.UpdateBuffer(_worldBuffer, 0, model.WorldMatrix);
            cl.SetGraphicsResourceSet(1, model.WorldTextureSet);
            cl.SetGraphicsResourceSet(2, model.BoneSet);
            cl.SetVertexBuffer(0, model.VertexBuffer);
            cl.SetIndexBuffer(model.IndexBuffer, IndexFormat.UInt16);
            cl.DrawIndexed((uint)model.Indices.Count, 1, 0, 0, 0);

            if (DrawSkeletonOverlay && drawnPoses!.Add(model.Pose))
                DrawSkeletonLines(cl, model);
        }

        cl.ResolveTexture(_msaaColorTarget, _sceneColorTarget);
        cl.End();
        _graphicsDevice.SubmitCommands(cl);
    }

    private void AdvanceAndUpload(CommandList cl, SkinnedModelData model, float deltaTime)
    {
        for (var i = 0; i < model.UploadMatrices.Length; i++)
            model.UploadMatrices[i] = Matrix4x4.Identity;

        if (!AnimateSkinned)
        {
            // Bind-pose mode: skin matrices are identity, so the shader's bridge
            // (B3dToEngine * I * EngineToB3d) cancels and the mesh renders as raw B3D
            // verts in their unposed positions. Populate the overlay pose from the
            // cached world bind matrices so the skeleton lines stay in sync.
            var bones = model.GrSkeleton.Skeleton.Bones;
            model.Pose ??= new GrannyFormats.SkeletonPose(bones.Count);
            for (var i = 0; i < bones.Count; i++)
            {
                model.Pose.WorldMatrices[i] = bones[i].WorldBindMatrix;
                model.Pose.SkinMatrices[i] = Matrix4x4.Identity;
            }
            cl.UpdateBuffer(model.BoneBuffer, 0, model.UploadMatrices);
            return;
        }

        // Loop the animation across its duration. If duration is 0/missing, just hold
        // the start frame (Sample handles that gracefully via clamping).
        var duration = model.GrAnim.Duration;
        if (duration > 0f)
        {
            model.Time = (model.Time + deltaTime) % duration;
        }

        model.Pose = GrannyFormats.Sample(model.GrSkeleton, model.GrAnim, model.Time, model.Pose);

        // Bridge engine→B3D so the shader can apply the matrix directly to a B3D-space
        // vertex. b3dToGr2 maps the B3D bone index used by the vertex attribute back to
        // the GR2 bone whose pose matrix we want.
        var skin = model.Pose.SkinMatrices;
        for (var b3dIdx = 0; b3dIdx < model.B3dToGr2.Length && b3dIdx < model.UploadMatrices.Length; b3dIdx++)
        {
            var gr2Idx = model.B3dToGr2[b3dIdx];
            if (gr2Idx < 0 || gr2Idx >= skin.Length) continue;
            model.UploadMatrices[b3dIdx] = B3dToEngine * skin[gr2Idx] * EngineToB3d;
        }

        cl.UpdateBuffer(model.BoneBuffer, 0, model.UploadMatrices);
    }

    // Builds parent→child line segments from the current pose and submits a single
    // line draw. Bone world positions live in engine space (×3 scale, swapped axes);
    // multiplying by EngineToB3d puts them in the same frame the mesh is rendered in,
    // so the overlay tracks the visible skeleton 1:1. _worldBuffer was just written
    // with this model's WorldMatrix for the mesh draw — we reuse it.
    private bool _skeletonOverlayDiagPrinted;
    private void DrawSkeletonLines(CommandList cl, SkinnedModelData model)
    {
        var bones = model.GrSkeleton.Skeleton.Bones;
        var worlds = model.Pose.WorldMatrices;
        var n = 0;
        for (var i = 0; i < bones.Count; i++)
        {
            var parent = bones[i].ParentIndex;
            if (parent < 0) continue;
            var childPos = Vector3.Transform(worlds[i].Translation, EngineToB3d);
            var parentPos = Vector3.Transform(worlds[parent].Translation, EngineToB3d);
            if (n + 2 > _skeletonLineScratch.Length) break;
            _skeletonLineScratch[n++] = parentPos;
            _skeletonLineScratch[n++] = childPos;
        }
        if (!_skeletonOverlayDiagPrinted)
        {
            _skeletonOverlayDiagPrinted = true;
            var firstChild = n >= 2 ? _skeletonLineScratch[1] : Vector3.Zero;
            Console.WriteLine($"[Skeleton overlay] {bones.Count} bones, {n / 2} segments, first child at {firstChild}");
        }
        if (n == 0) return;

        cl.UpdateBuffer(_skeletonLineBuffer, 0, _skeletonLineScratch);
        cl.SetPipeline(_linePipeline);
        cl.SetGraphicsResourceSet(0, _lineProjViewSet);
        cl.SetGraphicsResourceSet(1, _lineWorldSet);
        cl.SetVertexBuffer(0, _skeletonLineBuffer);
        cl.Draw((uint)n);
    }

    public void FocusCamera()
    {
        if (_models.Count == 0 && _skinnedModels.Count == 0) return;

        Vector3 min = new Vector3(float.MaxValue);
        Vector3 max = new Vector3(float.MinValue);
        bool hasVerts = false;

        foreach(var m in _models)
        {
            foreach(var v in m.Vertices)
            {
                 min = Vector3.Min(min, v.Position);
                 max = Vector3.Max(max, v.Position);
                 hasVerts = true;
            }
        }
        foreach (var m in _skinnedModels)
        {
            foreach (var v in m.Vertices)
            {
                min = Vector3.Min(min, v.Position);
                max = Vector3.Max(max, v.Position);
                hasVerts = true;
            }
        }

        if (!hasVerts) return;
        
        var center = (min + max) / 2f;
        var extent = max - min;
        var maxDim = Math.Max(extent.X, Math.Max(extent.Y, extent.Z));
        
        if (maxDim < 1f) maxDim = 10f; // Ensure minimum zoom

        Console.WriteLine($"Map Bounds: Min {min} Max {max} Center {center} Size {maxDim}");

        // Set camera position to see the whole bounds
        // Assuming Y-up: 
        // Position should be Up (Y+) and Back (Z+) from center.
        
        float dist = maxDim * 1.2f;
        
        _camera.Position = center + new Vector3(0, dist * 0.8f, dist);
        _camera.Yaw = 0; // Look along Z (?)
        _camera.Pitch = -0.5f; // Look down slightly
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct LightInfo
    {
        public Vector3 LightPosition;
        private float _pad1;
        public Vector3 LightColor;
        private float _pad2;
        public Vector3 CameraPosition;
        private float _pad3;
    }

    public class ModelData
    {
        public List<VertexPositionTextureNormal> Vertices { get; set; } = new List<VertexPositionTextureNormal>();
        public List<ushort> Indices { get; set; } = new List<ushort>();
        public DeviceBuffer VertexBuffer { get; set; }
        public DeviceBuffer IndexBuffer { get; set; }
        public Texture SurfaceTexture { get; set; }
        public TextureView SurfaceTextureView { get; set; }
        public ResourceSet WorldTextureSet { get; set; }
        public Pipeline Pipeline { get; set; }
        public Matrix4x4 WorldMatrix { get; set; } = Matrix4x4.Identity;
        public string Name { get; set; } = "";
    }
    
    public struct VertexPositionTextureNormal
    {
        public Vector3 Position;
        public Vector2 TexCoords;
        public Vector3 Normal;

        public const uint SizeInBytes = 4 * 8 + 4 * 3; // 32 bytes

        public VertexPositionTextureNormal(Vector3 pos, Vector2 uv, Vector3 normal)
        {
            Position = pos;
            TexCoords = uv;
            Normal = normal;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionTextureNormalSkinned
    {
        public Vector3 Position;
        public Vector2 TexCoords;
        public Vector3 Normal;
        public Vector4 BoneWeights;
        public uint BoneIndex0, BoneIndex1, BoneIndex2, BoneIndex3;

        public const uint SizeInBytes = 12 + 8 + 12 + 16 + 16; // 64 bytes

        public VertexPositionTextureNormalSkinned(
            Vector3 pos, Vector2 uv, Vector3 normal,
            Vector4 weights, uint i0, uint i1, uint i2, uint i3)
        {
            Position = pos;
            TexCoords = uv;
            Normal = normal;
            BoneWeights = weights;
            BoneIndex0 = i0; BoneIndex1 = i1; BoneIndex2 = i2; BoneIndex3 = i3;
        }
    }

    public class SkinnedModelData
    {
        public List<VertexPositionTextureNormalSkinned> Vertices { get; set; } = new();
        public List<ushort> Indices { get; set; } = new();
        public DeviceBuffer VertexBuffer { get; set; }
        public DeviceBuffer IndexBuffer { get; set; }
        public DeviceBuffer BoneBuffer { get; set; }
        public Texture SurfaceTexture { get; set; }
        public TextureView SurfaceTextureView { get; set; }
        public ResourceSet WorldTextureSet { get; set; }
        public ResourceSet BoneSet { get; set; }
        public Matrix4x4 WorldMatrix { get; set; } = Matrix4x4.Identity;
        public string Name { get; set; } = "";

        // Animation state — owned per part. Multiple body parts driven by the same
        // skeleton currently sample independently; tolerable redundancy for now.
        public GrannyFormats.model GrSkeleton;
        public GrannyFormats.animation GrAnim;
        public GrannyFormats.SkeletonPose Pose;
        public int[] B3dToGr2;
        public Matrix4x4[] UploadMatrices;
        public float Time;
    }

    // Adds one or more skinned draws for a B3D mesh, one per material sub-model. The
    // skeleton/animation come from already-parsed GR2 file_infos (typically a .skel
    // + .anim pair). Each resulting SkinnedModelData owns a bone-palette buffer +
    // its own time accumulator. The mesh's per-vertex skinning lives on
    // B3DModel.Vertex_Bones, populated by VanBurenFormats. Verts with no weights get
    // a single full-weight binding to bone 0 so the shader still lands somewhere
    // sensible; renderable but visually "rigid to bone 0".
    public void AddSkinnedB3DModel(
        B3DModel b3d,
        GrannyFormats.model gr2Skeleton,
        GrannyFormats.animation gr2Anim,
        Matrix4x4? worldMatrix = null,
        string textureOverride = null,
        List<Vector2>? uvs = null)
    {
        if (!Initialized)
            throw new InvalidOperationException("SceneRenderer must be initialized before adding models");
        if (gr2Skeleton.Skeleton.Bones.Count > MaxBones)
            throw new InvalidOperationException(
                $"Skeleton has {gr2Skeleton.Skeleton.Bones.Count} bones, max {MaxBones}. Raise MaxBones (and the GLSL array size) to support it.");

        var b3dToGr2 = GrannyFormats.MapB3dBonesToGr2Skeleton(b3d.Bones, gr2Skeleton.Skeleton);

        for (var matIdx = 0; matIdx < b3d.SubModels.Keys.Count; matIdx++)
        {
            AddSkinnedSubModel(b3d, matIdx, gr2Skeleton, gr2Anim, b3dToGr2, worldMatrix, textureOverride, uvs);
        }
        foreach (var zone in b3d.OcclusionZones)
            _ignoredZones.Add(zone);
    }

    private void AddSkinnedSubModel(
        B3DModel b3d,
        int materialIndex,
        GrannyFormats.model gr2Skeleton,
        GrannyFormats.animation gr2Anim,
        int[] b3dToGr2,
        Matrix4x4? worldMatrix,
        string textureOverride,
        List<Vector2>? uvOverride = null)
    {
        var texId = b3d.SubModels.Keys.ElementAt(materialIndex);
        if (_ignoredZones.Contains(texId)) return;

        var factory = _graphicsDevice.ResourceFactory;
        var modelData = new SkinnedModelData
        {
            Name = texId,
            WorldMatrix = worldMatrix ?? Matrix4x4.Identity,
            GrSkeleton = gr2Skeleton,
            GrAnim = gr2Anim,
            B3dToGr2 = b3dToGr2,
            UploadMatrices = new Matrix4x4[MaxBones]
        };

        var mtl_id = new string(b3d.MaterialDict[texId].Mtl_ID);
        if (mtl_id is "INVISIBLE") return;

        if (mtl_id is "NO_TEXTURE")
        {
            var color = b3d.MaterialDict[texId].Mater.Diffuse;
            modelData.SurfaceTexture = Create1x1TextureFromColor(_graphicsDevice, factory, color.r, color.g, color.b, color.a);
            modelData.SurfaceTextureView = factory.CreateTextureView(modelData.SurfaceTexture);
        }
        else
        {
            var textureFileName = string.IsNullOrEmpty(textureOverride) ? b3d.MatNameToTexture[texId] : textureOverride;
            ImageSharpTexture tx = ResourceSystem.FindResource(textureFileName);
            if (tx is not null)
            {
                modelData.SurfaceTextureView = factory.CreateTextureView(tx.CreateDeviceTexture(_graphicsDevice, factory));
            }
            else
            {
                Console.WriteLine($"Texture {textureFileName} not found, using default black texture.");
                modelData.SurfaceTexture = Create1x1TextureFromColor(_graphicsDevice, factory, 0.1f, 0.1f, 0.1f);
                modelData.SurfaceTextureView = factory.CreateTextureView(modelData.SurfaceTexture);
            }
        }

        modelData.WorldTextureSet = factory.CreateResourceSet(new ResourceSetDescription(
            _worldTextureLayout, _worldBuffer, modelData.SurfaceTextureView, _graphicsDevice.Aniso4xSampler));

        // Build skinned vertex buffer. We blit positions/normals/uvs from the existing
        // flat B3D vertex arrays + look up per-vertex (bone, weight) lists from
        // Vertex_Bones (aligned to Model_Vertex_Position by VanBurenFormats).
        var uvs = uvOverride ?? b3d.Model_Vertex_Texcoords;
        for (var i = 0; i < b3d.Model_Vertex_Position.Count; i++)
        {
            var bones = i < b3d.Vertex_Bones.Count ? b3d.Vertex_Bones[i] : null;
            ExpandToFour(bones, out var weights, out var i0, out var i1, out var i2, out var i3);
            modelData.Vertices.Add(new VertexPositionTextureNormalSkinned(
                b3d.Model_Vertex_Position[i],
                i < uvs.Count ? uvs[i] : Vector2.Zero,
                b3d.Model_Vertex_Normal[i],
                weights, i0, i1, i2, i3));
        }

        foreach (var face in b3d.SubModels[texId])
            modelData.Indices.Add((ushort)face);

        modelData.VertexBuffer = factory.CreateBuffer(new BufferDescription(
            (uint)(modelData.Vertices.Count * VertexPositionTextureNormalSkinned.SizeInBytes),
            BufferUsage.VertexBuffer));
        _graphicsDevice.UpdateBuffer(modelData.VertexBuffer, 0, modelData.Vertices.ToArray());

        modelData.IndexBuffer = factory.CreateBuffer(new BufferDescription(
            (uint)(modelData.Indices.Count * sizeof(ushort)),
            BufferUsage.IndexBuffer));
        _graphicsDevice.UpdateBuffer(modelData.IndexBuffer, 0, modelData.Indices.ToArray());

        modelData.BoneBuffer = factory.CreateBuffer(new BufferDescription(
            MaxBones * 64, BufferUsage.UniformBuffer));
        modelData.BoneSet = factory.CreateResourceSet(new ResourceSetDescription(
            _boneLayout, modelData.BoneBuffer));

        _skinnedModels.Add(modelData);
    }

    // Pads a per-vertex bone list into the fixed (weights, indices) shape the shader
    // expects. Drops entries past the 4th (rare in B3D, but defensive). Re-normalizes
    // weights so they always sum to 1.0 — Granny exporters sometimes produce slightly
    // off totals, and unnormalized weights cause volume changes when posed.
    private static void ExpandToFour(
        List<B3DModel.TVert_Bones_Data> bones,
        out Vector4 weights, out uint i0, out uint i1, out uint i2, out uint i3)
    {
        Span<float> w = stackalloc float[4] { 0f, 0f, 0f, 0f };
        Span<uint> idx = stackalloc uint[4] { 0u, 0u, 0u, 0u };

        if (bones != null)
        {
            var n = Math.Min(bones.Count, 4);
            float sum = 0f;
            for (var k = 0; k < n; k++)
            {
                w[k] = bones[k].Weight;
                idx[k] = (uint)Math.Max(0, bones[k].Bone);
                sum += w[k];
            }
            if (sum > 0f)
                for (var k = 0; k < n; k++) w[k] /= sum;
            else
                w[0] = 1f; // no usable weights, anchor to bone 0 (already idx[0]=0)
        }
        else
        {
            w[0] = 1f;
        }

        weights = new Vector4(w[0], w[1], w[2], w[3]);
        i0 = idx[0]; i1 = idx[1]; i2 = idx[2]; i3 = idx[3];
    }
    
    public void PrintCameraDiagnostics()
    {
        Console.WriteLine($"Camera FOV: {_camera.FieldOfView}");
        Console.WriteLine($"Camera Near Plane: {_camera.NearDistance}");
        Console.WriteLine($"Camera Far Plane: {_camera.FarDistance}");
        Console.WriteLine($"Camera Aspect Ratio: {_camera.AspectRatio}");
        Console.WriteLine($"Camera Position: {_camera.Position}");
        Console.WriteLine($"Camera Yaw: {_camera.Yaw}");
        Console.WriteLine($"Camera Pitch: {_camera.Pitch}");
    }
}