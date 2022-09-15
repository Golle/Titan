namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_RESOURCE_TRANSITION_BARRIER
{
    public ID3D12Resource* pResource;
    public uint Subresource;
    public D3D12_RESOURCE_STATES StateBefore;
    public D3D12_RESOURCE_STATES StateAfter;
}