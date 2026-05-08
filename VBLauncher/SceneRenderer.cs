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
layout(location = 3) in vec4 Color;      // per-vertex color (white if unused)

layout(location = 0) out vec2  fsin_texCoords;
layout(location = 1) out vec3  fsin_normal;
layout(location = 2) out vec3  fsin_worldPos;
layout(location = 3) out vec4  fsin_color;

void main() {
    // world‐space position
    vec4 worldPos4 = World * vec4(Position, 1.0);
    fsin_worldPos = worldPos4.xyz;

    // transform & normalize normal into world‐space
    fsin_normal   = normalize(mat3(World) * Normal);

    // pass UVs and vertex color through
    fsin_texCoords = TexCoords;
    fsin_color     = Color;

    // MVP
    gl_Position = Projection * View * worldPos4;
}";

    // Multi-light fragment shader for B3D entities (critters, props, etc).
    // Sums per-light diffuse contributions inline — no helper function, no
    // early returns. SPIRV-Cross emits do-while(false) blocks for shaders
    // with early returns + a containing for-loop, and fxc SEH-crashes on
    // that combo. Branchless masking keeps the HLSL output as a flat
    // sequence of arithmetic ops which fxc handles cleanly.
    //
    // Per-light layout (four vec4s):
    //   v0: xyz = Position,  w = Type
    //   v1: xyz = Color,     w = Radius
    //   v2: xyz = Direction, w = FalloffStart
    //   v3: x = ConeInnerCos, y = ConeOuterCos, z = HasDirection (0/1), w = unused
    private static readonly string FragmentCode = @"#version 450
#define MAX_LIGHTS 16

layout(location = 0) in vec2  fsin_texCoords;
layout(location = 1) in vec3  fsin_normal;
layout(location = 2) in vec3  fsin_worldPos;
layout(location = 3) in vec4  fsin_color;

layout(location = 0) out vec4 fsout_color;

layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler  SurfaceSampler;

layout(set = 0, binding = 3) uniform LightInfo {
    vec4 Header;             // x = LightCount, yzw unused
    vec4 AmbientCamera;      // xyz = AmbientColor, w unused
    vec4 CameraPos;          // xyz = camera position, w unused
    vec4 Lights[MAX_LIGHTS * 4];
} uLights;

void main() {
    vec4 tex = texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords);
    if (tex.a < 0.45) discard;
    vec3 albedo = tex.rgb * fsin_color.rgb;

    vec3 N = normalize(fsin_normal);
    vec3 lighting = uLights.AmbientCamera.xyz;
    int lightCount = int(uLights.Header.x);

    for (int i = 0; i < MAX_LIGHTS; i++) {
        // Branchless ‘slot is in use’ mask — multiplying the contribution
        // by this scalar nukes the slot when i >= lightCount, which keeps
        // the loop body free of early-exits (fxc-friendly).
        float slotMask = step(float(i), float(lightCount - 1));

        int base = i * 4;
        vec4 v0 = uLights.Lights[base + 0];
        vec4 v1 = uLights.Lights[base + 1];
        vec4 v2 = uLights.Lights[base + 2];
        vec4 v3 = uLights.Lights[base + 3];

        vec3 toLight = v0.xyz - fsin_worldPos;
        float dist = length(toLight);
        vec3 L = toLight / max(dist, 0.0001);
        float NdotL = max(dot(N, L), 0.0);

        float radius = v1.w;
        float startDist = radius * v2.w;
        float distAtt = clamp((radius - dist) / max(radius - startDist, 0.0001), 0.0, 1.0);
        distAtt = distAtt * distAtt * step(dist, radius);

        // Directional light bypass: Type == 4 (used for dominant world light)
        // behaves as an infinite directional light with no distance falloff.
        float isDirectional = step(3.5, v0.w) * step(v0.w, 4.5);
        distAtt = mix(distAtt, 1.0, isDirectional);
        L = mix(L, normalize(v0.xyz), isDirectional);
        NdotL = max(dot(N, L), 0.0);

        // Spotlight cone — branchless. Pull a safe direction (sub in +Z
        // when the slot has no direction so normalize() doesn't see zero
        // or NaN), compute the smoothstep cone mask unconditionally, then
        // pick between the cone mask and a constant 1.0 via mix() based
        // on the precomputed ‘is spotlight with direction’ flag.
        float coneInnerCos = v3.x;
        float coneOuterCos = v3.y;
        float hasDirection = v3.z;
        vec3 safeDir = mix(vec3(0.0, 0.0, 1.0), v2.xyz, hasDirection);
        float cosTheta = dot(-normalize(safeDir), L);
        float spotMask = smoothstep(coneOuterCos, coneInnerCos, cosTheta);
        // useSpot = 1 only when Type == 0 AND HasDirection.  Type is stored
        // in v0.w; abs(type) is 0 for spot and ≥1 for the point variants,
        // so 1 - min(abs(type), 1) collapses to 1.0 for spots only.
        float useSpot = (1.0 - min(abs(v0.w), 1.0)) * hasDirection;
        float coneFactor = mix(1.0, spotMask, useSpot);

        lighting += v1.xyz * NdotL * distAtt * coneFactor * slotMask;
    }

    fsout_color = vec4(albedo * lighting, fsin_color.a);
}";

    // .8 map geometry has lighting baked into the per-vertex DIFU colour
    // (fsin_color.rgb), so we don't run the per-fragment light loop here —
    // it would just double-up on what bake already captured. We do still
    // read AmbientCamera.xyz: partly as a tiny scene-mood top-up for fully
    // shadowed corners, and partly so SPIRV-Cross can't strip the cbuffer
    // declaration. Without that reference the cross-compiler emits HLSL
    // with no register(b2) binding, the D3D11 shader's resource interface
    // stops matching the pipeline's projViewLayout, and the whole opaque
    // pipeline silently refuses to draw.
    private const string OpaqueFragmentCode = @"#version 450
#define MAX_LIGHTS 16

layout(location = 0) in vec2  fsin_texCoords;
layout(location = 1) in vec3  fsin_normal;
layout(location = 2) in vec3  fsin_worldPos;
layout(location = 3) in vec4  fsin_color;

layout(location = 0) out vec4 fsout_color;

layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler  SurfaceSampler;

layout(set = 0, binding = 3) uniform LightInfo {
    vec4 Header;             // x = LightCount, yzw unused
    vec4 AmbientCamera;      // xyz = AmbientColor, w unused
    vec4 CameraPos;          // xyz = camera position, w unused
    vec4 Lights[MAX_LIGHTS * 4];
} uLights;

void main() {
    vec4 tex = texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords);
    vec3 albedo = tex.rgb * fsin_color.rgb;

    vec3 N = normalize(fsin_normal);
    vec3 lighting = uLights.AmbientCamera.xyz;
    int lightCount = int(uLights.Header.x);

    for (int i = 0; i < MAX_LIGHTS; i++) {
        float slotMask = step(float(i), float(lightCount - 1));

        int base = i * 4;
        vec4 v0 = uLights.Lights[base + 0];
        vec4 v1 = uLights.Lights[base + 1];
        vec4 v2 = uLights.Lights[base + 2];
        vec4 v3 = uLights.Lights[base + 3];

        vec3 toLight = v0.xyz - fsin_worldPos;
        float dist = length(toLight);
        vec3 L = toLight / max(dist, 0.0001);
        float NdotL = max(dot(N, L), 0.0);

        float radius = v1.w;
        float startDist = radius * v2.w;
        float distAtt = clamp((radius - dist) / max(radius - startDist, 0.0001), 0.0, 1.0);
        distAtt = distAtt * distAtt * step(dist, radius);

        // Directional light bypass: Type == 4 (used for dominant world light)
        // behaves as an infinite directional light with no distance falloff.
        float isDirectional = step(3.5, v0.w) * step(v0.w, 4.5);
        distAtt = mix(distAtt, 1.0, isDirectional);
        L = mix(L, normalize(v0.xyz), isDirectional);
        NdotL = max(dot(N, L), 0.0);

        float coneInnerCos = v3.x;
        float coneOuterCos = v3.y;
        float hasDirection = v3.z;
        vec3 safeDir = mix(vec3(0.0, 0.0, 1.0), v2.xyz, hasDirection);
        float cosTheta = dot(-normalize(safeDir), L);
        float spotMask = smoothstep(coneOuterCos, coneInnerCos, cosTheta);
        float useSpot = (1.0 - min(abs(v0.w), 1.0)) * hasDirection;
        float coneFactor = mix(1.0, spotMask, useSpot);

        lighting += v1.xyz * NdotL * distAtt * coneFactor * slotMask;
    }

    fsout_color = vec4(albedo * lighting, 1.0);
}";

    // Vertex shader for the lightmap-modulated .8 path. Takes a second
    // float2 (LightmapCoords) — the LMUV channel from VTXB — and forwards
    // it to the fragment stage. Otherwise mirrors the static VertexCode.
    private const string LightmappedVertexCode = @"#version 450

layout(set = 0, binding = 0) uniform ProjectionBuffer { mat4 Projection; };
layout(set = 0, binding = 1) uniform ViewBuffer       { mat4 View;       };
layout(set = 1, binding = 0) uniform WorldBuffer      { mat4 World;      };

layout(location = 0) in vec3 Position;
layout(location = 1) in vec2 TexCoords;
layout(location = 2) in vec3 Normal;
layout(location = 3) in vec4 Color;
layout(location = 4) in vec2 LightmapCoords;

layout(location = 0) out vec2 fsin_texCoords;
layout(location = 1) out vec3 fsin_normal;
layout(location = 2) out vec3 fsin_worldPos;
layout(location = 3) out vec4 fsin_color;
layout(location = 4) out vec2 fsin_lmCoords;

void main() {
    vec4 worldPos4 = World * vec4(Position, 1.0);
    fsin_worldPos = worldPos4.xyz;
    fsin_normal   = normalize(mat3(World) * Normal);
    fsin_texCoords = TexCoords;
    fsin_lmCoords  = LightmapCoords;
    fsin_color     = Color;
    gl_Position = Projection * View * worldPos4;
}";

    // Fragment shader for the lightmap-modulated .8 path. Mirrors the engine's
    // BASE_LM_2X fixed-function setup: stage 0 = base * vertex diffuse, stage 1
    // = lightmap * current * 2X. The vertex DIFU is pre-doubled at .8-load time
    // (VanBurenFormats half-brightness decode), so the two doublings collapse —
    // we sample the lightmap at native and don't multiply by 2 again. For
    // batches with no lightmap (lmPage == -1) the renderer binds a 1×1 white
    // texture, which makes this shader degrade to plain `tex × diffuse`.
    //
    // We touch uLights.AmbientCamera the same way OpaqueFragmentCode does — not
    // because we need it for shading (the bake covers static lighting and the
    // LM covers atlased detail), but to keep the binding alive across
    // SPIRV-Cross → HLSL → fxc; without the reference the cbuffer drops out of
    // the D3D11 shader and the pipeline silently refuses to draw.
    private const string LightmappedFragmentCode = @"#version 450
#define MAX_LIGHTS 16

layout(location = 0) in vec2 fsin_texCoords;
layout(location = 1) in vec3 fsin_normal;
layout(location = 2) in vec3 fsin_worldPos;
layout(location = 3) in vec4 fsin_color;
layout(location = 4) in vec2 fsin_lmCoords;

layout(location = 0) out vec4 fsout_color;

layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler   SurfaceSampler;
layout(set = 1, binding = 3) uniform texture2D LightmapTexture;
layout(set = 1, binding = 4) uniform sampler   LightmapSampler;

layout(set = 0, binding = 3) uniform LightInfo {
    vec4 Header;
    vec4 AmbientCamera;
    vec4 CameraPos;
    vec4 Lights[MAX_LIGHTS * 4];
} uLights;

void main() {
    vec4 tex = texture(sampler2D(SurfaceTexture,  SurfaceSampler),  fsin_texCoords);
    vec4 lm  = texture(sampler2D(LightmapTexture, LightmapSampler), fsin_lmCoords);
    // No alpha-discard: many .8 TGAs ship with junk/zero alpha (same reason
    // OpaqueFragmentCode skips it). Pipeline is SingleOverrideBlend so alpha
    // is ignored at output anyway. IMAGE_ALPHA cutout cases are routed to
    // the existing alpha-tested pipeline, not here.
    //
    // No fsin_color multiply on the LM path. The engine's BASE_LM_2X stage
    // chain technically does `tex × diffuse × LM × 2X`, but on lightmapped
    // surfaces the per-vertex DIFU bake encodes the *same* directional
    // lighting that already lives in the LM atlas — applying both produces
    // a double-shaded look (visually reads as a normal-direction tint
    // because both bakes correlate with surface normals). The .8 reader
    // already pre-doubles DIFU for the no-LM path; keeping that intact and
    // sampling LM at native gives the correct result on lightmapped faces.
    //
    // AmbientCamera ref is REAL, not a `* 0.0` placeholder — SPIRV-Cross /
    // fxc happily constant-fold a `* 0` away, which strips the LightInfo
    // cbuffer from the compiled D3D11 shader and misaligns the resource set
    // (projViewLayout has 3 bindings, the compiled shader would have 2).
    // Symptom: draws silently produce no output. We mix a small ambient
    // lift into the LM term so the binding survives every backend.
    vec3 ambient = uLights.AmbientCamera.xyz * 0.1;
    fsout_color = vec4(tex.rgb * (lm.rgb + ambient), 1.0);
}";

    private static readonly string SkinnedVertexCode = @"#version 450

layout(set = 0, binding = 0) uniform ProjectionBuffer { mat4 Projection; };
layout(set = 0, binding = 1) uniform ViewBuffer       { mat4 View;       };
layout(set = 1, binding = 0) uniform WorldBuffer      { mat4 World;      };
layout(set = 2, binding = 0) uniform BoneBuffer       { mat4 Bones[" + MaxBones + @"]; };

layout(location = 0) in vec3  Position;
layout(location = 1) in vec2  TexCoords;
layout(location = 2) in vec3  Normal;
layout(location = 3) in vec4  Color;
layout(location = 4) in vec4  BoneWeights;
layout(location = 5) in uvec4 BoneIndices;

layout(location = 0) out vec2 fsin_texCoords;
layout(location = 1) out vec3 fsin_normal;
layout(location = 2) out vec3 fsin_worldPos;
layout(location = 3) out vec4 fsin_color;

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
    fsin_color     = Color;
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
    private static readonly Matrix4x4 B3dToEngine = new(
        0.3f,  0f,    0f,   0f,
        0f,    0f,    0.3f, 0f,
        0f,   -0.3f,  0f,   0f,
        0f,    0f,    0f,   1f);
    private static readonly Matrix4x4 EngineToB3d = new(
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

    private List<ModelData> _models = new();
    private List<SkinnedModelData> _skinnedModels = new();
    private Camera _camera;

    private DeviceBuffer _lightBuffer;
    private Pipeline _pipeline;
    private Pipeline _skinnedPipeline;
    private Pipeline _opaqueSkinnedPipeline;
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
    
    // Legacy/VT=3 b3ds (doors mostly?) are 10x smaller than VT=28 (which are correct relative to .8, .skl, etc.)
    static readonly Matrix4x4 LegacyB3DCorrection = Matrix4x4.CreateScale(10f);

    // EXPERIMENTAL: bbox-based per-model rotation. Hypothesis: every B3D's geometry
    // is authored along one specific horizontal axis (the one its bbox extends most
    // along), and the engine rotates each model so that primary axis aligns with +Z
    // before placement. Three known samples agree perfectly:
    //   Console (works)  → primary axis Y → 0°
    //   VEntrance (+180°) → primary axis -Z → +180°
    //   Glass (-90°)     → primary axis +X → -90°
    // If this rule holds across more models, it confirms the encoding is something
    // that correlates with bbox shape — almost certainly a field we're already
    // parsing (e.g. Bone[0].Translation, an unread material flag, etc.) but not
    // currently using to derive rotation. If the rule fails, the actual encoding
    // is somewhere else entirely.
    static float BboxOrientationCorrectionRadians(B3DModel model)
    {
        if (model.Model_Vertex_Position.Count == 0) return 0f;
        float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;
        foreach (var v in model.Model_Vertex_Position)
        {
            if (v.X < minX) minX = v.X; if (v.X > maxX) maxX = v.X;
            if (v.Y < minY) minY = v.Y; if (v.Y > maxY) maxY = v.Y;
            if (v.Z < minZ) minZ = v.Z; if (v.Z > maxZ) maxZ = v.Z;
        }
        var sizeX = maxX - minX;
        var sizeY = maxY - minY;
        var sizeZ = maxZ - minZ;
        var cx = (minX + maxX) * 0.5f;
        var cz = (minZ + maxZ) * 0.5f;

        // If the primary extent is vertical, no horizontal rotation is needed.
        if (sizeY >= sizeX && sizeY >= sizeZ) return 0f;

        // Pick X vs Z as the dominant horizontal axis. The sign of the *center* on
        // that axis tells us whether the geometry is biased toward + or -, which is
        // the direction the model "points".
        if (sizeX > sizeZ)
            // Geometry extends primarily along X. Rotate to align that direction with +Z.
            // +X bias → -90° (rotates +X to +Z); -X bias → +90°.
            return cx >= 0f ? -MathF.PI / 2f : MathF.PI / 2f;
        else
            // Geometry extends primarily along Z. -Z bias → +180° to flip to +Z.
            return cz < 0f ? MathF.PI : 0f;
    }

    static Matrix4x4? ApplyLegacyCorrection(B3DModel model, Matrix4x4? worldMatrix)
    {
        // Bbox-rotation experiment failed past n=3 (false positive on Tribal shelf /
        // Tech computer; false negative on Tech generator). Keeping the predictor
        // alive only as a diagnostic — see B3DModel.DescribeTransformSignals.
        // V35-legacy critters (CR_Sheep/Bear/Badger) report FormatVer==1 too because
        // they share the legacy material/vertex wire format, but their world-space
        // coordinates already match the modern critters and don't need the
        // door-mesh correction matrix.
        if (model.FormatVer != 1 || model.IsLegacyV35) return worldMatrix;
        return LegacyB3DCorrection * (worldMatrix ?? Matrix4x4.Identity);
    }

    public void AddB3DModel(B3DModel model, Matrix4x4? worldMatrix = null, List<Vector2>? uvs = null)
    {
        worldMatrix = ApplyLegacyCorrection(model, worldMatrix);
        for (var i = 0; i < model.SubModels.Keys.Count; i++)
        {
            AddModel(model, i, null, worldMatrix, uvs);
        }
        foreach (var zone in model.OcclusionZones)
            _ignoredZones.Add(zone);
    }

    public void AddB3DModel(B3DModel model, string textureOverride, Matrix4x4? worldMatrix = null, List<Vector2>? uvs = null)
    {
        worldMatrix = ApplyLegacyCorrection(model, worldMatrix);
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
    
    public void AddEntity(Entity entity, Matrix4x4? worldMatrix = null)
    {
        var baseModelName = entity.EEN2.skl;
        if (string.IsNullOrEmpty(baseModelName))
        {
            Console.WriteLine("Entity has no base model, skipping.");
            return;
        }
        B3DModel baseModel = ResourceSystem.FindResource(baseModelName, "b3d");
        if (baseModel is null) {
            Console.WriteLine($"Base model {baseModelName}.b3d not found for entity, skipping.");
            return;
        }
        
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
            var baseName = baseModelName.StartsWith("Hum") ? baseModelName[..4] : baseModelName;
            var resource = ResourceSystem.FindResource(baseName + "_Def_Una_Idle_01", "gr2");
            if (resource is null) resource = ResourceSystem.FindResource(baseName + "_Def_1H_Idle_01", "gr2");
            if (resource is null) resource = ResourceSystem.FindResource(baseName + "_Com_Una_Idle_01", "gr2");
            if (resource is GrannyFormats.file_info { Animations: { Count: > 0 } anims })
                gr2Anim = anims[0];
            else
                Console.WriteLine($"CRT animation not found for {baseName}, falling back to static pose.");
        }
        if (!gr2Anim.HasValue) gr2Skel = null; // need both for the skinned path
        
        if (entity is CRT crt && baseModelName.StartsWith("Hum"))
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
                AddSkinnedB3DModel(baseModel, gr2Skel.Value, gr2Anim.Value, worldMatrix, entity.EEN2.EEOV.s4, baseUVs);
            else
                AddB3DModel(baseModel, entity.EEN2.EEOV.s4, worldMatrix, baseUVs);
            
            return;
        }
        
        if (gr2Skel.HasValue && gr2Anim.HasValue)
            AddSkinnedB3DModel(baseModel, gr2Skel.Value, gr2Anim.Value, worldMatrix, entity.EEN2.EEOV.s4);
        else
            AddB3DModel(baseModel, entity.EEN2.EEOV.s4, worldMatrix);
    }

    public void AddMap(Map map)
    {
        // Add Map Geometry
        _8Model? loaded8 = null;
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
                    loaded8 = _8;
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
                // Per-template rotation correction (if any) is now derived from the
                // model's geometry inside ApplyLegacyCorrection — see
                // BboxOrientationCorrectionRadians. This loop just applies the EME2
                // placement transform.
                var transform = Matrix4x4.CreateRotationY(ent.l.W) *
                                Matrix4x4.CreateTranslation(-ent.l.X, ent.l.Y, ent.l.Z);

                var entity = ResourceSystem.FindResource(ent.name);
                if (entity != null) AddEntity(entity, transform);
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
                    AddEntity(crt, transform);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding default PC to map: {ex.Message}");
            }
            // Clear ignored zones here too
            _ignoredZones.Clear();
        }
        
        // Lighting setup. The .map file ships an EMAP.col global tint that we
        // fold into AmbientColor (so even pitch-black corners pick up the map
        // designer's mood colour). Every LGTR record from the .8 file is
        // forwarded to the shader which sums them per-fragment with hard-cutoff
        // attenuation. Static .8 geometry uses the opaque shader which ignores
        // these — its lighting is pre-baked into per-vertex DIFU — but every
        // dynamic entity (critter, prop, default-PC) shades against this list.
        var emapTint = (new Vector3(1) + EditorWindow.ColToVec3(map.EMAP.col)) / 2;
        UploadMapLights(loaded8?.Lights, emapTint, loaded8?.GetDominantLight());

        FocusCamera();
    }

    // Translates a list of LightRecord entries into the GpuLight array the
    // shader expects, applies the map's ambient tint, and pushes both header
    // and array into _lightBuffer in a single contiguous upload. When the
    // map has no LGTD chunk we fall back to a single hardcoded "outdoor sun"
    // so B3D viewers (no map loaded) still get sensible shading.
    private void UploadMapLights(List<LightRecord>? lights, Vector3 ambient, LightRecord? dominant = null)
    {
        Array.Clear(_gpuLights);

        var n = 0;
        if (lights != null)
        {
            foreach (var l in lights)
            {
                if (n >= MaxLights) break;
                if (!float.IsFinite(l.Position.X) || !float.IsFinite(l.Position.Y) || !float.IsFinite(l.Position.Z))
                    continue;

                var radius = l.Radius > 0 ? l.Radius : 1f;
                // Fold Intensity into Color on the CPU so the shader doesn't
                // multiply per-fragment.
                var color = l.Color * (l.Intensity > 0 ? l.Intensity : 1f);

                // Cone angles default to a near-omni cone for type-0 records
                // that ship without explicit angles (Junktown's distant
                // "sun" marker hits this).
                var inner = l.ConeInnerRad > 0 ? l.ConeInnerRad : MathF.PI;
                var outer = l.ConeOuterRad > inner ? l.ConeOuterRad : inner + 0.01f;

                // hasDirection lets the shader skip the spot cone for type-0
                // records that ship a NaN Direction (rather than NaN-checking
                // inside the shader, which fxc handled poorly). We also nuke
                // any non-finite Direction component so the GPU never sees a
                // NaN — D3D11's HLSL backend treats NaNs in cbuffer data as
                // undefined behaviour.
                bool dirFinite = float.IsFinite(l.Direction.X) && float.IsFinite(l.Direction.Y) && float.IsFinite(l.Direction.Z);
                var dir = dirFinite ? l.Direction : Vector3.Zero;

                _gpuLights[n] = new GpuLight
                {
                    PositionType     = new Vector4(l.Position.X, l.Position.Y, l.Position.Z, l.Type),
                    ColorRadius      = new Vector4(color.X, color.Y, color.Z, radius),
                    DirectionFalloff = new Vector4(dir.X, dir.Y, dir.Z, MathF.Max(0f, MathF.Min(l.FalloffStart, 0.99f))),
                    ConeAndFlags     = new Vector4(MathF.Cos(inner), MathF.Cos(outer), dirFinite ? 1f : 0f, 0f),
                };
                n++;
            }

            // Inject the dominant light as a directional light if it exists.
            // B3D map geometry is already lit by vertex colors (DIFU), but
            // dynamic critters/props need a global "sun" to look correct in
            // large outdoor maps where point lights are sparse. We treat the
            // dominant light as an infinite directional light (Type 4) to
            // bypass distance attenuation.
            if (dominant != null && n < MaxLights)
            {
                var color = dominant.Color * (dominant.Intensity > 0 ? dominant.Intensity : 1f);
                _gpuLights[n] = new GpuLight
                {
                    // For Type 4, Position field is used as Direction vector
                    PositionType     = new Vector4(dominant.Position.X, dominant.Position.Y, dominant.Position.Z, 4), 
                    ColorRadius      = new Vector4(color.X, color.Y, color.Z, 1_000_000f),
                    DirectionFalloff = new Vector4(0f, 0f, 0f, 0.99f),
                    ConeAndFlags     = new Vector4(1f, 1f, 0f, 0f),
                };
                n++;
            }
        }

        if (n == 0 || (dominant == null && n == 0))
        {
            // Outdoor-sun fallback — matches the old hardcoded LightPosition
            // that B3D viewers used to get from the single-light shader.
            _gpuLights[0] = new GpuLight
            {
                PositionType     = new Vector4(5000, 4000, 10000, 4), // Type 4 = Directional
                ColorRadius      = new Vector4(ambient.X, ambient.Y, ambient.Z, 1_000_000f),
                DirectionFalloff = new Vector4(0f, 0f, 0f, 0.99f),
                ConeAndFlags     = new Vector4(1f, 1f, 0f, 0f),
            };
            n = 1;
        }

        _lightHeader.Header = new Vector4(n, 0f, 0f, 0f);
        // matches the old shader's `0.3 * albedo` ambient baseline.
        var amb = ambient * 0.3f;
        _lightHeader.AmbientCamera = new Vector4(amb, 0f);
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
        
        // Group faces by (texture, material, lightmap page). MDX (1-based into
        // MAT) picks the bound base texture; MFL (0-based into Materials)
        // picks the technique; LMI is the per-face lightmap atlas index (-1
        // = no LM). The same base texture atlas can appear with multiple
        // materials AND multiple lightmap pages, so we have to split on all
        // three. MDX==0 collapses to a single "no texture" bucket regardless
        // of the material/lightmap it carries.
        var materialFaces = new Dictionary<(int Mdx, int Mat, int Lm), List<int>>();

        for (int i = 0; i < model.IDX.Count; i++)
        {
            var mdxindex = model.MDX[i];
            if (mdxindex == 0) mdxindex = -1;
            var matIdx = i < model.MFL.Count ? (int)model.MFL[i] : 0;
            var lmPage = i < model.LMI.Count ? model.LMI[i] : -1;
            // Force "no LM" when the file didn't ship VTXB.LMUV at all — the
            // GVL list will be empty in that case, so any lmPage we picked up
            // from IDXS is unusable.
            if (model.GVL.Count == 0) lmPage = -1;
            var key = (mdxindex, matIdx, lmPage);
            if (!materialFaces.TryGetValue(key, out var bucket))
            {
                bucket = new List<int>();
                materialFaces[key] = bucket;
            }
            bucket.Add(i);
        }

        Console.WriteLine($"[SceneRenderer] Add8Model: GVP Count {model.GVP.Count}, IDX Count {model.IDX.Count}, MAT Count {model.MAT.Count}, Materials Count {model.Materials.Count}, GVL Count {model.GVL.Count}, LightmapStart {model.LightmapStart}");
        Console.WriteLine($"[SceneRenderer] Add8Model: Material Faces Breakdown: {string.Join(", ", materialFaces.Select(k => $"(Tex={k.Key.Mdx}, Mat={k.Key.Mat}, Lm={k.Key.Lm}): {k.Value.Count} faces"))}");

        foreach (var kvp in materialFaces)
        {
            var matIndex = kvp.Key.Mdx;
            var matRecordIndex = kvp.Key.Mat;
            var lmPage = kvp.Key.Lm;
            var faceIndices = kvp.Value;
            if (faceIndices.Count == 0) continue;

            // Materials whose technique name is literally "INVISIBLE" are
            // collision/trigger meshes the engine never draws (e.g. mineshaft
            // walkability volumes in Test_Int_Mines1_10x10.8). The B3D path
            // already returns early on this name; mirror that here so .8
            // collision geometry doesn't bleed through as visible polygons.
            if (matRecordIndex >= 0 && matRecordIndex < model.Materials.Count
                && model.Materials[matRecordIndex].Name == "INVISIBLE")
            {
                continue;
            }

            string textureName = null;
            if (matIndex > 0 && matIndex <= model.MAT.Count)
            {
                 textureName = Path.GetFileNameWithoutExtension(model.MAT[matIndex - 1].FileName);
            }

            // Pipeline choice is driven by the per-face material's technique name
            // (MATR offset +4, matched against the engine's off_6FC800 table).
            // Names containing IMAGE_ALPHA opt into texture-alpha sampling
            // (1-bit cutout); everything else stays on the opaque pipeline.
            var useImageAlpha = matRecordIndex >= 0
                && matRecordIndex < model.Materials.Count
                && model.Materials[matRecordIndex].UsesImageAlpha;

            // Lightmap routing: only enable the LM pipeline when the IDXS group
            // actually selected a baked atlas AND the file ships LMUVs. Faces
            // with lmPage == -1 keep the existing opaque/alpha pipelines so the
            // unlit "skybox-style" non-LM batches don't pay for a 1×1 white
            // sample — and we don't lose the dynamic-light additive on them.
            var useLightmap = lmPage >= 0 && model.GVL.Count > 0;

            // Resolve the lightmap atlas filename via TXTD: lmPage is an offset
            // into the .ctx-suffixed segment, so the actual entry is
            // TextureNames[LightmapStart + lmPage]. The engine swaps the
            // extension to .dds/.tga at load time; we strip and look up .tga.
            string lmTextureName = null;
            if (useLightmap)
            {
                var txtdIdx = model.LightmapStart + lmPage;
                if (txtdIdx > 0 && txtdIdx < model.TextureNames.Count)
                    lmTextureName = Path.GetFileNameWithoutExtension(model.TextureNames[txtdIdx]);
                else
                    useLightmap = false; // out of range = no usable lightmap entry
            }

            Pipeline chosenPipeline;
            if (useLightmap)        chosenPipeline = _lightmappedPipeline;
            else if (useImageAlpha) chosenPipeline = null; // _pipeline (alpha-tested)
            else                    chosenPipeline = _opaquePipeline;

            var modelData = new ModelData
            {
                Name = $"8Model_Tex{matIndex}_Mat{matRecordIndex}_Lm{lmPage}",
                WorldMatrix = Matrix4x4.Identity,
                Pipeline = chosenPipeline
            };

            // Base texture (shared between LM and non-LM paths)
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

            // Lightmap texture (only on the LM path). Atlases are clamped at
            // edges to keep neighbouring tiles from bleeding through, and
            // sampled with the engine's standard linear filter — atlases are
            // already pre-baked at native resolution so anisotropic filtering
            // gains nothing.
            if (useLightmap)
            {
                ImageSharpTexture lmTx = ResourceSystem.FindResource(lmTextureName, "tga");
                if (lmTx != null)
                {
                    modelData.LightmapTextureView = factory.CreateTextureView(lmTx.CreateDeviceTexture(_graphicsDevice, factory));
                }
                else
                {
                    Console.WriteLine($"[SceneRenderer] Add8Model: Missing lightmap '{lmTextureName}' (lmPage {lmPage}, TXTD[{model.LightmapStart + lmPage}]); falling back to white");
                    modelData.LightmapTextureView = _whiteLightmapView;
                }

                modelData.WorldTextureSet = factory.CreateResourceSet(new ResourceSetDescription(
                    _worldTextureLayoutLM,
                    _worldBuffer,
                    modelData.SurfaceTextureView, _graphicsDevice.Aniso4xSampler,
                    modelData.LightmapTextureView, _graphicsDevice.LinearSampler));
            }
            else
            {
                modelData.WorldTextureSet = factory.CreateResourceSet(new ResourceSetDescription(
                    _worldTextureLayout, _worldBuffer, modelData.SurfaceTextureView, _graphicsDevice.Aniso4xSampler));
            }

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
                var capacity = (chunkEnd - faceCursor) * 3;
                var uniqueVertices = useLightmap ? null : new List<VertexPositionTextureNormal>(capacity);
                var lmVertices = useLightmap ? new List<VertexPositionTextureNormalLM>(capacity) : null;
                var newIndices = new List<ushort>(capacity);

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

                    // Use per-vertex normals from the file (same Y/Z un-swap as positions).
                    // Fall back to a flat normal if the chunk had no NRML sub-chunk.
                    Vector3 n0 = model.GVN.Count > idx0 ? new Vector3(model.GVN[idx0].X, model.GVN[idx0].Y, model.GVN[idx0].Z) : Vector3.Zero;
                    Vector3 n1 = model.GVN.Count > idx1 ? new Vector3(model.GVN[idx1].X, model.GVN[idx1].Y, model.GVN[idx1].Z) : Vector3.Zero;
                    Vector3 n2 = model.GVN.Count > idx2 ? new Vector3(model.GVN[idx2].X, model.GVN[idx2].Y, model.GVN[idx2].Z) : Vector3.Zero;

                    if (n0.LengthSquared() < 1e-8f || n1.LengthSquared() < 1e-8f || n2.LengthSquared() < 1e-8f)
                    {
                        Vector3 flat = Vector3.Cross(p1 - p0, p2 - p0);
                        if (flat.LengthSquared() < 1e-12f || float.IsNaN(flat.X) || float.IsNaN(flat.Y) || float.IsNaN(flat.Z))
                            flat = Vector3.UnitY;
                        else
                            flat = Vector3.Normalize(flat);
                        if (n0.LengthSquared() < 1e-8f) n0 = flat;
                        if (n1.LengthSquared() < 1e-8f) n1 = flat;
                        if (n2.LengthSquared() < 1e-8f) n2 = flat;
                    }
                    else
                    {
                        n0 = Vector3.Normalize(n0);
                        n1 = Vector3.Normalize(n1);
                        n2 = Vector3.Normalize(n2);
                    }

                    // Get UVs
                    Vector2 uv0 = model.GVT.Count > idx0 ? model.GVT[idx0] : Vector2.Zero;
                    Vector2 uv1 = model.GVT.Count > idx1 ? model.GVT[idx1] : Vector2.Zero;
                    Vector2 uv2 = model.GVT.Count > idx2 ? model.GVT[idx2] : Vector2.Zero;

                    // Per-vertex diffuse colors (DIFU sub-chunk). VanBurenFormats already
                    // decodes BGRA + half-brightness, so we just normalize 0..255 → 0..1.
                    Vector4 c0 = model.GVD.Count > idx0 ? ColorToVec4(model.GVD[idx0]) : Vector4.One;
                    Vector4 c1 = model.GVD.Count > idx1 ? ColorToVec4(model.GVD[idx1]) : Vector4.One;
                    Vector4 c2 = model.GVD.Count > idx2 ? ColorToVec4(model.GVD[idx2]) : Vector4.One;

                    if (useLightmap)
                    {
                        Vector2 lm0 = model.GVL.Count > idx0 ? model.GVL[idx0] : Vector2.Zero;
                        Vector2 lm1 = model.GVL.Count > idx1 ? model.GVL[idx1] : Vector2.Zero;
                        Vector2 lm2 = model.GVL.Count > idx2 ? model.GVL[idx2] : Vector2.Zero;
                        lmVertices.Add(new VertexPositionTextureNormalLM(p0, uv0, n0, c0, lm0));
                        lmVertices.Add(new VertexPositionTextureNormalLM(p1, uv1, n1, c1, lm1));
                        lmVertices.Add(new VertexPositionTextureNormalLM(p2, uv2, n2, c2, lm2));
                        ushort baseIdx = (ushort)(lmVertices.Count - 3);
                        newIndices.Add(baseIdx);
                        newIndices.Add((ushort)(baseIdx + 1));
                        newIndices.Add((ushort)(baseIdx + 2));
                    }
                    else
                    {
                        uniqueVertices.Add(new VertexPositionTextureNormal(p0, uv0, n0, c0));
                        uniqueVertices.Add(new VertexPositionTextureNormal(p1, uv1, n1, c1));
                        uniqueVertices.Add(new VertexPositionTextureNormal(p2, uv2, n2, c2));
                        ushort baseIdx = (ushort)(uniqueVertices.Count - 3);
                        newIndices.Add(baseIdx);
                        newIndices.Add((ushort)(baseIdx + 1));
                        newIndices.Add((ushort)(baseIdx + 2));
                    }
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
                        LightmapTextureView = modelData.LightmapTextureView,
                        WorldTextureSet = modelData.WorldTextureSet
                    };

                chunkData.Indices = newIndices;

                if (useLightmap)
                {
                    chunkData.LightmappedVertices = lmVertices;
                    chunkData.VertexBuffer = factory.CreateBuffer(new BufferDescription(
                        (uint)(lmVertices.Count * VertexPositionTextureNormalLM.SizeInBytes),
                        BufferUsage.VertexBuffer));
                    _graphicsDevice.UpdateBuffer(chunkData.VertexBuffer, 0, lmVertices.ToArray());
                }
                else
                {
                    chunkData.Vertices = uniqueVertices;
                    chunkData.VertexBuffer = factory.CreateBuffer(new BufferDescription(
                        (uint)(uniqueVertices.Count * VertexPositionTextureNormal.SizeInBytes),
                        BufferUsage.VertexBuffer));
                    _graphicsDevice.UpdateBuffer(chunkData.VertexBuffer, 0, uniqueVertices.ToArray());
                }

                chunkData.IndexBuffer = factory.CreateBuffer(new BufferDescription(
                    (uint)(chunkData.Indices.Count * sizeof(ushort)),
                    BufferUsage.IndexBuffer));
                _graphicsDevice.UpdateBuffer(chunkData.IndexBuffer, 0, chunkData.Indices.ToArray());

                _models.Add(chunkData);
                var vcount = useLightmap ? lmVertices.Count : uniqueVertices.Count;
                Console.WriteLine($"Added 8Model mesh part {matIndex} chunk {chunkIndex + 1}/{totalChunks}: {vcount} vertices, {newIndices.Count} indices. Texture: {textureName ?? "None"}, LM: {lmTextureName ?? "—"}");
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
        // Light uniform buffer = LightInfoHeader (48 bytes) + MaxLights × GpuLight (64 bytes each).
        var lightBufferSize = (uint)(Marshal.SizeOf<LightInfoHeader>() + MaxLights * Marshal.SizeOf<GpuLight>());
        _lightBuffer = factory.CreateBuffer(new BufferDescription(lightBufferSize, BufferUsage.UniformBuffer));

        var vert = new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(VertexCode), "main");
        var frag = new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(FragmentCode), "main");
        var fragOpaque = new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(OpaqueFragmentCode), "main");
        var shaders = factory.CreateFromSpirv(vert, frag);
        var opaqueShaders = factory.CreateFromSpirv(vert, fragOpaque);

        var projViewLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("LightBuffer", ResourceKind.UniformBuffer, ShaderStages.Fragment)));

        _worldTextureLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
            new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

        // Alpha-tested + alpha-blended pipeline: the fragment shader discards texels
        // below the texture-alpha threshold (so transparent texture areas don't write
        // depth and don't need sorting), while vertex-color alpha is still blended
        // through normally for smooth fades.
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
                            VertexElementFormat.Float3),
                        new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float4))
                },
                shaders),
            new[] { projViewLayout, _worldTextureLayout },
            _sceneFramebuffer.OutputDescription));

        // Opaque Pipeline: ignores texture alpha entirely. Used by the 8Model map
        // path where many TGAs are 32-bit with junk/zero alpha — running them through
        // the alpha-tested fragment shader would discard the entire map.
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
                            VertexElementFormat.Float3),
                        new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float4))
                },
                opaqueShaders),
            new[] { projViewLayout, _worldTextureLayout },
            _sceneFramebuffer.OutputDescription));

        _projViewSet = factory.CreateResourceSet(new ResourceSetDescription(
            projViewLayout, _projectionBuffer, _viewBuffer, _lightBuffer));

        // Seed the light buffer with the no-map fallback so the B3D-only
        // model viewer (which never calls AddMap) still has a usable sun.
        UploadMapLights(null, new Vector3(1f));

        // ---- Lightmapped .8 pipeline ----
        _whiteLightmapTexture = Create1x1TextureFromColor(_graphicsDevice, factory, 1f, 1f, 1f);
        _whiteLightmapView = factory.CreateTextureView(_whiteLightmapTexture);

        _worldTextureLayoutLM = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("WorldBuffer",      ResourceKind.UniformBuffer,    ShaderStages.Vertex),
            new ResourceLayoutElementDescription("SurfaceTexture",   ResourceKind.TextureReadOnly,  ShaderStages.Fragment),
            new ResourceLayoutElementDescription("SurfaceSampler",   ResourceKind.Sampler,          ShaderStages.Fragment),
            new ResourceLayoutElementDescription("LightmapTexture",  ResourceKind.TextureReadOnly,  ShaderStages.Fragment),
            new ResourceLayoutElementDescription("LightmapSampler",  ResourceKind.Sampler,          ShaderStages.Fragment)));

        var lmVert = new ShaderDescription(ShaderStages.Vertex,   Encoding.UTF8.GetBytes(LightmappedVertexCode),   "main");
        var lmFrag = new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(LightmappedFragmentCode), "main");
        var lmShaders = factory.CreateFromSpirv(lmVert, lmFrag);

        var lmVertexLayout = new VertexLayoutDescription(
            new VertexElementDescription("Position",       VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("TexCoords",      VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
            new VertexElementDescription("Normal",         VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("Color",          VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
            new VertexElementDescription("LightmapCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));

        _lightmappedPipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
            BlendStateDescription.SingleOverrideBlend,
            DepthStencilStateDescription.DepthOnlyLessEqual,
            new RasterizerStateDescription(FaceCullMode.None, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, false),
            PrimitiveTopology.TriangleList,
            new ShaderSetDescription(new[] { lmVertexLayout }, lmShaders),
            new[] { projViewLayout, _worldTextureLayoutLM },
            _sceneFramebuffer.OutputDescription));

        // Skinned pipeline. Reuses the existing fragment shader so lighting/texturing
        // stays consistent. Resource sets: 0=projView/light, 1=world+texture, 2=bones.
        _boneLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("BoneBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

        var skinnedVert = new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(SkinnedVertexCode), "main");
        var skinnedShaders = factory.CreateFromSpirv(skinnedVert, frag);
        var skinnedOpaqueShaders = factory.CreateFromSpirv(skinnedVert, fragOpaque);

        var skinnedVertexLayout = new VertexLayoutDescription(
            new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate,
                VertexElementFormat.Float3),
            new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate,
                VertexElementFormat.Float2),
            new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate,
                VertexElementFormat.Float3),
            new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate,
                VertexElementFormat.Float4),
            new VertexElementDescription("BoneWeights", VertexElementSemantic.TextureCoordinate,
                VertexElementFormat.Float4),
            new VertexElementDescription("BoneIndices", VertexElementSemantic.TextureCoordinate,
                VertexElementFormat.UInt4));

        _skinnedPipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
            BlendStateDescription.SingleAlphaBlend,
            DepthStencilStateDescription.DepthOnlyLessEqual,
            new RasterizerStateDescription(
                FaceCullMode.None,
                PolygonFillMode.Solid,
                FrontFace.CounterClockwise,
                true, false),
            PrimitiveTopology.TriangleList,
            new ShaderSetDescription(new[] { skinnedVertexLayout }, skinnedShaders),
            new[] { projViewLayout, _worldTextureLayout, _boneLayout },
            _sceneFramebuffer.OutputDescription));

        // Opaque counterpart of _skinnedPipeline. Used for skinned B3D submeshes whose
        // material technique doesn't carry IMAGE_ALPHA — the alpha-tested fragment
        // shader would otherwise discard texels on TGAs with junk/zero alpha and
        // make solid character parts render see-through.
        _opaqueSkinnedPipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
            BlendStateDescription.SingleOverrideBlend,
            DepthStencilStateDescription.DepthOnlyLessEqual,
            new RasterizerStateDescription(
                FaceCullMode.None,
                PolygonFillMode.Solid,
                FrontFace.CounterClockwise,
                true, false),
            PrimitiveTopology.TriangleList,
            new ShaderSetDescription(new[] { skinnedVertexLayout }, skinnedOpaqueShaders),
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

    // Lightmapped pipeline + its 5-binding resource layout. Set 1 carries:
    //   b0 WorldBuffer, b1 SurfaceTexture, b2 SurfaceSampler,
    //   b3 LightmapTexture, b4 LightmapSampler.
    private Pipeline _lightmappedPipeline;
    private ResourceLayout _worldTextureLayoutLM;

    // Shared 1×1 white texture bound to the lightmap stage for batches whose
    // IDXS group declared "no lightmap" (lmPage == -1). Sampling white through
    // the LM stage degrades the shader's `tex × diffuse × lm` to plain
    // `tex × diffuse`, so non-lightmapped faces in an otherwise lightmapped .8
    // mesh render correctly without a separate pipeline.
    private Texture _whiteLightmapTexture;
    private TextureView _whiteLightmapView;

    public void AddModel(B3DModel model, int materialIndex, List<Vector2>? uvs = null)
    {
        AddModel(model, materialIndex, null, null, uvs);
    }
    
    // Helper to keep similar signature but with matrix
    public void AddModel(B3DModel model, int materialIndex, Matrix4x4? worldMatrix, List<Vector2>? uvs = null)
    {
        AddModel(model, materialIndex, null, worldMatrix, uvs);
    }
    
    private static Vector4 ColorToVec4(System.Drawing.Color c) => new(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);

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
        var matRecord = model.MaterialDict[texId];
        var modelData = new ModelData
        {
            Name = texId,
            WorldMatrix = worldMatrix ?? Matrix4x4.Identity, // Use provided matrix or Identity
            // Default-opaque so meshes with junk-alpha TGAs don't render see-through.
            // Opt into the alpha pipeline only for materials the engine treats as
            // transparent: IMAGE_ALPHA cutout, VERTEX_ALPHA (e.g. hair), or any
            // non-OPAQUE blend state.
            Pipeline = matRecord.NeedsAlphaPipeline ? null : _opaquePipeline
        };

        var mtl_id = matRecord.TechniqueName;

        if (mtl_id is "NO_TEXTURE")
        {
            var color = matRecord.Mater.Diffuse;
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
        
        // Load model vertices and indices. Per-vertex color (RGBA) drives blending
        // for VERTEX_ALPHA techniques — without it, hair/etc. samples alpha=1 and
        // renders fully opaque.
        for (var i = 0; i < model.Model_Vertex_Position.Count; i++)
        {
            var color = i < model.Model_Vertex_Color.Count ? model.Model_Vertex_Color[i] : Vector4.One;
            modelData.Vertices.Add(
                new VertexPositionTextureNormal(
                    model.Model_Vertex_Position[i],
                    uvs[i],
                    model.Model_Vertex_Normal[i],
                    color));
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

    public void Reset()
    {
        foreach (var model in _models)
        {
            model.VertexBuffer?.Dispose();
            model.IndexBuffer?.Dispose();
            model.SurfaceTexture?.Dispose();
            model.SurfaceTextureView?.Dispose();
            // Don't dispose the shared white-fallback view — it's owned by the
            // renderer for the lifetime of the SceneRenderer and reused across
            // every "no LM" batch.
            if (model.LightmapTextureView != null && model.LightmapTextureView != _whiteLightmapView)
                model.LightmapTextureView.Dispose();
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
        _lightHeader = new LightInfoHeader();
        Array.Clear(_gpuLights);
        UploadMapLights(null, new Vector3(1f));
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
            if (model.LightmapTextureView != null && model.LightmapTextureView != _whiteLightmapView)
                model.LightmapTextureView.Dispose();
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

        cl.Begin();
        cl.SetFramebuffer(_sceneFramebuffer);
        cl.ClearColorTarget(0, RgbaFloat.Black);
        cl.ClearDepthStencil(1f);

        cl.UpdateBuffer(_projectionBuffer, 0, _camera.ProjectionMatrix);
        cl.UpdateBuffer(_viewBuffer, 0, _camera.ViewMatrix);
        _lightHeader.CameraPos = new Vector4(_camera.Position, 0f);
        // Header at offset 0, lights array immediately after. Two uploads
        // because Veldrid's UpdateBuffer doesn't take a struct + tail.
        cl.UpdateBuffer(_lightBuffer, 0, _lightHeader);
        cl.UpdateBuffer(_lightBuffer, (uint)Marshal.SizeOf<LightInfoHeader>(), _gpuLights);

        void draw(ModelData model)
        {
            cl.SetPipeline(model.Pipeline ?? _pipeline);
            cl.SetGraphicsResourceSet(0, _projViewSet);
            cl.UpdateBuffer(_worldBuffer, 0, model.WorldMatrix);
            cl.SetGraphicsResourceSet(1, model.WorldTextureSet);
            cl.SetVertexBuffer(0, model.VertexBuffer);
            cl.SetIndexBuffer(model.IndexBuffer, IndexFormat.UInt16);
            cl.DrawIndexed((uint)model.Indices.Count, 1, 0, 0, 0);
        }

        // Render each model. Opaque + lightmapped first (depth-write), then
        // alpha-blended last so partial transparencies composite over them.
        foreach (var model in _models.Where(m => m.Pipeline == _opaquePipeline || m.Pipeline == _lightmappedPipeline)) draw(model);
        foreach (var model in _models.Where(m => m.Pipeline != _opaquePipeline && m.Pipeline != _lightmappedPipeline)) draw(model);

        // Skinned models: advance time, sample animation, bridge into B3D space, upload
        // bone palette, draw. Each model owns its own time accumulator + cached pose so
        // characters tick independently.
        // For the skeleton overlay we'd otherwise re-draw identical lines for every
        // sub-model on the same character (head/body/hair/etc. all share a skeleton).
        // Tracking poses we've already drawn this frame dedups them by reference.
        
        var drawnPoses = DrawSkeletonOverlay ? new HashSet<GrannyFormats.SkeletonPose>() : null;
        
        void drawSkinned(SkinnedModelData model)
        {
            AdvanceAndUpload(cl, model, deltaTime);

            cl.SetPipeline(model.Pipeline ?? _skinnedPipeline);
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
        
        foreach (var model in _skinnedModels.Where(m => m.Pipeline == _opaqueSkinnedPipeline)) drawSkinned(model);
        foreach (var model in _skinnedModels.Where(m => m.Pipeline != _opaqueSkinnedPipeline)) drawSkinned(model);

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
            foreach(var v in m.LightmappedVertices)
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

    // Maximum number of LGTR records we forward to the shader. The biggest
    // ship-with map (Vault, Shelter) has 14; rounding up to 16 covers them
    // and stays well clear of fxc's flakier behaviour with very large
    // cbuffer arrays. If a map has more than this they're truncated by
    // AddMap. Keep in sync with FragmentCode's MAX_LIGHTS.
    private const int MaxLights = 16;

    // Per-light data laid out as four vec4s. The GLSL struct in FragmentCode
    // unpacks this layout — keep the two in sync. We pack as vec4-only
    // (instead of mixed vec3+scalar) because SPIRV-Cross → HLSL → FXC
    // crashes with an SEH on the mixed layout.
    //   v0: xyz = Position,  w = Type (cast to int in shader)
    //   v1: xyz = Color,     w = Radius
    //   v2: xyz = Direction, w = FalloffStart
    //   v3: x = ConeInnerCos, y = ConeOuterCos, z = HasDirection (0/1), w = unused
    [StructLayout(LayoutKind.Sequential)]
    private struct GpuLight
    {
        public Vector4 PositionType;
        public Vector4 ColorRadius;
        public Vector4 DirectionFalloff;
        public Vector4 ConeAndFlags;
    }

    // Header that precedes the Lights[] array in the same uniform buffer.
    // Layout matches the GLSL `LightInfo` block — three leading vec4s of
    // globals, then MaxLights × sizeof(GpuLight) bytes of array.
    [StructLayout(LayoutKind.Sequential)]
    private struct LightInfoHeader
    {
        public Vector4 Header;            // x = LightCount
        public Vector4 AmbientCamera;     // xyz = AmbientColor
        public Vector4 CameraPos;

        public LightInfoHeader()
        {
            Header = Vector4.Zero;
            AmbientCamera = new Vector4(0.3f, 0.3f, 0.3f, 0f);
            CameraPos = Vector4.Zero;
        }
    }

    // Backing storage for the uniform buffer. Header lives at offset 0;
    // Lights[] follows immediately. We keep them as separate fields so we
    // can update them independently (e.g. bump CameraPos every frame
    // without rewriting the light array).
    private LightInfoHeader _lightHeader = new();
    private readonly GpuLight[] _gpuLights = new GpuLight[MaxLights];

    public class ModelData
    {
        public List<VertexPositionTextureNormal> Vertices { get; set; } = new();
        // Lightmapped batches use this list instead of `Vertices`. We keep both
        // because the two formats have different stride (48 vs 56 bytes) and
        // share no Veldrid pipeline. Only one of the two is non-empty for any
        // given ModelData; FocusCamera reads positions out of whichever is
        // populated.
        public List<VertexPositionTextureNormalLM> LightmappedVertices { get; set; } = new();
        public bool IsLightmapped => LightmappedVertices.Count > 0;
        public List<ushort> Indices { get; set; } = new();
        public DeviceBuffer VertexBuffer { get; set; }
        public DeviceBuffer IndexBuffer { get; set; }
        public Texture SurfaceTexture { get; set; }
        public TextureView SurfaceTextureView { get; set; }
        // Set only on lightmapped batches. The 1×1 white fallback is shared
        // across all batches with lmPage == -1 — see _whiteLightmapView.
        public TextureView LightmapTextureView { get; set; }
        public ResourceSet WorldTextureSet { get; set; }
        public Pipeline Pipeline { get; set; }
        public Matrix4x4 WorldMatrix { get; set; } = Matrix4x4.Identity;
        public string Name { get; set; } = "";
    }
    
    // Per-vertex RGBA color. Normal is at location 2, Color at 3.
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionTextureNormal
    {
        public Vector3 Position;
        public Vector2 TexCoords;
        public Vector3 Normal;
        public Vector4 Color;

        public const uint SizeInBytes = 12 + 8 + 12 + 16; // 48 bytes

        public VertexPositionTextureNormal(Vector3 pos, Vector2 uv, Vector3 normal)
            : this(pos, uv, normal, Vector4.One) { }

        public VertexPositionTextureNormal(Vector3 pos, Vector2 uv, Vector3 normal, Vector4 color)
        {
            Position = pos;
            TexCoords = uv;
            Normal = normal;
            Color = color;
        }
    }

    // Lightmapped variant: same Position/UV/Normal/Color as the base vertex,
    // plus a second float2 for the LMUV channel from .8 VTXB. Used only by the
    // _lightmappedPipeline; ordinary B3D and non-LM .8 batches keep the 48-byte
    // VertexPositionTextureNormal so we don't pay 8 bytes/vertex everywhere.
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionTextureNormalLM
    {
        public Vector3 Position;
        public Vector2 TexCoords;
        public Vector3 Normal;
        public Vector4 Color;
        public Vector2 LightmapCoords;

        public const uint SizeInBytes = 12 + 8 + 12 + 16 + 8; // 56 bytes

        public VertexPositionTextureNormalLM(Vector3 pos, Vector2 uv, Vector3 normal, Vector4 color, Vector2 lmUv)
        {
            Position = pos;
            TexCoords = uv;
            Normal = normal;
            Color = color;
            LightmapCoords = lmUv;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionTextureNormalSkinned
    {
        public Vector3 Position;
        public Vector2 TexCoords;
        public Vector3 Normal;
        public Vector4 Color;
        public Vector4 BoneWeights;
        public uint BoneIndex0, BoneIndex1, BoneIndex2, BoneIndex3;

        public const uint SizeInBytes = 12 + 8 + 12 + 16 + 16 + 16; // 80 bytes

        public VertexPositionTextureNormalSkinned(
            Vector3 pos, Vector2 uv, Vector3 normal,
            Vector4 weights, uint i0, uint i1, uint i2, uint i3)
            : this(pos, uv, normal, Vector4.One, weights, i0, i1, i2, i3) { }

        public VertexPositionTextureNormalSkinned(
            Vector3 pos, Vector2 uv, Vector3 normal, Vector4 color,
            Vector4 weights, uint i0, uint i1, uint i2, uint i3)
        {
            Position = pos;
            TexCoords = uv;
            Normal = normal;
            Color = color;
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
        public Pipeline Pipeline { get; set; }
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
        var matRecord = b3d.MaterialDict[texId];
        var modelData = new SkinnedModelData
        {
            Name = texId,
            WorldMatrix = worldMatrix ?? Matrix4x4.Identity,
            GrSkeleton = gr2Skeleton,
            GrAnim = gr2Anim,
            B3dToGr2 = b3dToGr2,
            UploadMatrices = new Matrix4x4[MaxBones],
            // Mirror the static B3D path: alpha pipeline whenever the material wants
            // texture-alpha, vertex-alpha (hair), or non-OPAQUE blending.
            Pipeline = matRecord.NeedsAlphaPipeline ? null : _opaqueSkinnedPipeline
        };

        var mtl_id = matRecord.TechniqueName;
        if (mtl_id is "INVISIBLE") return;

        if (mtl_id is "NO_TEXTURE")
        {
            var color = matRecord.Mater.Diffuse;
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
            var color = i < b3d.Model_Vertex_Color.Count ? b3d.Model_Vertex_Color[i] : Vector4.One;
            modelData.Vertices.Add(new VertexPositionTextureNormalSkinned(
                b3d.Model_Vertex_Position[i],
                i < uvs.Count ? uvs[i] : Vector2.Zero,
                b3d.Model_Vertex_Normal[i],
                color,
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