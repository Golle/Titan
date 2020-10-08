using System.Runtime.InteropServices;

namespace Titan.Windows.Win32.Native
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
