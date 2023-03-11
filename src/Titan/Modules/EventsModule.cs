using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Events;
using Titan.Setup;
using Titan.Setup.Configs;
using Titan.Systems;

namespace Titan.Modules;
public record EventsConfig(uint MaxEventTypes, uint MaxSize) : IConfiguration, IDefault<EventsConfig>
{
    public static EventsConfig Default => new(DefaultMaxEventTypes, DefaultMaxSize);
    public const uint DefaultMaxEventTypes = 64;                            // 64 event types
    public static readonly uint DefaultMaxSize = MemoryUtils.MegaBytes(2);  // 2 Mb
}

internal struct EventsModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddManagedResource<IEventsManager>(new EventsManager())
            .AddSystemToStage<EventSystem>(SystemStage.First, RunCriteria.AlwaysInline)
            ;

        return true;
    }

    public static bool Init(IApp app)
    {
        var eventConfig = app.GetConfigOrDefault<EventsConfig>();
        var eventManager = (EventsManager)app.GetManagedResource<IEventsManager>();
        var memoryManager = app.GetManagedResource<IMemoryManager>();

        if (!eventManager.Init(memoryManager, eventConfig.MaxSize, eventConfig.MaxEventTypes))
        {
            Logger.Error<EventsModule>($"Failed to init the {nameof(EventsManager)}.");
            return false;
        }
        return true;
    }

    public static bool Shutdown(IApp app)
    {
        var eventManager = (EventsManager)app.GetManagedResource<IEventsManager>();
        eventManager.Shutdown();
        return true;
    }
}
