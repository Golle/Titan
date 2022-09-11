using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.DXGI;

[StructLayout(LayoutKind.Sequential)]
public struct LUID
{
    public uint LowPart;
    public int HighPart;
}
