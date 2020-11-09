namespace Titan.Graphics.Materials
{
    public record MaterialConfiguration(string Name, string Diffuse, string Ambient, string Specular, string Emissive, string NormalMap, bool IsTextured, bool IsTransparent);
    public interface IMaterialsLoader
    {
        MaterialConfiguration[] LoadMaterials(string filename);
    }
}
