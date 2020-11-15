using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.ECS.Entities;

namespace Titan.ECS.World
{
    internal class World : IDisposable
    {
        private static uint _worldCounter = 0;
        public uint Id { get; } = Interlocked.Increment(ref _worldCounter);

        private readonly IEntityManager _entityManager;
        public World()
        {
            _entityManager = new EntityManager(Id);
            WorldContainer.Add(this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity() => _entityManager.Create();


        public void Dispose()
        {
            WorldContainer.Remove(this);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyEntity(in Entity entity) => _entityManager.Destroy(entity);
    }
}
