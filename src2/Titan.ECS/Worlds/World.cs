using System;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Events;

namespace Titan.ECS.Worlds
{
    public record WorldConfiguration(uint MaxEntities, ComponentConfiguration[] Components)
    {
        public uint Id { get; init; }
    }

    public record ComponentConfiguration(Type Type, ComponentPoolTypes PoolType, uint Count = 0);

    public class World : IDisposable
    {
        private readonly uint _id;
        private readonly EntityManager _entityManager;
        private readonly EntityInfoManager _entityInfoManager;
        private readonly ComponentRegistry _componentRegistry;

        private static readonly IdContainer WorldIds = new(100);
        private static readonly World[] Worlds = new World[100];

        public World(WorldConfiguration config)
        {
            _id = WorldIds.Next();
            Logger.Trace<World>($"Creating world {_id}");
            config = config with {Id = _id};
            _entityManager = new(config);
            _componentRegistry = new (config);
            _entityInfoManager = new(config);
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
            _entityInfoManager.Dispose();
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
        public void AddComponent<T>(in Entity entity) where T : unmanaged
        {
            _componentRegistry.GetPool<T>().Create(entity);
            ComponentAdded<T>(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in Entity entity, in T value) where T : unmanaged
        {
            _componentRegistry.GetPool<T>().Create(entity, value);
            ComponentAdded<T>(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ComponentAdded<T>(in Entity entity) where T : unmanaged
        {
            ref var info = ref _entityInfoManager.Get(entity);
            var componentId = ComponentId<T>.Id;
            info.Components += componentId;
            EventManager.Push(new ComponentAddedEvent(entity, componentId));
            EventManager.Push(new EntityChangedEvent(entity, info.Components));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent<T>(in Entity entity) where T : unmanaged
        {
            // TODO: should this be done in 2 frames? flag for deletion, and delete in next frame?
            _componentRegistry.GetPool<T>().Destroy(entity);
            ref var info = ref _entityInfoManager.Get(entity);
            var componentId = ComponentId<T>.Id;
            info.Components -= componentId;
            EventManager.Push(new ComponentRemovedEvent(entity, componentId));
            EventManager.Push(new EntityChangedEvent(entity, info.Components));
        }


        public void Update()
        {
            _entityManager.Update();
            _componentRegistry.Update();
        }
    }

    
}
