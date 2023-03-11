using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Events;

namespace Titan.ECS.Entities;

internal unsafe class EntityRegistry
{
    private EntityIDContainer _container;
    private TitanArray<Relationship> _relationships;
    private IEventsManager _eventsManager;
    private IMemoryManager _memoryManager;
    private const int MaxEntityDepth = 32;

    internal bool Init(IMemoryManager memoryManager, IEventsManager eventsManager, uint maxEntities)
    {
        Logger.Trace<EntityRegistry>($"Reserving space for {maxEntities} entities");

        if (!_container.Init(memoryManager, maxEntities))
        {
            Logger.Error<EntityRegistry>("Failed to init the EntityID container");
            return false;
        }

        if (!memoryManager.TryAllocArray(out _relationships, maxEntities, true))
        {
            Logger.Error<EntityRegistry>("Failed to allocate memory for the entity relationships");
            return false;
        }

        _eventsManager = eventsManager;
        _memoryManager = memoryManager;
        return true;
    }

    public Entity Create()
    {
        var entity = _container.Next();
        _eventsManager.Send(new EntityCreated(entity));
        return entity;
    }

    /// <summary>
    /// Called when an entity is really destroyed and can be re-used. Call BeginDestroy to flag the entity and clear the relationships
    /// </summary>
    /// <param name="entity"></param>
    internal void Destroy(in Entity entity)
    {
        _container.Return(entity);
    }

    public void BeginDestroy(in Entity entity)
    {
        ref var relationship = ref _relationships[entity];
        //NOTE(Jens): We don't have to check siblings, because the only case where an entity has a sibling is if there's a parent, in which case we call detach.
        if (relationship.Parent.IsValid)
        {
            // if the entity has a parent, detach it first.
            //NOTE(Jens): this is not thread safe, which means that if a system flags an entity for destruction and another entity reads the parent it will crash.
            //NOTE(Jens): we can implement events for this so we can control it better, for example EntityBeingDetached.
            Detach(entity);
        }
        DestroyRecursive(relationship.Child);
        _eventsManager.Send(new EntityBeingDestroyed(entity));

        // this should also be done in the next frame
        relationship = default;
    }

    private void DestroyRecursive(in Entity entity)
    {
        if (entity.IsInvalid)
        {
            return;
        }

        ref var relationship = ref _relationships[entity];
        if (relationship.Next.IsValid)
        {
            DestroyRecursive(relationship.Next);
        }
        DestroyRecursive(relationship.Child);
        relationship = default;
        _eventsManager.Send(new EntityBeingDestroyed(entity));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasParent(in Entity entity) => _relationships[entity].Parent.IsValid;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Entity GetParent(in Entity entity)
    {
        Debug.Assert(_relationships[entity].Parent.IsValid, $"ParentId for entity {entity.Id} is 0. Trying to get a parent for an entity without a parent.");
        return _relationships[entity].Parent;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetChildren(in Entity entity, Span<Entity> childrenOut)
    {
        Debug.Assert(entity.IsValid);
        var rel = _relationships.GetPointer(entity);
        var count = 0;
        var child = rel->Child;
        while (child.IsValid)
        {
            Debug.Assert(childrenOut.Length > count);
            childrenOut[count++] = child;
            child = _relationships[child].Next;
        }
        return count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetParent(in Entity entity, out Entity parent)
    {
        parent = _relationships[entity].Parent;
        return parent.IsValid;
    }

    public void Attach(in Entity parent, in Entity child)
    {
        ref var relationship = ref _relationships[child.Id];
        if (relationship.Parent.IsValid)
        {
            // The child already has a parent, so this is a "move" operation.
            Detach(child);
        }
        relationship.Parent = parent;
        var parentRel = _relationships.GetPointer(parent.Id);
        if (parentRel->Child.IsValid)
        {
            var sibling = _relationships.GetPointer(parentRel->Child);
            // find the last sibling
            while (sibling->Next.IsValid)
            {
                sibling = _relationships.GetPointer(sibling->Next);
            }
            sibling->Next = child;
        }
        else
        {
            // this is the first child on this parent
            parentRel->Child = child;
        }
        UpdateParentCount(ref relationship);
        _eventsManager.Send(new EntityAttached(parent, child));
    }
    public void Detach(in Entity child)
    {
        ref var relationship = ref _relationships[child];
        if (relationship.Parent.IsInvalid) // trying to detach an entity without a parent
        {
            //assert?
            return;
        }
        ref var parent = ref _relationships[relationship.Parent];
        if (parent.Child == child)
        {
            // This child is the first or only child, set the parent to point to next (next ID or Invalid if empty)
            parent.Child = relationship.Next;
            relationship.Next = Entity.Invalid;
        }
        else
        {
            var pSibling = _relationships.GetPointer(parent.Child);
            // Go through all siblings to the current entity
            while (pSibling->Next != child)
            {
                Debug.Assert(pSibling->Next.Id != 0, "NextId is 0, this should never happen.");
                pSibling = _relationships.GetPointer(pSibling->Next);
            }
            // replace the current siblings next with the detached entitys next
            pSibling->Next = relationship.Next;
            relationship.Next = Entity.Invalid;
        }
        _eventsManager.Send(new EntityDetached(relationship.Parent, child));
        relationship.Parent = Entity.Invalid;
        UpdateParentCount(ref relationship);
    }

    [SkipLocalsInit]
    private void UpdateParentCount(ref Relationship relationship)
    {
        relationship.ParentCount = relationship.Parent.IsValid ? _relationships[relationship.Parent].ParentCount + 1 : 0;

        // No children, just return
        if (relationship.Child.IsInvalid)
        {
            return;
        }
        var parentCount = relationship.ParentCount + 1;

        var count = 0;
        var searchStack = stackalloc Relationship*[MaxEntityDepth];
        var current = _relationships.GetPointer(relationship.Child);

        while (true)
        {
            current->ParentCount = parentCount;
            if (current->Child.IsValid)
            {
                // Only push to the seach stack if there are siblings
                if (current->Next.IsValid)
                {
                    searchStack[count++] = current;
                }
                current = _relationships.GetPointer(current->Child);
                parentCount++;
                continue;
            }
            if (current->Next.IsValid)
            {
                current = _relationships.GetPointer(current->Next);
                continue;
            }

            if (count == 0)
            {
                break;
            }
            current = _relationships.GetPointer(searchStack[--count]->Next);
            parentCount--;
        }
    }
    private struct Relationship
    {
        public Entity Child;
        public Entity Parent;
        public Entity Next;
        public uint ParentCount; // not sure we'll use this, but maybe it will be nice for sorting.
    }

    internal void Shutdown()
    {
        _container.Shutdown();
        _memoryManager.Free(ref _relationships);
    }

    [Conditional("DEBUG")]
    public void DebugPrint(in Entity entity)
        => TraverseRelation(entity.Id, _relationships.GetPointer(entity));

    [Conditional("DEBUG")]
    private void TraverseRelation(uint currentId, Relationship* rel)
    {
        Logger.Trace<EntityRegistry>($"E: {currentId} Parent count: {rel->ParentCount} (ParentID: {rel->Parent})");
        if (rel->Child.IsValid)
        {
            TraverseRelation(rel->Child, _relationships.GetPointer(rel->Child));
        }

        if (rel->Next.IsValid)
        {
            TraverseRelation(rel->Next, _relationships.GetPointer(rel->Next));
        }
    }
}
