using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.TheNew;

namespace Titan.NewStuff;

internal unsafe class UnmanagedResources : IDisposable
{
    private readonly MemoryBlock _memory;
    private readonly void* _memoryPtr;
    private volatile int _currentOffset;
    private readonly void*[] _indices;

    public UnmanagedResources(uint maxSize, uint maxResourceTypes, IPersistentMemoryAllocator allocator)
    {
        _memory = allocator.GetBlock(maxSize, true);
        _memoryPtr = _memory.AsPointer();
        _indices = new void*[maxResourceTypes];
    }


    public void InitResource<T>(in T value = default) where T : unmanaged
    {
        var id = ResourceId.Id<T>();
        if (_indices[id] == null)
        {
            Logger.Trace<UnmanagedResources>($"Resource type: {typeof(T).Name} with Id {id} does not exist, creating.");
            var size = sizeof(T);
            Debug.Assert(_currentOffset + size < _memory.Size);

            var offset = Interlocked.Add(ref _currentOffset, size) - size;
            _indices[id] = (byte*)_memoryPtr + offset;
            *(T*)_indices[id] = value;
        }
        else
        {
            throw new InvalidOperationException($"Resource type {typeof(T)} has already been registered.");
        }

    }

    public ref T GetResource<T>() where T : unmanaged
    {
        var id = ResourceId.Id<T>();
        ref var ptr = ref _indices[id];
        if (ptr == null)
        {
            InitResource<T>();
        }

        return ref *(T*)_indices[id];
    }

    public T* GetResourcePointer<T>() where T : unmanaged
    {
        ref readonly var ptr = ref _indices[ResourceId.Id<T>()];
        if (ptr == null)
        {
            InitResource<T>();
        }
        return (T*)ptr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasResource<T>() => _indices[ResourceId.Id<T>()] != null;
    public void Dispose()
    {
        
        // TODO: handle memory cleanup? it should be persistent, means that it will live for the applications lifecycle.
    }
}
