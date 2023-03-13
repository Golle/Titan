using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Titan.Core.Maths;

[DebuggerDisplay("Width: {Width} Height: {Height}")]
public struct SizeF
{
    public float Width;
    public float Height;

    public static readonly Size Zero = default;

    [SkipLocalsInit]
    public SizeF(float size)
    {
        Width = Height = size;
    }

    [SkipLocalsInit]
    public SizeF(float width, float height)
    {
        Width = width;
        Height = height;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Vector2(in SizeF size) => new(size.Width, size.Height);

#if DEBUG
    public override string ToString() => $"{Width}x{Height}";
#endif
}
