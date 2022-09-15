using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.D3D11;

[StructLayout(LayoutKind.Explicit)]
public struct D3D11_BUFFER_SRV
{
    [FieldOffset(0)]
    public uint FirstElement;
    [FieldOffset(sizeof(uint))]
    public uint ElementOffset;
    [FieldOffset(0)]
    public uint NumElements;
    [FieldOffset(sizeof(uint))]
    public uint ElementWidth;
}
