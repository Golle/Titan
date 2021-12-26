using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Messaging;
using Titan.ECS.Entities;
using Titan.ECS.Events;

namespace Titan.ECS.Components
{
    public class DynamicPackedComponentPool<T> : IComponentPool<T> where T : unmanaged
    {
        private static readonly ComponentId ComponentId = ComponentId<T>.Id;
        private readonly MemoryChunk<uint> _indexers;
        private MemoryChunk<T> _components;
        private uint _maxCount;
        private uint _count;
        private readonly uint _worldId;
        private readonly Queue<uint> _freeComponents = new();
        private readonly unsafe uint _componentSize = (uint)sizeof(T);

        private bool _componentBeingRemoved;

        public DynamicPackedComponentPool(uint initialComponents, uint maxEntities, uint worldId)
        {
            _maxCount = initialComponents;
            _worldId = worldId;
            _indexers = MemoryUtils.AllocateBlock<uint>(maxEntities);
            _components = MemoryUtils.AllocateBlock<T>(initialComponents, true);
            for (var i = 0; i < maxEntities; ++i)
            {
                _indexers[i] = uint.MaxValue;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Create(in Entity entity, in T initialValue)
        {
            ref var component = ref Create(entity);
            component = initialValue;
            return ref component;
        }

        public ref T Create(in Entity entity)
        {
            if (_count >= _maxCount && _freeComponents.Count == 0)
            {
                _maxCount *= 2;
                Logger.Trace<DynamicPackedComponentPool<T>>($"Reallocating memory. Old: {_components.Size} New: {_maxCount * _componentSize}");
                _components = MemoryUtils.ReAllocate(ref _components, _maxCount);
            }

            if (!_freeComponents.TryDequeue(out var index))
            {
                index = _count++;
            }
            _indexers[entity.Id] = index;
            ref var components = ref _components[index];
            EventManager.Push(new ComponentAddedEvent(entity, ComponentId));
            return ref components;
        }

        public ref T CreateOrReplace(in Entity entity, in T value = default)
        {
            if (_indexers[entity.Id] == uint.MaxValue)
            {
                return ref Create(entity, value);
            }

            ref var component = ref _components[_indexers[entity.Id]];
            component = value;
            return ref component;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in Entity entity) => _indexers[entity.Id] != uint.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy(in Entity entity)
        {
            var index = _indexers[entity.Id];
            if (index != uint.MaxValue)
            {
                EventManager.Push(new ComponentBeingRemovedEvent(entity, ComponentId));
                _componentBeingRemoved = true;
            }
        }

        public void Update()
        {
            if (!_componentBeingRemoved)
            {
                return;
            }
            foreach (ref readonly var @event in EventManager.GetEvents())
            {
                if (@event.Type == ComponentBeingRemovedEvent.Id)
                {
                    ref readonly var e = ref @event.As<ComponentBeingRemovedEvent>();
                    if (e.Entity.WorldId != _worldId || e.Component != ComponentId)
                    {
                        continue;
                    }

                    ref var index = ref _indexers[e.Entity.Id];
                    Debug.Assert(index != uint.MaxValue, "Trying to destroy a component that has not been created.");
                    Logger.Trace<DynamicPackedComponentPool<T>>($"Removed {typeof(T).Name} from Entity {e.Entity.Id}");
                    _freeComponents.Enqueue(index);
                    index = uint.MaxValue;
                    EventManager.Push(new ComponentRemovedEvent(e.Entity, ComponentId));
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(in Entity entity) => ref _components[_indexers[entity.Id]];
        public ref T this[in Entity entity]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Get(entity);
        }

        public void Dispose()
        {
            _components.Free();
            _indexers.Free();
        }
    }
}
