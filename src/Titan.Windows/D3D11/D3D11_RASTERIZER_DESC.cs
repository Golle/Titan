// ReSharper disable InconsistentNaming
namespace Titan.Windows.D3D11;

public struct D3D11_RASTERIZER_DESC
{
    public D3D11_FILL_MODE FillMode;
    public D3D11_CULL_MODE CullMode;
    public int FrontCounterClockwise;
    public int DepthBias;
    public float DepthBiasClamp;
    public float SlopeScaledDepthBias;
    public int DepthClipEnable;
    public int ScissorEnable;
    public int MultisampleEnable;
    public int AntialiasedLineEnable;
}
