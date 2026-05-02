using System.Numerics;

namespace VBLauncher;

// Maps critter altas UVs to the base UV for that texture, removing the need to stitch the atlas.
public static class UVAtlasMapper
{
    private const float AtlasSize = 512f;

    public static Vector2 ToBaseUV(Vector2 uv) 
        => TransformUV(uv,  0f,   0f,   256f, 256f);

    public static Vector2 ToHeaUV(Vector2 uv)  
        => TransformUV(uv, 384f,   0f,   128f, 128f);

    public static Vector2 ToShoUV(Vector2 uv)  
        => TransformUV(uv, 256f,   0f,   128f, 128f);

    public static Vector2 ToHaiUV(Vector2 uv)  
        => TransformUV(uv, 256f, 128f,   128f, 128f);

    public static Vector2 ToVanUV(Vector2 uv)  
        => TransformUV(uv, 320f, 128f,   128f, 128f);

    public static Vector2 ToHanUV(Vector2 uv)  
        => TransformUV(uv, 384f, 128f,   128f, 128f);

    public static Vector2 ToBeaUV(Vector2 uv) 
        => TransformUV(uv, 256f, 192f,    32f,  32f);

    public static Vector2 ToEyeUV(Vector2 uv)  
        => TransformUV(uv, 320f, 192f,    64f,  64f);

    public static Vector2 ToPonUV(Vector2 uv)  
        => TransformUV(uv, 288f, 224f,    32f,  32f);

    public static Vector2 ToMusUV(Vector2 uv)  
        => TransformUV(uv, 256f, 224f,    32f,  32f);

    public static Vector2 ToBodUV(Vector2 uv)  
        => TransformUV(uv,   0f, 256f,   256f, 256f);

    public static Vector2 ToBacUV(Vector2 uv)  
        => TransformUV(uv, 256f, 256f,   128f, 128f);

    public static Vector2 ToFeeUV(Vector2 uv)  
        => TransformUV(uv, 384f, 256f,   128f, 128f);

    public static Vector2 Default(Vector2 uv) => uv;

    private static Vector2 TransformUV(Vector2 uv, float offsetX, float offsetY, float w, float h)
    {
        return new Vector2(
            (uv.X * AtlasSize - offsetX) / w,
            (uv.Y * AtlasSize - offsetY) / h
        );
    }
}
