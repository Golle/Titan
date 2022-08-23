using System.Text.Json.Serialization;

namespace Titan.Tools.ManifestBuilder.Models;

public class TextureItem : ManifestItem
{
    public required string Path { get; init; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TextureType Type { get; set; }
}
