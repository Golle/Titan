using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Entities;

namespace Titan.ECS.SystemsV2.Components;

public unsafe struct ComponentPoolVTable<T> where T : unmanaged
{
    public delegate*<void*, uint, uint, void*> Init;
    public delegate*<uint, uint, uint> CalculateSize;
    public delegate*<void*, in Entity, ref T> Get;
    public delegate*<void*, in Entity, in T, ref T> Create;
    public delegate*<void*, in Entity, in T, ref T> CreateOrReplace;
    public delegate*<void*, in Entity, bool> Contains;
    public delegate*<void*, in Entity, void> Destroy;
}

public unsafe struct PackedComponentPool<T> : IComponentPool<T> where T : unmanaged, IComponent
{
    public static readonly ComponentPoolVTable<T>* Vtbl;
    private readonly uint _maxEntities;
    private readonly uint _maxComponents;

    private readonly int* _indices;
    private readonly T* _components;

    private uint _componentCount;

    static PackedComponentPool()
    {
        Vtbl = (ComponentPoolVTable<T>*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(T), sizeof(ComponentPoolVTable<T>));
        *Vtbl = new ComponentPoolVTable<T>
        {
            CalculateSize = &CalculateSize,
            Init = &Init,
            Contains = &Contains,
            Create = &Create,
            CreateOrReplace = &CreateOrReplace,
            Destroy = &Destroy,
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
        Unsafe.InitBlockUnaligned(mem,0, size);
        var pool = (PackedComponentPool<T>*)mem;
        var indices = (int*)(pool + 1);
        var components = (T*)(indices + maxEntities);
        *pool = new PackedComponentPool<T>(maxEntities, maxComponents, indices, components);
        return pool;
    }


    public static ref T Get(void* data, in Entity entity)
    {
        var pool = (PackedComponentPool<T>*)data;
        return ref pool->_components[pool->_indices[entity.Id]];
    }

    public static ref T Create(void* data, in Entity entity, in T value)
    {
        var pool = (PackedComponentPool<T>*)data;
        ref var component = ref pool->_components[pool->_indices[entity.Id]];
        component = value;
        return ref component;
    }

    public static ref T CreateOrReplace(void* data, in Entity entity, in T value) => ref Create(data, entity, value);
    public static bool Contains(void* data, in Entity entity) =>
        ((PackedComponentPool<T>*)data)->_indices[entity.Id] != 0;

    public static void Destroy(void* data, in Entity entity) =>
        ((PackedComponentPool<T>*)data)->_indices[entity.Id] = 0;
}
