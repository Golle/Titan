using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Titan.ECS.Entities;

namespace Titan.ECS.Registry
{

    public class ManagedComponentPool<T> where T : unmanaged
    {
        private readonly T[] _components;
        private readonly int[] _indexers;

        private int _numberOfComponents;

        public ManagedComponentPool(uint numberOfComponents, uint maxEntities)
        {
            _components = new T[numberOfComponents];
            _indexers = new int[maxEntities];
            Array.Fill(_indexers, -1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Create(in Entity entity)
        {
            ref var index = ref _indexers[entity.Id];
            if (index != -1)
            {
                throw new InvalidOperationException($"A component of type {typeof(T)} has already been added to this entity.");
            }
            index = Interlocked.Increment(ref _numberOfComponents) - 1;
            return ref _components[index];
        }

        public ref T this[in Entity entity]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref var index = ref _indexers[entity.Id];
                if (index == -1)
                {
                    throw new InvalidOperationException($"Component of type {typeof(T)} has not been added to the entity.");
                }
                return ref _components[index];
            }
        }
    }

    public unsafe class ComponentPool<T> where T : unmanaged
    {
        private readonly T* _components;
        private readonly T** _indexers;

        private int _numberOfComponents;

        public ComponentPool(uint numberOfComponents, uint maxEntities)
        {
            var componentsSize = sizeof(T) * numberOfComponents;
            var entitiesSize = sizeof(T*) * maxEntities;
            _components = (T*)Marshal.AllocHGlobal((int) (componentsSize + entitiesSize));
            _indexers = (T**)_components + numberOfComponents;
            for (var i = 0; i < maxEntities; ++i)
            {
                _indexers[i] = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Create(in Entity entity)
        {
            ref var pComponent = ref _indexers[entity.Id];
            if (pComponent != null)
            {
                throw new InvalidOperationException($"A component of type {typeof(T)} has already been added to this entity.");
            }
            pComponent = &_components[Interlocked.Increment(ref _numberOfComponents)];
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

    }

    public unsafe class ComponentMemoryAllocator : IDisposable
    {
        private readonly void*[] _allocatedChunks = new void*[1000];
        
        private readonly uint _chunkSize;

        private int _numberOfChunks;

        public ComponentMemoryAllocator(uint chunkSize)
        {
            _chunkSize = chunkSize;
        }

        // TODO: make it possible to allocate several chunks with a single call
        public int AllocateChunk(out void* chunk)
        {
            lock (_allocatedChunks)
            {
                chunk = (void*)Marshal.AllocHGlobal((int)_chunkSize);
                if (chunk == null)
                {
                    throw new OutOfMemoryException($"Failed to allocated a memory block of size {_chunkSize}");
                }
                var index = Interlocked.Increment(ref _numberOfChunks) - 1;
                _allocatedChunks[index] = chunk;
                return index;
            }
        }

        public void Dispose()
        {
            for (var i = 0; i < _numberOfChunks; ++i)
            {
                ref var chunk = ref _allocatedChunks[i];
                Marshal.FreeHGlobal((nint)chunk);
                chunk = null;
            }
            _numberOfChunks = 0;
        }
    }

    internal class Registry
    {
        public Registry()
        {
            
        }
    }
}
