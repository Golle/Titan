using System;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.Rendering2;
using Titan.IOC;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2
{
    public class GraphicsSystem : IDisposable
    {
        private readonly IContainer _container;
        private Device _device;
        private Swapchain _swapchain;
        private Context _context;

        public GraphicsSystem(IContainer container)
        {
            _container = container;
        }

        public void Initialize(DeviceConfiguration configuration)
        {
            (_device, _swapchain) = _container
                .CreateInstance<DeviceFactory>() // Use create instance so that the factory till be GCed when the device has been created.
                .Create(configuration);

            _container
                .RegisterSingleton(_device, dispose: true)
                .RegisterSingleton(_swapchain, dispose: true);

            _context = _container
                .CreateInstance<ContextFactory>()
                .CreateImmediateContext();

            var config = _container.CreateInstance<RenderPipelineReader>()
                .Read("render_pipeline_v2.json");
        }

        public void RenderFrame()
        {
            ID3D11DeviceContext a;
            
            _swapchain.Present();
        }

        public void Dispose()
        {
            _context.Dispose();
            _swapchain.Dispose();
            _device.Dispose();
            _container.Dispose();
        }
    }
}
