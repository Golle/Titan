using System.Runtime.CompilerServices;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Systems;

public readonly struct ReadOnlyStorage<T> where T : unmanaged
{
    private readonly IComponentPool<T> _pool;
    public ReadOnlyStorage(IComponentPool<T> pool) => _pool = pool;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly T Get(in Entity entity) => ref _pool.Get(entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in Entity entity) => _pool.Contains(entity);
}
