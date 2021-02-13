using System;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Shaders;
using Titan.GraphicsV2.Rendering;
using Titan.GraphicsV2.Resources;
using Titan.IOC;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2
{
    public class GraphicsSystem : IDisposable
    {
        private readonly IContainer _container;
        private Device _device;
        private Swapchain _swapchain;
        private RenderPass[] _passes;
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

            _passes = _container.CreateInstance<RenderingPipeline>().Initialize();

        }

        public void RenderFrame()
        {
            foreach (var renderPass in _passes)
            {
                renderPass.Execute(_context);
            }
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
