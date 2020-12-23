using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Logging;
using Titan.ECS.Entities;
using Titan.ECS.Registry;
using Titan.IOC;

namespace Titan.ECS.World
{
    public record WorldConfiguration(uint MaxEntities, uint WorldId);

    public class WorldBuilder
    {
        private static uint _worldCounter = 0;// TODO: move this to some other class

        private uint _maxEntities = 1000;
        private readonly List<(Type componentType, uint maxComponents, bool isManaged)> _components = new ();

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
        
        public IWorld Build(IContainer baseContainer)
        {
            var container = baseContainer.CreateChildContainer()
                .Register<ComponentRegistry>()
                .Register<SystemsRegistry>()
                .Register<IEntityFactory, EntityFactory>()
                .Register<IEntityInfoRepository, EntityInfoRepository>(dispose: true)
                .Register<IEntityManager, EntityManager>(dispose: true)
                .Register<IEntityFilterManager, EntityFilterManager>(dispose: true)
                .RegisterSingleton(new WorldConfiguration(_maxEntities, Interlocked.Increment(ref _worldCounter)));
            
            try
            {
                LOGGER.Debug("Creating world");
                LOGGER.Debug("Max entities: {0}", _maxEntities);
                LOGGER.Debug("Number of Components: {0}", _components.Count);

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
