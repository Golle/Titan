using System;
using Titan.Core.Logging;
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
        if (window == null)
        {
            Logger.Error<Engine>("Failed to create the Window.");
            return;
        }
        window.Show();


        
        
        
        while (window.Update())
        {
            
        }
    }
}
