using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Titan.Tools.ManifestBuilder.Models;

public class Manifest
{

    [JsonIgnore]
    public string? Path { get; set; }
    public required string Name { get; set; }
    public List<TextureItem> Textures { get; set; } = new();
    public List<ModelItem> Models { get; set; } = new();
}
