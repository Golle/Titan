using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.UI.Common
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
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
    }
}
