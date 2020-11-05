using Titan.Graphics.D3D11;

namespace Titan.Graphics.Materials
{
    // TODO: should this be a class?
    public class Material
    {
        public Color Diffuse;
        public Color Ambient;
        public Color Specular;
        public Color Emissive;

        public bool IsTextured;
        public bool HasNormalMap;
        public bool IsTransparent;

        public ShaderResourceView Texture;
    }
}
