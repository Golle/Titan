using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Queries;
using Titan.Events;
using Titan.Modules;
using Titan.Setup;
using Titan.Setup.Configs;
using Titan.Systems;

namespace Titan.ECS;

internal record ComponentConfig(ComponentId Id, uint Index, uint Size, uint Count, ComponentPoolType Type, string ComponentName) : IConfiguration
{
    public static unsafe ComponentConfig Create<T>(uint count, ComponentPoolType type) where T : unmanaged, IComponent
        => new(ComponentId<T>.Id, ComponentId<T>.Index, (uint)sizeof(T), count, type, typeof(T).Name);
}

public record ECSConfig : IConfiguration, IDefault<ECSConfig>
{
    public uint MaxEntities { get; init; }
    public uint MaxComponentsSize { get; init; }
    public uint MaxQueriesSize { get; init; }
    public uint MaxQueries { get; init; }
    public const uint DefaultMaxEntities = 10_000;
    public static readonly uint DefaultMaxTotalComponentsSize = MemoryUtils.MegaBytes(128);
    public static readonly uint DefaultMaxQuerySize = MemoryUtils.MegaBytes(16);
    public static readonly uint DefaultMaxQueries = 128;
    public static ECSConfig Default => new()
    {
        MaxEntities = DefaultMaxEntities,
        MaxComponentsSize = DefaultMaxTotalComponentsSize,
        MaxQueries = DefaultMaxQueries,
        MaxQueriesSize = DefaultMaxQuerySize
    };
}


internal struct ECSModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddManagedResource(new EntityRegistry())
            .AddManagedResource(new ComponentsRegistry())
            .AddManagedResource(new EntityQueryRegistry())
            .AddResource<EntityInfoRegistry>()
            .AddSystemToStage<ComponentSystem>(SystemStage.PreUpdate)
            .AddSystemToStage<EntityInfoSystem>(SystemStage.PreUpdate, priority: 1)
            .AddSystemToStage<EntityQuerySystem>(SystemStage.PreUpdate, priority: -1)
            .AddSystemToStage<EntityLifetimeSystem>(SystemStage.PreUpdate)
            ;

        return true;
    }

    public static bool Init(IApp app)
    {
        var memoryManager = app.GetManagedResource<IMemoryManager>();
        var eventsManager = app.GetManagedResource<IEventsManager>();

        var registry = app.GetManagedResource<EntityRegistry>();
        var componentRegistry = app.GetManagedResource<ComponentsRegistry>();
        var queryRegistry = app.GetManagedResource<EntityQueryRegistry>();
        ref var entityInfo = ref app.GetResource<EntityInfoRegistry>();

        var config = app.GetConfigOrDefault<ECSConfig>();
        var componentConfigs = app.GetConfigs<ComponentConfig>();

        if (!registry.Init(memoryManager, eventsManager, config.MaxEntities))
        {
            Logger.Error<ECSModule>($"Failed to initialize the {nameof(EntityRegistry)}");
            return false;
        }

        if (!entityInfo.Init(memoryManager, config.MaxEntities))
        {
            Logger.Error<ECSModule>($"Failed to initialize the {nameof(EntityInfoRegistry)}");
            return false;
        }

        if (!componentRegistry.Init(memoryManager, eventsManager, componentConfigs.ToArray(), config.MaxEntities, config.MaxComponentsSize))
        {
            Logger.Error<ECSModule>($"Failed to initialize the {nameof(ComponentsRegistry)}");
            return false;
        }

        if (!queryRegistry.Init(memoryManager, componentRegistry, config.MaxQueriesSize, config.MaxQueries, config.MaxEntities))
        {
            Logger.Error<ECSModule>($"Failed to initialize the {nameof(EntityQueryRegistry)}");
            return false;
        }

        return true;
    }

    public static bool Shutdown(IApp app)
    {
        var entityManager = app.GetManagedResource<EntityRegistry>();

        var componentRegistry = app.GetManagedResource<ComponentsRegistry>();
        var entityQueryRegistry = app.GetManagedResource<EntityQueryRegistry>();
        ref var entityInfo = ref app.GetResource<EntityInfoRegistry>();

        entityInfo.Shutdown();
        entityQueryRegistry.Shutdown();
        entityManager.Shutdown();
        componentRegistry.Shutdown();

        return true;
    }
}
