using System;
using System.Threading;
using Titan.Core.Logging;
using Titan.ECS.TheNew;

namespace Titan;

internal class GameLoop : IDisposable
{

    private World_ _current;
    private readonly World_[] _worlds;

    private readonly Thread _worldThread;

    private volatile bool _active;
    public GameLoop(World_[] worlds)
    {
        _worlds = worlds;
        _worldThread = new Thread(Run)
        {
            Name = "WorldThread",
            Priority = ThreadPriority.Normal
        };
    }

    public static GameLoop InitAndStart(World_[] worlds)
    {
        var gameloop = new GameLoop(worlds);
        gameloop._current = worlds[0]; //TODO: make this configurable
        
        gameloop.Start();
        return gameloop;
    }

    public void Start()
    {
        _worldThread.Start();
    }

    private void Run()
    {
        _active = true;

        Logger.Info<GameLoop>($"Starting {nameof(GameLoop)}");
        Logger.Info<GameLoop>($"World: {_current.Name} Init");

        _current.Init();
        while (_active)
        {
            // Check for change world event
            // Change the world (call teardown + init)
            
            _current.Update();
        }
        Logger.Info<GameLoop>($"World: {_current.Name} Teardown");
        _current.Teardown();
        Logger.Info<GameLoop>("Exiting GameLoop");
    }

    public void Stop()
    {
        if (_active)
        {
            _active = false;
            _worldThread.Join();
        }
    }

    public void Dispose()
    {
        Stop();
    }
}
