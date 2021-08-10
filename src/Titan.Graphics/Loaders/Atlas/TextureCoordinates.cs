using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Graphics.Loaders.Atlas
{
    [StructLayout(LayoutKind.Sequential, Size = sizeof(float) * 2 * 16)]
    public unsafe struct TextureCoordinates
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2* AsPointer() => (Vector2*)Unsafe.AsPointer(ref this);
        public ref readonly Vector2 this[int index]
        {
            get
            {
                Debug.Assert(index < 16, "Index must be between 0 and 16");
                return ref *((Vector2*)Unsafe.AsPointer(ref this) + index);
            }
        }
    }
}
