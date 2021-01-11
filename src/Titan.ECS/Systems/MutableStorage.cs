using System.Runtime.CompilerServices;
using Titan.ECS.Entities;
using Titan.ECS.Registry;

namespace Titan.ECS.Systems
{
    public readonly struct MutableStorage<T> where T : unmanaged
    {
        private readonly IComponentPool<T> _pool;
        public MutableStorage(IComponentPool<T> pool) => _pool = pool;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(in Entity entity) => ref _pool[entity];
    }
}