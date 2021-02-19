using System;
using Titan.Core.Logging;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Buffers;
using Titan.GraphicsV2.D3D11.Shaders;
using Titan.GraphicsV2.D3D11.Textures;
using Titan.GraphicsV2.Rendering;
using Titan.GraphicsV2.Rendering2;
using Titan.GraphicsV2.Resources;
using Titan.IOC;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2
{
    public class GraphicsSystem : IDisposable
    {
        private readonly IContainer _container;
        private Device _device;

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

            
            var config = _container.CreateInstance<RenderPipelineReader>()
                .Read("render_pipeline_v2.json");

            {
                var textureManager = _device.TextureManager;
                var handle = textureManager.Create(new TextureCreation
                {
                    Binding = TextureBindFlags.RenderTarget | TextureBindFlags.ShaderResource,
                    Format = DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_UINT,
                    Width = 0,
                    Height = 1024,
                    Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT
                });
                ref readonly var texture = ref textureManager.Access(handle);
                textureManager.Release(handle);
            }
            {
                var bufferManager = _device.BufferManager;
                var handle = bufferManager.Create(new BufferCreation
                {
                    Type = BufferTypes.IndexBuffer,
                    Count = 1000,
                    Stride = sizeof(uint),
                    CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                    MiscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFIED,
                    Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT
                });

                ref readonly var indexbuffer = ref bufferManager.Access(handle);

                bufferManager.Release(handle);
            }


        }

        public void RenderFrame()
        {
           
            _device.Swapchain.Present();
        }

        public void Dispose()
        {
            _device.Dispose();
            _container.Dispose();
        }
    }

}
