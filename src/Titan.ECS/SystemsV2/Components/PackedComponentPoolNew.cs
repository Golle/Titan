using System.Runtime.CompilerServices;
using Titan.Core.Memory;
using Titan.ECS.Entities;

namespace Titan.ECS.SystemsV2.Components;

public unsafe struct ComponentPoolVTable<T> where T : unmanaged
{
    public delegate*<IMemoryAllocator, uint, uint, void*> Init;
    public delegate*<void*, in Entity, ref T> Get;
    public delegate*<void*, in Entity, in T, ref T> Create;
    public delegate*<void*, in Entity, in T, ref T> CreateOrReplace;
    public delegate*<void*, in Entity, bool> Contains;
    public delegate*<void*, in Entity, void> Destroy;
}

public unsafe struct PackedComponentPoolNew<T> : IComponentPool<T> where T : unmanaged
{
    public static readonly ComponentPoolVTable<T>* Vtbl;
    private readonly uint _maxEntities;
    private readonly uint _maxComponents;

    private readonly int* _indices;
    private readonly T* _components;

    private uint _componentCount;

    static PackedComponentPoolNew()
    {
        Vtbl = (ComponentPoolVTable<T>*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(T), sizeof(ComponentPoolVTable<T>));
        *Vtbl = new ComponentPoolVTable<T>
        {
            Init = &Init,
            Contains = &Contains,
            Create = &Create,
            CreateOrReplace = &CreateOrReplace,
            Destroy = &Destroy,
            Get = &Get
        };
    }

    public PackedComponentPoolNew(uint maxEntities, uint maxComponents, int* indices, T* components)
    {
        _maxEntities = maxEntities;
        _maxComponents = maxComponents;
        _indices = indices;
        _components = components;
        _componentCount = 0;
    }

    public static void* Init(IMemoryAllocator allocator, uint maxEntities, uint maxComponents)
    {
        var poolSize = sizeof(PackedComponentPoolNew<T>);
        var componentCount = maxComponents == 0 ? maxEntities : maxComponents;
        var entitiesSize = sizeof(uint) * maxEntities;
        var dataSize = poolSize + entitiesSize + sizeof(T) * componentCount;

        var data = allocator.GetPointer((uint)dataSize, true);
        var pool = (PackedComponentPoolNew<T>*)data;
        var indices = (int*)(pool + 1);
        var components = (T*)(indices + maxEntities);
        *pool = new PackedComponentPoolNew<T>(maxEntities, maxComponents, indices, components);

        return pool;
    }

    public static ref T Get(void* data, in Entity entity)
    {
        var pool = (PackedComponentPoolNew<T>*)data;
        return ref pool->_components[pool->_indices[entity.Id]];
    }

    public static ref T Create(void* data, in Entity entity, in T value)
    {
        var pool = (PackedComponentPoolNew<T>*)data;
        ref var component = ref pool->_components[pool->_indices[entity.Id]];
        component = value;
        return ref component;
    }

    public static ref T CreateOrReplace(void* data, in Entity entity, in T value) => ref Create(data, entity, value);
    public static bool Contains(void* data, in Entity entity) =>
        ((PackedComponentPoolNew<T>*)data)->_indices[entity.Id] != 0;

    public static void Destroy(void* data, in Entity entity) =>
        ((PackedComponentPoolNew<T>*)data)->_indices[entity.Id] = 0;
}
