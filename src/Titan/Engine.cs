using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using Titan.Assets;
using Titan.Assets.Materials;
using Titan.Assets.Models;
using Titan.Assets.Shaders;
using Titan.Components;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.Core.Threading;
using Titan.ECS.Components;
using Titan.ECS.Worlds;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.Images;
using Titan.Graphics.Windows;
using Titan.Input;
using Titan.Rendering;
using Titan.Systems;

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

        private Window _window;
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
            _window = Window.Create(windowConfig);
            Trace($"Showing the {nameof(Window)}");
            _window.Show();
            _app.Window = new GameWindow(_window);

            Trace($"Configure {nameof(GraphicsDevice)}");
            var deviceConfig = _app.ConfigureDevice(new DeviceConfiguration(60, true, true));
            Trace($"Init {typeof(GraphicsDevice).FullName}");
            GraphicsDevice.Init(_window, deviceConfig);

            Trace($"Init {nameof(Resources)}");
            Resources.Init();
            
            Info("Engine has been initialized.");
            Info("Initialize Application.");
            _app.OnStart();
            try
            {
                Run();
            }
            finally
            {
                Shutdown();
            }
        }
        
        private unsafe void Run()
        {
            var assetsManager = new AssetsManager()
                .Register(AssetTypes.Texture, new TextureLoader(new WICImageLoader()))
                .Register(AssetTypes.Model, new ModelLoader(Resources.Models))
                .Register(AssetTypes.VertexShader, new VertexShaderLoader())
                .Register(AssetTypes.PixelShader, new PixelShaderLoader())
                .Register(AssetTypes.Material, new MaterialsLoader())
                .Init(new AssetManagerConfiguration("manifest.json", 2));

            var renderQueue = new SimpleRenderQueue(1000);
            
            var color = stackalloc float[4];
            color[0] = 1f;
            color[1] = 0.4f;
            color[2] = 0f;
            color[3] = 1f;

            var timer = Stopwatch.StartNew();
            var frameCount = 0;
            
            var pipelineBuilder = new PipelineBuilder(assetsManager, renderQueue);
            pipelineBuilder.LoadResources();
            // Preload assets for rendering pipeline
            while (_window.Update() && !pipelineBuilder.IsReady())
            {
                assetsManager.Update();
            }
            
            var pipeline = pipelineBuilder.Create();
            using var graphicsSystem = new GraphicsSystem(pipeline);

            var systemCollection = new SystemsCollection()
                .Add(new Transform3DSystem())
                .Add(new Render3DSystem(assetsManager, renderQueue))
                .Add(new CameraSystem(graphicsSystem))
                .Add(new ModelLoaderSystem(assetsManager))
                ;
            _app.ConfigureSystems(systemCollection);

            using var world = new World(new WorldConfiguration(10_000, new[]
                {
                    new ComponentConfiguration(typeof(Transform3D), ComponentPoolTypes.Packed),
                    new ComponentConfiguration(typeof(CameraComponent), ComponentPoolTypes.Packed),
                    new ComponentConfiguration(typeof(AssetComponent<Model>), ComponentPoolTypes.Packed),
                    new ComponentConfiguration(typeof(ModelComponent), ComponentPoolTypes.Packed)
                },
                systemCollection.Systems.ToArray()
            ));

            for (var i = 0; i < 10; ++i)
            {
                for (var j = 0; j < 10; ++j)
                {
                    {
                        var tree = world.CreateEntity();
                        tree.AddComponent(new Transform3D { Scale = Vector3.One, Rotation = Quaternion.Identity, Position = new Vector3(i * 4.15f, 0, j * 2.2f) });
                        tree.AddComponent(new AssetComponent<Model>("models/tree"));
                    }
                }
            }
            
            var entity2 = world.CreateEntity();
            entity2.AddComponent(new Transform3D{Position = new Vector3(0, 10, 60), Rotation = Quaternion.Identity, Scale = Vector3.One});
            entity2.AddComponent(CameraComponent.CreatePerspective(2560, 1440, 0.5f, 10000f));
            

            // star the main loop
            while (_window.Update())
            {
                renderQueue.Update();

                EventManager.Update();
                InputManager.Update();

                world.Update();

                if (timer.Elapsed.Seconds >= 1f)
                {
                    var elapsed = timer.Elapsed;
                    
                    _window.SetTitle($"FPS: {(int)(frameCount/elapsed.TotalSeconds)}");

                    timer.Restart();
                    frameCount = 0;
                }
                assetsManager.Update();
                graphicsSystem.Render();

                frameCount++;
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
            _window.Dispose();

            Logger.Trace<Engine>($"Terminate {nameof(FileSystem)}");
            FileSystem.Terminate();

            Logger.Trace<Engine>($"Terminate {nameof(EventManager)}");
            EventManager.Terminate();
        }
    }
}
