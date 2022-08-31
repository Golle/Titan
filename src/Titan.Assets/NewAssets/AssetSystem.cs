using System.Diagnostics.Tracing;
using Titan.Core.Logging;
using Titan.ECS.Events;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;

namespace Titan.Assets.NewAssets;

internal struct AssetSystem : IStructSystem<AssetSystem>
{
    private MutableResource<AssetRegistry> AssetRegistry;
    private MutableResource<AssetLoader> AssetLoader;

    private EventsReader<AssetLoadRequested> AssetLoad;
    private EventsReader<AssetUnloadRequested> AssetUnload;

    public static void Init(ref AssetSystem system, in SystemsInitializer init)
    {
        //NOTE(Jens): we can replace these resources with raw pointers. might not make a difference.
        system.AssetLoader = init.GetMutableResource<AssetLoader>();

        system.AssetLoad = init.GetEventsReader<AssetLoadRequested>();
        system.AssetUnload = init.GetEventsReader<AssetUnloadRequested>();
    }

    public static void Update(ref AssetSystem system)
    {
        ref var loader = ref system.AssetLoader.Get();
        
        //NOTE(Jens): Handle cases where Unload and Load are called the same frame. Assert or just "leave" the asset as is?

        if (system.AssetUnload.HasEvents())
        {
            foreach (ref readonly var @event in system.AssetUnload)
            {
                loader.Unload(@event.Handle);
                Logger.Trace<AssetSystem>($"Unloaded asset {@event.Handle}");
            }
        }

        if (system.AssetLoad.HasEvents())
        {
            foreach (ref readonly var @event in system.AssetLoad)
            {
                loader.Load(@event.Handle);
                Logger.Trace<AssetSystem>($"Load asset {@event.Handle}");
            }
        }
        
        loader.Update();
    }

    public static bool ShouldRun(in AssetSystem system)
        => system.AssetLoad.HasEvents() || system.AssetUnload.HasEvents() || system.AssetLoader.Get().RequiresUpdate();
}
