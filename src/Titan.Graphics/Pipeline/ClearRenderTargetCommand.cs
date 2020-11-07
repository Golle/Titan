using Titan.Graphics.D3D11;
using Titan.Graphics.Resources;

namespace Titan.Graphics.Pipeline
{
    internal struct ClearRenderTargetCommand
    {
        public RenderTargetViewHandle RenderTarget;
        public Color Color;
    }
}
