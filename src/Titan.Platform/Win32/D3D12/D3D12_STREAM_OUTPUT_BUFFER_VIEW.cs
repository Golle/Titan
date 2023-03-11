namespace Titan.Platform.Win32.D3D12;

public struct D3D12_STREAM_OUTPUT_BUFFER_VIEW
{
    public D3D12_GPU_VIRTUAL_ADDRESS BufferLocation;
    public ulong SizeInBytes;
    public D3D12_GPU_VIRTUAL_ADDRESS BufferFilledSizeLocation;
}
