using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Titan.Core.Maths;

[DebuggerDisplay("Width: {Width} Height: {Height}")]
public struct Size
{
    public int Width;
    public int Height;

    public static readonly Size Zero = default;

    [SkipLocalsInit]
    public Size(int size)
    {
        Width = Height = size;
    }

    [SkipLocalsInit]
    public Size(int width, int height)
    {
        Width = width;
        Height = height;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator SizeF(in Size size) => new(size.Width, size.Height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF operator /(in Size size, int divisor) => size / (float)divisor;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF operator /(in Size size, float divisor) => new(size.Width / divisor, size.Height / divisor);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF operator /(in Size lhs, in Size rhs) => new(lhs.Width / (float)rhs.Width, lhs.Height / (float)rhs.Height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF operator *(in Size size, int multiplier) => new(size.Width * multiplier, size.Height * multiplier);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF operator *(in Size size, float multiplier) => new(size.Width * multiplier, size.Height * multiplier);
}
