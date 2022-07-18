using System.Diagnostics;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.App;
using Titan.ECS.Entities;
using Titan.ECS.EntitiesNew;
using Titan.ECS.Events;
using Titan.ECS.Systems;
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


public struct EntityManager : IApi
{



}



public readonly unsafe struct EntityHandler : IApi
{
    private readonly EventsWriter<EntityCreated> _entityCreated;
    private readonly EventsWriter<EntityBeingDestroyed> _entityBeingDestroyed;
    private readonly EntityIdContainer* _idContainer;

    internal EntityHandler(EventsWriter<EntityCreated> entityCreated, EventsWriter<EntityBeingDestroyed> entityBeingDestroyed, EntityIdContainer* idContainer)
    {
        _entityCreated = entityCreated;
        _entityBeingDestroyed = entityBeingDestroyed;
        _idContainer = idContainer;
    }

    public Entity Create()
    {
        var entityId = _idContainer->Next();
        Debug.Assert(entityId != 0, "Failed to create a new entity.");
        var entity = new Entity(entityId, 0); // TODO: remove World ID
        _entityCreated.Send(new EntityCreated(entity));
        return entity;
    }

    public void Destroy(in Entity entity)
        => _entityBeingDestroyed.Send(new EntityBeingDestroyed(entity));

    public void Attach(in Entity parent, in Entity child)
    {
        // TODO: add relationship implementation
    }

    public void Detach(in Entity child)
    {
        // TODO: add relationship implementation
    }
}



