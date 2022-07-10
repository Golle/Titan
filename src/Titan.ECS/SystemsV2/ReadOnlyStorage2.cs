using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Entities;
using Titan.ECS.SystemsV2.Components;

namespace Titan.ECS.SystemsV2;

public readonly unsafe struct ReadOnlyStorage2<T> where T : unmanaged, IComponent
{
    private readonly Components<T>* _pool;
    internal ReadOnlyStorage2(Components<T>*  pool) => _pool = pool;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly T Get(in Entity entity) => ref _pool->Get(entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in Entity entity) => _pool->Contains(entity);
}
