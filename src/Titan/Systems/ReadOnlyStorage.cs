using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.Systems;

public readonly unsafe struct ReadOnlyStorage<T> where T : unmanaged, IComponent
{
    private readonly ComponentPool* _pool;
    internal ReadOnlyStorage(ComponentPool* pool) => _pool = pool;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly T Get(in Entity entity) => ref *_pool->Access<T>(entity);

    [MethodImpl]
    internal T* GetPointer(in Entity entity) => _pool->Access<T>(entity);

    public ref readonly T this[in Entity entity]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Debug.Assert(entity.IsValid);
            return ref Get(entity);
        }
    }
}
