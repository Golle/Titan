using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Logging;
using Titan.ECS.Entities;

namespace Titan.ECS.World
{

    public class EntityRelationship
    {
        private readonly uint _worldId;
        private readonly uint[] _parents = new uint[10_000];

        public EntityRelationship(uint worldId)
        {
            _worldId = worldId;
        }

        public void Attach(in Entity parent, in Entity child)
        {
            if (parent.Id == child.Id)
            {
                throw new InvalidOperationException("Can't attach entity to itself");
            }
            ref var currentParent = ref _parents[child];
            if (currentParent != 0)
            {
                // detach from current parent
                LOGGER.Trace("Current parent exists, detaching Child: {0} Current parent: {1}", child.Id, currentParent);
                Detach(child);

            }
            LOGGER.Trace("Set parent {0} for child {1}", parent.Id, child.Id);
            currentParent = parent.Id;
        }

        public void Detach(in Entity child)
        {
            ref var currentParent = ref _parents[child];
            if (currentParent != 0)
            {
                LOGGER.Trace("Detached parent {0} from entity {1}", currentParent, child.Id);
            }
            else
            {
                LOGGER.Trace("Trying to detach a non existing parent from entity {0}", child.Id);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasParent(in Entity entity) => _parents[entity.Id] != 0;
        
        // TODO: memory vs CPU usage. Can store Entities instead of uints and return a readonly ref
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public Entity GetParent(in Entity entity) => new(entity.Id, _worldId);

    }

    public class World : IDisposable
    {
        private static uint _worldCounter = 0;
        public uint Id { get; } = Interlocked.Increment(ref _worldCounter);
        public EntityRelationship Relationship { get; }

        private readonly IEntityManager _entityManager;
        public World()
        {
            _entityManager = new EntityManager(Id);
            Relationship = new EntityRelationship(Id);

            WorldContainer.Add(this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity() => _entityManager.Create();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Attach(in Entity parent, in Entity child) => Relationship.Attach(parent, child);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Detach(in Entity child) => Relationship.Detach(child);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => WorldContainer.Remove(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyEntity(in Entity entity) => _entityManager.Destroy(entity);
    }
}
