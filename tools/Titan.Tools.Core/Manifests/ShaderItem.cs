using System.Text.Json.Serialization;
using Titan.Shaders.Windows;

namespace Titan.Tools.Core.Manifests;

public class ShaderItem : ManifestItem
{
    public required string Path { get; init; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ShaderModels ShaderModel { get; set; }
    public required string EntryPoint { get; set; }
}
