namespace Titan.Windows.D3D12;

public struct D3D12_QUERY_HEAP_DESC
{
    public D3D12_QUERY_HEAP_TYPE Type;
    public uint Count;
    public uint NodeMask;
}