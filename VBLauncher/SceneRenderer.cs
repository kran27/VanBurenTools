using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
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
    
    public bool AllowInput = false;
    public bool Initialized = false;

    public uint Height;
    public uint Width;
    
    public IntPtr ImGuiBinding;
    
    private List<ModelData> _models = new List<ModelData>();
    private Camera _camera;

    private DeviceBuffer _lightBuffer;
    private Pipeline _pipeline;
    
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

    void AddModelIfValid(string modelName, string textureFileName, Func<Vector2, Vector2> uvMapper, Matrix4x4? worldMatrix = null)
    {
        if (modelName.EndsWith('_')) return;
        B3DModel model = ResourceSystem.Resources["Critters"]["b3d"][modelName];
        List<Vector2> uvs = model.Model_Vertex_Texcoords.Select(uvMapper).ToList();
        AddB3DModel(model, string.IsNullOrEmpty(textureFileName) ? null : textureFileName, worldMatrix, uvs);
    }

    public void AddARM(ARM arm, string modelPrefix = "HumMNor", Matrix4x4? worldMatrix = null)
    {
        AddModelIfValid(modelPrefix + '_' + arm.GITM.Bac.Model, arm.GITM.Bac.Tex, UVAtlasMapper.ToBacUV, worldMatrix);
        AddModelIfValid(modelPrefix + '_' + arm.GITM.Bod.Model, arm.GITM.Bod.Tex, UVAtlasMapper.ToBodUV, worldMatrix);
        AddModelIfValid(modelPrefix + '_' + arm.GITM.Eye.Model, arm.GITM.Eye.Tex, UVAtlasMapper.ToEyeUV, worldMatrix);
        AddModelIfValid(modelPrefix + '_' + arm.GITM.Fee.Model, arm.GITM.Fee.Tex, UVAtlasMapper.ToFeeUV, worldMatrix);
        AddModelIfValid(modelPrefix + '_' + arm.GITM.Han.Model, arm.GITM.Han.Tex, UVAtlasMapper.ToHanUV, worldMatrix);
        AddModelIfValid(modelPrefix + '_' + arm.GITM.Hea.Model, arm.GITM.Hea.Tex, UVAtlasMapper.ToHeaUV, worldMatrix);
        AddModelIfValid(modelPrefix + '_' + arm.GITM.Sho.Model, arm.GITM.Sho.Tex, UVAtlasMapper.ToShoUV, worldMatrix);
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
                if (item.ToLower().Split('.')[1] is not "arm") continue;
                ARM arm = ResourceSystem.FindResource(item);
                // we check the conditional flags so we don't overwrite a previously set hide flag
                if (arm.GITM.hHai) hideHair = true;
                if (arm.GITM.hBea) hideBeard = true;
                if (arm.GITM.hMus) hideMoustache = true;
                if (arm.GITM.hEye) hideEye = true;
                if (arm.GITM.hPon) hidePonytail = true;
                if (arm.GITM.hVan) hideVanity = true;
                
                AddARM(arm, baseModelName, worldMatrix);
                // idk how to handle in-hand items yet, they aren't a part of armour anyway so ignored.
            }

            AddModelIfValid(baseModelName + '_' + crt.GCRE.Hea.Model, crt.GCRE.Hea.Tex, UVAtlasMapper.ToHeaUV, worldMatrix);
            if (!hideEye)
                AddModelIfValid(baseModelName + '_' + crt.GCRE.Eye.Model, crt.GCRE.Eye.Tex, UVAtlasMapper.ToEyeUV, worldMatrix);
            if (!hideHair)
                AddModelIfValid(baseModelName + '_' + crt.GCRE.Hai.Model, crt.GCRE.Hai.Tex, UVAtlasMapper.ToHaiUV, worldMatrix);
            if (!hidePonytail)
                AddModelIfValid(baseModelName + '_' + crt.GCRE.Pon.Model, crt.GCRE.Pon.Tex, UVAtlasMapper.ToPonUV, worldMatrix);
            if (!hideMoustache)
                AddModelIfValid(baseModelName + '_' + crt.GCRE.Mus.Model, crt.GCRE.Mus.Tex, UVAtlasMapper.ToMusUV, worldMatrix);
            if (!hideBeard)
                AddModelIfValid(baseModelName + '_' + crt.GCRE.Bea.Model, crt.GCRE.Bea.Tex, UVAtlasMapper.ToBeaUV, worldMatrix);
            AddModelIfValid(baseModelName + '_' + crt.GCRE.Han.Model, crt.GCRE.Han.Tex, UVAtlasMapper.ToHanUV, worldMatrix);
            if (!hideVanity)
                AddModelIfValid(baseModelName + '_' + crt.GCRE.Van.Model, crt.GCRE.Van.Tex, UVAtlasMapper.ToVanUV, worldMatrix);
            AddModelIfValid(baseModelName + '_' + crt.GCRE.Bod.Model, crt.GCRE.Bod.Tex, UVAtlasMapper.ToBodUV, worldMatrix);
            AddModelIfValid(baseModelName + '_' + crt.GCRE.Sho.Model, crt.GCRE.Sho.Tex, UVAtlasMapper.ToShoUV, worldMatrix);
            AddModelIfValid(baseModelName + '_' + crt.GCRE.Fee.Model, crt.GCRE.Fee.Tex, UVAtlasMapper.ToFeeUV, worldMatrix);
            AddModelIfValid(baseModelName + '_' + crt.GCRE.Bac.Model, crt.GCRE.Bac.Tex, UVAtlasMapper.ToBacUV, worldMatrix);

            var baseUVs = baseModel.Model_Vertex_Texcoords.Select(UVAtlasMapper.ToBaseUV).ToList();
            AddB3DModel(baseModel, crt.EEN2.EEOV.s4, worldMatrix, baseUVs);
        }
        else
        {
            AddB3DModel(baseModel, crt.EEN2.EEOV.s4, worldMatrix);
        }
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
                else 
                {
                    Console.WriteLine($"Could not find map geometry for {map.EMAP.s1} (checked .b3d and .8 for key {searchName})");
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
                // Entities
                // User reported map/entities are vertical with Z/Y swap. 
                // This implies source is already Y-up (or we shouldn't swap).
                // Reverting to direct X, Y, Z translation.
                // Assuming W is rotation around Y (Yaw).
                var transform = Matrix4x4.CreateRotationY((float)ent.l.W) * 
                                Matrix4x4.CreateTranslation((float)ent.l.X, (float)ent.l.Y, (float)ent.l.Z);

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
        }

        // Add Default PC
        foreach (var ep in map.EMEP)
        {
            try {
                var crt = ResourceSystem.FindResource("default_pc.crt");
                if (crt != null)
                {
                    var transform = Matrix4x4.CreateRotationY((float)ep.r) * 
                                    Matrix4x4.CreateTranslation(ep.p.X, ep.p.Y, ep.p.Z);
                    AddCRT(crt, transform);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding default PC to map: {ex.Message}");
            }
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
        
        // Populate indices (actually just collecting faces now)
        // We will just store the FACE index (raw index in IDX) so we can look it up later
        // because we are going to explode the vertices anyway.
        var materialFaces = new Dictionary<int, List<int>>();
        for (int i = 0; i <= model.MAT.Count; i++) materialFaces[i] = new List<int>();

        for (int i = 0; i < model.IDX.Count; i++)
        {
            var mdxindex = model.MDX[i];
            if (mdxindex == 0) mdxindex = model.MAT.Count;
            materialFaces[mdxindex].Add(i);
        }

        Console.WriteLine($"[SceneRenderer] Add8Model: GVP Count {model.GVP.Count}, IDX Count {model.IDX.Count}, MAT Count {model.MAT.Count}");
        Console.WriteLine($"[SceneRenderer] Add8Model: Material Faces Breakdown: {string.Join(", ", materialFaces.Where(k => k.Value.Count > 0).Select(k => $"Mat {k.Key}: {k.Value.Count} faces"))}");

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

            var uniqueVertices = new List<VertexPositionTextureNormal>();
            var newIndices = new List<ushort>();
            
            foreach (var faceIdx in faceIndices)
            {
                var face = model.IDX[faceIdx];
                int idx0 = face[0];
                int idx1 = face[1];
                int idx2 = face[2];
                
                // Get Positions, Swap Y/Z for Y-up, and Apply Scaling (10x to match B3D format ver 1)
                // Get Positions and Un-Swap Y/Z (Undo VanBurenFormats swap)
                // VanBurenFormats does: new Vector3(x, z, y)
                // We want: (x, y, z)
                // So we take (v.X, v.Z, v.Y) to reverse it.
                Vector3 p0 = new Vector3(model.GVP[idx0].X, model.GVP[idx0].Z, model.GVP[idx0].Y);
                Vector3 p1 = new Vector3(model.GVP[idx1].X, model.GVP[idx1].Z, model.GVP[idx1].Y);
                Vector3 p2 = new Vector3(model.GVP[idx2].X, model.GVP[idx2].Z, model.GVP[idx2].Y);
                
                // Calculate Flat Normal
                // Note: v1/v2 are based on swapped indices from VanBurenFormats (a, c, b).
                // This swaps the winding, so Cross(v1, v2) points Inwards/Down.
                // We need to swap the Cross product operands to get the Outwards/Up normal.
                Vector3 v1 = p1 - p0;
                Vector3 v2 = p2 - p0;
                Vector3 normal = Vector3.Normalize(Vector3.Cross(v2, v1));
                
                // Check if Normal is valid (non-NaN)
                if (float.IsNaN(normal.X) || float.IsNaN(normal.Y) || float.IsNaN(normal.Z))
                {
                    normal = Vector3.UnitY;
                }

                // Get UVs
                Vector2 uv0 = model.GVT.Count > idx0 ? model.GVT[idx0] : Vector2.Zero;
                Vector2 uv1 = model.GVT.Count > idx1 ? model.GVT[idx1] : Vector2.Zero;
                Vector2 uv2 = model.GVT.Count > idx2 ? model.GVT[idx2] : Vector2.Zero;

                // Add vertices
                uniqueVertices.Add(new VertexPositionTextureNormal(p0, uv0, normal));
                uniqueVertices.Add(new VertexPositionTextureNormal(p1, uv1, normal));
                uniqueVertices.Add(new VertexPositionTextureNormal(p2, uv2, normal));
                
                // Add Indices (0, 1, 2 relative to this batch)
                // Actually we just append sequential indices since we explode everything
                ushort baseIdx = (ushort)(uniqueVertices.Count - 3);
                newIndices.Add(baseIdx);
                newIndices.Add((ushort)(baseIdx + 1));
                newIndices.Add((ushort)(baseIdx + 2));
            }
            
            modelData.Vertices = uniqueVertices;
            modelData.Indices = newIndices;

            // Create buffers
            modelData.VertexBuffer = factory.CreateBuffer(new BufferDescription(
                (uint)(modelData.Vertices.Count * VertexPositionTextureNormal.SizeInBytes),
                BufferUsage.VertexBuffer));
            _graphicsDevice.UpdateBuffer(modelData.VertexBuffer, 0, modelData.Vertices.ToArray());

            modelData.IndexBuffer = factory.CreateBuffer(new BufferDescription(
                (uint)(modelData.Indices.Count * sizeof(ushort)),
                BufferUsage.IndexBuffer));
            _graphicsDevice.UpdateBuffer(modelData.IndexBuffer, 0, modelData.Indices.ToArray());

            _models.Add(modelData);
            Console.WriteLine($"Added 8Model mesh part {matIndex}: {uniqueVertices.Count} vertices, {newIndices.Count} indices. Texture: {textureName ?? "None"}");
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
        return _models.Count == 0;
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
        _ignoredZones.Clear();
    }

    public int GetModelCount()
    {
        return _models.Count;
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
        if (!Initialized || _models.Count == 0)
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
        
        cl.ResolveTexture(_msaaColorTarget, _sceneColorTarget);
        cl.End();
        _graphicsDevice.SubmitCommands(cl);
    }

    public void FocusCamera()
    {
        if (_models.Count == 0) return;
        
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
}