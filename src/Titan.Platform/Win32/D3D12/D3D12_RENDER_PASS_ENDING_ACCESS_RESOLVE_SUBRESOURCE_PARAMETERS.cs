namespace Titan.Platform.Win32.D3D12;

public struct D3D12_RENDER_PASS_ENDING_ACCESS_RESOLVE_SUBRESOURCE_PARAMETERS
{
    public uint SrcSubresource;
    public uint  DstSubresource;
    public uint  DstX;
    public uint  DstY;
    public D3D12_RECT SrcRect;
}