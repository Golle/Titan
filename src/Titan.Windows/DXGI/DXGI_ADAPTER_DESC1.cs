using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Titan.Windows.DXGI;

[StructLayout(LayoutKind.Sequential)]
public struct DXGI_ADAPTER_DESC1
{
    public unsafe fixed char Description[128];
    public uint VendorId;
    public uint DeviceId;
    public uint SubSysId;
    public uint Revision;
    public nuint DedicatedVideoMemory;
    public nuint DedicatedSystemMemory;
    public nuint SharedSystemMemory;
    public LUID AdapterLuid;
    public DXGI_ADAPTER_FLAG Flags;
}
