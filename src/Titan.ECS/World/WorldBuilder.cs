using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Logging;
using Titan.ECS.Assets;
using Titan.ECS.Entities;
using Titan.ECS.Messaging;
using Titan.ECS.Registry;
using Titan.IOC;

namespace Titan.ECS.World
{
    public record WorldConfiguration(uint MaxEntities, uint WorldId, uint MaxEvents);

    public class WorldBuilder
    {
        private static uint _worldCounter = 0;// TODO: move this to some other class

        private uint _maxEntities = 1000;
        private uint _maxEvents = 10_000;
        private readonly List<(Type componentType, uint maxComponents, bool isManaged)> _components = new ();
        private readonly List<Type> _assetLoaders = new();

        public WorldBuilder WithMaxEntities(uint maxEntities)
        {
            _maxEntities = maxEntities;
            return this;
        }
    
        public WorldBuilder WithComponent<T>(uint maxComponents = 0u) where T : struct
        {
            _components.Add((typeof(T), maxComponents, RuntimeHelpers.IsReferenceOrContainsReferences<T>()));
            return this;
        }

        public WorldBuilder WithAssetsLoader<T>() where T : IAssetLoader
        {
            _assetLoaders.Add(typeof(T));
            return this;
        }

        public WorldBuilder WithMaxEvents(uint maxEvents)
        {
            _maxEvents = Math.Clamp(maxEvents, 1000, 1_000_000); // Not less than 1000 and not more than 1 000 000.
            return this;
        }
        public IWorld Build(IContainer baseContainer)
        {
            var container = baseContainer.CreateChildContainer()
                .Register<ComponentRegistry>()
                .Register<SystemsRegistry>()
                .Register<IEntityFactory, EntityFactory>()
                .Register<IEntityInfoRepository, EntityInfoRepository>(dispose: true)
                .Register<IEntityManager, EntityManager>(dispose: true)
                .Register<IEntityFilterManager, EntityFilterManager>(dispose: true)
                .Register<IEventManager, EventManager>(dispose: true)
                .RegisterSingleton(new WorldConfiguration(_maxEntities, Interlocked.Increment(ref _worldCounter), _maxEvents));
            
            try
            {
                LOGGER.Debug("Creating world");
                LOGGER.Debug("Max entities: {0}", _maxEntities);
                LOGGER.Debug("Max events: {0}", _maxEvents);
                LOGGER.Debug("Number of Components: {0}", _components.Count);
                LOGGER.Debug("Number of asset loaders: {0}", _assetLoaders.Count);

                var componentRegistry = container.GetInstance<ComponentRegistry>();
                foreach (var (componentType, maxComponents, isManaged) in _components)
                {
                    componentRegistry.Register(componentType, maxComponents, isManaged);
                }

                //TODO: Add something that keeps track of the container instance so it can be disposed later
                // Create the world
                var world = container.CreateInstance<World>();
                container.RegisterSingleton<IWorld>(world);

                return world;
            }
            catch(Exception)
            {
                // if any error occurs dispose the container
                container.Dispose();
                throw;
            }
        }
    }
}
