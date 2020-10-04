using System;
using System.Runtime.InteropServices;

namespace Titan.Windows.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WndClassExA
    {
        internal uint CbSize;
        internal uint Style;
        internal IntPtr LpFnWndProc;
        internal int CbClsExtra;
        internal int CbWndExtra;
        internal IntPtr HInstance;
        internal IntPtr HIcon;
        internal IntPtr HCursor;
        internal IntPtr HbrBackground;
        internal string LpszMenuName;
        internal string LpszClassName;
        internal IntPtr HIconSm;
    }
}
