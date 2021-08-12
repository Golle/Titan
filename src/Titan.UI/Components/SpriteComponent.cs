using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Graphics.Loaders.Atlas;

namespace Titan.UI.Components
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SpriteComponent
    {
        public Handle<TextureAtlas> TextureAtlas;
        public byte TextureIndex;
        public Margins Margins;
    }
}
