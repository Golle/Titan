using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.Entities;

namespace Titan.ECS.Components
{
    internal unsafe class PackedComponentPool<T> : IComponentPool<T> where T : unmanaged
    {
        private T* _components;
        private T** _indexers;
        private readonly MemoryChunk _memoryBlock;
        
        private int _numberOfComponents;
        private readonly uint _maxNumberOfComponents;
        private readonly ConcurrentQueue<nint> _freeComponents = new(); // use type nint because T* cant be stored in the queue

        public PackedComponentPool(uint numberOfComponents, uint maxEntities)
        {
            _maxNumberOfComponents = numberOfComponents;
            var componentsSize = sizeof(T) * numberOfComponents;
            var entitiesSize = sizeof(T*) * maxEntities;
            var totalSize = (uint)(componentsSize + entitiesSize);
            _memoryBlock = MemoryUtils.AllocateBlock(totalSize, true);
            _components = (T*) _memoryBlock.AsPointer();
            _indexers = (T**)(_components + numberOfComponents);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Create(in Entity entity, in T initialValue)
        {
            ref var value = ref Create(entity);
            value = initialValue;
            return ref this[entity];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Create(in Entity entity)
        {
            if (_numberOfComponents >= _maxNumberOfComponents && _freeComponents.IsEmpty)
            {
                throw new InvalidOperationException($"Maximum components of type {typeof(T)} reached({_numberOfComponents})");
            }

            Logger.Trace<PackedComponentPool<T>>($"Add {typeof(T).Name} to Entity {entity.Id}");
            ref var pComponent = ref _indexers[entity.Id];
            if (pComponent != null)
            {
                throw new InvalidOperationException($"A component of type {typeof(T)} has already been added to this entity.");
            }

            // TODO: if components are created by multiple threads this section of the code wont work reliable.
            if (_freeComponents.TryDequeue(out var pComp))
            {
                pComponent = (T*)pComp;
            }
            else
            {
                pComponent = &_components[Interlocked.Increment(ref _numberOfComponents)];
            }
            return ref *pComponent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(in Entity entity)
        {
            ref var pComponent = ref _indexers[entity.Id];
            Debug.Assert(pComponent != null, $"Component of type {typeof(T)} has not been added to the entity.");
            return ref *pComponent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in Entity entity) => _indexers[entity.Id] != null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy(in Entity entity)
        {
            ref var pComponent = ref _indexers[entity.Id];
            if (pComponent == null)
            {
                throw new InvalidOperationException("Trying to destroy component that has not been created.");
            }
            Logger.Trace<PackedComponentPool<T>>($"Removed {typeof(T).Name} from Entity {entity.Id}");
            _freeComponents.Enqueue((nint)pComponent);

            pComponent = null;
        }


        public ref T this[in Entity entity]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Get(entity);
        }

        public void Dispose()
        {
            _memoryBlock.Free();
            _components = null;
            _indexers = null;
        }
    }
}
