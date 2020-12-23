using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Titan.ECS.World;

namespace Titan.ECS.Registry
{
    struct Apa
    {
        public string Apan;
    }

    public class ComponentRegistry : IDisposable
    {
        private readonly IDictionary<Type, IComponentPool> _pools = new Dictionary<Type, IComponentPool>();
        private readonly uint _maxEntities;

        public ComponentRegistry(WorldConfiguration configuration)
        {
            _maxEntities = configuration.MaxEntities;
        }

        public void Register<T>(uint maxComponents = 0) where  T : unmanaged => Register(typeof(T), maxComponents);
        public void RegisterManaged<T>(uint maxComponents = 0) where T : struct => Register(typeof(T), maxComponents, true);

        public void Register(Type type, uint maxComponents = 0, bool isManaged = false)
        {
            maxComponents = maxComponents == 0 ? _maxEntities : maxComponents;
            if (_pools.ContainsKey(type))
            {
                throw new InvalidOperationException($"ComponentPool for type {type} has already been created");
            }

            var componentPoolType = isManaged ? typeof(ManagedComponentPool<>).MakeGenericType(type) : typeof(ComponentPool<>).MakeGenericType(type);
            var instance = Activator.CreateInstance(componentPoolType, maxComponents, _maxEntities);
            if (instance == null)
            {
                throw new InvalidOperationException($"Failed to register component pool for type {type}.");
            }

            _pools.Add(type, (IComponentPool) instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IComponentPool<T> GetPool<T>() where T : unmanaged => (IComponentPool<T>) _pools[typeof(T)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IManagedComponentPool<T> GetManagedComponentPool<T>() where T : struct => (IManagedComponentPool<T>)_pools[typeof(T)];

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
