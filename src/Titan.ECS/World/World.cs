using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.ECS.Entities;
using Titan.ECS.Registry;

namespace Titan.ECS.World
{
    public class World : IDisposable
    {
        private readonly ComponentRegistry _registry;

        private static uint _worldCounter = 0;
        public uint Id { get; } = Interlocked.Increment(ref _worldCounter);
        public EntityRelationship Relationship { get; }

        private readonly IEntityManager _entityManager;
        public World(ComponentRegistry registry)
        {
            _registry = registry;
            _entityManager = new EntityManager(Id);
            Relationship = new EntityRelationship(Id);

            WorldContainer.Add(this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity() => _entityManager.Create();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Attach(in Entity parent, in Entity child) => Relationship.Attach(parent, child);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Detach(in Entity child) => Relationship.Detach(child);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => WorldContainer.Remove(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyEntity(in Entity entity) => _entityManager.Destroy(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in Entity entity) where T : unmanaged => _registry.GetPool<T>().Create(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in Entity entity, in T value) where T : unmanaged => _registry.GetPool<T>().Create(entity, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent<T>(in Entity entity) where T : unmanaged => _registry.GetPool<T>().Destroy(entity);
    }
}
