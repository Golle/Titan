using System;
using System.Diagnostics;
using Titan.Assets;
using Titan.Components;
using Titan.Core;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.Core.Threading;
using Titan.ECS;
using Titan.ECS.Worlds;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.Images;
using Titan.Graphics.Loaders;
using Titan.Graphics.Loaders.Atlas;
using Titan.Graphics.Loaders.Fonts;
using Titan.Graphics.Loaders.Materials;
using Titan.Graphics.Loaders.Models;
using Titan.Graphics.Loaders.Shaders;
using Titan.Graphics.Windows;
using Titan.Input;
using Titan.Rendering;
using Titan.Systems;
using Titan.UI;
using Titan.UI.Rendering;

namespace Titan
{
    public record EngineConfiguration
    {
        public string AssetsPath { get; init; }
        public uint MaxEvents { get; init; }
    }

    public class Engine
    {
        private readonly Application _app;

        public static void StartNew<T>() where T : Application, new()
        {
            try
            {
                new Engine(new T())
                    .Start();
            }
            catch
            {
                // ignored
            }
            finally
            {
                Logger.Shutdown();
            }
        }

        private Engine(Application app)
        {
            _app = app;
        }

        public void Start()
        {
            static void Info(string message) => Logger.Info<Engine>(message);
            static void Trace(string message) => Logger.Trace<Engine>(message);

            Logger.Start();

            var engineConfig = _app.ConfigureEngine(new EngineConfiguration { MaxEvents = 10_000 });
            if (string.IsNullOrWhiteSpace(engineConfig.AssetsPath))
            {
                Logger.Error<Engine>($"{nameof(EngineConfiguration.AssetsPath)} is not set. Must be a valid relative path.");
                return;
            }

            if (engineConfig.MaxEvents == 0)
            {
                Logger.Error<Engine>($"{nameof(EngineConfiguration.MaxEvents)} is set to 0. Must be a valid positive number.");
                return;
            }

            Trace($"Init {nameof(EventManager)}");
            EventManager.Init(new EventManagerConfiguration(engineConfig.MaxEvents));

            Trace($"Init {nameof(FileSystem)}");
            FileSystem.Init(new FileSystemConfiguration(engineConfig.AssetsPath));

            Trace($"Init {nameof(WorkerPool)}");
            WorkerPool.Init(new WorkerPoolConfiguration(100, (uint) ((Environment.ProcessorCount/2) - 1)));
            
            Trace($"Init {nameof(IOWorkerPool)}");
            IOWorkerPool.Init(2, 100);
            
            Trace($"Configure the {nameof(Window)}");
            var windowConfig = _app.ConfigureWindow(new WindowConfiguration(_app.GetType().Name, 800, 600, true));
            Trace($"Creating the {nameof(Window)}");
            if (!Window.Init(windowConfig))
            {
                Logger.Error("Failed to init the window.", typeof(Engine));
            }

            Trace($"Showing the {nameof(Window)}");
            Window.Show();
            _app.Window = new GameWindow();

            Trace($"Configure {nameof(GraphicsDevice)}");
            var deviceConfig = _app.ConfigureDevice(new DeviceConfiguration(windowConfig.Width, windowConfig.Height, 144, windowConfig.Windowed, true, true, true));
            Trace($"Init {typeof(GraphicsDevice).FullName}");
            GraphicsDevice.Init(deviceConfig, Window.Handle);

            Trace($"Init {nameof(Resources)}");
            Resources.Init();
            
            Info("Engine has been initialized.");

            try
            {
                Run();
            }
            catch (Exception e)
            {
                Logger.Error("Exception was thrown at startup.");
                Logger.Error(e.Message);
                Logger.Error(e.StackTrace);
            }
            finally
            {
                Shutdown();
            }
        }
        
        private unsafe void Run()
        {
            using var fontManager = new FontManager();
            using var atlasManager = new AtlasManager(100);
            var assetsManager = new AssetsManager()
                .Register(AssetTypes.Texture, new TextureLoader(new WICImageLoader()))
                .Register(AssetTypes.Model, new ModelLoader(Resources.Models))
                .Register(AssetTypes.VertexShader, new VertexShaderLoader())
                .Register(AssetTypes.PixelShader, new PixelShaderLoader())
                .Register(AssetTypes.Material, new MaterialsLoader())
                .Register(AssetTypes.Atlas, new AtlasLoader(atlasManager))
                .Register(AssetTypes.Font, new FontLoader(fontManager))
                .Init(new AssetManagerConfiguration("manifest.json", 2));

            var renderQueue = new SimpleRenderQueue(1000);
            var uiRenderQueue = new UIRenderQueue(1000);

            var color = stackalloc float[4];
            color[0] = 1f;
            color[1] = 0.4f;
            color[2] = 0f;
            color[3] = 1f;

            var pipelineBuilder = new PipelineBuilder(assetsManager, renderQueue, uiRenderQueue);
            pipelineBuilder.LoadResources();
            // Preload assets for rendering pipeline
            while (Window.Update() && !pipelineBuilder.IsReady())
            {
                assetsManager.Update();
            }
            
            var pipeline = pipelineBuilder.Create();
            using var graphicsSystem = new GraphicsSystem(pipeline);

            var worldBuilder = new WorldBuilder(defaultMaxEntities: 10_000)
                .WithComponent<Transform3D>()
                .WithComponent<CameraComponent>()
                .WithComponent<AssetComponent<Model>>()
                .WithComponent<ModelComponent>()

                .WithSystem(new Transform3DSystem())
                .WithSystem(new Render3DSystem(assetsManager, renderQueue))
                .WithSystem(new CameraSystem(graphicsSystem))
                .WithSystem(new ModelLoaderSystem(assetsManager))

                
                .WithDefaultUI(new UIConfiguration(), uiRenderQueue, assetsManager, atlasManager)
                ;
            _app.ConfigureWorld(worldBuilder);

            Logger.Info<Engine>("Initialize starter world");
            using var starterWorld = new World(worldBuilder.Build());
            _app.OnStart(starterWorld);

            var timer = Stopwatch.StartNew();
            // star the main loop
            while (Window.Update())
            {
                renderQueue.Update();
            
                timer.Restart();
                EventManager.Update();
                EngineStats.SetStats(nameof(EventManager), timer.Elapsed.TotalMilliseconds);
                timer.Restart();
                InputManager.Update();
                EngineStats.SetStats(nameof(InputManager), timer.Elapsed.TotalMilliseconds);
                timer.Restart();

                starterWorld.Update();
                EngineStats.SetStats(nameof(World), timer.Elapsed.TotalMilliseconds);
                timer.Restart();
                assetsManager.Update();
                EngineStats.SetStats(nameof(AssetsManager), timer.Elapsed.TotalMilliseconds);
                timer.Restart();
                graphicsSystem.Render();
                EngineStats.SetStats(nameof(GraphicsSystem), timer.Elapsed.TotalMilliseconds);
                timer.Restart();
            }
        }

        private void Shutdown()
        {
            Logger.Info<Engine>("Disposing the application");
            _app.OnTerminate();

            Logger.Info<Engine>("Disposing the engine");

            Logger.Trace<Engine>($"Terminate {nameof(WorkerPool)}");
            WorkerPool.Terminate();

            Logger.Trace<Engine>($"Terminate {nameof(IOWorkerPool)}");
            IOWorkerPool.Terminate();
            
            Logger.Trace<Engine>($"Terminate {nameof(GraphicsDevice)}");
            GraphicsDevice.Terminate();

            Logger.Trace<Engine>($"Terminate {nameof(Resources)}");
            Resources.Terminate();

            Logger.Trace<Engine>($"Close/Dispose {nameof(Window)}");
            Window.Destroy();

            Logger.Trace<Engine>($"Terminate {nameof(FileSystem)}");
            FileSystem.Terminate();

            Logger.Trace<Engine>($"Terminate {nameof(EventManager)}");
            EventManager.Terminate();
        }
    }
}
