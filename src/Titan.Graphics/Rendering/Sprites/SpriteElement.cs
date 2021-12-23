using Titan.Core;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Graphics.Rendering.Sprites
{
    public struct SpriteElement
    {
        public Handle<Texture> Texture;
        public uint StartIndex;
        public uint Count;
    }
}