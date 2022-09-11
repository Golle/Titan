namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_ROOT_DESCRIPTOR_TABLE
{
    public uint NumDescriptorRanges;
    public D3D12_DESCRIPTOR_RANGE* pDescriptorRanges;
}