using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Memory.Allocators;
using Titan.ECS.Components.Pools;
using Titan.ECS.Entities;
using Titan.Events;

namespace Titan.ECS.Components;

internal unsafe class ComponentsRegistry
{
    private TitanArray<ComponentPool> _pools;
    private ILinearAllocator _allocator;
    private IEventsManager _eventsManager;

    public bool Init(IMemoryManager memoryManager, IEventsManager eventsManager, ComponentConfig[] configs, uint maxEntities, uint maxComponentsSize)
    {
        if (!memoryManager.TryCreateLinearAllocator(AllocatorStrategy.Permanent, maxComponentsSize, out var allocator))
        {
            Logger.Trace<ComponentsRegistry>($"Failed to create an allocator with size {maxComponentsSize} bytes.");
            return false;
        }
        _pools = allocator.AllocArray<ComponentPool>(configs.Length);
        for (var i = 0; i < configs.Length; ++i)
        {
            var config = configs[i];
            var numberOfComponents = config.Count == 0 ? maxEntities : config.Count;
            Debug.Assert(numberOfComponents <= maxEntities);
            var size = GetSize(config.Type);
            var context = allocator.Alloc(size, true);
            _pools[i] = CreatePool(config.Type, context);
            if (!_pools[i].Init(allocator, new PoolConfig(config.Size, numberOfComponents, maxEntities, config.ComponentName)))
            {
                Logger.Error<ComponentsRegistry>($"Failed to init the ComponentPool at index {i}");
                return false;
            }
        }

        _allocator = allocator;
        _eventsManager = eventsManager;
        return true;

        static ComponentPool CreatePool(ComponentPoolType type, void* context)
        {
            Debug.Assert(context != null);
            return type switch
            {
                ComponentPoolType.Packed => ComponentPool.CreatePool<PackedComponentPool>(context),
                ComponentPoolType.Sparse => ComponentPool.CreatePool<SparseComponentPool>(context),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        static uint GetSize(ComponentPoolType type) => type switch
        {
            ComponentPoolType.Packed => MemoryUtils.AlignToUpper(sizeof(PackedComponentPool)),
            ComponentPoolType.Sparse => MemoryUtils.AlignToUpper(sizeof(SparseComponentPool)),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy(uint poolIndex, in Entity entity)
    {
        Debug.Assert(poolIndex < _pools.Length);
        _pools[poolIndex].Destroy(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DestroyEntity(in Entity entity)
    {
        foreach (ref readonly var pool in _pools.AsReadOnlySpan())
        {
            if (pool.Contains(entity))
            {
                pool.Destroy(entity);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Create<T>(in Entity entity) where T : unmanaged, IComponent
    {
        var id = ComponentId<T>.Id;
        var index = ComponentId<T>.Index;
        var component = GetPool(index)
            ->Create<T>(entity);
        Debug.Assert(component != null, "Component is null");
        _eventsManager.Send(new ComponentAdded(id, index, entity));
        return ref *component;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentPool* GetPool<T>() where T : unmanaged, IComponent
        => GetPool(ComponentId<T>.Index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentPool* GetPool(uint index)
    {
        Debug.Assert(index < _pools.Length);
        return _pools.GetPointer(index);
    }

    public void Remove<T>(in Entity entity) where T : unmanaged, IComponent
    {
        var id = ComponentId<T>.Id;
        var index = ComponentId<T>.Index;
        _eventsManager.Send(new ComponentBeingRemoved(id, index, entity));
    }

    public void Shutdown()
    {
        _allocator.Destroy();
    }
}
