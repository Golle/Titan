using System.Runtime.InteropServices;

namespace Titan.Windows.Win32.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WndClassExA
    {
        internal uint CbSize;
        internal uint Style;
        internal unsafe delegate*<nint, WindowsMessage, nuint, nuint, nint> LpFnWndProc;
        internal int CbClsExtra;
        internal int CbWndExtra;
        internal nint HInstance;
        internal nint HIcon;
        internal nint HCursor;
        internal nint HbrBackground;
        internal string LpszMenuName;
        internal string LpszClassName;
        internal nint HIconSm;
    }
}
