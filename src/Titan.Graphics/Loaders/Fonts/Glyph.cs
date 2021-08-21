using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.Graphics.Loaders.Fonts
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct Glyph
    {
        public Vector2 TopLeft;
        public Vector2 BottomRight;
        public short XOffset;
        public short YOffset;
        public short XAdvance;
        public short Height;
        public short Width;
    }
}
