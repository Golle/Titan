using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Titan.Core.Logging;
using Titan.ECS.Entities;

namespace Titan.ECS.Registry
{
    public unsafe class ComponentPool<T> : IComponentPool<T> where T : unmanaged
    {
        private T* _components;
        private T** _indexers;

        private readonly ConcurrentQueue<nint> _freeComponents = new(); // use type nint because T* cant be stored in the queue

        private int _numberOfComponents;
        
        private readonly uint _maxNumberOfComponents;
        public ComponentPool(uint numberOfComponents, uint maxEntities)
        {
            LOGGER.Trace("Create ComponentPool<{0}> with a maximum of {1} components", typeof(T).Name, maxEntities);
            var componentsSize = sizeof(T) * numberOfComponents;
            var entitiesSize = sizeof(T*) * maxEntities;
            _components = (T*)Marshal.AllocHGlobal((int) (componentsSize + entitiesSize));
            if (_components == null)
            {
                throw new OutOfMemoryException($"Failed to allocated {componentsSize + entitiesSize} bytes of memory for ComponentPool<{typeof(T)}>.");
            }
            _indexers = (T**)(_components + numberOfComponents);

            for (var i = 0; i < maxEntities; ++i)
            {
                _indexers[i] = null;
            }

            _maxNumberOfComponents = numberOfComponents;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy(in Entity entity)
        {
            ref var pComponent = ref _indexers[entity.Id];
            if (pComponent == null)
            {
                throw new InvalidOperationException("Trying to destroy component that has not been created.");
            }
            LOGGER.Trace("Remove {0} from entity {1}", typeof(T).Name, entity.Id);
            _freeComponents.Enqueue((nint) pComponent);

            pComponent = null;
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

            LOGGER.Trace("Add {0} to entity {1}", typeof(T).Name, entity.Id);
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

        public ref T this[in Entity entity]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref var pComponent = ref _indexers[entity.Id];
                if (pComponent == null)
                {
                    throw new InvalidOperationException($"Component of type {typeof(T)} has not been added to the entity.");
                }
                return ref *pComponent;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in Entity entity) => _indexers[entity.Id] != null;

        public void Dispose()
        {
            if (_components != null)
            {
                Marshal.FreeHGlobal((nint)_components);
                _components = null;
                _indexers = null;
            }
        }
    }
}
