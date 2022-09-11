using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.Memory;

namespace Titan.ECS.Modules;

public struct ECSModule : IModule
{
    public static bool Build(AppBuilder builder)
    {
        if (!CheckPrerequisites(builder)) { }

        ref readonly var config = ref builder.GetResourceOrDefault<ECSConfiguration>();

        Logger.Trace<ECSModule>($"MaxEntities: {config.MaxEntities}");
        Logger.Trace<ECSModule>($"Event Stream Size: {config.EventStreamSize}");
        Logger.Trace<ECSModule>($"Event max types count: {config.MaxEventTypes}");
        // Create entity manager?

        ref readonly var allocator = ref builder.GetResource<PlatformAllocator>();

        builder
            .AddResource(EntityInfoRegistry.Create(allocator, config.MaxEntities))
            .AddResource(EntityIdContainer.Create(allocator, config.MaxEntities))
            .AddResource(EntityFilterRegistry.Create(allocator, 100, config.MaxEntities, 5000))
            ;

        builder
            .AddSystemToStage<EntityInfoSystem>(Stage.PreUpdate, RunCriteria.Always)
            .AddSystemToStage<ComponentSystem>(Stage.PreUpdate)
            .AddSystemToStage<EntityFilterSystem>(Stage.PreUpdate)
            //.AddEvent<EntityCreated>(MaxEvents)
            //.AddEvent<EntityBeingDestroyed>(MaxEvents)
            //.AddEvent<EntityDestroyed>(MaxEvents)
            //.AddEvent<ComponentDestroyed>(MaxEvents)
            //.AddEvent<ComponentBeingDestroyed>(MaxEvents)
            //.AddEvent<ComponentAdded>(MaxEvents)
            ;

        return true;


        static bool CheckPrerequisites(AppBuilder builder)
        {
            if (!builder.HasResource<PlatformAllocator>())
            {
                Logger.Error<ECSModule>($"Failed to find the resource {nameof(PlatformAllocator)}. Make sure you've added the CoreModule");
                return false;
            }
            return true;
        }
    }
}
