namespace Titan.Windows.D3D12;

public struct D3D12_TEXCUBE_ARRAY_SRV
{
    public uint MostDetailedMip;
    public uint MipLevels;
    public uint First2DArrayFace;
    public uint NumCubes;
    public float ResourceMinLODClamp;
}