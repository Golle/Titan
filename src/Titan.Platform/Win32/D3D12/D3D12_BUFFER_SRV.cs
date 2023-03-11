namespace Titan.Platform.Win32.D3D12;

public struct D3D12_BUFFER_SRV
{
    public ulong FirstElement;
    public uint NumElements;
    public uint StructureByteStride;
    public D3D12_BUFFER_SRV_FLAGS Flags;
}