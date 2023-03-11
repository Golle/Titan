using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Memory;
using Titan.Core.Memory.Allocators;
using Titan.ECS.Entities;

namespace Titan.ECS.Components.Pools;

internal unsafe struct SparseComponentPool : IComponentPool
{
    private TitanArray _components;
#if DEBUG
    private TitanArray<bool> _entities;
#endif
    public bool Init(ILinearAllocator allocator, in PoolConfig config)
    {
        Debug.Assert(config.Count == config.MaxEntities, "To use a sparse component pool the number of components must be the same as the max entities");

        _components = allocator.AllocArray(config.Count, config.Stride, true);
#if DEBUG
        _entities = allocator.AllocArray<bool>(config.MaxEntities);
#endif
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* Create(in Entity entity)
    {

#if DEBUG
        Debug.Assert(_entities[entity] == false);
        _entities[entity] = true;
#endif
        return _components.GetPointer(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy(in Entity entity)
    {
        //NOTE(Jens): destroy does not do anything in a sparse array, there are no ids to keep track of
#if DEBUG
        Debug.Assert(_entities[entity]);
        _entities[entity.Id] = false;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in Entity entity)
    {
        //NOTE(Jens): we don't track entities in this, there's always a component available.
#if DEBUG
        return _entities[entity];
#endif
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* Access(in Entity entity)
    {
#if DEBUG
        Debug.Assert(_entities[entity]);
#endif
        return _components.GetPointer(entity);
    }
}
