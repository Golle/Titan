using Titan.Core;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Graphics.Loaders.Fonts
{
    public ref struct FontCreation
    {
        public uint Width;
        public uint Height;
        public Handle<Texture> Texture;
        public ushort LineHeight;
        public ushort Base;
        public ReadOnlySpan<GlyphDescriptor> Characters;
        public ushort FontSize;
    }
}
