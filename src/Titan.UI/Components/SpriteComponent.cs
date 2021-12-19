using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Graphics;
using Titan.Graphics.Loaders.Atlas;

namespace Titan.UI.Components
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct SpriteComponent
    {
        public Handle<TextureAtlas> TextureAtlas;
        public Margins Margins;
        public Color Color;
        public byte TextureIndex;
    }
}
