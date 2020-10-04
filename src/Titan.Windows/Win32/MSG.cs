using System;
using System.Runtime.InteropServices;

namespace Titan.Windows.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Msg
    {
        internal nint Hwnd;
        internal WindowsMessage Message;
        internal nuint WParam;
        internal nuint LParam;
        internal int Time;
        internal Point Point;
    }
}
