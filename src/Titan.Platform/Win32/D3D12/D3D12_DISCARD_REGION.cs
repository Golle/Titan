namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_DISCARD_REGION
{
    public uint NumRects;
    public D3D12_RECT* pRects;
    public uint FirstSubresource;
    public uint NumSubresources;
}
