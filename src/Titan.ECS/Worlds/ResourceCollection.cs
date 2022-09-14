using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Memory;
using Titan.Memory.Allocators2;

namespace Titan.ECS.Worlds;

public unsafe struct ResourceCollection
{
    private LinearAllocator _allocator;
    private readonly void** _indices;
    private readonly uint _maxTypes;

    private ResourceCollection(LinearAllocator allocator, void** indices, uint maxTypes)
    {
        _allocator = allocator;
        _indices = indices;
        _maxTypes = maxTypes;
    }

    public static bool Create(MemoryManager* memoryManager, uint maxSize, uint maxTypes, out ResourceCollection resourceCollection)
    {
        resourceCollection = default;
        var indicesSize = (uint)sizeof(void*) * maxTypes;
        var totalSize = maxSize + indicesSize;
        if (!memoryManager->CreateLinearAllocator(AllocatorArgs.Permanent(totalSize), out var allocator))
        {
            Logger.Error<ResourceCollection>("Failed to create the allocator.");
            return false;
        }
        var indices = (void**)allocator.Alloc(indicesSize);
        Debug.Assert(indices != null);
        resourceCollection = new ResourceCollection(allocator, indices, maxTypes);
        return true;
    }

    public void InitResource<T>(in T value = default) where T : unmanaged
    {
        var id = ResourceId.Id<T>();
        Debug.Assert(id <= _maxTypes);
        if (_indices[id] == null)
        {
            Logger.Trace<ResourceCollection>($"Resource type: {typeof(T).FormattedName()} with Id {id} does not exist, creating.");
            _indices[id] = _allocator.Alloc<T>();
            *(T*)_indices[id] = value;
        }
        else
        {
            throw new InvalidOperationException($"Resource type {typeof(T).FormattedName()} has already been registered.");
        }
    }

    public readonly ref T GetResource<T>() where T : unmanaged
        => ref *GetResourcePointer<T>();
    public readonly T* GetResourcePointer<T>() where T : unmanaged
    {
        ref readonly var ptr = ref _indices[ResourceId.Id<T>()];
        Debug.Assert(ptr != null, $"Resource of type {typeof(T).Name} has not been added. Please use {nameof(InitResource)}<{typeof(T).Name}>()");
        return (T*)ptr;
    }

    public ref T GetOrCreateResource<T>() where T : unmanaged
    {
        ref readonly var ptr = ref _indices[ResourceId.Id<T>()];
        if (ptr == null)
        {
            InitResource<T>();
        }
        return ref *(T*)_indices[ResourceId.Id<T>()];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasResource<T>() => _indices[ResourceId.Id<T>()] != null;
}
