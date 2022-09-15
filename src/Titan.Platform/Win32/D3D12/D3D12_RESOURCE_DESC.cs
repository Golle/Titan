using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D12;

public struct D3D12_RESOURCE_DESC
{
    public D3D12_RESOURCE_DIMENSION Dimension;
    public ulong Alignment;
    public ulong Width;
    public uint Height;
    public ushort DepthOrArraySize;
    public ushort MipLevels;
    public DXGI_FORMAT Format;
    public DXGI_SAMPLE_DESC SampleDesc;
    public D3D12_TEXTURE_LAYOUT Layout;
    public D3D12_RESOURCE_FLAGS Flags;
}