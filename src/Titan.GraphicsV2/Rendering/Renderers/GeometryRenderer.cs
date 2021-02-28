using Titan.GraphicsV2.D3D11;

namespace Titan.GraphicsV2.Rendering.Renderers
{

    internal class DeferredShadingRenderer : IRenderer
    {
        private readonly Device _device;

        public DeferredShadingRenderer(Device device)
        {
            _device = device;
        }
        public void Render(Context context)
        {
            context.Draw(0, 0);
        }

        public void Dispose()
        {
        }
    }

    internal class GeometryRenderer : IRenderer
    {
        public GeometryRenderer()
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
