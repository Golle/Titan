using System;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Events;
using Titan.ECS.Systems;
using Titan.ECS.Systems.Dispatcher;

namespace Titan.ECS.Worlds
{
    public record WorldConfiguration(uint MaxEntities, ComponentConfiguration[] Components, EntitySystem[] Systems)
    {
        public uint Id { get; init; }
    }

    public record ComponentConfiguration(Type Type, ComponentPoolTypes PoolType, uint Count = 0);

    public class World : IDisposable
    {
        internal uint Id => Config.Id;
        internal EntityManager Manager { get; }
        internal EntityInfoManager InfoManager { get; }
        internal ComponentRegistry Registry { get; }
        internal WorldConfiguration Config { get; }
        internal EntityFilterManager FilterManager { get; }

        private static readonly IdContainer WorldIds = new(100);
        private static readonly World[] Worlds = new World[100];
        
        private readonly SystemsDispatcher _dispatcher;

        public World(WorldConfiguration config)
        {
            Config = config with {Id = WorldIds.Next() };
            Logger.Trace<World>($"Creating world {Id}");
            Manager = new(Config);
            Registry = new (Config);
            InfoManager = new(Config);
            FilterManager = new(Config);
            Worlds[Id] = this;

            foreach (var system in config.Systems)
            {
                system.InitSystem(this);
            }
            _dispatcher = new SystemsDispatcher(SystemNodeFactory.Create(config.Systems));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AttachEntity(in Entity parent, in Entity entity) => Manager.Attach(parent, entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DetachEntity(in Entity entity) => Manager.Detach(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyEntity(in Entity entity) => Manager.Destroy(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity() => Manager.Create();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in Entity entity) where T : unmanaged
        {
            Registry.GetPool<T>().Create(entity);
            ComponentAdded<T>(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in Entity entity, in T value) where T : unmanaged
        {
            Registry.GetPool<T>().Create(entity, value);
            ComponentAdded<T>(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ComponentAdded<T>(in Entity entity) where T : unmanaged
        {
            ref var info = ref InfoManager.Get(entity);
            var componentId = ComponentId<T>.Id;
            info.Components += componentId;
            EventManager.Push(new ComponentAddedEvent(entity, componentId));
            EventManager.Push(new EntityChangedEvent(entity, info.Components));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent<T>(in Entity entity) where T : unmanaged
        {
            // TODO: should this be done in 2 frames? flag for deletion, and delete in next frame?
            Registry.GetPool<T>().Destroy(entity);
            ref var info = ref InfoManager.Get(entity);
            var componentId = ComponentId<T>.Id;
            info.Components -= componentId;
            EventManager.Push(new ComponentRemovedEvent(entity, componentId));
            EventManager.Push(new EntityChangedEvent(entity, info.Components));
        }

        public void Update()
        {
            Manager.Update();
            Registry.Update();
            FilterManager.Update();
            
            _dispatcher.Execute();
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
            Logger.Trace<World>($"Disposing world {Id}");
            Manager.Dispose();
            InfoManager.Dispose();
            FilterManager.Dispose();
            Worlds[Id] = null;
            WorldIds.Return(Id);
        }
    }
}
