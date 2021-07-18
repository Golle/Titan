using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11.Rasterizer
{
    public unsafe struct RasterizerState
    {
        public ID3D11RasterizerState* State;
        public CullMode CullMode;
        public FillMode FillMode;
    }
}
