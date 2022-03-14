using System.Runtime.InteropServices;

namespace Titan.Windows.Win32;

[StructLayout(LayoutKind.Sequential)]
public struct HID_DEVICE_INFO_HID
{
    public DWORD dwVendorId;
    public DWORD dwProductId;
    public DWORD dwVersionNumber;
    public HID_USAGE_PAGE usUsagePage;
    public HID_USAGE usUsage;
}
