namespace Titan.Platform.Win32.D3D12;

public struct D3D12_DEPTH_STENCILOP_DESC
{
    public D3D12_STENCIL_OP StencilFailOp;
    public D3D12_STENCIL_OP StencilDepthFailOp;
    public D3D12_STENCIL_OP StencilPassOp;
    public D3D12_COMPARISON_FUNC StencilFunc;
}