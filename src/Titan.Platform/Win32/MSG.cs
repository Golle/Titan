using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32;

[SkipLocalsInit]
[StructLayout(LayoutKind.Sequential)]
public struct MSG
{
    public HWND Hwnd;
    public WINDOW_MESSAGE Message;
    public nuint WParam;
    public nuint LParam;
    public int Time;
    public POINT Point;
}
