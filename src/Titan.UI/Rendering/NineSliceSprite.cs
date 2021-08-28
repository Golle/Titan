using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Graphics;
using Titan.Graphics.D3D11.Textures;
using Titan.Graphics.Loaders.Atlas;
using Titan.UI.Common;

namespace Titan.UI.Rendering
{
    [SkipLocalsInit]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct NineSliceSprite
    {
        public Vector2 Position;
        public Size Size;
        public Color Color;
        public Margins Margins;
        public NineSliceCoordinates Coordinates;
        public Handle<Texture> Texture;
    }

    [SkipLocalsInit]
    [StructLayout(LayoutKind.Sequential, Size = sizeof(float) * 2 * 16)]
    public unsafe struct NineSliceCoordinates
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2* AsPointer() => (Vector2*)Unsafe.AsPointer(ref this);
        public ref readonly Vector2 this[int index]
        {
            get
            {
                Debug.Assert(index < 16, "Index must be between 0 and 15");
                return ref *((Vector2*)Unsafe.AsPointer(ref this) + index);
            }
        }
    }
}
