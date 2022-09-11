using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.Win32;


// NOTE(Jens): The hardcoded size is because MOUSE and KEYBOARD has not been added, they are bigger than Hid. (The size with just Hid is 24 bytes, which causes the method to fail)
[StructLayout(LayoutKind.Sequential, Size = 32)]
public struct RID_DEVICE_INFO
{
    public DWORD cbSize;
    public DWORD dwType;

    // These should share the memory
    //public RID_DEVICE_INFO_MOUSE mouse;
    //public RID_DEVICE_INFO_KEYBOARD keyboard;
    public HID_DEVICE_INFO_HID Hid;
}
