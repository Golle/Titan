using Titan.Core.Memory;
using Titan.ECS.Scheduler;
using Titan.ECS.World;

namespace Titan.ECS.App;

/*
 * State (This would be the Scene management in Titan)
 *  OnLoad,
 *  OnEnter,
 *  OnLeave,
 *  OnUnload
 *
 * Sample state: Splash, MainMenu, WorldGeneration, Game, GameEnded etc.
 */


/*
 * Entities and Components are registered in the Global space
 * Systems are global by default, but can be a part of a state as well. A state has different states
 *
 * Entities - Global
 * Components - Global
 * Resources - Global (No local in first version)
 * Systems - Global  (No local in first version)
 *
 *
 */

public struct App
{
    private MemoryPool _pool;
    private ResourceCollection _resources;
    private World.World _world;

    internal void Init(in MemoryPool pool, in ResourceCollection resources)
    {
        _pool = pool;
        _resources = resources;
        _world.Init(pool, resources);
        
        resources
            .GetResource<Scheduler.Scheduler>()
            .Init(_resources, ref _world);
    }

    public void Run()
    {
        ref var scheduler = ref _resources.GetResource<Scheduler.Scheduler>();

        _resources
            .GetResource<Runner>()
            .Run(ref scheduler, ref _world);

        Cleanup();
    }

    private void Cleanup()
    {
        _pool.Dispose();
    }
}
