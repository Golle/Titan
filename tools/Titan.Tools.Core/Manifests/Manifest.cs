using System.Collections;
using System.Text.Json.Serialization;

namespace Titan.Tools.Core.Manifests;

public class Manifest
{

    [JsonIgnore]
    public string? Path { get; set; }
    public int Order { get; set; }
    public required string Name { get; set; }
    public List<TextureItem> Textures { get; set; } = new();
    public List<ModelItem> Models { get; set; } = new();
    public List<MaterialItem> Materials { get; set; } = new();
    public List<ShaderItem> Shaders { get; set; } = new();
}
