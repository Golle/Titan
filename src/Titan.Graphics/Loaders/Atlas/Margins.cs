using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Graphics.Loaders.Atlas
{
    [DebuggerDisplay("<{Top} {Bottom} {Left} {Right}>")]
    [SkipLocalsInit]
    public struct Margins
    {
        public byte Top;
        public byte Bottom;
        public byte Left;
        public byte Right;
        public Margins(byte margin) => Top = Bottom = Left = Right = margin;
        public Margins(byte top, byte bottom, byte left, byte right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Margins(byte margin) => new(margin);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Margins(in (byte top, byte bottom, byte left, byte right) val) => new(val.top, val.bottom, val.left, val.right);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Margins operator *(in Margins margins, byte value) =>
            new(
                (byte)(margins.Top * value),
                (byte)(margins.Bottom * value),
                (byte)(margins.Left * value),
                (byte)(margins.Right * value)
            );
    }
}
