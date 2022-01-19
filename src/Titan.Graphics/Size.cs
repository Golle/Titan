using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    [DebuggerDisplay(@"\{<{Width} {Height}>\}")]
    public struct Size
    {
        public int Width { get; set; }
        public int Height { get; set; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Size (in (int width, int height) val) => new (val.width, val.height);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Size( uint width, uint height)
        {
            unchecked
            {
                Width = (int)width;
                Height = (int)height;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => $"{{<{Width} {Height}>}}";
    }
}
