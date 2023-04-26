using System.Runtime.CompilerServices;

namespace Titan.Tools.Core.Models.WavefrontObj.Lexer;

internal record struct TypeDecl(TokenType Type, string Value);
internal static class BuiltInTypes
{
    private static readonly TypeDecl[] _types;
    static BuiltInTypes()
    {
        _types = new TypeDecl[]
        {
            new(TokenType.VertexPosition, "v"),
            new(TokenType.VertexNormal, "vn"),
            new(TokenType.TextureCoordinates, "vt"),
            new(TokenType.ParameterSpace, "vp"),
            new(TokenType.Line, "l"),
            new(TokenType.Face, "f"),
            new(TokenType.Object, "o"),
            new(TokenType.Group, "g"),
            new(TokenType.Smooth, "s"),
            new(TokenType.UseMaterial, "usemtl"),
            new(TokenType.MaterialLib, "mtllib"),
            new(TokenType.NewMaterial, "newmtl"),
            new(TokenType.AmbientColor, "Ka"),
            new(TokenType.DiffuseColor, "Kd"),
            new(TokenType.EmissiveColor, "Ke"),
            new(TokenType.SpecularColor, "Ks"),
            new(TokenType.DiffuseTexture, "map_Kd"),
            new(TokenType.AmbientTexture, "map_Ka"),
            new(TokenType.SpecularTexture, "map_Ks"),
            new(TokenType.DisplacementTexture, "map_Disp"),
            new(TokenType.SpecularExponent, "Ns"),
            new(TokenType.Transparent, "d"),
            new(TokenType.TransparentInverted, "Tr"),
            new(TokenType.TransmissionFilter, "Tf"),
            new(TokenType.OpticalDensity, "Ni"),
            new(TokenType.Illumination, "illum")
        };
    }

    public static bool TryGetType(ReadOnlySpan<char> value, out TypeDecl typeOut)
    {
        Unsafe.SkipInit(out typeOut);
        foreach (var type in _types)
        {
            if (type.Value.AsSpan().Equals(value, StringComparison.Ordinal))
            {
                typeOut = type;
                return true;
            }
        }
        return false;
    }
}
