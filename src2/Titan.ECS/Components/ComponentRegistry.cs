using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Titan.ECS.Components
{
    internal class ComponentRegistry : IDisposable
    {
        private readonly Dictionary<Type, IComponentPool> _pools = new();
        public void Register<T>(IComponentPool<T> pool) where T : unmanaged
        {
            var type = typeof(T);
            if (_pools.ContainsKey(type))
            {
                throw new InvalidOperationException($"A component pool of type {type.Name} has already been registered");
            }
            _pools[type] = pool;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IComponentPool<T> GetPool<T>() where T : unmanaged => (IComponentPool<T>)_pools[typeof(T)];

        public void Dispose()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Dispose();
            }
            _pools.Clear();
        }
    }
}
