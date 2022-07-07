using System;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.ECS.TheNew;

public readonly unsafe struct ResourceCollection
{
    private readonly PermanentMemory _allocator;
    private readonly void** _indices;
    private ResourceCollection(PermanentMemory allocator, void** indices)
    {
        _indices = indices;
        _allocator = allocator;
    }

    public static ResourceCollection Create(uint size, uint maxTypes, in MemoryPool memoryPool)
    {
        var allocator = memoryPool.CreateAllocator<PermanentMemory>(size, true);
        var indices = memoryPool.GetPointer((uint)(sizeof(void*) * maxTypes));
        return new ResourceCollection(allocator, (void**)indices);
    }

    public void InitResource<T>(in T value = default) where T : unmanaged
    {
        var id = ResourceId.Id<T>();
        if (_indices[id] == null)
        {
            Logger.Trace<ResourceCollection>($"Resource type: {typeof(T).Name} with Id {id} does not exist, creating.");
            _indices[id] = _allocator.GetPointer<T>();
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
}
