namespace Titan.Windows.D3D12;

public struct D3D12_TEX2D_ARRAY_SRV
{
    public uint MostDetailedMip;
    public uint MipLevels;
    public uint FirstArraySlice;
    public uint ArraySize;
    public uint PlaneSlice;
    public float ResourceMinLODClamp;
}