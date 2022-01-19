using System;
using System.Diagnostics;
using Titan.Assets;
using Titan.Components;
using Titan.Core;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.Core.Services;
using Titan.Core.Threading;
using Titan.ECS;
using Titan.ECS.Components;
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
using Titan.Graphics.Rendering;
using Titan.Graphics.Rendering.Geometry;
using Titan.Graphics.Rendering.Sprites;
using Titan.Graphics.Rendering.Text;
using Titan.Graphics.Windows;
using Titan.Graphics.Windows.Events;
using Titan.Input;
using Titan.Pipeline;
using Titan.Sound;
using Titan.Sound.Loaders;
using Titan.Systems;
using Titan.UI;
using Titan.UI.Debugging;

namespace Titan;

public record EngineConfiguration
{
    public string AssetsPath { get; init; }
    public uint MaxEvents { get; init; }
    public string BasePathSearchPattern { get; init; }
}

public class Engine
{
    private readonly Game _app;

    public static void Start(Game game)
    {
        try
        {
            new Engine(game)
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
    private Engine(Game app)
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
        FileSystem.Init(new FileSystemConfiguration(engineConfig.AssetsPath, engineConfig.BasePathSearchPattern));

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

    private void Run()
    {
        using var services = new ServiceCollection()
            .Register(new GameWindow())
            .Register(new FontManager())
            .Register(new AtlasManager(100))
            .Register(new TextManager(200))
            .Register(new SoundManager());
            
        var assetsManager = new AssetsManager()
            .Register(AssetTypes.Texture, new TextureLoader(new WICImageLoader()))
            .Register(AssetTypes.Model, new ModelLoader(Resources.Models))
            .Register(AssetTypes.VertexShader, new VertexShaderLoader())
            .Register(AssetTypes.PixelShader, new PixelShaderLoader())
            .Register(AssetTypes.Material, new MaterialsLoader())
            .Register(AssetTypes.Atlas, new AtlasLoader(services.Get<AtlasManager>()))
            .Register(AssetTypes.Font, new FontLoader(services.Get<FontManager>()))
            .Register(AssetTypes.Wave, new WaveLoader(services.Get<SoundManager>()))
            .Init(new AssetManagerConfiguration(new[]
            {
                "manifest.json",
                "builtin/manifest.json",
                "builtin/debug_manifest.json"
            }, MaxConcurrentFileReads: 2));

        services
            .Register(assetsManager)
            //Rendering (Queues)
            .Register(new SimpleRenderQueue(1000))
            .Register(new SpriteRenderQueue(new UIRenderQueueConfiguration(), services.Get<TextManager>(), services.Get<FontManager>()))
            
            // NOTE(jens): These should be excluded by some compile flag
            .Register(new BoundingBoxRenderQueue())
            .Register(new BoundingBoxRenderer(services.Get<BoundingBoxRenderQueue>()))
            .Register(new DeferredShadingRenderer())
            // Note(jens): End note
            
            // Renderers
            .Register(new FullscreenRenderer())
            .Register(new SpriteRenderer(services.Get<SpriteRenderQueue>()))
            .Register(new GeometryRenderer(services.Get<SimpleRenderQueue>()))
            
            // Sound system
            .Register(new SoundSystem(services.Get<SoundManager>()))
            ;

        var renderingPipeline = _app.ConfigureRenderingPipeline();
        {
            var config = _app.ConfigureCollisionMatrix();
            if (config != null)
            {
                services.Register(config);
            }
        }

        GraphicsSystem graphicsSystem;
        PipelineBuilder pipelineBuilder;
        {
            // Rendering pipeline
            pipelineBuilder = renderingPipeline switch
            {
                RenderingPipeline.Render2D => new PipelineBuilder2D(),
                RenderingPipeline.Render3D => new PipelineBuilder3D(),
                RenderingPipeline.Custom => throw new NotSupportedException("Custom pipeline is not supported yet."),
                _ => throw new ArgumentOutOfRangeException()
            };
            pipelineBuilder.LoadResources(assetsManager);
            // Preload assets for rendering pipeline
            while (Window.Update() && !pipelineBuilder.IsReady(assetsManager))
            {
                assetsManager.Update();
            }
            
            graphicsSystem = new GraphicsSystem();
            graphicsSystem.Init(pipelineBuilder.BuildPipeline(services));
            services.Register(graphicsSystem);
        }



        World starterWorld;
        // The starter world
        {
            var worldBuilder = new WorldBuilder(defaultMaxEntities: 10_000)
                    .WithComponent<CameraComponent>(ComponentPoolTypes.DynamicPacked, 2)
                    .WithComponent<Transform3D>()
                    .WithSystem<Transform3DSystem>()
                    .WithSystem<CameraSystem>()
                ;
            Logger.Info<Engine>("Initialize starter world");
            _app.ConfigureStarterWorld(worldBuilder);
            starterWorld = World.CreateWorld(worldBuilder, services, true);
        }
            
        _app.OnStart(starterWorld, new UIManager(starterWorld, services.Get<TextManager>()));
            
        var timer = Stopwatch.StartNew();
        // Collect any garbage created at setup
        GC.Collect();

        // star the main loop
        while (Window.Update())
        {
            timer.Restart();
            services.PreUpdate();
            EngineStats.SetStats("Services.PreUpdate()", timer.Elapsed.TotalMilliseconds);
            timer.Restart();
            EventManager.Update();
            EngineStats.SetStats(nameof(EventManager), timer.Elapsed.TotalMilliseconds);
            timer.Restart();
            InputManager.Update();
            EngineStats.SetStats(nameof(InputManager), timer.Elapsed.TotalMilliseconds);
            timer.Restart();
            services.Update();
            EngineStats.SetStats("Services.Update()", timer.Elapsed.TotalMilliseconds);
            timer.Restart();
            World.UpdateWorlds();
            EngineStats.SetStats(nameof(World), timer.Elapsed.TotalMilliseconds);
            timer.Restart();
            services.PostUpdate();
            EngineStats.SetStats("Services.PostUpdate()", timer.Elapsed.TotalMilliseconds);
            timer.Restart();
            assetsManager.Update();
            EngineStats.SetStats(nameof(AssetsManager), timer.Elapsed.TotalMilliseconds);
            timer.Restart();

            graphicsSystem.Render();
            EngineStats.SetStats(nameof(GraphicsSystem), timer.Elapsed.TotalMilliseconds);
            timer.Restart();

            foreach (ref readonly var @event in EventManager.GetEvents())
            {
                if (@event.Type == WindowResizedEvent.Id)
                {
                    ref readonly var resizeEvent = ref @event.As<WindowResizedEvent>();
                    GraphicsDevice.Resize(resizeEvent.Width, resizeEvent.Height);
                    graphicsSystem.Init(pipelineBuilder.BuildPipeline(services));
                }
            }
            
        }

        World.DisposeWorlds();
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
