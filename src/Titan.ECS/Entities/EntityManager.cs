using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core.Messaging;
using Titan.ECS.Events;
using Titan.ECS.World;

namespace Titan.ECS.Entities
{
    internal unsafe class EntityManager : IEntityManager
    {

        private readonly IEntityFactory _entityFactory;
        private readonly IEventQueue _eventQueue;

        private Relationship* _relationships;
        private readonly uint _worldId;

        public EntityManager(WorldConfiguration configuration, IEntityFactory entityFactory, IEventQueue eventQueue)

        {
            _worldId = configuration.WorldId;
            _entityFactory = entityFactory;
            _eventQueue = eventQueue;
            var size = sizeof(Relationship) * configuration.MaxEntities;
            _relationships = (Relationship*)Marshal.AllocHGlobal((int)size);
            Unsafe.InitBlockUnaligned(_relationships, 0, (uint)size);

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Create()
        {
            var entity = _entityFactory.Create();
            _eventQueue.Push(new EntityCreatedEvent(entity));
            return entity;
        }

        public void Attach(in Entity parent, in Entity entity)
        {
            Debug.Assert(!parent.IsNull() || !entity.IsNull(), "Parent or child is null");
            ref var ent = ref _relationships[entity.Id];
            if (ent.ParentId != 0u)
            {
                Detach(entity);
            }
            ent.ParentId = parent.Id; // Add check if swapping parents

            var parentRel = &_relationships[parent.Id];
            if (parentRel->ChildId != 0u)
            {
                var sibling = &_relationships[parentRel->ChildId];
                // find the last sibling
                while (sibling->NextId != 0u)
                {
                    sibling = &_relationships[sibling->NextId];
                }
                sibling->NextId = entity.Id;
            }
            else
            {
                // this is the first child on this parent
                parentRel->ChildId = entity.Id;
            }
            _eventQueue.Push(new EntityAttachedEvent(parent, entity));
        }

        public void Detach(in Entity entity)
        {
            var entityId = entity.Id;

            ref var relationship = ref _relationships[entityId];
            if (relationship.ParentId == 0u) // trying to detach an entity without a parent
            {
                return;
            }
            ref var parent = ref _relationships[relationship.ParentId];
            

            if (parent.ChildId == entityId)
            {
                // This child is the first or only child, set the parent to point to next (next ID or 0 if empty)
                parent.ChildId = relationship.NextId;
            }
            else
            {
                var pSibling = &_relationships[parent.ChildId];
                // Go through all siblings to the current entity
                while (pSibling->NextId != entityId)
                {
                    Debug.Assert(pSibling->NextId != 0, "NextId is 0, this should never happen." );
                    pSibling = &_relationships[pSibling->NextId];
                }
                // replace the current siblings next with the detached entitys next
                pSibling->NextId = relationship.NextId;
                relationship.NextId = 0u;
            }
            _eventQueue.Push(new EntityDetachedEvent(new Entity(relationship.ParentId, _worldId), entity));
            relationship.ParentId = 0u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasParent(in Entity entity) => _relationships[entity.Id].ParentId != 0u;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity GetParent(in Entity entity)
        {
            Debug.Assert(_relationships[entity.Id].ParentId != 0u, $"ParentId for entoty {entity.Id} is 0. Trying to get a parent for an entity without a parent.");
            return new(_relationships[entity.Id].ParentId, _worldId);
        }

        public void Destroy(in Entity entity)
        {
            ref var relationship = ref _relationships[entity.Id];
            if (relationship.ParentId != 0u)
            {
                // if there's a parent, detach the entity before destroying it
                Detach(entity);
            }
            DestroyRecursive(relationship.ChildId);
            _eventQueue.Push(new EntityBeingDestroyedEvent(entity));
            
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

            ref var relationship = ref _relationships[entityId];
            var nextId = relationship.NextId;
            // go through all siblings and destroy their children
            while (nextId != 0)
            {
                // cache the id so we can destroy the entities in the correct order (leaf nodes first)
                var idToDestroy = nextId; 
                nextId = _relationships[nextId].NextId;
                DestroyRecursive(idToDestroy);
            }
            DestroyRecursive(relationship.ChildId);
            _eventQueue.Push(new EntityBeingDestroyedEvent(new Entity(entityId, _worldId)));
            relationship.Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update()
        {
            foreach (ref readonly var @event in _eventQueue.GetEvents())
            {
                if (@event.Type == EntityBeingDestroyedEvent.Id)
                {
                    _eventQueue.Push(new EntityDestroyedEvent(@event.As<EntityBeingDestroyedEvent>().Entity.Id));
                }
            }
        }

        public void Dispose()
        {
            if (_relationships != null)
            {
                Marshal.FreeHGlobal((nint)_relationships);
                _relationships = null;
            }
        }

        private struct Relationship
        {
            internal uint ChildId;
            internal uint ParentId;
            internal uint NextId;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                ChildId = 0u;
                ParentId = 0u;
                NextId = 0u;
            }
        }
    }
}
