namespace Titan.Windows.D3D12;

public unsafe struct D3D12_INPUT_LAYOUT_DESC
{
    public D3D12_INPUT_ELEMENT_DESC* pInputElementDescs;
    public uint NumElements;
}