using System;
using System.Runtime.InteropServices;

namespace Titan.Windows.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Msg
    {
        internal IntPtr Hwnd;
        internal WindowsMessage Message;
        internal UIntPtr WParam;
        internal UIntPtr LParam;
        internal int Time;
        internal Point Point;
    }
}
