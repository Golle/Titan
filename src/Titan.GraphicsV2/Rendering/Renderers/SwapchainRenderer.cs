using Titan.GraphicsV2.D3D11;

namespace Titan.GraphicsV2.Rendering.Renderers
{
    internal class SwapchainRenderer : IRenderer
    {
        public SwapchainRenderer()
        {

        }
        public void Render(Context context)
        {
            context.Draw(0, 0);
        }

        public void Dispose()
        {

        }
    }
}
