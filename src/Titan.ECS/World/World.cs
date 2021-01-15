using System.Runtime.CompilerServices;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Messaging;
using Titan.ECS.Messaging.Events;
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
            _eventManager.Update();
            EntityManager.Update();
            _entityFactory.Update();
            FilterManager.Update();
        }
        public IEntityManager EntityManager { get; }
        public IEntityInfoRepository EntityInfo { get; }
        public IEntityFilterManager FilterManager { get; }

        private readonly ComponentRegistry _registry;
        private readonly IEntityFactory _entityFactory;
        private readonly IEventManager _eventManager;

        public World(WorldConfiguration configuration, ComponentRegistry registry, IEntityManager entityManager, IEntityFactory entityFactory, IEntityInfoRepository entityInfoRepository, IEntityFilterManager entityFilterManager, IEventManager eventManager)
        {
            Id = configuration.WorldId;
            MaxEntities = configuration.MaxEntities;
            _registry = registry;
            EntityManager = entityManager;
            _entityFactory = entityFactory;
            _eventManager = eventManager;
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
            GetComponentPool<T>().Create(entity, value);
            ComponentAdded<T>(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddManagedComponent<T>(in Entity entity, in T initialValue) where T : struct
        {
            GetManagedComponentPool<T>().Create(entity, initialValue);
            ComponentAdded<T>(entity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ComponentAdded<T>(Entity entity) where T : struct
        {
            ref var info = ref EntityInfo[entity];
            var componentId = ComponentId<T>.Id;
            info.Components += componentId;
            _eventManager.Push(new ComponentAddedEvent(entity, componentId));
            _eventManager.Push(new EntityChangedEvent(entity, info.Components));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent<T>(in Entity entity) where T : unmanaged
        {
            _registry.GetPool<T>().Destroy(entity);
            ComponentRemoved<T>(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveManagedComponent<T>(in Entity entity) where T : struct
        {
            _registry.GetManagedComponentPool<T>().Destroy(entity);
            ComponentRemoved<T>(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ComponentRemoved<T>(Entity entity) where T : struct
        {
            ref var info = ref EntityInfo[entity];
            var componentId = ComponentId<T>.Id;
            info.Components -= componentId;
            _eventManager.Push(new ComponentRemovedEvent(entity, componentId));
            _eventManager.Push(new EntityChangedEvent(entity, info.Components));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IComponentPool<T> GetComponentPool<T>() where T : unmanaged => _registry.GetPool<T>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IManagedComponentPool<T> GetManagedComponentPool<T>() where T : struct => _registry.GetManagedComponentPool<T>();

        public void Dispose() => WorldContainer.Remove(this);
    }
}
