using System;
using System.Diagnostics;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Messaging;
using Titan.Core.Threading;
using Titan.ECS.Assets;
using Titan.ECS.Systems.Dispatcher;
using Titan.ECS.World;
using Titan.EntitySystem;
using Titan.EntitySystem.Assets;
using Titan.EntitySystem.Components;
using Titan.Graphics;
using Titan.Graphics.Materials;
using Titan.Graphics.Resources;
using Titan.Graphics.States;
using Titan.Graphics.Textures;
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
        private readonly WorkerPool _workerPool;

        private IStartup _startup;
        private IWorld _world;
        private SystemsDispatcher _dispatcher;
        private IAssetTestInterface[] _loaders; // TEMP

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
        
        private Application(IWindow window, GraphicsSystem graphicsSystem, IEventQueue eventQueue, IMemoryManager memoryManager, IInputHandler inputHandler, WorkerPool workerPool, ILog log, IContainer container)
        {
            _window = window;
            _graphicsSystem = graphicsSystem;
            _eventQueue = eventQueue;
            _memoryManager = memoryManager;
            _inputHandler = inputHandler;
            _workerPool = workerPool;
            _log = log;
            _container = container;
        }


        private static WorldBuilder DefaultWorldBuilder() => new WorldBuilder()
            .WithComponent<Transform3D>()
            .WithComponent<Transform2D>()
            .WithComponent<Asset<Texture>>()
            .WithComponent<Texture>()
            .WithSystem<Transform3DSystem>()
            .WithAssetsLoader<Texture2DAssetsManager>();

        public void Run()
        {
            _log.Debug("Application starting");

            _log.Debug("Create and Configure the World");

            
            (_world, _dispatcher, _loaders) = _startup.ConfigureWorld(DefaultWorldBuilder()).Build(_container);



            _startup.OnStart(_world);
            
            StartMainLoop();
            
            _startup.OnStop(_world);
            _log.Debug("Application finished, exiting.");
        }

        private void StartMainLoop()
        {
            var s = Stopwatch.StartNew();
            var frames = 0;
            while (_window.Update()) // Window events + inputs (mouse and keyboard)
            {
                _eventQueue.Update(); // Make the last frame + new inputs avaialable in this frame
                _inputHandler.Update();


                {
                    // Currently only supports a single world
                    _world.Update();
                    foreach (var assetTestInterface in _loaders)
                    {
                        assetTestInterface.Update();
                    }

                    _dispatcher.Execute(_workerPool);
                }
                
                    
                _graphicsSystem.RenderFrame();
                
                // Temp code to see FPS
                frames++;
                if (s.Elapsed.TotalSeconds >= 1.0)
                {
                    var fps =(int) (frames / s.Elapsed.TotalSeconds);
                    _window.SetTitle(fps.ToString());
                    frames = 0;
                    s.Restart();
                }
            }
        }

        private void Initialize(GameConfiguration configuration)
        {
            _container.RegisterSingleton(new TitanConfiguration(configuration.AssetsDirectory.Path, 144, 1/60f, true)); // TODO: not sure if we want this
            
            // Use default logger for now
            LOGGER.InitializeLogger(_log);
            LOGGER.Debug("LOGGER initialized with type: {0}", _log.GetType().Name);

            LOGGER.Debug("Initialize EventQueue with max event queue size {0}", configuration.EventsConfiguration.MaxEventQueueSize);
            _eventQueue.Initialize(configuration.EventsConfiguration.MaxEventQueueSize);

            LOGGER.Debug("Initialize {0} with {1} workers and maximum of {2} queued jobs", nameof(WorkerPool), Environment.ProcessorCount - 1, 1000u);
            _workerPool.Initialize(new WorkerPoolConfiguration(1000u, (uint) (Environment.ProcessorCount - 1)));

            InitMemoryManager();

            LOGGER.Debug("Initialize Window with title '{0}' and dimensions {1}x{2}", configuration.DisplayConfiguration.Title, configuration.DisplayConfiguration.Width, configuration.DisplayConfiguration.Height);
            _window.Initialize((int) configuration.DisplayConfiguration.Width, (int) configuration.DisplayConfiguration.Height, configuration.DisplayConfiguration.Title);

            LOGGER.Debug("Initialize the Graphics System");
            _graphicsSystem.Initialize(configuration.AssetsDirectory.Path, configuration.DisplayConfiguration.RefreshRate, configuration.PipelineConfiguration, true);

            _startup = (IStartup)_container.CreateInstance(configuration.Startup);
        }
        
        private unsafe void InitMemoryManager()
        {
            LOGGER.Debug("Initialize memory manager");
            // TODO: not sure how to do this yet. Could have each manager "request" a memory chunk. This should be a part of the graphics system
            _memoryManager.Initialize(new[]
            {
                new ChunkDescriptor("VertexBuffer", (uint) sizeof(VertexBuffer), 2048),
                new ChunkDescriptor("IndexBuffer", (uint) sizeof(IndexBuffer), 2048),
                new ChunkDescriptor("ConstantBuffer", (uint) sizeof(ConstantBuffer), 100),
                new ChunkDescriptor("Materials", (uint) sizeof(Material), 256),
                new ChunkDescriptor("Shaders", (uint) IntPtr.Size, 1024),
                new ChunkDescriptor("Texture", (uint) sizeof(Texture2D), 1024),
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
            _workerPool.Dispose();
            (_startup as IDisposable)?.Dispose();
        }
    }
}
