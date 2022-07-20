using System.Diagnostics;
using Titan.Core;
using Titan.ECS.Entities;
using Titan.ECS.Systems;

namespace Titan.ECS.EntitiesNew;

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