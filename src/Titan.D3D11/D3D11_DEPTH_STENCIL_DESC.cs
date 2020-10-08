// ReSharper disable InconsistentNaming
namespace Titan.D3D11
{
    public struct D3D11_DEPTH_STENCIL_DESC
    {
        public int DepthEnable;
        public D3D11_DEPTH_WRITE_MASK DepthWriteMask;
        public D3D11_COMPARISON_FUNC DepthFunc;
        public int StencilEnable;
        public byte StencilReadMask;
        public byte StencilWriteMask;
        public D3D11_DEPTH_STENCILOP_DESC FrontFace;
        public D3D11_DEPTH_STENCILOP_DESC BackFace;
    }
}
