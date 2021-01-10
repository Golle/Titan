using System.Runtime.CompilerServices;
using Titan.ECS.Entities;
using Titan.ECS.Registry;

namespace Titan.ECS.Systems
{
    public readonly struct ReadOnlyStorage<T> where T : unmanaged
    {
        private readonly IComponentPool<T> _pool;
        public ReadOnlyStorage(IComponentPool<T> pool) => _pool = pool;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T Get(in Entity entity) => ref _pool[entity];

    }
}
