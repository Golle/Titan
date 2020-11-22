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


            //// Test
            ///
            using var queue = new EventQueue(configuration);

            queue.Push(new EntityCreatedEvent(new Entity(10, 23)));
            queue.Push(new EntityCreatedEvent(new Entity(11, 23)));
            queue.Push(new EntityCreatedEvent(new Entity(12, 23)));
            queue.Push(new EntityCreatedEvent(new Entity(13, 23)));
            queue.Push(new EntityCreatedEvent(new Entity(14, 23)));
            queue.Update();
            var value1 = EventId<EntityCreatedEvent>.Value;
            var value2 = EventId<ComponentAddedEvent>.Value;

            var s = Stopwatch.StartNew();
            for (var i = 0; i < 9900; ++i)
            {
                queue.Push(new ComponentAddedEvent(new Entity(10, 23), new ComponentId(10, 27)));
            }
            queue.Update();
            for (var i = 0; i < 9900; ++i)
            {
                queue.Push(new ComponentAddedEvent(new Entity(10, 23), new ComponentId(10, 27)));
            }
            queue.Update();
            for (var i = 0; i < 9900; ++i)
            {
                queue.Push(new ComponentAddedEvent(new Entity(10, 23), new ComponentId(10, 27)));
            }
            queue.Update();
            for (var i = 0; i < 9900; ++i)
            {
                queue.Push(new ComponentAddedEvent(new Entity(10, 23), new ComponentId(10, 27)));
            }
            queue.Update();
            for (var i = 0; i < 9900; ++i)
            {
                queue.Push(new ComponentAddedEvent(new Entity(10, 23), new ComponentId(10, 27)));
            }
            
            queue.Update();
            
            foreach (ref readonly var @event in queue.GetEvents())
            {
                if (@event.Type == value1)
                {
                    ref readonly var e = ref @event.As<EntityCreatedEvent>();
                    //Console.WriteLine($"ID: {e.Entity.Id} : {e.Entity.WorldId}");
                }
                if (@event.Type == value2)
                {
                    ref readonly var e = ref @event.As<ComponentAddedEvent>();
                    //Console.WriteLine($"ID: {e.Entity.Id} : {e.Entity.WorldId} Component: {e.Component}");
                }
            }
            s.Stop();
            Console.WriteLine("Elapsed: {0}", s.Elapsed.TotalMilliseconds);

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
