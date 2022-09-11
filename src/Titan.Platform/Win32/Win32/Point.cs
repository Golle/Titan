using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Titan.Platform.Win32.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static POINT operator-(in POINT lh, in POINT rh) => new POINT(lh.X - rh.X, lh.Y - rh.Y);
        public static POINT operator+(in POINT lh, in POINT rh) => new POINT(lh.X + rh.X, lh.Y + rh.Y);
    }
}
