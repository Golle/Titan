namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_CACHED_PIPELINE_STATE
{
    public void* pCachedBlob;
    public nuint CachedBlobSizeInBytes;
}