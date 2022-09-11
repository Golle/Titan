namespace Titan.Platform.Win32.D3D12;

public struct D3D12_BUFFER_UAV
{
    public ulong FirstElement;
    public uint NumElements;
    public uint StructureByteStride;
    public ulong CounterOffsetInBytes;
    public D3D12_BUFFER_UAV_FLAGS Flags;
}