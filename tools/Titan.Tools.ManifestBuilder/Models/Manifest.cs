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

public class TextureItem
{
    public required string Path { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TextureType Type { get; set; }
}

public class ModelItem
{
    public required string Path { get; set; }
}

public enum TextureType
{
    Unknown,
    PNG,
    BMP,
    JPG
}
