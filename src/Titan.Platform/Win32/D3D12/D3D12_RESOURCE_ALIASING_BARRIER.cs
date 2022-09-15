namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_RESOURCE_ALIASING_BARRIER
{
    public ID3D12Resource* pResourceBefore;
    public ID3D12Resource* pResourceAfter;
}