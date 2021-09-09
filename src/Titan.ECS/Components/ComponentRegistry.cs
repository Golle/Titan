using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.ECS.Events;
using Titan.ECS.Worlds;

namespace Titan.ECS.Components
{
    internal class ComponentRegistry : IDisposable
    {
        private readonly WorldConfiguration _config;
        private readonly Dictionary<Type, IComponentPool> _pools = new();

        private static readonly Type SparseType = typeof(SparseComponentPool<>);
        private static readonly Type PackedType = typeof(PackedComponentPool<>);
        private readonly uint _worldId;
        public ComponentRegistry(WorldConfiguration config)
        {
            _config = config;
            _worldId = config.Id;
            Logger.Trace<ComponentRegistry>("Register Components");
            foreach (var (type, poolType, count) in config.Components)
            {
                Register(type, poolType, count);
            }
            Logger.Trace<ComponentRegistry>("Register Components Finished");
        }

        public void Register<T>(ComponentPoolTypes poolType, uint numberOfComponents = 0) where T : unmanaged => Register(typeof(T), poolType, numberOfComponents);
        public void Register(Type componentType, ComponentPoolTypes poolType, uint numberOfComponents = 0)
        {
            if (_pools.ContainsKey(componentType))
            {
                throw new InvalidOperationException($"A component pool of type {componentType.Name} has already been registered");
            }
            
            var count = numberOfComponents == 0 ? _config.MaxEntities : numberOfComponents;
            var pool = poolType switch
            {
                ComponentPoolTypes.Packed => (IComponentPool)Activator.CreateInstance(PackedType.MakeGenericType(componentType), count, _config.MaxEntities),
                ComponentPoolTypes.Sparse => (IComponentPool)Activator.CreateInstance(SparseType.MakeGenericType(componentType), _config.MaxEntities),
                _ => throw new NotSupportedException()
            };

            Logger.Trace<ComponentRegistry>($"Added a {poolType} component pool for type {componentType.Name} with a max capacity {count}");
            _pools[componentType] = pool;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IComponentPool<T> GetPool<T>() where T : unmanaged => (IComponentPool<T>)_pools[typeof(T)];

        public void Update()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Update();
            }

            foreach (ref readonly var @event in EventManager.GetEvents())
            {
                if (@event.Type == EntityBeingDestroyedEvent.Id)
                {
                    ref readonly var entityEvent = ref @event.As<EntityBeingDestroyedEvent>();
                    if (entityEvent.Entity.WorldId == _worldId)
                    {
                        foreach (var pool in _pools.Values)
                        {
                            pool.Destroy(entityEvent.Entity);
                        }
                    }
                }
            }
        }

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
