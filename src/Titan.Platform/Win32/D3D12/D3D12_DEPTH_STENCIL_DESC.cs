namespace Titan.Platform.Win32.D3D12;

public struct D3D12_DEPTH_STENCIL_DESC
{
    public int DepthEnable; //unmanaged bool
    public D3D12_DEPTH_WRITE_MASK DepthWriteMask;
    public D3D12_COMPARISON_FUNC DepthFunc;
    public int StencilEnable;//unmanaged bool
    public byte StencilReadMask;
    public byte StencilWriteMask;
    public D3D12_DEPTH_STENCILOP_DESC FrontFace;
    public D3D12_DEPTH_STENCILOP_DESC BackFace;
}