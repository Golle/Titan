using System.Runtime.CompilerServices;
using Titan.ECS.Entities;
using Titan.ECS.Registry;

namespace Titan.ECS.World
{
    internal class World : IWorld
    {
        private readonly ComponentRegistry _registry;
        private readonly IEntityManager _entityManager;
        private readonly IEntityRelationship _relationship;

        public uint Id { get; }
        public IEntityInfoRepository EntityInfo { get; }

        public World(ECSConfiguration configuration, ComponentRegistry registry, IEntityManager entityManager, IEntityInfoRepository entityInfoRepository, IEntityRelationship entityRelationship)
        {
            Id = configuration.WorldId;
            _registry = registry;
            _entityManager = entityManager;
            _relationship = entityRelationship;
            
            EntityInfo = entityInfoRepository;
            
            WorldContainer.Add(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity() => _entityManager.Create();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Attach(in Entity parent, in Entity child) => _relationship.Attach(parent, child);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Detach(in Entity child) => _relationship.Detach(child);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyEntity(in Entity entity) => _entityManager.Destroy(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in Entity entity) where T : unmanaged => _registry.GetPool<T>().Create(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in Entity entity, in T value) where T : unmanaged => _registry.GetPool<T>().Create(entity, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent<T>(in Entity entity) where T : unmanaged => _registry.GetPool<T>().Destroy(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => WorldContainer.Remove(this);
    }
}
