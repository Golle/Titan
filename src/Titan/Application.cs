using System;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Messaging;
using Titan.Graphics;
using Titan.Graphics.Materials;
using Titan.Graphics.Resources;
using Titan.Graphics.States;
using Titan.Input;
using Titan.IOC;
using Titan.Windows;

namespace Titan
{
    public class Application : IDisposable
    {
        private readonly IContainer _container;
        private readonly ILog _log;

        private readonly IWindow _window;
        private readonly GraphicsSystem _graphicsSystem;
        private readonly IEventQueue _eventQueue;
        private readonly IMemoryManager _memoryManager;
        private readonly IInputHandler _inputHandler;

        public static Application Create(GameConfigurationBuilder configurationBuilder)
        {
            var container = Bootstrapper.CreateContainer();
            var loader = container.CreateInstance<ConfigurationFileLoader>();

            var configuration = configurationBuilder.Build(loader);
            
            container.RegisterSingleton(container);
            
            var application = container.CreateInstance<Application>();
            application.Initialize(configuration);
            return application;
        }
        
        private Application(IWindow window, GraphicsSystem graphicsSystem, IEventQueue eventQueue, IMemoryManager memoryManager, IInputHandler inputHandler, ILog log, IContainer container)
        {
            _window = window;
            _graphicsSystem = graphicsSystem;
            _eventQueue = eventQueue;
            _memoryManager = memoryManager;
            _inputHandler = inputHandler;
            _log = log;
            _container = container;
        }
        
        public void Run()
        {
            StartMainLoop();
        }

        private void StartMainLoop()
        {
            while (_window.Update()) // Window events + inputs (mouse and keyboard)
            {
                _eventQueue.Update();
                _inputHandler.Update();
                
                /*
                 *Insert multithreaded game system update here
                 */
                
                _graphicsSystem.RenderFrame();
            }
        }

        private void Initialize(GameConfiguration configuration)
        {
            _container.RegisterSingleton(new TitanConfiguration(configuration.AssetsDirectory.Path, 144, 0.1f, true)); // TODO: not sure if we want this
            
            // Use default logger for now
            LOGGER.InitializeLogger(_log);
            LOGGER.Debug("LOGGER initialized with type: ", _log.GetType().Name);

            LOGGER.Debug("Initialize EventQueue with max event queue size {0}", configuration.EventsConfiguration.MaxEventQueueSize);
            _eventQueue.Initialize(configuration.EventsConfiguration.MaxEventQueueSize);
            
            InitMemoryManager();

            LOGGER.Debug("Initialize Window with title '{0}' and dimensions {1}x{2}", configuration.DisplayConfiguration.Title, configuration.DisplayConfiguration.Width, configuration.DisplayConfiguration.Height);
            _window.Initialize((int) configuration.DisplayConfiguration.Width, (int) configuration.DisplayConfiguration.Height, configuration.DisplayConfiguration.Title);

            LOGGER.Debug("Initialize the Graphics System");
            _graphicsSystem.Initialize(configuration.AssetsDirectory.Path, configuration.DisplayConfiguration.RefreshRate, configuration.PipelineConfiguration, true);
        }
        
        private unsafe void InitMemoryManager()
        {
            LOGGER.Debug("Initialize memory manager");
            // TODO: not sure how to do this yet. Could have each manager "request" a memory chunk.
            _memoryManager.Initialize(new[]
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
        }

        public void Dispose()
        {
            _graphicsSystem?.Dispose();
            _window?.Dispose();
            _container.Dispose();
            _memoryManager.Dispose();
        }
    }
}
