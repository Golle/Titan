using Titan.Core;
using Titan.Events;
using Titan.Systems;

namespace Titan.Assets;

internal struct AssetLoaderSystem : ISystem
{
    private ObjectHandle<AssetLoader> _loader;
    private EventsReader<AssetUnloadRequested> _unload;
    private EventsReader<AssetLoadRequested> _load;
    private EventsReader<AssetReloadRequested> _reload;

    public void Init(in SystemInitializer init)
    {
        _loader = init.GetManagedApi<AssetLoader>();
        _unload = init.GetEventsReader<AssetUnloadRequested>();
        _load = init.GetEventsReader<AssetLoadRequested>();
        _reload = init.GetEventsReader<AssetReloadRequested>();
    }

    public void Update()
    {
        var loader = _loader.Value;
        //NOTE(Jens): Read the Unload events first, so if we have both unload and load in the same frame we can treat that as a Load.
        foreach (ref readonly var @event in _unload)
        {
            loader.Unload(@event.Handle);
        }

        foreach (ref readonly var @event in _load)
        {
            loader.Load(@event.Handle);
        }

        foreach (ref readonly var @event in _reload)
        {
            loader.Reload(@event.Handle);
        }
        _loader.Value!.Update();
    }

    public bool ShouldRun() => _loader.Value.IsActive() || _unload.HasEvents() || _load.HasEvents() ||_reload.HasEvents();
}
