using System;
using Titan.Core.Logging;
using Titan.ECS.Scheduler;
using Titan.ECS.Worlds;

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
    private ResourceCollection _resources;
    private World _world;
    public static void SetupAndRun(Action<AppBuilder> setup) => SetupAndRun(setup, AppCreationArgs.Default);
    public static void SetupAndRun(Action<AppBuilder> setup, AppCreationArgs args)
    {
        var appBuilder = AppBuilder.Create(args);
        try
        {
            setup(appBuilder);
            appBuilder
                .Build()
                .Run();

        }
        catch (Exception e)
        {
            Logger.Error<App>($"Build failed with {e.GetType().Name} and message {e.Message}");
            Logger.Shutdown();
        }
    }


    internal static App Create(ResourceCollection resources) =>
        new()
        {
            _resources = resources,
            _world = World.Create(resources)
        };

    internal void Run()
    {
        ref var scheduler = ref _resources.GetResource<Scheduler.Scheduler>();
        scheduler.Init(_resources, ref _world);

        _resources
            .GetResource<Runner>()
            .Run(ref scheduler, ref _world);


        Cleanup();
    }

    private void Cleanup()
    {
        //NOTE(Jens): release all resources and world maybe?
        //_resources.Reset();
    }
}
