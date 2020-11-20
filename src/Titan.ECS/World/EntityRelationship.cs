using System;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.ECS.Entities;

namespace Titan.ECS.World
{
    internal class EntityRelationship : IEntityRelationship
    {
        private readonly uint _worldId;
        private readonly uint[] _parents;

        public EntityRelationship(WorldConfiguration configuration)
        {
            _worldId = configuration.WorldId;
            _parents = new uint[configuration.MaxEntities];
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
                currentParent = 0;
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
        public Entity GetParent(in Entity entity) => new(_parents[entity.Id], _worldId);

        public void Dispose()
        {
            // Might need this if we move to unmanaged memory
        }
    }
}
