using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
    }
}
