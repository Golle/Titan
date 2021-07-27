using Titan.Core;
using Titan.Graphics.D3D11.Textures;

namespace Titan.UI.Rendering
{
    public struct UIElement
    {
        public Handle<Texture> Texture;
        public uint StartIndex;
        public uint Count;
    }
}