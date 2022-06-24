using System;
using System.Linq;
using Titan.Core.Logging;
using Titan.Core.Threading;
using Titan.ECS;
using Titan.ECS.TheNew;
using Titan.Graphics.Windows;

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

        


        using var gameloop = GameLoop.InitAndStart(worlds);
        while (window.Update())
        {
        }
    }

    private static SystemsBuilder SetupSystemsBuilder() =>
        new SystemsBuilder()
            .WithSystem<TestSystem1>()
            .WithSystem<TestSystem2>()
            .WithSystem<TestSystem3>()
            .WithSystem<TestSystem4>();

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
