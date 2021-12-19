using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Systems
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct TextLine
    {
        public ushort Start;
        public ushort End;
        public float Width;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextLine(int start, int end, float width)
        {
            Start = (ushort)start;
            End = (ushort)end;
            Width = width;
        }
    }
}
