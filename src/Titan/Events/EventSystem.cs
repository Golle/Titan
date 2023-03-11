using Titan.Systems;

namespace Titan.Events;

internal struct EventSystem : ISystem
{
    //NOTE(Jens): we can store this in a static variable to avoid casts and lookups.
    private static EventsManager _eventsManager;
    public void Init(in SystemInitializer init) => _eventsManager = (EventsManager)init.GetManagedApi<IEventsManager>();
    public void Update() => _eventsManager.Swap();

}
