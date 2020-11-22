using System.Threading;
using Titan.ECS.Entities;
using Titan.ECS.Registry;
using Titan.IOC;

namespace Titan.ECS.World
{
    public record EventsConfiguration(uint MaxEventQueueSize);
    public record WorldConfiguration(uint MaxEntities, uint WorldId, EventsConfiguration EventsConfiguration);

    public class WorldBuilder
    {
        private static uint _worldCounter = 0;// TODO: move this to some other class

        private readonly IContainer _container;
        private readonly ComponentRegistry _registry;

        public WorldBuilder(IContainer baseContainer, uint maxEntities = 100_000u)
        {
            // TODO: container must be disposed (and all classes inside it must be disposed)

            _container = baseContainer.CreateChildContainer()
                .Register<ComponentRegistry>()
                .Register<IEntityManager, EntityManager>()
                .Register<IEntityInfoRepository, EntityInfoRepository>(dispose: true)
                .Register<IEntityRelationship, EntityRelationship>(dispose: true)
                .Register<IEntityFilterManager, EntityFilterManager>(dispose: true)
                .RegisterSingleton(new WorldConfiguration(maxEntities, Interlocked.Increment(ref _worldCounter), new EventsConfiguration(10_000)));

            _registry = _container.GetInstance<ComponentRegistry>();
        }


        public WorldBuilder WithComponent<T>(uint maxComponents = 0u) where T : unmanaged
        {
            _registry.Register<T>(maxComponents);
            return this;
        }

        public IWorld Build()
        {

            return _container.CreateInstance<World>();
        }
    }
}
