using System.Diagnostics;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.ECS.Entities;

internal unsafe struct EntityIDContainer
{
    private IMemoryManager _memoryManager;
    private uint* _entities;
    private volatile uint _head;
    private uint _max;

    public Entity Next()
    {
        var index = Interlocked.Decrement(ref _head);
        Debug.Assert(index != 0, "Max number of entities have been created");
        //Logger.Trace<EntityIDContainer>($"Entity created: {_entities[index]}");
        return _entities[index];
    }

    public void Return(Entity entity)
    {
        AssertDuplicate(entity);
        var index = Interlocked.Increment(ref _head) - 1;
        //Logger.Trace<EntityIDContainer>($"Entity returned: {entity.Id}");
        _entities[index] = entity;
    }

    [Conditional("DEBUG")]
    private void AssertDuplicate(Entity entity)
    {
        for (var i = 0; i < _head; ++i)
        {
            if (_entities[i] == entity)
            {
                Debug.Fail($"Entity with ID {entity.Id} has already been returned.");
            }
        }
    }

    public bool Init(IMemoryManager memoryManager, uint maxEntities)
    {
        Debug.Assert(_entities == null);
        Debug.Assert(maxEntities > 0);

        var entities = memoryManager.Alloc<uint>(maxEntities);
        if (entities == null)
        {
            Logger.Error<EntityIDContainer>($"Failed to alloc memory for {maxEntities} entities.");
            return false;
        }

        for (var i = 1u; i < maxEntities; ++i)
        {
            entities[i] = maxEntities - i;
        }

        _head = maxEntities;
        _entities = entities;
        _max = maxEntities;
        _memoryManager = memoryManager;

        return true;
    }

    public void Shutdown()
    {
        Debug.Assert(_entities != null);
        _memoryManager.Free(_entities);
        // reset the instance so it can't be used again.
        this = default;
    }
}
