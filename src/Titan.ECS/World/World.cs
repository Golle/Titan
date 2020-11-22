using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        public IEntityRelationship Relationship { get; }
        public IEntityInfoRepository EntityInfo { get; }
        public IEntityFilterManager FilterManager { get; }

        private readonly ComponentRegistry _registry;
        private readonly IEntityManager _entityManager;

        public World(WorldConfiguration configuration, ComponentRegistry registry, IEntityManager entityManager, IEntityInfoRepository entityInfoRepository, IEntityRelationship entityRelationship, IEntityFilterManager entityFilterManager)
        {
            Id = configuration.WorldId;
            MaxEntities = configuration.MaxEntities;
            _registry = registry;
            _entityManager = entityManager;
            FilterManager = entityFilterManager;
            Relationship = entityRelationship;
            
            EntityInfo = entityInfoRepository;
            
            WorldContainer.Add(this);

        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity() => _entityManager.Create();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Attach(in Entity parent, in Entity child) => Relationship.Attach(parent, child);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Detach(in Entity child) => Relationship.Detach(child);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyEntity(in Entity entity)
        {
            // TODO: this should be delayed by a single update because of how filters works.
            _entityManager.Destroy(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in Entity entity) where T : unmanaged => AddComponent<T>(entity, default);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in Entity entity, in T value) where T : unmanaged
        {
            _registry.GetPool<T>().Create(entity, value);
            ref var info = ref EntityInfo[entity];
            info.ComponentMask += ComponentId<T>.Id;
            FilterManager.EntityChanged(entity, info.ComponentMask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent<T>(in Entity entity) where T : unmanaged
        {
            _registry.GetPool<T>().Destroy(entity);
            ref var info = ref EntityInfo[entity];
            info.ComponentMask -= ComponentId<T>.Id;
            FilterManager.EntityChanged(entity, info.ComponentMask);
        }

        public IComponentPool<T> GetComponentPool<T>() where T : unmanaged => _registry.GetPool<T>();

        public void Dispose() => WorldContainer.Remove(this);
    }
}
