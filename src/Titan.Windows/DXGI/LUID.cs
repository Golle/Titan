using System.Runtime.InteropServices;

namespace Titan.Windows.DXGI;

[StructLayout(LayoutKind.Sequential)]
public struct LUID
{
    public uint LowPart;
    public int HighPart;
}
