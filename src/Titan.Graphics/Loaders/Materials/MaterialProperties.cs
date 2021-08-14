using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Graphics.Loaders.Materials
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MaterialProperties
    {
        public float Alpha;
        public float Transparency;
        public float Shininess;
        public float Illumination;

        public Color AmbientColor;
        public Color DiffuseColor;
        public Color SpecularColor;
        public Color EmissiveColor;

        public Handle<Texture> DiffuseMap;
        public Handle<Texture> BumpMap;
        public Handle<Texture> AmbientMap;
        public Handle<Texture> AlphaMap;
    }
}
