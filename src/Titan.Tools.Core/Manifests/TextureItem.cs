using System.Text.Json.Serialization;

namespace Titan.Tools.Core.Manifests;

public class TextureItem : ManifestItemWithPath
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TextureType Type { get; set; }
}
