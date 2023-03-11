using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D12;

public struct D3D12_SUBRESOURCE_FOOTPRINT
{
    public DXGI_FORMAT Format;
    public uint Width;
    public uint Height;
    public uint Depth;
    public uint RowPitch;
}