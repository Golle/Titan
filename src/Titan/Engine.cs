using System;
using System.IO;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Messaging;
using Titan.Graphics.D3D11;
using Titan.Graphics.Pipeline;
using Titan.Graphics.Resources;
using Titan.IOC;
using Titan.Windows;
using Titan.Windows.Win32.D3D11;

namespace Titan
{
    internal class Engine : IEngine
    {
        private readonly IWindow _window;
        private readonly GraphicsDevice _device;
        
        private readonly IContainer _container;
        private readonly IGraphicsPipeline _pipeline;

        public Engine(EngineConfiguration configuration, IWindowFactory windowFactory, IEventQueue eventQueue, ILog log, IContainer container)
        {
            LOGGER.InitializeLogger(log);
            LOGGER.Debug("Initialize EventQueue with {0}", typeof(ScanningEventTypeProvider));
            eventQueue.Initialize(new ScanningEventTypeProvider());

            _window = windowFactory.Create((int) configuration.Width, (int) configuration.Height, configuration.Title);
            _device = new GraphicsDevice(_window, configuration.RefreshRate, configuration.Debug);
            
            container
                .RegisterSingleton<IGraphicsDevice>(_device)
                .RegisterSingleton(_window)
                .RegisterSingleton(new TitanConfiguration(configuration.ResourceBasePath));


            InitializeMemoryManager(container);


            _pipeline = container.GetInstance<IGraphicsPipeline>();
            LOGGER.Debug("Initialize GraphicsPipeline");
            _pipeline.Initialize("render_pipeline.json");


            _container = container;
        }

        private unsafe void InitializeMemoryManager(IContainer container)
        {
            LOGGER.Debug("Initialize memory manager");
            // TODO: not sure how to do this yet. Could have each manager "request" a memory chunk.
            var memoryManager = new MemoryManager(new[]
            {
                new ChunkDescriptor("VertexBuffer", (uint) sizeof(VertexBuffer), 2048),
                new ChunkDescriptor("IndexBuffer", (uint) sizeof(IndexBuffer), 2048),
                new ChunkDescriptor("Texture2D", (uint) sizeof(ID3D11Texture2D), 2048),
            });
            container.RegisterSingleton<IMemoryManager>(memoryManager);
        }

        public void Dispose()
        {
            _pipeline.Dispose();
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
