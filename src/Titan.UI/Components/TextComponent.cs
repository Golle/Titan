using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Graphics;
using Titan.Graphics.D3D11.Textures;
using Titan.Graphics.Loaders.Fonts;
using Titan.Graphics.Rendering.Text;

namespace Titan.UI.Components
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TextComponent
    {
        public Handle<TextBlock> Handle;
        public Handle<Font> Font;
        // TODO: these should be internal
        public Handle<Texture> CachedTexture;
        public ushort VisibleChars;
        // TODO: END: these should be internal
        public ushort LineHeight;
        public ushort FontSize;
        public VerticalOverflow VerticalOverflow;
        public HorizontalOverflow HorizontalOverflow;
        public TextAlign TextAlign;
        public VerticalAlign VerticalAlign;
        public Color Color;
        public bool IsDirty;
        
    }
}
