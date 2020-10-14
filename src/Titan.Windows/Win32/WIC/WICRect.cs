using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32.WIC
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WICRect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }
}
