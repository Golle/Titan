using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        internal int X;
        internal int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static POINT operator-(in POINT lh, in POINT rh) => new POINT(lh.X - rh.X, lh.Y - rh.Y);
        public static POINT operator+(in POINT lh, in POINT rh) => new POINT(lh.X + rh.X, lh.Y + rh.Y);
    }
}
