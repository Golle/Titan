using System.Text.Json.Serialization;
using Titan.Tools.Core.Shaders;

namespace Titan.Tools.Core.Manifests;

public class ShaderItem : ManifestItemWithPath
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ShaderModels ShaderModel { get; set; }

    public string EntryPoint { get; set; } = string.Empty;
}
