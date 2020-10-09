// ReSharper disable InconsistentNaming
namespace Titan.Windows.Win32.D3D11
{
    public struct D3D11_RENDER_TARGET_BLEND_DESC
    {
        public int BlendEnable;
        public D3D11_BLEND SrcBlend;
        public D3D11_BLEND DestBlend;
        public D3D11_BLEND_OP BlendOp;
        public D3D11_BLEND SrcBlendAlpha;
        public D3D11_BLEND DestBlendAlpha;
        public D3D11_BLEND_OP BlendOpAlpha;
        public byte RenderTargetWriteMask;
    }
}
