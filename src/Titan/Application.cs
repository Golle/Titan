using System;
using System.IO;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Messaging;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.Materials;
using Titan.Graphics.Pipeline;
using Titan.Graphics.Resources;
using Titan.Graphics.States;
using Titan.IOC;
using Titan.Windows;

namespace Titan
{
    public class Application : IDisposable
    {
        private readonly IContainer _container = Bootstrapper.CreateContainer();
        private IWindow _window;
        private IGraphicsDevice _device;
        private IGraphicsPipeline _pipeline;
        private IEventQueue _eventQueue;

        public static Application Create(GameConfigurationBuilder configuration) => new(configuration.Build());
        private Application(GameConfiguration configuration) => Initialize(configuration);
        
        public void Run()
        {
            StartMainLoop();
        }

        private void StartMainLoop()
        {
            while (_window.Update())
            {
                _eventQueue.Update();
                _pipeline.Execute();
            }
        }

        private void Initialize(GameConfiguration configuration)
        {
            _container.RegisterSingleton(_container);
            _container.RegisterSingleton(new TitanConfiguration(configuration.AssetsDirectory.Path, 144, 0.1f, true)); // TODO: not sure if we want this
            InitLogger(configuration.LoggerConfiguration);
            InitEventsQueue();
            
            InitMemoryManager();
            
            InitDisplay(configuration.DisplayConfiguration);
            InitPipeline(configuration.PipelineConfiguration);
            

        }

        private unsafe void InitMemoryManager()
        {
            LOGGER.Debug("Initialize memory manager");
            // TODO: not sure how to do this yet. Could have each manager "request" a memory chunk.
            var memoryManager = new MemoryManager(new[]
            {
                new ChunkDescriptor("VertexBuffer", (uint) sizeof(VertexBuffer), 2048),
                new ChunkDescriptor("IndexBuffer", (uint) sizeof(IndexBuffer), 2048),
                new ChunkDescriptor("ConstantBuffer", (uint) sizeof(ConstantBuffer), 100),
                new ChunkDescriptor("Materials", (uint) sizeof(Material), 256),
                new ChunkDescriptor("Shaders", (uint) IntPtr.Size, 1024),
                new ChunkDescriptor("Texture", (uint) sizeof(Graphics.Resources.Texture), 1024),
                new ChunkDescriptor("ShaderResourceView", (uint) sizeof(ShaderResourceView), 1024),
                new ChunkDescriptor("RenderTargetView", (uint) sizeof(RenderTargetView), 1024),
                new ChunkDescriptor("DepthStencilView", (uint) sizeof(DepthStencilView), 10),
                new ChunkDescriptor("DepthStencilState", (uint) sizeof(DepthStencilState), 10),
                new ChunkDescriptor("SamplerState", (uint) sizeof(SamplerState), 20),
            });
            _container.RegisterSingleton<IMemoryManager>(memoryManager);
        }

        private void InitPipeline(PipelineConfiguration config)
        {
            
            _container.RegisterSingleton(_pipeline = _container.CreateInstance<GraphicsPipeline>());
            LOGGER.Debug("Initialize Graphics pipeline from {0}", config.Path);
            _pipeline.Initialize(Path.GetFileName(config.Path)); // TODO: temp since we expect just a filename in the pipeline at the moment
        }

        private void InitLogger(LoggerConfiguration _)
        {
            // Use default logger for now
            var logger = _container.GetInstance<ILog>();
            LOGGER.InitializeLogger(logger);
            LOGGER.Debug("LOGGER initialized with type: ", logger.GetType().Name);
        }

        private void InitEventsQueue()
        {
            _container.RegisterSingleton(_eventQueue = _container.CreateInstance<EventQueue>());
            LOGGER.Debug("Initialize EventQueue with {0}", typeof(ScanningEventTypeProvider));
            _eventQueue.Initialize(new ScanningEventTypeProvider());
        }

        private void InitDisplay(DisplayConfiguration config)
        {
            LOGGER.Debug("Initialize Window with title '{0}' and dimensions {1}x{2}", config.Title, config.Width, config.Height);
            _container.RegisterSingleton(_window = _container.GetInstance<IWindowFactory>().Create(config.Width, config.Height, config.Title));

            LOGGER.Debug("Initialize the D3D11 Device");
            _container.RegisterSingleton(_device = _container.CreateInstance<GraphicsDevice>());
        }

        public void Dispose()
        {
            _pipeline?.Dispose();
            _device?.Dispose();
            _window?.Dispose();
            _container.Dispose();
        }
    }
}
