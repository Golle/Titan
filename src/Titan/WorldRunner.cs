using System;
using System.Linq;
using System.Threading;
using Titan.Core.Logging;
using Titan.Core.Services;
using Titan.ECS.TheNew;
using Titan.ECS.Worlds;

namespace Titan;

// NOTE(Jens): this is a bad name, rename it when we know what it does
internal class WorldRunner
{
    private readonly Type[] _baseSystems;
    private readonly WorldConfiguration[] _worlds;
    private readonly IServiceCollection _services;
    private Thread _worldThread;
    private volatile bool _active;

    public WorldRunner(Type[] baseSystems, WorldConfiguration[] worlds, IServiceCollection services)
    {
        _baseSystems = baseSystems;
        _worlds = worlds;
        _services = services;
    }

    public void Start(string name)
    {
        _worldThread = new Thread(Run);
        _active = true;
        _worldThread.Start(name);
    }

    public void Stop()
    {
        _active = false;
        _worldThread.Join();
        // Add stop function
    }

    public void Run(object obj)
    {
        var worldConfig = _worlds.First(w => w.Name == (string)obj);
        var nodes = CreateAndInitSystemNodes(worldConfig, _services);
        var dispatcher = new SystemsDispatcher_(nodes);


        while (_active)
        {
            dispatcher.Execute();
        }

        Logger.Info("Exiting the World Thread");
    }

    private Node[] CreateAndInitSystemNodes(WorldConfiguration config, IServiceCollection services)
    {
        var world = World.CreateWorld(config, services, true);
        var systems = config
            .Systems
            .Select(s => s.Type)
            .Concat(_baseSystems)
            .Select(Activator.CreateInstance)
            .Cast<BaseSystem>()
            .ToArray();

        return new DispatchTreeFactory()
            .Construct(systems);
    }
}
