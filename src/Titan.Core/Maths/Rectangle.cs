using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Core.Maths;

[DebuggerDisplay("X: {X} Y: {Y} Width: {Width} Height: {Height}")]
#pragma warning disable CS0660, CS0661
public struct Rectangle
#pragma warning restore CS0660, CS0661
{
    public int X, Y, Width, Height;
    public Rectangle(int value)
        => X = Y = Width = Height = value;

    public Rectangle(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in Rectangle lhs, in Rectangle rhs)
        => !(lhs.Height != rhs.Height || lhs.Y != rhs.Y || lhs.X != rhs.X || lhs.Width != rhs.Width);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in Rectangle lhs, in Rectangle rhs)
        => lhs.Height != rhs.Height || lhs.Y != rhs.Y || lhs.X != rhs.X || lhs.Width != rhs.Width;
}
