using System;
using Titan.Core.Memory;
using Titan.ECS.Entities;

namespace Titan.ECS.SystemsV2.Components;

public unsafe struct SparseComponentPool<T> : IComponentPool<T> where T : unmanaged
{
    public static Components<T> CreatePool(in PermanentMemory allocator, uint maxEntities, uint maxComponents = 0)
    {
        throw new NotImplementedException();
    }

    public static ref T Get(void* pool, in Entity entity)
    {
        throw new System.NotImplementedException();
    }

    public static ref T Create(void* data, in Entity entity, in T value)
    {
        throw new NotImplementedException();
    }

    public static ref T CreateOrReplace(void* data, in Entity entity, in T value)
    {
        throw new NotImplementedException();
    }

    public static bool Contains(void* data, in Entity entity)
    {
        throw new NotImplementedException();
    }

    public static void Destroy(void* data, in Entity entity)
    {
        throw new NotImplementedException();
    }
}
