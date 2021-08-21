using Titan.Core;
using Titan.Graphics.D3D11.Textures;
using Titan.Graphics.Loaders.Fonts;
using Titan.UI.Text;

namespace Titan.UI.Components
{
    public struct TextComponent
    {
        public Handle<TextBlock> Handle;
        public Handle<Font> Font;
        internal Handle<Texture> CachedTexture;
        public ushort LineHeight;
        public ushort VisibleChars;
        public VerticalOverflow VerticalOverflow;
        public HorizontalOverflow HorizontalOverflow;
        public bool IsDirty;
    }
}
