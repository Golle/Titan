namespace Titan.Windows.D3D12;

public struct D3D12_DESCRIPTOR_HEAP_DESC
{
    public D3D12_DESCRIPTOR_HEAP_TYPE Type;
    public uint NumDescriptors;
    public D3D12_DESCRIPTOR_HEAP_FLAGS Flags;
    public uint NodeMask;
}