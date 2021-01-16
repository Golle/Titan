using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Logging;
using Titan.ECS.Entities;

namespace Titan.ECS.Registry
{
    internal class ManagedComponentPool<T> : IManagedComponentPool<T> where T : struct
    {
        private readonly T[] _components;
        private readonly int[] _indexers;
        private readonly ConcurrentQueue<int> _freeComponents = new();

        private volatile int _numberOfComponents;
        private readonly uint _maxNumberOfComponents;

        public ManagedComponentPool(uint numberOfComponents, uint maxEntities)
        {
            LOGGER.Trace("Create ManagedComponentPool<{0}> with a maximum of {1} components", typeof(T).ToString(), numberOfComponents);
            _maxNumberOfComponents = numberOfComponents;
            _components = new T[numberOfComponents];
            _indexers = new int[maxEntities];
            Array.Fill(_indexers, -1);

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy(in Entity entity)
        {
            ref var index = ref _indexers[entity.Id];
            if (index == -1)
            {
                throw new InvalidOperationException("Trying to destroy component that has not been created.");
            }
            _freeComponents.Enqueue(index);
            index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Create(in Entity entity, in T initialValue)
        {
            ref var component = ref Create(entity);
            component = initialValue;
            return ref component;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Create(in Entity entity)
        {
            if (_numberOfComponents >= _maxNumberOfComponents && _freeComponents.IsEmpty)
            {
                throw new InvalidOperationException($"Maximum components of type {typeof(T).Name} reached({_numberOfComponents})");
            }

            ref var index = ref _indexers[entity.Id];
            if (index == -1)
            {
                if (!_freeComponents.TryDequeue(out index))
                {
                    index = Interlocked.Increment(ref _numberOfComponents) - 1;
                }
            }
            else
            {
                throw new InvalidOperationException($"A component of type {typeof(T).Name} has already been added to this entity.");
            }
            return ref _components[index];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in Entity entity) => _indexers[entity.Id] != -1;

        public ref T this[in Entity entity]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref readonly var index = ref _indexers[entity.Id];
                if (index == -1)
                {
                    throw new InvalidOperationException($"Component of type {typeof(T).Name} has not been added to the entity.");
                }
                return ref _components[index];
            }
        }

        public void Dispose()
        {
        }
    }
}
