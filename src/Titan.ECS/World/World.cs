using System.Runtime.CompilerServices;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Messaging;
using Titan.ECS.Messaging.Events;
using Titan.ECS.Registry;

namespace Titan.ECS.World
{
    internal sealed class World : IWorld
    {
        public uint Id { get; }
        public uint MaxEntities { get; }
        public void Update()
        {
            // TODO: this should not be in the World class, every service/manager should be added to a game loop where they can be executed async if needed.
            _eventManager.Update();
            _entityManager.Update();
            _entityFactory.Update();
            _filterManager.Update();
        }

        private readonly ComponentRegistry _registry;
        private readonly IEntityInfoRepository _entityInfo;
        private readonly IEntityFilterManager _filterManager;
        private readonly IEntityFactory _entityFactory;
        private readonly IEventManager _eventManager;
        private readonly IEntityManager _entityManager;

        public World(WorldConfiguration configuration, ComponentRegistry registry, IEntityManager entityManager, IEntityFactory entityFactory, IEntityInfoRepository entityInfoRepository, IEntityFilterManager entityFilterManager, IEventManager eventManager)
        {
            Id = configuration.WorldId;
            MaxEntities = configuration.MaxEntities;
            _registry = registry;
            _entityManager = entityManager;
            _entityFactory = entityFactory;
            _eventManager = eventManager;
            _filterManager = entityFilterManager;
            
            _entityInfo = entityInfoRepository;

            WorldContainer.Add(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity() => _entityManager.Create();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Attach(in Entity parent, in Entity child) => _entityManager.Attach(parent, child);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Detach(in Entity child) => _entityManager.Detach(child);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyEntity(in Entity entity) => _entityManager.Destroy(entity);

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
        private void ComponentAdded<T>(in Entity entity) where T : struct
        {
            ref var info = ref _entityInfo[entity];
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
            ref var info = ref _entityInfo[entity];
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
