using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.App;
using Titan.ECS.Components;
using Titan.ECS.EntitiesNew;
using Titan.ECS.Systems;

namespace Titan.ECS.Modules;

public struct ECSModule : IModule
{
    public static void Build(AppBuilder builder)
    {
        if (!CheckPrerequisites(builder)) { }

        ref readonly var config = ref builder.GetResourceOrDefault<ECSConfiguration>();

        Logger.Trace<ECSModule>($"MaxEntities: {config.MaxEntities}");
        // Create entity manager?

        ref readonly var pool = ref builder.GetResource<MemoryPool>();

        builder
            .AddResource(EntityInfoRegistry.Create(pool, config.MaxEntities))
            .AddResource(EntityIdContainer.Create(pool, config.MaxEntities))
            .AddResource(EntityFilterRegistry.Create(pool, 100, config.MaxEntities, 5000))
            ;

        Logger.Warning<ECSModule>("All events are created with a size of 1000. This is because the current memory pool implementation does not support ReAlloc/Free.");
        // These numbers probably needs tweaking.
        const uint MaxEvents = 1000;
        builder
            .AddSystemToStage<EntityInfoSystem>(Stage.PreUpdate, RunCriteria.Always)
            .AddSystemToStage<ComponentSystem>(Stage.PreUpdate)
            .AddSystemToStage<EntityFilterSystem>(Stage.PreUpdate)
            .AddEvent<EntityCreated>(MaxEvents)
            .AddEvent<EntityBeingDestroyed>(MaxEvents)
            .AddEvent<EntityDestroyed>(MaxEvents)
            .AddEvent<ComponentDestroyed>(MaxEvents)
            .AddEvent<ComponentBeingDestroyed>(MaxEvents)
            .AddEvent<ComponentAdded>(MaxEvents)
            ;


        static bool CheckPrerequisites(AppBuilder builder)
        {
            if (!builder.HasResource<MemoryPool>())
            {
                Logger.Error<ECSModule>($"Failed to find the resource {nameof(MemoryPool)}. Make sure you've added the CoreModule");
                return false;
            }
            return true;
        }
    }
}
