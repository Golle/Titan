using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Graphics.Rendering.Sprites
{
    [SkipLocalsInit]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct NormalSprite
    {
        public Vector2 Position;
        public Size Size;
        public Color Color;
        public SpriteCoordinates Coordinates;
    }

    [StructLayout(LayoutKind.Sequential, Size = sizeof(float) * 2 * 4)]
    public unsafe struct SpriteCoordinates
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2* AsPointer() => (Vector2*)Unsafe.AsPointer(ref this);
        public ref readonly Vector2 this[int index]
        {
            get
            {
                Debug.Assert(index < 4, "Index must be between 0 and 3");
                return ref *((Vector2*)Unsafe.AsPointer(ref this) + index);
            }
        }
    }
}
