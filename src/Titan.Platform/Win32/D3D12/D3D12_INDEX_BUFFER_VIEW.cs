using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D12;

public struct D3D12_INDEX_BUFFER_VIEW
{
    public D3D12_GPU_VIRTUAL_ADDRESS BufferLocation;
    public uint SizeInBytes;
    public DXGI_FORMAT Format;
}
