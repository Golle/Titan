using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Titan.Graphics.Loaders.Fonts
{
    [DebuggerDisplay("<{Id} {X} {Y} {Width} {Height}>")]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CharacterDescriptor
    {
        public char Id;
        public short X;
        public short Y;
        public short Width;
        public short Height;
        public short XOffset;
        public short YOffset;
        public short XAdvance;
    }
}
