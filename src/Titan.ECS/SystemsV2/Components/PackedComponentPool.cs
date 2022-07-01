using Titan.Core.Memory;
using Titan.ECS.Entities;

namespace Titan.ECS.SystemsV2.Components;

public unsafe struct PackedComponentPool<T> : IComponentPool<T> where T : unmanaged
{
    private readonly uint _maxEntities;
    private readonly uint _maxComponents;

    private readonly int* _indices;
    private readonly T* _components;

    private uint _componentCount;

    public PackedComponentPool(uint maxEntities, uint maxComponents, int* indices, T* components)
    {
        _maxEntities = maxEntities;
        _maxComponents = maxComponents;
        _indices = indices;
        _components = components;
        _componentCount = 0;
    }

    public static Components<T> CreatePool(IMemoryAllocator allocator, uint maxEntities, uint maxComponents = 0)
    {
        var poolSize = sizeof(PackedComponentPool<T>);
        var componentCount = maxComponents == 0 ? maxEntities : maxComponents;
        var entitiesSize = sizeof(uint) * maxEntities;
        var dataSize = poolSize + entitiesSize + sizeof(T) * componentCount;

        var data = allocator.GetBlock((uint)dataSize, true).AsPointer();
        var pool = (PackedComponentPool<T>*)data;
        var indices = (int*)(pool + 1);
        var components = (T*)(indices + maxEntities);
        *pool = new PackedComponentPool<T>(maxEntities, maxComponents, indices, components);

        return new Components<T>
        {
            CreateFunc = &Create,
            CreateOrReplaceFunc = &CreateOrReplace,
            GetFunc = &Get,
            Data = data
        };
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
