namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_SAMPLER_DESC
{
    public D3D12_FILTER Filter;
    public D3D12_TEXTURE_ADDRESS_MODE AddressU;
    public D3D12_TEXTURE_ADDRESS_MODE AddressV;
    public D3D12_TEXTURE_ADDRESS_MODE AddressW;
    public float MipLODBias;
    public uint MaxAnisotropy;
    public D3D12_COMPARISON_FUNC ComparisonFunc;
    public fixed float BorderColor[4];
    public float MinLOD;
    public float MaxLOD;
}