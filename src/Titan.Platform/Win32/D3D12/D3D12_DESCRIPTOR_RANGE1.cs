namespace Titan.Platform.Win32.D3D12;

public struct D3D12_DESCRIPTOR_RANGE1
{
    public D3D12_DESCRIPTOR_RANGE_TYPE RangeType;
    public uint NumDescriptors;
    public uint BaseShaderRegister;
    public uint RegisterSpace;
    public D3D12_DESCRIPTOR_RANGE_FLAGS Flags;
    public uint OffsetInDescriptorsFromTableStart;
}