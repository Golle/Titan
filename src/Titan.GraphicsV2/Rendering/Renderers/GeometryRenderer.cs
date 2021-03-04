using Titan.GraphicsV2.D3D11;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering.Renderers
{
    internal class GeometryRenderer : IRenderer
    {
        private readonly Device _device;
        private ViewPort _viewPort;

        public GeometryRenderer(Device device)
        {
            _device = device;
        }

        public void Init()
        {
            _viewPort = new ViewPort((int) _device.Swapchain.Width, (int) _device.Swapchain.Height);
        }

        public void Render(Context context)
        {
            context.SetTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetViewPort(_viewPort);


            context.Draw(0, 0);
        }

        public void Dispose()
        {

        }
    }
}
