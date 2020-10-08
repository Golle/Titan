using System.Runtime.InteropServices;

namespace Titan.Windows.Win32.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Point
    {
        internal int X;
        internal int Y;
    }
}
