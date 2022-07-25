using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.ECS.Components;
using Titan.Memory;

namespace Titan.ECS.Entities;

internal unsafe struct EntityFilterRegistry : IApi
{
    private readonly uint _maxFilters;
    private readonly uint _filterSize;
    private readonly byte* _mem;
    private int _count;

    private EntityFilterRegistry(byte* mem, uint filterSize, uint maxFilters, uint maxEntities, uint maxEntitiesPerFilter)
    {
        _mem = mem;
        _filterSize = filterSize;
        _maxFilters = maxFilters;
        _count = 0;

        /*
         * Memory layout
         * -----------
         * FilterInternal
         * Indexers (max entities)
         * Entities  (maxEntitiesPerFilter)
         * -----------
         */
        for (var i = 0; i < maxFilters; ++i)
        {
            var filter = GetFilter(i);
            filter->Count = 0;
            filter->FilterKey = default;
            filter->Indexers = (int*)(filter + 1);
            filter->Entities = (Entity*)(filter->Indexers + maxEntities);

            Unsafe.InitBlockUnaligned(filter->Indexers, byte.MaxValue, sizeof(int) * maxEntities); // set indexers to -1
        }

    }

    public static EntityFilterRegistry Create(in PlatformAllocator allocator, uint maxFilters, uint maxEntities, uint maxEntitiesPerFilter)
    {
        Logger.Info<EntityFilterRegistry>($"Create {nameof(EntityFilterRegistry)} with max filters: {maxFilters}, max entites: {maxEntities} and max entities per filter: {maxEntitiesPerFilter}");
        var filterSize = maxEntities * sizeof(uint) + maxEntitiesPerFilter * sizeof(Entity) + sizeof(FilterInternal);
        var totalSize = maxFilters * filterSize;
        Logger.Info<EntityFilterRegistry>($"Allocating {totalSize} bytes for {nameof(EntityFilter)}s");

        var filters = (byte*)allocator.Allocate((nuint)totalSize, true);
        return new EntityFilterRegistry(filters, (uint)filterSize, maxFilters, maxEntities, maxEntitiesPerFilter);
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
            GetFilter(i)->EntityChanged(entity, components);
        }
    }

    public void EntityDestroyed(in Entity entity)
    {
        for (var i = 0; i < _count; ++i)
        {
            GetFilter(i)->Remove(entity);
        }
    }

    public EntityFilter GetOrCreate(in EntityFilterConfig entityFilter)
    {
        Debug.Assert(!entityFilter.Include.IsEmpty(), "Defining a filter without any includes have an undefined behavior and is not allowed at the moment.");
        var filter = FindExisting(entityFilter);
        if (filter == null)
        {
            Debug.Assert(_count < _maxFilters, $"Max number of filters has been reached: {_maxFilters}");
            filter = GetFilter(_count++);
            filter->FilterKey = entityFilter;
            filter->Count = 0;
        }
        return new EntityFilter(filter->Entities, &filter->Count);
    }

    private FilterInternal* FindExisting(in EntityFilterConfig config)
    {
        for (var i = 0; i < _count; ++i)
        {
            var filter = GetFilter(i);
            if (filter->FilterKey == config)
            {
                return filter;
            }
        }
        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private FilterInternal* GetFilter(int index) => (FilterInternal*)(_mem + index * _filterSize);

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
