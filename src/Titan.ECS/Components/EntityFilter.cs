using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.ECS.Entities;

namespace Titan.ECS.Components
{
    public sealed unsafe class EntityFilter : IEntityFilter, IDisposable
    {
        private Entity* _entities;
        private int* _indexers;
        private int _numberOfEntities;
        private readonly ComponentId _components;
        private readonly ComponentId _excludeComponents;
        public ref readonly ComponentId Components => ref _components;
        public ref readonly ComponentId Exclude => ref _excludeComponents;
        public EntityFilter(in ComponentId components, in ComponentId excludeComponents, uint maxEntities)
        {
            _components = components;
            _excludeComponents = excludeComponents;
            var size = (sizeof(Entity) + sizeof(int)) * maxEntities;

            _entities = (Entity*) Marshal.AllocHGlobal((int) size);
            _indexers = (int*) (_entities + maxEntities);
            for (var i = 0; i < maxEntities; ++i) // TODO: replace with Unsafe.Init
            {
                _indexers[i] = -1;
            }
        }

        public void OnEntityChanged(in Entity entity, in ComponentId components)
        {
            var isMatch = _components.IsSubsetOf(components) && _excludeComponents.MatchesNone(components);
            ref var index = ref _indexers[entity.Id];
            if (index != -1 && !isMatch)
            {
                // Entity is in this filter with no match on the mask, remove the entity and replace it with the last one in the list.
                _entities[index] = _entities[--_numberOfEntities];
                _indexers[_entities[index]] = index;
                index = -1;
            }
            else if(index == -1 && isMatch)
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
