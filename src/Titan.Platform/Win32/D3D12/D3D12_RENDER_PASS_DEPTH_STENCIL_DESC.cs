namespace Titan.Platform.Win32.D3D12;

public struct D3D12_RENDER_PASS_DEPTH_STENCIL_DESC
{
    public D3D12_CPU_DESCRIPTOR_HANDLE cpuDescriptor;
    public D3D12_RENDER_PASS_BEGINNING_ACCESS DepthBeginningAccess;
    public D3D12_RENDER_PASS_BEGINNING_ACCESS StencilBeginningAccess;
    public D3D12_RENDER_PASS_ENDING_ACCESS DepthEndingAccess;
    public D3D12_RENDER_PASS_ENDING_ACCESS StencilEndingAccess;
}
