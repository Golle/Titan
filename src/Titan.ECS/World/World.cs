using System.Runtime.CompilerServices;
using Titan.Core.Messaging;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Events;
using Titan.ECS.Registry;

namespace Titan.ECS.World
{
    internal class World : IWorld
    {
        public uint Id { get; }
        public uint MaxEntities { get; }
        public void Update()
        {
            // TODO: this should not be in the World class, every service/manager should be added to a game loop where they can be executed async if needed.
            _eventQueue.Update();
            EntityManager.Update();
            _entityFactory.Update();
        }
        public IEntityManager EntityManager { get; }
        public IEntityInfoRepository EntityInfo { get; }
        public IEntityFilterManager FilterManager { get; }

        private readonly ComponentRegistry _registry;
        private readonly IEntityFactory _entityFactory;
        private readonly IEventQueue _eventQueue;

        public World(WorldConfiguration configuration, ComponentRegistry registry, IEntityManager entityManager, IEntityFactory entityFactory, IEntityInfoRepository entityInfoRepository, IEntityFilterManager entityFilterManager, IEventQueue eventQueue)
        {
            Id = configuration.WorldId;
            MaxEntities = configuration.MaxEntities;
            _registry = registry;
            EntityManager = entityManager;
            _entityFactory = entityFactory;
            _eventQueue = eventQueue;
            FilterManager = entityFilterManager;
            
            EntityInfo = entityInfoRepository;
            
            WorldContainer.Add(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity() => EntityManager.Create();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Attach(in Entity parent, in Entity child) => EntityManager.Attach(parent, child);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Detach(in Entity child) => EntityManager.Detach(child);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyEntity(in Entity entity) => EntityManager.Destroy(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in Entity entity) where T : unmanaged => AddComponent<T>(entity, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in Entity entity, in T value) where T : unmanaged
        {
            _registry.GetPool<T>().Create(entity, value);
            ref var info = ref EntityInfo[entity];
            var componentId = ComponentId<T>.Id;
            info.ComponentMask += componentId;
            _eventQueue.Push(new ComponentAddedEvent(entity, componentId));
            _eventQueue.Push(new EntityChangedEvent(entity, info.ComponentMask));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent<T>(in Entity entity) where T : unmanaged
        {
            _registry.GetPool<T>().Destroy(entity);
            ref var info = ref EntityInfo[entity];
            var componentId = ComponentId<T>.Id;
            info.ComponentMask -= componentId;
            _eventQueue.Push(new ComponentRemovedEvent(entity, componentId));
            _eventQueue.Push(new EntityChangedEvent(entity, info.ComponentMask));
        }

        public IComponentPool<T> GetComponentPool<T>() where T : unmanaged => _registry.GetPool<T>();

        public void Dispose() => WorldContainer.Remove(this);
    }
}
