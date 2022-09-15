namespace Titan.Platform.Win32.D3D12;

public struct D3D12_RENDER_TARGET_BLEND_DESC
{
    public int BlendEnable;// Unmanaged BOOL
    public int LogicOpEnable;// Unmanaged BOOL
    public D3D12_BLEND SrcBlend;
    public D3D12_BLEND DestBlend;
    public D3D12_BLEND_OP BlendOp;
    public D3D12_BLEND SrcBlendAlpha;
    public D3D12_BLEND DestBlendAlpha;
    public D3D12_BLEND_OP BlendOpAlpha;
    public D3D12_LOGIC_OP LogicOp;
    public D3D12_COLOR_WRITE_ENABLE RenderTargetWriteMask;
}