using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Messaging;
using Titan.ECS.Events;
using Titan.ECS.Worlds;

namespace Titan.ECS.Entities;

public class EntityManager : IDisposable
{
    private readonly MemoryChunk<Relationship> _relationship;

    private readonly IdContainer _entityIds;
    private readonly uint _worldId;

    private const int MaxEntityDepth = 64;

    public EntityManager(WorldConfigurationOld config)
    {
        _worldId = config.Id;
        _entityIds = new IdContainer(config.MaxEntities);
        _relationship = MemoryUtils.AllocateBlock<Relationship>(config.MaxEntities, true);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Entity Create()
    {
        Entity entity = new(_entityIds.Next(), _worldId);
        EventManager.Push(new EntityCreatedEvent(entity));
        return entity;
    }

    public unsafe void Attach(in Entity parent, in Entity entity)
    {
        Debug.Assert(!parent.IsNull() || !entity.IsNull(), "Parent or child is null");
        ref var relationship = ref _relationship[entity.Id];
        if (relationship.ParentId != 0u)
        {
            Detach(entity);
        }
        relationship.ParentId = parent.Id; // Add check if swapping parents
        var parentRel = _relationship.GetPointer(parent.Id);
        if (parentRel->ChildId != 0u)
        {
            var sibling = _relationship.GetPointer(parentRel->ChildId);
            // find the last sibling
            while (sibling->NextId != 0u)
            {
                sibling = _relationship.GetPointer(sibling->NextId);
            }
            sibling->NextId = entity.Id;
        }
        else
        {
            // this is the first child on this parent
            parentRel->ChildId = entity.Id;
        }
        UpdateParentCount(ref relationship);
        EventManager.Push(new EntityAttachedEvent(parent, entity));
    }

    // TODO: might need to optimize this depending on the type of game
    private unsafe void UpdateParentCount(ref Relationship relationship)
    {
        relationship.ParentCount = relationship.ParentId != 0u ? _relationship[relationship.ParentId].ParentCount + 1 : 0;

        // No children, just return
        if (relationship.ChildId == 0u)
        {
            return;
        }
        var parentCount = relationship.ParentCount + 1;

        var count = 0;
        var searchStack = stackalloc Relationship*[MaxEntityDepth];
        var current = _relationship.GetPointer(relationship.ChildId);

        while (true)
        {
            current->ParentCount = parentCount;
            if (current->ChildId != 0)
            {
                // Only push to the seach stack if there are siblings
                if (current->NextId != 0)
                {
                    searchStack[count++] = current;
                }
                current = _relationship.GetPointer(current->ChildId);
                parentCount++;
                continue;
            }
            if (current->NextId != 0)
            {
                current = _relationship.GetPointer(current->NextId);
                continue;
            }

            if (count == 0)
            {
                break;
            }
            current = _relationship.GetPointer(searchStack[--count]->NextId);
            parentCount--;
        }
    }

    public unsafe void Detach(in Entity entity)
    {
        var entityId = entity.Id;

        ref var relationship = ref _relationship[entityId];
        if (relationship.ParentId == 0u) // trying to detach an entity without a parent
        {
            return;
        }
        ref var parent = ref _relationship[relationship.ParentId];

        if (parent.ChildId == entityId)
        {
            // This child is the first or only child, set the parent to point to next (next ID or 0 if empty)
            parent.ChildId = relationship.NextId;
            relationship.NextId = 0;
        }
        else
        {
            var pSibling = _relationship.GetPointer(parent.ChildId);
            // Go through all siblings to the current entity
            while (pSibling->NextId != entityId)
            {
                Debug.Assert(pSibling->NextId != 0, "NextId is 0, this should never happen.");
                pSibling = _relationship.GetPointer(pSibling->NextId);
            }
            // replace the current siblings next with the detached entitys next
            pSibling->NextId = relationship.NextId;
            relationship.NextId = 0u;
        }
        EventManager.Push(new EntityDetachedEvent(new Entity(relationship.ParentId, _worldId), entity));
        relationship.ParentId = 0u;
        UpdateParentCount(ref relationship);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasParent(in Entity entity) => _relationship[entity.Id].ParentId != 0u;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Entity GetParent(in Entity entity)
    {
        Debug.Assert(_relationship[entity.Id].ParentId != 0u, $"ParentId for entity {entity.Id} is 0. Trying to get a parent for an entity without a parent.");
        return new(_relationship[entity.Id].ParentId, _worldId);
    }

    public void Destroy(in Entity entity)
    {
        ref var relationship = ref _relationship[entity.Id];
        if (relationship.ParentId != 0u)
        {
            // if there's a parent, detach the entity before destroying it
            Detach(entity);
        }
        DestroyRecursive(relationship.ChildId);
        EventManager.Push(new EntityBeingDestroyedEvent(entity));

        relationship.Reset();
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void DestroyRecursive(uint entityId)
    {
        // nothing to destroy
        if (entityId == 0u)
        {
            return;
        }

        ref var relationship = ref _relationship[entityId];
        var nextId = relationship.NextId;
        // go through all siblings and destroy their children
        while (nextId != 0)
        {
            // cache the id so we can destroy the entities in the correct order (leaf nodes first)
            var idToDestroy = nextId;
            nextId = _relationship[nextId].NextId;
            DestroyRecursive(idToDestroy);
        }
        DestroyRecursive(relationship.ChildId);
        EventManager.Push(new EntityBeingDestroyedEvent(new Entity(entityId, _worldId)));
        relationship.Reset();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetParent(in Entity entity, out Entity parent)
    {
        Unsafe.SkipInit(out parent);
        var parentId = _relationship[entity.Id].ParentId;
        if (parentId != 0u)
        {
            parent = new(parentId, _worldId);
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update()
    {
        foreach (ref readonly var @event in EventManager.GetEvents())
        {
            if (@event.Type == EntityDestroyedEvent.Id)
            {
                ref readonly var e = ref @event.As<EntityDestroyedEvent>();
                if (e.Entity.WorldId == _worldId)
                {
                    _entityIds.Return(e.Entity.Id);
                }
            }
            else if (@event.Type == EntityBeingDestroyedEvent.Id)
            {
                var e = @event.As<EntityBeingDestroyedEvent>();
                if (e.Entity.WorldId == _worldId)
                {
                    EventManager.Push(new EntityDestroyedEvent(e.Entity));
                }
            }
        }
    }

    public void Dispose() => _relationship.Free();


    public unsafe void DebugPrint(in Entity entity)
    {
        var relationship = _relationship.GetPointer(entity.Id);
        //Logger.Error($"E: {entity.Id} Parents: {relationship->ParentCount}");
        TraverseRelation(entity.Id, relationship);
    }

    private unsafe void TraverseRelation(uint currentId, Relationship* rel)
    {
        Logger.Error($"E: {currentId} Parents: {rel->ParentCount}");
        if (rel->ChildId != 0)
        {
            TraverseRelation(rel->ChildId, _relationship.GetPointer(rel->ChildId));
        }

        if (rel->NextId != 0)
        {
            TraverseRelation(rel->NextId, _relationship.GetPointer(rel->NextId));
        }
    }

    private struct Relationship
    {
        public uint ChildId;
        public uint ParentId;
        public uint NextId;
        public uint ParentCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => this = default;
    }
}
