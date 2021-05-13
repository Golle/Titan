using Titan.Graphics;
using Titan.Graphics.D3D11;

namespace Titan.Rendering
{
    internal class GeometryRenderer : IRenderer

    {
        private readonly ViewPort _viewport;
        public GeometryRenderer()
        {
            _viewport = new ViewPort((int) GraphicsDevice.SwapChain.Width, (int) GraphicsDevice.SwapChain.Height);
        }


        public void Render(Context context)
        {
            //Logger.Warning<GeometryRenderer>("RENDER");
        }

        public void Dispose()
        {

        }
    }

    internal class BackbufferRenderer : IRenderer
    {
        public BackbufferRenderer()
        {
            
        }
        public void Render(Context context)
        {
            //Logger.Warning<BackbufferRenderer>("RENDER");
        }

        public void Dispose()
        {
        }
    }
}
