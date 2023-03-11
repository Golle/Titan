using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.DXGI;

[StructLayout(LayoutKind.Sequential)]
public struct DXGI_ADAPTER_DESC
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
}
