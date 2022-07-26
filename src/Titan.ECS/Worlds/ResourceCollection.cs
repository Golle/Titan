using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Memory;
using Titan.Memory.Arenas;

namespace Titan.ECS.Worlds;

public unsafe struct ResourceCollection
{
    private FixedSizeLinearArena _arena;
    private readonly void** _indices;
    private readonly uint _indicesCount;

    private ResourceCollection(in FixedSizeLinearArena arena, void** indices, uint indicesCount)
    {
        _arena = arena;
        _indices = indices;
        _indicesCount = indicesCount;
    }

    public static ResourceCollection Create(uint initialSize, uint maxTypes, in PlatformAllocator allocator)
    {
        var indicesSize = (nuint)sizeof(void*) * maxTypes;
        var totalSize = initialSize + indicesSize;
        var backingBuffer = allocator.Allocate(totalSize);
        Debug.Assert(backingBuffer != null, $"Failed to create the backing buffer for {nameof(ResourceCollection)}");
        var arena = FixedSizeLinearArena.Create(backingBuffer, totalSize);
        var indices = (void**)arena.Allocate(indicesSize);
        return new ResourceCollection(arena, indices, maxTypes);
    }

    public void InitResource<T>(in T value = default) where T : unmanaged
    {
        var id = ResourceId.Id<T>();
        if (_indices[id] == null)
        {
            Logger.Trace<ResourceCollection>($"Resource type: {typeof(T).FormattedName()} with Id {id} does not exist, creating.");
            _indices[id] = _arena.Allocate<T>();
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
