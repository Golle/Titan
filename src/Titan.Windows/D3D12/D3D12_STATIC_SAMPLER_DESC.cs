namespace Titan.Windows.D3D12;

public struct D3D12_STATIC_SAMPLER_DESC
{
    public D3D12_FILTER Filter;
    public D3D12_TEXTURE_ADDRESS_MODE AddressU;
    public D3D12_TEXTURE_ADDRESS_MODE AddressV;
    public D3D12_TEXTURE_ADDRESS_MODE AddressW;
    public float MipLODBias;
    public uint MaxAnisotropy;
    public D3D12_COMPARISON_FUNC ComparisonFunc;
    public D3D12_STATIC_BORDER_COLOR BorderColor;
    public float MinLOD;
    public float MaxLOD;
    public uint ShaderRegister;
    public uint RegisterSpace;
    public D3D12_SHADER_VISIBILITY ShaderVisibility;
}