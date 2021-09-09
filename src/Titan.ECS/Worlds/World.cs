using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Systems;
using Titan.ECS.Systems.Dispatcher;


namespace Titan.ECS.Worlds
{
    public record WorldConfiguration
    {
        public uint Id { get; init; }
        public float FixedTimeStep { get; init; } = 1f / 60f;
        public uint MaxEntities { get; init; }
        public ComponentConfiguration[] Components { get; init; }
        public EntitySystem[] Systems { get; init; } 
    }

    public record ComponentConfiguration(Type Type, ComponentPoolTypes PoolType, uint Count = 0);

    public class World : IDisposable
    {
        internal uint Id => Config.Id;
        internal GameTime GameTime { get; }
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
            FilterManager = new(Config, InfoManager);
            GameTime = new(Config);
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
        public void AddComponent<T>(in Entity entity) where T : unmanaged => Registry.GetPool<T>().Create(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in Entity entity, in T value) where T : unmanaged => Registry.GetPool<T>().Create(entity, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent<T>(in Entity entity) where T : unmanaged
        {
            // TODO: should this be done in 2 frames? flag for deletion, and delete in next frame?
            Registry.GetPool<T>().Destroy(entity);
        }

        public void Update()
        {
            InfoManager.Update();
            GameTime.Update();
            Manager.Update();
            var s = Stopwatch.StartNew();
            Registry.Update();
            s.Stop();
            EngineStats.SetStats("Registry", s.Elapsed.TotalMilliseconds);
            // Filter should be executed last (before dispatcher)
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
