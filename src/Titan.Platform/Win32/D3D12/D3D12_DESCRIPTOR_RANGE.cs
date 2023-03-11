namespace Titan.Platform.Win32.D3D12;

public struct D3D12_DESCRIPTOR_RANGE
{
    public D3D12_DESCRIPTOR_RANGE_TYPE RangeType;
    public uint NumDescriptors;
    public uint BaseShaderRegister;
    public uint RegisterSpace;
    public uint OffsetInDescriptorsFromTableStart;
}