using Titan.Tools.ManifestBuilder.DataTemplates.Attributes;

namespace Titan.Tools.ManifestBuilder.Models;

public class ManifestItem
{
    [EditorString]
    public required string Name { get; set; }
}
