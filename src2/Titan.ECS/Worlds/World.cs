using System;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Worlds
{
    public record WorldConfiguration(uint MaxEntities, ComponentConfiguration[] Components);
    public record ComponentConfiguration(Type Type, ComponentPoolTypes PoolType, uint Count = 0);

    public class World : IDisposable
    {
        private readonly uint _id;
        private readonly EntityManager _entityManager;
        private readonly ComponentRegistry _componentRegistry;

        private static readonly IdContainer WorldIds = new(100);
        private static readonly World[] Worlds = new World[100];

        public World(WorldConfiguration config)
        {
            _id = WorldIds.Next();
            Logger.Trace<World>($"Creating world {_id}");
            _entityManager = new(_id, config);
            _componentRegistry = new (config);
            Worlds[_id] = this;
        }

        internal static World GetWorldById(uint worldId)
        {
            var world = Worlds[worldId];
#if DEBUG
            if (world == null)
            {
                throw new InvalidOperationException($"World with id {worldId} does not exist.");
            }
#endif
            return world;
        }

        public void Dispose()
        {
            Logger.Trace<World>($"Disposing world {_id}");
            _entityManager.Dispose();
            Worlds[_id] = null;
            WorldIds.Return(_id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AttachEntity(in Entity parent, in Entity entity) => _entityManager.Attach(parent, entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DetachEntity(in Entity entity) => _entityManager.Detach(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyEntity(in Entity entity) => _entityManager.Destroy(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity() => _entityManager.Create();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in Entity entity) where T : unmanaged => _componentRegistry.GetPool<T>().Create(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in Entity entity, in T value) where T : unmanaged => _componentRegistry.GetPool<T>().Create(entity, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent<T>(in Entity entity) where T : unmanaged => _componentRegistry.GetPool<T>().Destroy(entity);
    }
}
