using System;
using Titan.Core.Logging;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.Rendering.Pipepline;
using Titan.IOC;

namespace Titan.GraphicsV2
{
    public class GraphicsSystem : IDisposable
    {
        private readonly IContainer _container;
        private Device _device;
        private RenderPipeline _pipeline;

        public GraphicsSystem(IContainer container)
        {
            _container = container;
        }

        public void Initialize(DeviceConfiguration configuration)
        {
            LOGGER.Trace("Create device");
            _device = new Device(configuration);

            LOGGER.Trace("Register factories and managers");
            _container
                .RegisterSingleton(_device);
            
            _pipeline = _container
                    .CreateInstance<RenderPipelineFactory>()
                .CreateFromFile("render_pipeline_v2.json")
;
        }

        public void RenderFrame()
        {
           _pipeline.Render(_device.Context);

           _device.Swapchain.Present();
        }

        public void Dispose()
        {
            _pipeline.Dispose();

            _device.Dispose();
            _container.Dispose();
        }
    }

}
