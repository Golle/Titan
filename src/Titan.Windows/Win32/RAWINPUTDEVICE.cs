using System.Runtime.InteropServices;

namespace Titan.Windows.Win32;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct RAWINPUTDEVICE
{
    public HID_USAGE_PAGE usUsagePage;
    public HID_USAGE usUsage;
    public DWORD dwFlags;
    public HWND hwndTarget;
}
