using System;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Messaging;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.Pipeline;
using Titan.Graphics.Resources;
using Titan.IOC;
using Titan.Windows;
using DepthStencilView = Titan.Graphics.Resources.DepthStencilView;

namespace Titan
{
    internal class Engine : IEngine
    {
        private readonly IWindow _window;
        private readonly GraphicsDevice _device;
        
        private readonly IContainer _container;
        private readonly IGraphicsPipeline _pipeline;
        private IMemoryManager _memoryManager;

        public Engine(EngineConfiguration configuration, IWindowFactory windowFactory, IEventQueue eventQueue, ILog log, IContainer container)
        {
            LOGGER.InitializeLogger(log);
            LOGGER.Debug("Initialize EventQueue with {0}", typeof(ScanningEventTypeProvider));
            eventQueue.Initialize(new ScanningEventTypeProvider());

            _window = windowFactory.Create((int) configuration.Width, (int) configuration.Height, configuration.Title);


            _memoryManager = CreateMemoryManager();

            _device = new GraphicsDevice(_window, _memoryManager, configuration.RefreshRate, configuration.Debug);
            
            container
                .RegisterSingleton<IGraphicsDevice>(_device)
                .RegisterSingleton(_window)
                .RegisterSingleton(new TitanConfiguration(configuration.ResourceBasePath));


            _pipeline = container.GetInstance<IGraphicsPipeline>();
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
                new ChunkDescriptor("Texture", (uint) sizeof(Texture), 1024),
                new ChunkDescriptor("ShaderResourceView", (uint) sizeof(ShaderResourceView), 1024),
                new ChunkDescriptor("RenderTargetView", (uint) sizeof(RenderTargetView), 1024),
                new ChunkDescriptor("DepthStencilView", (uint) sizeof(DepthStencilView), 10),
            });
        }

        public void Dispose()
        {
            _pipeline.Dispose();
            _memoryManager.Dispose();
            _device.Dispose();
            _window.Dispose();
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
