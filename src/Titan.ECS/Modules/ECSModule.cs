using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.App;
using Titan.ECS.Components;
using Titan.ECS.EntitiesNew;
using Titan.ECS.SystemsV2;
using Titan.ECS.World;

namespace Titan.ECS.Modules;

public struct ECSModule : IModule2
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
            ;

        Logger.Warning<ECSModule>("All events are created with a size of 1000. This is because the current memory pool implementation does not support ReAlloc/Free.");
        // These numbers probably needs tweaking.
        builder
            .AddSystemToStage<EntityInfoSystem>(Stage.PreUpdate, RunCriteria.Always) // 
            .AddSystemToStage<ComponentSystem>(Stage.PreUpdate)
            .AddEvent<EntityCreated>(1000)
            .AddEvent<EntityBeingDestroyed>(1000)
            .AddEvent<EntityDestroyed>(1000)
            .AddEvent<ComponentDestroyed>(1000)
            .AddEvent<ComponentBeingDestroyed>(1000)
            .AddEvent<ComponentAdded>(1000)
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
