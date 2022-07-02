using System;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.ECS.TheNew;

public unsafe class UnmanagedResources
{
    private readonly PermanentMemory _allocator;
    private readonly void** _indices;

    public UnmanagedResources(uint resourceTypes, PermanentMemory allocator)
    {
        _allocator = allocator;
        _indices = (void**)allocator.GetPointer((uint)(sizeof(void*) * resourceTypes), true);
    }

    public void InitResource<T>(in T value = default) where T : unmanaged
    {
        var id = ResourceId.Id<T>();
        if (_indices[id] == null)
        {
            Logger.Trace<UnmanagedResources>($"Resource type: {typeof(T).Name} with Id {id} does not exist, creating.");
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
