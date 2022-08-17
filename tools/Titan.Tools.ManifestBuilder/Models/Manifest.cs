using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Titan.Tools.ManifestBuilder.Models;

internal class Manifest
{
    public required string Name { get; set; }
    public List<TextureItem> Textures { get; set; } = new();
    public List<ModelItem> Models { get; set; } = new();
}

internal class TextureItem
{
    public required string Path { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TextureType Type { get; set; }
}

internal class ModelItem
{
    public required string Path { get; set; }
}


internal enum TextureType
{
    Unknown,
    PNG,
    BMP,
    JPG
}
