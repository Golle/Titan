using System.Runtime.InteropServices;

namespace Titan.Windows.Win32.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
    }
}
