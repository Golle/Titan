using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Entities;

namespace Titan.ECS.Components;

public unsafe struct ComponentPoolCommonVTable
{
    public delegate*<void*, in Entity, bool> Contains;
    public delegate*<void*, in Entity, void> Destroy;
}

public unsafe struct ComponentPoolVTable<T> where T : unmanaged
{
    public ComponentPoolCommonVTable Common;
    public delegate*<void*, uint, uint, void*> Init;
    public delegate*<uint, uint, uint> CalculateSize;
    public delegate*<void*, in Entity, ref T> Get;
    public delegate*<void*, in Entity, in T, bool> Create;
    public delegate*<void*, in Entity, in T, bool> CreateOrReplace;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy(void* context, in Entity entity) => Common.Destroy(context, entity);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(void* context, in Entity entity) => Common.Contains(context, entity);
}

public unsafe struct PackedComponentPool<T> : IComponentPool<T> where T : unmanaged, IComponent
{
    public static readonly ComponentPoolVTable<T>* Vtbl;
    private readonly uint _maxEntities;
    private readonly uint _maxComponents;

    private readonly int* _indices;
    private readonly T* _components;

    private int _componentCount;

    static PackedComponentPool()
    {
        Vtbl = (ComponentPoolVTable<T>*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(T), sizeof(ComponentPoolVTable<T>));
        *Vtbl = new ComponentPoolVTable<T>
        {
            CalculateSize = &CalculateSize,
            Init = &Init,
            Common = new ComponentPoolCommonVTable
            {
                Contains = &Contains,
                Destroy = &Destroy
            },
            Create = &Create,
            CreateOrReplace = &CreateOrReplace,
            Get = &Get
        };
    }

    public PackedComponentPool(uint maxEntities, uint maxComponents, int* indices, T* components)
    {
        _maxEntities = maxEntities;
        _maxComponents = maxComponents;
        _indices = indices;
        _components = components;
        _componentCount = 0;
    }

    private static uint CalculateSize(uint maxEntities, uint maxComponents)
    {
        var poolSize = sizeof(PackedComponentPool<T>);
        var componentCount = maxComponents == 0 ? maxEntities : maxComponents;
        var entitiesSize = sizeof(uint) * maxEntities;
        var dataSize = poolSize + entitiesSize + sizeof(T) * componentCount;
        return (uint)dataSize;
    }

    private static void* Init(void* mem, uint maxEntities, uint maxComponents)
    {
        var size = CalculateSize(maxEntities, maxComponents);
        Unsafe.InitBlockUnaligned(mem, 0, size);
        var pool = (PackedComponentPool<T>*)mem;
        
        var indices = (int*)(pool + 1);
        Unsafe.InitBlockUnaligned(indices, byte.MaxValue, sizeof(int) * maxEntities); // initialize indices with -1
        var components = (T*)(indices + maxEntities);
        *pool = new PackedComponentPool<T>(maxEntities, maxComponents, indices, components);
        return pool;
    }


    public static ref T Get(void* data, in Entity entity)
    {
        var pool = (PackedComponentPool<T>*)data;
        var index = pool->_indices[entity.Id];
        Debug.Assert(index != -1, $"Tried to get a component on entity {entity.Id} that was never added.");
        return ref pool->_components[index];
    }

    // NOTE(Jens): This has not been decided yet, what should Create do if the component has already been added? Right now it will just return false.

    // NOTE(Jens): When we have a propery allocator in place which supports realloc/resize we can start with a small set of entity indices and increase as we go. For example the max might be 10 000 000 but we only create 2000 during runtime, and a specific pool might only use the first 100.
    // NOTE(Jens): making this dynamic would save us a lot of data. Another thing we can do is to implement a map/hashmap for entity lookup. This should not be used for Transform for example, but maybe pools that have very few components.
    public static bool Create(void* data, in Entity entity, in T value)
    {
        var created = false;
        var pool = (PackedComponentPool<T>*)data;
        Debug.Assert(pool->_componentCount < pool->_maxComponents, $"Max number of components for type {typeof(T).Name} has been reached. ({pool->_maxComponents})");
        ref var index = ref pool->_indices[entity.Id];
        if (index == -1) // does not exist
        {
            created = true;
            index = pool->_componentCount++;
            pool->_components[index] = value;
        }
        return created;
    }

    // This method will replace the current value if it exists.
    public static bool CreateOrReplace(void* data, in Entity entity, in T value)
    {
        var created = false;
        var pool = (PackedComponentPool<T>*)data;
        var index = pool->_indices[entity.Id];
        if (index == -1) // does not exist
        {
            Debug.Assert(pool->_componentCount < pool->_maxComponents, $"Max number of components for type {typeof(T).Name} has been reached. ({pool->_maxComponents})");
            created = true;
            index = pool->_componentCount++;
        }
        pool->_components[index] = value;
        return created;
    }

    public static bool Contains(void* data, in Entity entity) => ((PackedComponentPool<T>*)data)->_indices[entity.Id] != -1;
    public static void Destroy(void* data, in Entity entity)
    {
        //throw new NotImplementedException("This has not been implemented, must swap the last with the deleted one and decrement the counter");
        ((PackedComponentPool<T>*)data)->_indices[entity.Id] = -1;
    }
}
