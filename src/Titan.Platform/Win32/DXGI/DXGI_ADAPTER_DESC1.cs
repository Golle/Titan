using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.DXGI;

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

    public unsafe ReadOnlySpan<char> DescriptionString()
    {
        var length = 128;
        for (var i = 0; i < 128; ++i)
        {
            if (Description[i] == '\0')
            {
                length = i; 
                break;
            }
        }
        fixed (char* pDesc = Description)
        {
            return new(pDesc, length);
        }
    }
}
