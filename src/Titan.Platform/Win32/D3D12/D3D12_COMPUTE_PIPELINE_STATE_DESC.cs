namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_COMPUTE_PIPELINE_STATE_DESC
{
    public ID3D12RootSignature* pRootSignature;
    public D3D12_SHADER_BYTECODE CS;
    public uint NodeMask;
    public D3D12_CACHED_PIPELINE_STATE CachedPSO;
    public D3D12_PIPELINE_STATE_FLAGS Flags;
}
