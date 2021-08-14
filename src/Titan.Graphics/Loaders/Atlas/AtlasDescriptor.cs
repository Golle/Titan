using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Graphics.Loaders.Atlas
{
    [StructLayout(LayoutKind.Sequential, Size = 4, Pack = 4)]
    [SkipLocalsInit]
    public struct AtlasDescriptor
    {
        public byte Start;
        public byte Length;
        public SpriteType Type;
    }
}
