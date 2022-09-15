using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.App;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.Memory;

namespace Titan.ECS.Modules;

public unsafe struct ECSModule : IModule
{
    public static bool Build(AppBuilder builder)
    {
        ref readonly var config = ref builder.GetResourceOrDefault<ECSConfiguration>();

        Logger.Trace<ECSModule>($"MaxEntities: {config.MaxEntities}");
        Logger.Trace<ECSModule>($"Event Stream Size: {config.EventStreamSize}");
        Logger.Trace<ECSModule>($"Event max types count: {config.MaxEventTypes}");
        // Create entity manager?

        var memoryManager = builder.GetResourcePointer<MemoryManager>();
        var allocatorSize = MemoryUtils.MegaBytes(512);

        //TODO(Jens): need a way to handle the life time of this allocator
        if (!memoryManager->CreateLinearAllocator(LinearAllocatorArgs.Permanent(allocatorSize), out var allocator))
        {
            Logger.Error<ECSModule>($"Failed to create an allocator with {allocatorSize} bytes.");
            return false;
        }

        //NOTE(Jens): rework these create methods, to detect failures
        builder
            .AddResource(EntityInfoRegistry.Create(&allocator, config.MaxEntities))
            .AddResource(EntityIdContainer.Create(&allocator, config.MaxEntities))
            .AddResource(EntityFilterRegistry.Create(memoryManager, 100, config.MaxEntities, 5000))
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
    }
}
