using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32
{
    [SkipLocalsInit]
    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public HWND Hwnd;
        public WindowsMessage Message;
        public nuint WParam;
        public nuint LParam;
        public int Time;
        public POINT Point;
    }
}
