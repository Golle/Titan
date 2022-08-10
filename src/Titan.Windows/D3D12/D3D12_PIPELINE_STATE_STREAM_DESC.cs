namespace Titan.Windows.D3D12;

public unsafe struct D3D12_PIPELINE_STATE_STREAM_DESC
{
    public nuint SizeInBytes;
    public void* pPipelineStateSubobjectStream;
}