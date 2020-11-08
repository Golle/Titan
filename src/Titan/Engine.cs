using System;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Messaging;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.Pipeline;
using Titan.Graphics.Resources;
using Titan.Graphics.States;
using Titan.IOC;
using Titan.Windows;

namespace Titan
{
    internal class Engine : IEngine
    {
        private readonly IWindow _window;
        private readonly IGraphicsDevice _device;
        
        private readonly IContainer _container;
        private readonly IGraphicsPipeline _pipeline;
        private readonly IMemoryManager _memoryManager;

        public Engine(EngineConfiguration configuration, IWindowFactory windowFactory, IEventQueue eventQueue, ILog log, IContainer container)
        {
            LOGGER.InitializeLogger(log);
            LOGGER.Debug("Initialize EventQueue with {0}", typeof(ScanningEventTypeProvider));
            eventQueue.Initialize(new ScanningEventTypeProvider());

            container
                .RegisterSingleton(new TitanConfiguration(configuration.ResourceBasePath, configuration.RefreshRate, configuration.Debug))
                .RegisterSingleton(_memoryManager = CreateMemoryManager())
                .RegisterSingleton(_window = windowFactory.Create((int) configuration.Width, (int) configuration.Height, configuration.Title))
                .RegisterSingleton(_device = container.CreateInstance<GraphicsDevice>())
                .RegisterSingleton(_pipeline = container.CreateInstance<GraphicsPipeline>());

            LOGGER.Debug("Initialize GraphicsPipeline");
            _pipeline.Initialize("render_pipeline.json");

            _container = container;
        }

        private unsafe IMemoryManager CreateMemoryManager()
        {
            LOGGER.Debug("Initialize memory manager");
            // TODO: not sure how to do this yet. Could have each manager "request" a memory chunk.
            return new MemoryManager(new[]
            {
                new ChunkDescriptor("VertexBuffer", (uint) sizeof(VertexBuffer), 2048),
                new ChunkDescriptor("IndexBuffer", (uint) sizeof(IndexBuffer), 2048),
                new ChunkDescriptor("ConstantBuffer", (uint) sizeof(ConstantBuffer), 100),
                new ChunkDescriptor("Shaders", (uint) IntPtr.Size, 1024),
                new ChunkDescriptor("Texture", (uint) sizeof(Texture), 1024),
                new ChunkDescriptor("ShaderResourceView", (uint) sizeof(ShaderResourceView), 1024),
                new ChunkDescriptor("RenderTargetView", (uint) sizeof(RenderTargetView), 1024),
                new ChunkDescriptor("DepthStencilView", (uint) sizeof(DepthStencilView), 10),
                new ChunkDescriptor("DepthStencilState", (uint) sizeof(DepthStencilState), 10),
                new ChunkDescriptor("SamplerState", (uint) sizeof(SamplerState), 20),
            });
        }

        public void Dispose()
        {
            _pipeline.Dispose();
            _device.Dispose();
            _window.Dispose();

            // memory manager must be cleared last
            _memoryManager.Dispose();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        IGraphicsDevice IEngine.Device => _device;
        IWindow IEngine.Window => _window;

        IContainer IEngine.Container => _container;
    }

    
}
