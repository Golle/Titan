using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Titan.ECS.Registry;

namespace Titan.ECS
{
    public class ComponentRegistry : IDisposable
    {
        private readonly IDictionary<Type, IComponentPool> _pools = new Dictionary<Type, IComponentPool>();
        private readonly uint _maxEntities;

        public ComponentRegistry(uint maxEntities)
        {
            _maxEntities = maxEntities;
        }

        public void Register<T>(uint maxComponents = 0) => Register(typeof(T), maxComponents);

        public void Register(Type type, uint maxComponents = 0)
        {
            maxComponents = maxComponents == 0 ? _maxEntities : maxComponents;
            if (_pools.ContainsKey(type))
            {
                throw new InvalidOperationException($"ComponentPool for type {type} has already been created");
            }

            var instance = Activator.CreateInstance(typeof(ComponentPool<>).MakeGenericType(type), maxComponents, _maxEntities);
            if (instance == null)
            {
                throw new InvalidOperationException($"Failed to register component pool for type {type}.");
            }

            _pools.Add(type, (IComponentPool) instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComponentPool<T> GetPool<T>() where T : unmanaged => (ComponentPool<T>) _pools[typeof(T)];

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
