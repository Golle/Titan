using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Titan.Core.Maths;

[DebuggerDisplay("X: {X} Y: {Y} Width: {Width} Height: {Height}")]
#pragma warning disable CS0661, CS0660
public struct RectangleF
#pragma warning restore CS0660, CS0661
{
    public float X, Y, Width, Height;
    public RectangleF(float value)
        => X = Y = Width = Height = value;

    public RectangleF(float x, float width, float y, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in RectangleF lhs, in RectangleF rhs)
    {
        //NOTE(Jens): Do a memory comparison for floats. Not sure if this is the best way to do it. Evalute later if needed.
        var left = Vector128.LoadUnsafe(ref Unsafe.As<RectangleF, byte>(ref Unsafe.AsRef(lhs)));
        var right = Vector128.LoadUnsafe(ref Unsafe.As<RectangleF, byte>(ref Unsafe.AsRef(rhs)));
        return left == right;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in RectangleF lhs, in RectangleF rhs)
    {
        //NOTE(Jens): Do a memory comparison for floats. Not sure if this is the best way to do it. Evalute later if needed.
        var left = Vector128.LoadUnsafe(ref Unsafe.As<RectangleF, byte>(ref Unsafe.AsRef(lhs)));
        var right = Vector128.LoadUnsafe(ref Unsafe.As<RectangleF, byte>(ref Unsafe.AsRef(rhs)));
        return left != right;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator RectangleF(in Rectangle r)
    {
        //NOTE(Jens): use SkipInit to avoid setting everything to 0 before setting the values. 
        Unsafe.SkipInit(out RectangleF rectangle);
        rectangle.X = r.X;
        rectangle.Y = r.Y;
        rectangle.Width  = r.Width;
        rectangle.Height = r.Height;
        return rectangle;
    }
}
