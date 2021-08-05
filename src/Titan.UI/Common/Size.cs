using System;
using System.Runtime.CompilerServices;

namespace Titan.UI.Common
{
    
    public struct Size
    {
        public int Width { get; set; }
        public int Height { get; set; }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Size (in (int width, int height) val) => new (val.width, val.height);
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
