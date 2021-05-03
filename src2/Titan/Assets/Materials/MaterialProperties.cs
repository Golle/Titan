using Titan.Core;
using Titan.Graphics;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Assets.Materials
{
    public struct MaterialProperties
    {
        public Color DiffuseColor;
        public Handle<Texture> DiffuseMap;
        public Handle<Texture> BumpMap;
    }
}
