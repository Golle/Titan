using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Memory.Allocators;
using Titan.ECS.Components.Pools;
using Titan.ECS.Entities;

namespace Titan.ECS.Components;

internal unsafe struct PackedComponentPool : IComponentPool
{
    private TitanArray<int> _indicies;
    private TitanArray<int> _freeList;
    private TitanArray _components;
    private volatile int _nextComponent; //NOTE(Jens): This must be volatile if we can create components from different threads.
    private volatile int _freeCount;
    public bool Init(ILinearAllocator allocator, in PoolConfig config)
    {
        if (config.Count == config.MaxEntities)
        {
            Logger.Warning<PackedComponentPool>($"The MaxEntites and the Component Count are the same. Using a {nameof(PackedComponentPool)} can degrade performance.");
        }
        var totalSize = config.Count * sizeof(int) + config.MaxEntities * sizeof(int) + config.Count * config.Stride;
#if DEBUG
        //NOTE(Jens): Compare the memory used with a Sparse pool to get an idea of the difference and if we can optimize the memory footprint or performance.
        var sparsePoolSize = config.MaxEntities * sizeof(int) + config.Stride * config.MaxEntities;
        if (totalSize >= sparsePoolSize)
        {
            Logger.Warning<PackedComponentPool>($"The size consumed by the {nameof(PackedComponentPool)}<{config.Type}> is equal or greater than a {nameof(SparseComponentPool)}<{config.Type}>. This will degrade performance with very litle benefit.");
        }
        else if (totalSize / (float)sparsePoolSize > 0.7)
        {
            Logger.Warning<PackedComponentPool>($"The size consumed by the {nameof(PackedComponentPool)}<{config.Type}> is very close to the {nameof(SparseComponentPool)}<{config.Type}> (~{(uint)(totalSize / (float)sparsePoolSize * 100)}%). This will degrade performance with very litle benefit.");
        }
#endif
        _indicies = allocator.AllocArray<int>(config.MaxEntities);
        MemoryUtils.InitArray(_indicies, byte.MaxValue);
        _components = allocator.AllocArray(config.Count, config.Stride, true);
        Logger.Trace<PackedComponentPool>($"{nameof(PackedComponentPool)} initialized. Component = {config.Type}. Count = {config.Count}. Stride = {config.Stride}. MaxEntities = {config.MaxEntities}. Size = {totalSize} bytes.");
        _freeList = allocator.AllocArray<int>(config.Count, true);
        return true;
    }

    public void* Create(in Entity entity)
    {
        ref var index = ref _indicies[entity];
        Debug.Assert(index == -1, $"A component has already been created. Entity = {entity.Id}");

        if (!TryGetFromFreeList(out index))
        {
            index = Interlocked.Increment(ref _nextComponent);
        }

        return _components.GetPointer(index);
    }

    private bool TryGetFromFreeList(out int index)
    {
        //NOTE(Jens): We only do this once, because it's not a big deal if we increment the other counter when a conflict happens.
        Unsafe.SkipInit(out index);
        var oldCount = _freeCount;
        if (oldCount <= 0)
        {
            return false;
        }
        var freeIndex = oldCount - 1;
        var oldValue = Interlocked.CompareExchange(ref _freeCount, freeIndex, oldCount);
        if (oldValue == oldCount)
        {
            index = _freeList[freeIndex];
            return true;
        }
        return false;
    }

    public void Destroy(in Entity entity)
    {
        ref var index = ref _indicies[entity];
        Debug.Assert(index != -1);
        // ReSharper disable once NonAtomicCompoundOperator 
        //NOTE(Jens): Destroy will never be called from different threads, so we can safely increase the count without Interlocked.
        _freeList[_freeCount++] = index;
        index = -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in Entity entity) => _indicies[entity.Id] != -1;

    public void* Access(in Entity entity)
    {
        Debug.Assert(_indicies[entity] != -1);
        return _components.GetPointer(_indicies[entity]);
    }
}
