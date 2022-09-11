using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.D3D11;

[StructLayout(LayoutKind.Sequential)]
public struct D3D11_BUFFER_UAV
{
    public uint FirstElement;
    public uint NumElements;
    public uint Flags;
}
