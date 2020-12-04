using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.ECS.Entities;

namespace Titan.ECS.Components
{
    public unsafe class EntityFilter : IEntityFilter, IDisposable
    {
        private Entity* _entities;
        private int* _indexers;
        private int _numberOfEntities;
        private readonly ComponentMask _mask;
        public ref readonly ComponentMask ComponentMask => ref _mask;
        public EntityFilter(in ComponentMask mask, uint maxEntities)
        {
            _mask = mask;
            var size = (sizeof(Entity) + sizeof(int)) * maxEntities;

            _entities = (Entity*) Marshal.AllocHGlobal((int) size);
            _indexers = (int*) (_entities + maxEntities);
            for (var i = 0; i < maxEntities; ++i) // TODO: replace with Unsafe.Init
            {
                _indexers[i] = -1;
            }
        }


        public void OnEntityChanged(in Entity entity, in ComponentMask components)
        {
            var isMatch = components.Contains(_mask);
            ref var index = ref _indexers[entity.Id];
            if (index != -1 && isMatch)
            {
                // Entity is in this filter with no match on the mask, remove the entity and replace it with the last one in the list.
                _entities[index] = _entities[--_numberOfEntities];
                _indexers[_entities[index]] = index;
                index = -1;
            }
            else if(isMatch)
            {
                // It's a match but it does not exist in the filter, add it
                index = _numberOfEntities++;
                _indexers[entity.Id] = index;
                _entities[index] = entity;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnEntityDestroyed(in Entity entity) => _indexers[entity.Id] = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<Entity> GetEntities() => new(_entities, _numberOfEntities);

        public void Dispose()
        {
            if (_entities != null)
            {
                Marshal.FreeHGlobal((nint)_entities);
                _entities = null;
                _indexers = null;
            }
        }
    }
}
