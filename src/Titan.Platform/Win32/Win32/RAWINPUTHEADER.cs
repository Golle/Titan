using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.Win32;

[StructLayout(LayoutKind.Sequential)]
public struct RAWINPUTHEADER
{
    public DWORD dwType;
    public DWORD dwSize;
    public HANDLE hDevice;
    public WPARAM wParam;
}
