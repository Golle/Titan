using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.ECS.World;

public readonly unsafe struct ResourceCollection
{
    private readonly MemoryAllocator _allocator;
    private readonly void** _indices;
    private readonly uint _indicesCount;

    private ResourceCollection(MemoryAllocator allocator, void** indices, uint indicesCount)
    {
        _allocator = allocator;
        _indices = indices;
        _indicesCount = indicesCount;
    }

    public static ResourceCollection Create(uint size, uint maxTypes, in MemoryPool memoryPool)
    {
        var allocator = memoryPool.CreateAllocator<MemoryAllocator>(size, true);
        var indices = memoryPool.GetPointer((uint)(sizeof(void*) * maxTypes));
        return new ResourceCollection(allocator, (void**)indices, maxTypes);
    }

    public void InitResource<T>(in T value = default) where T : unmanaged
    {
        var id = ResourceId.Id<T>();
        if (_indices[id] == null)
        {
            Logger.Trace<ResourceCollection>($"Resource type: {typeof(T).FormattedName()} with Id {id} does not exist, creating.");
            _indices[id] = _allocator.GetPointer<T>();
            *(T*)_indices[id] = value;
        }
        else
        {
            throw new InvalidOperationException($"Resource type {typeof(T).FormattedName()} has already been registered.");
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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset(bool clearMemory = false)
    {
        Unsafe.InitBlockUnaligned(_indices, 0, (uint)(sizeof(void*)*_indicesCount));
        _allocator.Reset(clearMemory);
    }
}
