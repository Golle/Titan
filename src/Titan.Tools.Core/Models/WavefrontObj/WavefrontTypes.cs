using System.Diagnostics;
using System.Numerics;
using Titan.Core.Maths;

namespace Titan.Tools.Core.Models.WavefrontObj;

public record WavefrontObjectResult(WavefrontMaterial[] Materials, WavefrontObject[] Objects, Vector3[] Vertices, Vector3[] Normals, Vector2[] Textures);

[DebuggerDisplay("Name={Name} (Groups={Groups.Count})")]
public class WavefrontObject
{
    public string Name { get; }
    public List<WavefrontGroup> Groups { get; } = new();
    public WavefrontObject(string name)
    {
        Name = name;
    }
}

[DebuggerDisplay("Name={Name} (Vertices={VertexIndices.Count}, Textures={TextureIndices.Count}, Normals={NormalIndices.Count})")]
public class WavefrontGroup
{
    public string Name { get; }
    public List<int> VertexIndices { get; } = new();
    public List<int> TextureIndices { get; } = new();
    public List<int> NormalIndices { get; } = new();
    public int MaterialIndex { get; set; } = -1;
    public int Smoothing { get; set; } = -1;
    public WavefrontGroup(string name)
    {
        Name = name;
    }
}

public record WavefrontMaterial(string Name)
{
    public Color Ambient { get; init; } = new(0.2f, 0.2f, 0.2f);
    public Color Diffuse { get; init; } = new(0.8f, 0.8f, 0.8f);
    public Color Specular { get; init; } = new(1.0f, 1.0f, 1.0f);
    public Color Emissive { get; init; } = default;
    public float Alpha { get; init; } = 1.0f;
    public float SpecularExponent { get; init; }
    public float OpticalDensity { get; init; }
    public int Illumination { get; init; }
    public string? DiffuseTexture { get; init; }
    public string? AmbientTexture { get; init; }
    public string? SpecularTexture { get; init; }
    public string? DisplacementTexture { get; init; }
}
