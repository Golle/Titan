using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.EntitiesNew;

internal unsafe struct EntityFilterRegistry : IApi
{
    private readonly uint _maxFilters;
    private readonly uint _maxEntities;
    private readonly uint _maxComponentsPerFilter;
    private readonly MemoryPool _memoryPool;
    private readonly FilterInternal* _filters;
    private int _count;

    private EntityFilterRegistry(FilterInternal* filters, uint maxFilters, uint maxEntities, uint maxComponentsPerFilter, in MemoryPool memoryPool)
    {
        _filters = filters;
        _maxFilters = maxFilters;
        _maxEntities = maxEntities;
        _maxComponentsPerFilter = maxComponentsPerFilter;
        _memoryPool = memoryPool;
        _count = 0;
    }

    //NOTE(Jens): This will allocate a lot of memory up front. This should be using dynamic allocators and only expand when needed.
    public static EntityFilterRegistry Create(in MemoryPool pool, uint maxFilters, uint maxEntities, uint maxComponentsPerFilter)
    {
        Logger.Info<EntityFilterRegistry>($"Create {nameof(EntityFilterRegistry)} with max filters: {maxFilters}, max entites: {maxEntities} and max components per filter: {maxComponentsPerFilter}");
        var size = (uint)(maxFilters * sizeof(FilterInternal));
        var filters = pool.GetPointer<FilterInternal>(size);
        return new EntityFilterRegistry(filters, maxFilters, maxEntities, maxComponentsPerFilter, pool);
    }

    /// <summary>
    /// Call this when an entity has changed the component list
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="components">The component mask on the entity to match agains the filter</param>
    public void EntityChanged(in Entity entity, in ComponentId components)
    {
        for (var i = 0; i < _count; ++i)
        {
            _filters[i].EntityChanged(entity, components);
        }
    }

    public void EntityDestroyed(in Entity entity)
    {
        for (var i = 0; i < _count; ++i)
        {
            _filters[i].Remove(entity);
        }
    }

    public EntityFilter GetOrCreate(in EntityFilterConfig entityFilter)
    {
        Debug.Assert(!entityFilter.Include.IsEmpty(), "Defining a filter without any includes have an undefined behavior and is not allowed at the moment.");
        var index = FindExisting(entityFilter);
        if (index == -1)
        {
            Debug.Assert(_count < _maxFilters, $"Max number of filters has been reached: {_maxFilters}");
            index = _count++;
            ref var filter = ref _filters[index];
            filter = new FilterInternal
            {
                Entities = _memoryPool.GetPointer<Entity>(_maxComponentsPerFilter),
                Indexers = _memoryPool.GetPointer<int>(_maxEntities),
                FilterKey = entityFilter,
                Count = 0
            };
            Unsafe.InitBlockUnaligned(_filters[index].Indexers, byte.MaxValue, sizeof(int) * _maxEntities); // set indexers to -1
        }

        return new EntityFilter(_filters[index].Entities, &_filters[index].Count);
    }

    private int FindExisting(in EntityFilterConfig filter)
    {
        for (var i = 0; i < _count; ++i)
        {
            if (_filters[i].FilterKey == filter)
            {
                return i;
            }
        }
        return -1;
    }

    private struct FilterInternal
    {
        public EntityFilterConfig FilterKey;
        public Entity* Entities;    // Max entities in specific filter
        public int* Indexers;       // size of MaxEntities
        public int Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void EntityChanged(in Entity entity, in ComponentId components)
        {
            var isMatch = FilterKey.Include.IsSubsetOf(components) && FilterKey.Exclude.MatchesNone(components);
            ref var index = ref Indexers[entity.Id];
            if (index != -1 && !isMatch) // remove
            {
                DecrementAndSwap(index);
                index = -1;
                Logger.Trace<FilterInternal>($"Entity {entity.Id} was removed from filter.");
            }
            else if (index == -1 && isMatch) // add
            {
                index = Count++;
                Entities[index] = entity;
                Logger.Trace<FilterInternal>($"Entity {entity.Id} was added to filter.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Remove(in Entity entity)
        {
            ref var index = ref Indexers[entity.Id];
            if (index != -1)
            {
                DecrementAndSwap(index);
                index = -1;
                Logger.Trace<FilterInternal>($"Entity {entity.Id} was removed from filter. (Entity destroyed)");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DecrementAndSwap(int index)
        {
            // replace the current one with the last one and decrement the counter
            Entities[index] = Entities[--Count];
            // retarget the indexer for the entity that was replaced
            Indexers[Entities[index]] = index;
        }
    }
}
