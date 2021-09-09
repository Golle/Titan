using System.Runtime.CompilerServices;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Systems
{
    public readonly struct MutableStorage<T> where T : unmanaged
    {
        private readonly IComponentPool<T> _pool;

        public MutableStorage(IComponentPool<T> pool) => _pool = pool;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(in Entity entity) => ref _pool.Get(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in Entity entity) => _pool.Contains(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Create(in Entity entity) => ref _pool.Create(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy(in Entity entity) => _pool.Destroy(entity);
    }
}
