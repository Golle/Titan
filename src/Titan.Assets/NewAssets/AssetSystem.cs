using Titan.Core.Logging;
using Titan.Core.Threading2;
using Titan.ECS.Events;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;

namespace Titan.Assets.NewAssets;

internal struct AssetSystem : IStructSystem<AssetSystem>
{
    private MutableResource<AssetRegistry> AssetRegistry;
    private ApiResource<JobApi> JobApi;

    private EventsReader<AssetLoadRequested> AssetLoad;
    private EventsReader<AssetUnloadRequested> AssetUnload;

    public static void Init(ref AssetSystem system, in SystemsInitializer init)
    {
        //NOTE(Jens): we can replace these resources with raw pointers. might not make a difference.
        system.JobApi = init.GetApi<JobApi>();
        system.AssetRegistry = init.GetMutableResource<AssetRegistry>(track: false);

        system.AssetLoad = init.GetEventsReader<AssetLoadRequested>();
        system.AssetUnload = init.GetEventsReader<AssetUnloadRequested>();
    }

    public static unsafe void Update(ref AssetSystem system)
    {
        ref readonly var jobApi = ref system.JobApi.Get();
        ref var registry = ref system.AssetRegistry.Get();

        //NOTE(Jens): Handle cases where Unload and Load are called the same frame. Assert or just "leave" the asset as is?
        foreach (ref readonly var @event in system.AssetUnload)
        {
            ref var asset = ref registry.Get(@event.Handle);
            if (asset.State != AssetState.Loaded)
            {
                Logger.Warning<AssetSystem>($"Asset with handle {@event.Handle} - {asset.State} can't be unloaded.");
                continue;
            }
            Logger.Trace<AssetSystem>($"Unloaded asset {@event.Handle}");
            asset.State = AssetState.Unloaded;
        }

        foreach (ref readonly var @event in system.AssetLoad)
        {
            ref var asset = ref registry.Get(@event.Handle);
            if (asset.State != AssetState.Unloaded)
            {
                Logger.Warning<AssetSystem>($"Asset with handle {@event.Handle} - {asset.State}");
                continue;
            }

            Logger.Trace<AssetSystem>($"Load asset: {@event.Handle}");
            asset.State = AssetState.Loaded;
        }


    }

    public static bool ShouldRun(in AssetSystem system)
        => system.AssetLoad.HasEvents() || system.AssetUnload.HasEvents();
}
