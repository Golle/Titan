using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_RENDER_PASS_ENDING_ACCESS_RESOLVE_PARAMETERS
{
    public ID3D12Resource* pSrcResource;
    public ID3D12Resource* pDstResource;
    public uint SubresourceCount;
    public D3D12_RENDER_PASS_ENDING_ACCESS_RESOLVE_SUBRESOURCE_PARAMETERS* pSubresourceParameters;
    public DXGI_FORMAT Format;
    public D3D12_RESOLVE_MODE ResolveMode;
    public int PreserveResolveSource;
}