using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MSG
    {
        internal HWND Hwnd;
        internal WindowsMessage Message;
        internal nuint WParam;
        internal nuint LParam;
        internal int Time;
        internal POINT Point;
    }
}
