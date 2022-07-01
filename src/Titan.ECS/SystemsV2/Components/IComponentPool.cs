using Titan.Core.Memory;
using Titan.ECS.Entities;

namespace Titan.ECS.SystemsV2.Components;

public unsafe interface IComponentPool<T> where T : unmanaged
{
    static abstract Components<T> CreatePool(IMemoryAllocator allocator, uint maxEntities, uint maxComponents = 0);
    static abstract ref T Get(void* data, in Entity entity);
    static abstract ref T Create(void* data, in Entity entity, in T value);
    static abstract ref T CreateOrReplace(void* data, in Entity entity, in T value);
    static abstract bool Contains(void* data, in Entity entity);
    static abstract void Destroy(void* data, in Entity entity);
}
