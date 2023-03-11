using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.Systems;

public readonly unsafe struct MutableStorage<T> where T : unmanaged, IComponent
{
    private readonly ComponentPool* _pool;
    private readonly ObjectHandle<ComponentsRegistry> _registry;

    internal MutableStorage(ComponentPool* pool, ObjectHandle<ComponentsRegistry> registry)
    {
        _pool = pool;
        _registry = registry;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get(in Entity entity) => ref *_pool->Access<T>(entity);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* GetPointer(in Entity entity) => _pool->Access<T>(entity);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Add(in Entity entity)
    {
        //NOTE(Jens): we can access the pool from here, but no event will be sent if we do that. so unless its a problem we use the registry.
        //return ref *_pool->Create<T>(entity);

        Debug.Assert(entity.IsValid);
        return ref _registry.Value.Create<T>(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove(in Entity entity)
    {
        Debug.Assert(entity.IsValid);
        _registry.Value.Remove<T>(entity);
    }

    public ref T this[in Entity entity]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Debug.Assert(entity.IsValid);
            return ref Get(entity);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in Entity entity) => _pool->Contains(entity);
}
