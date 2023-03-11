namespace Titan.Tools.Core.Manifests;

public class MaterialItem : ManifestItem
{
    public float Alpha { get; set; } = 1.0f;
    public float Transparency { get; set; } = 0.0f;
    public float Shininess { get; set; } = 1.0f;
    public float Illumination { get; set; } = 1.0f;

    public string AmbientColor { get; set; } = string.Empty;
    public string DiffuseColor { get; set; } = string.Empty;
    public string SpecularColor { get; set; } = string.Empty;
    public string EmissiveColor { get; set; } = string.Empty;
}
