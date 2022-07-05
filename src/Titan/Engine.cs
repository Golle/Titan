using System.Linq;
using Titan.Assets;
using Titan.Core.Logging;
using Titan.Core.Services;
using Titan.Core.Threading;
using Titan.ECS;
using Titan.ECS.TheNew;
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
using Titan.Sound;
using Titan.Sound.Loaders;
using Titan.UI.Debugging;

namespace Titan;




/// <summary>
/// The Engine startup class that will host the Game
/// </summary>
public class Engine
{
    private readonly Game _game;
    public static void Start(Game game)
    {
        Logger.Start();
        try
        {
            Logger.Info("Engine startup up!", typeof(Engine));
            SetupCoreSystems();
            new Engine(game)
                .Run();
            
        }
        catch (Exception e)
        {
            Logger.Error($"{e.GetType().Name} was thrown in the game loop with the message: {e.Message} ", typeof(Engine));
        }
        finally
        {
            Logger.Info("Engine shutting down!", typeof(Engine));
            TeardownCoreSystems();
            Logger.Shutdown();
        }
    }

    public Engine(Game game)
    {
        _game = game;
    }

    private void Run()
    {
        var engineConfig = _game.ConfigureEngine(new EngineConfiguration());
        var windowConfig = _game.ConfigureWindow(new WindowConfiguration("n/a", 800, 600));
        
        using var window = Window.Create(windowConfig);

        // Setup the Window
        {
            if (window == null)
            {
                Logger.Error<Engine>("Failed to create the Window.");
                return;
            }
            window.Show();
        }

        var systemsConfiguration = _game.ConfigureSystems(SetupSystemsBuilder());
        var worldConfigurations = _game.ConfigureWorlds();

        var worlds = worldConfigurations
            .Select(config => WorldFactory.Create(config, systemsConfiguration))
            .ToArray();


        // old
        //using var services = new ServiceCollection()
        //    .Register(new FontManager())
        //    .Register(new AtlasManager(100))
        //    .Register(new TextManager(200))
        //    .Register(new SoundManager());

        //var assetsManager = new AssetsManager()
        //    .Register(AssetTypes.Texture, new TextureLoader(new WICImageLoader()))
        //    .Register(AssetTypes.Model, new ModelLoader(Resources.Models))
        //    .Register(AssetTypes.VertexShader, new VertexShaderLoader())
        //    .Register(AssetTypes.PixelShader, new PixelShaderLoader())
        //    .Register(AssetTypes.ComputeShader, new ComputeShaderLoader())
        //    .Register(AssetTypes.Material, new MaterialsLoader())
        //    .Register(AssetTypes.Atlas, new AtlasLoader(services.Get<AtlasManager>()))
        //    .Register(AssetTypes.Font, new FontLoader(services.Get<FontManager>()))
        //    .Register(AssetTypes.Wave, new WaveLoader(services.Get<SoundManager>()))
        //    .Init(new AssetManagerConfiguration(new[]
        //    {
        //        "manifest.json",
        //        "builtin/manifest.json",
        //        "builtin/debug_manifest.json"
        //    }, MaxConcurrentFileReads: 2));

        //services
        //    .Register(assetsManager)
        //    //Rendering (Queues)
        //    .Register(new SimpleRenderQueue(1000))
        //    .Register(new SpriteRenderQueue(new UIRenderQueueConfiguration(), services.Get<TextManager>(), services.Get<FontManager>()))

        //    // NOTE(jens): These should be excluded by some compile flag
        //    .Register(new BoundingBoxRenderQueue())
        //    .Register(new BoundingBoxRenderer(services.Get<BoundingBoxRenderQueue>()))
        //    .Register(new DeferredShadingRenderer())
        //    // Note(jens): End note

        //    // Renderers
        //    .Register(new FullscreenRenderer())
        //    .Register(new SpriteRenderer(services.Get<SpriteRenderQueue>()))
        //    .Register(new GeometryRenderer(services.Get<SimpleRenderQueue>()))

        //    // Sound system
        //    .Register(new SoundSystem(services.Get<SoundManager>()))
        //    ;

        //var renderingPipeline = _game.ConfigureRenderingPipeline();

        using var gameloop = GameLoop.InitAndStart(worlds);
        while (window.Update())
        {
        }
    }

    private static SystemsBuilder SetupSystemsBuilder() =>
        new SystemsBuilder();

    private static void SetupCoreSystems()
    {
        Logger.Info($"Init {nameof(WorkerPool)}", typeof(Engine));
        WorkerPool.Init(new WorkerPoolConfiguration(1000, (uint)(Environment.ProcessorCount-1)));
    }

    private static void TeardownCoreSystems()
    {
        Logger.Info($"Teardown {nameof(WorkerPool)}", typeof(Engine));
        WorkerPool.Terminate();
    }
}
