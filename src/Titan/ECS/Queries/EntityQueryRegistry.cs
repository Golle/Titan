using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Memory.Allocators;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Queries;


internal struct InternalEntityQuery
{
    public ComponentId Includes;
    public ComponentId Excludes;

    public TitanArray<int> Indicies;
    public TitanArray<Entity> Entities;
    public int EntityCount;
}


internal unsafe class EntityQueryRegistry
{
    private ILinearAllocator _allocator;

    private TitanArray<InternalEntityQuery> _queries;
    private uint _queryCount;
    private uint _maxEntities;

    public bool Init(IMemoryManager memoryManager, ComponentsRegistry componentsRegistry, uint maxSize, uint maxQueries, uint maxEntities)
    {
        if (!memoryManager.TryCreateLinearAllocator(AllocatorStrategy.Permanent, maxSize, out _allocator))
        {
            Logger.Error<EntityQueryRegistry>($"Failed to allocate {maxSize} for the {nameof(ILinearAllocator)}");
            return false;
        }
        _queries = _allocator.AllocArray<InternalEntityQuery>(maxQueries);
        _queryCount = 0;
        _maxEntities = maxEntities;
        return true;
    }


    public EntityQuery CreateQuery(in EntityQueryArgs args)
    {
        var query = GetAlreadyExistingQuery(args);
        if (query == null)
        {
            Logger.Trace<EntityQueryRegistry>("Query does not exist, creating a new one");
            query = CreateNewQuery(args);
        }
        else
        {
            Logger.Trace<EntityQueryRegistry>("Query has already been defined, using the same.");
        }
        Debug.Assert(query != null);
        // set pointers etc
        Logger.Trace<EntityQueryRegistry>($"{_allocator.GetBytesAllocated()} bytes allocated for Entity Filters");

        return new(query->Entities, &query->EntityCount);
    }

    private InternalEntityQuery* CreateNewQuery(in EntityQueryArgs args)
    {
        Debug.Assert(args.Entities <= _maxEntities, $"The entities count for the filter({args.Entities}) is greater than the max entities({_maxEntities})");
        var query = _queries.GetPointer(_queryCount++);
        var entityCount = args.Entities == 0 ? _maxEntities : args.Entities;

        query->Excludes = args.Exclude;
        query->Includes = args.Include;
        query->Entities = _allocator.AllocArray<Entity>(entityCount, true);
        query->Indicies = _allocator.AllocArray<int>(_maxEntities);
        MemoryUtils.InitArray(query->Indicies, byte.MaxValue); // Set all indices to -1
        return query;
    }

    private InternalEntityQuery* GetAlreadyExistingQuery(in EntityQueryArgs args)
    {
        for (var i = 0; i < _queryCount; ++i)
        {
            var query = _queries.GetPointer(i);
            if (query->Excludes == args.Exclude && query->Includes == args.Include)
            {
                var length = query->Indicies.Length;
                if (args.Entities == 0 && length != _maxEntities)
                {
                    Logger.Warning<EntityQueryRegistry>("A query has same signature but the existing one has a smaller size of entities. This will create a new query and use more memory.");
                    return null;
                }
                if (args.Entities != 0 && args.Entities != length)
                {
                    Logger.Warning<EntityQueryRegistry>("A query has same signature but different sizes. This will create a new query and use more memory.");
                    return null;
                }
                return _queries.GetPointer(i);
            }
        }
        return null;
    }


    public void EntityDestroyed(in Entity entity)
    {
        for (var i = 0; i < _queryCount; ++i)
        {
            ref var query = ref _queries[i];
            ref var index = ref query.Indicies[entity.Id];
            if (index != -1)
            {
                DecrementAndSwap(ref query, index);
                index = -1;
            }
        }
    }
    /// <summary>
    /// When an entity's component composition changes, call this method.
    /// </summary>
    /// <param name="entity">The entity that was changed</param>
    /// <param name="components">The new component structure</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void EntityChanged(in Entity entity, in ComponentId components)
    {
        for (var i = 0; i < _queryCount; ++i)
        {
            ref var query = ref _queries[i];
            var isMatch = query.Includes.IsSubsetOf(components) && query.Excludes.MatchesNone(components);
            ref var index = ref query.Indicies[entity.Id];
            if (index != -1 && !isMatch) // remove
            {
                DecrementAndSwap(ref query, index);
                index = -1;
                //Logger.Trace<EntityQueryRegistry>($"Entity {entity.Id} was removed from filter.");
            }
            else if (index == -1 && isMatch) // add
            {
                index = query.EntityCount++;
                query.Entities[index] = entity;
                //Logger.Trace<EntityQueryRegistry>($"Entity {entity.Id} was added to filter.");
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void DecrementAndSwap(ref InternalEntityQuery query, int index)
    {
        // Decrease the count on the query
        var entityCount = --query.EntityCount;
        // replace the current one with the last one and decrement the counter
        var entityToBeReplaced = query.Entities[entityCount];
        query.Entities[index] = entityToBeReplaced;
        // retarget the indexer for the entity that was replaced
        query.Indicies[entityToBeReplaced] = index;
    }

    public void Shutdown()
    {
        _allocator.Destroy();
    }
}
