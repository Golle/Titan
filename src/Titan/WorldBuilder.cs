using System.Collections.Generic;
using Titan.ECS.Components;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;

namespace Titan
{
    public class WorldBuilder
    {
        private uint _maxEntities;

        private readonly List<ComponentConfiguration> _components = new();
        private readonly List<EntitySystem> _systems = new();
        internal WorldBuilder(uint defaultMaxEntities) => _maxEntities = defaultMaxEntities;
        public WorldBuilder MaxEntities(uint maxEntities)
        {
            _maxEntities = maxEntities;
            return this;
        }
        public WorldBuilder WithComponent<T>(ComponentPoolTypes type = ComponentPoolTypes.Packed, uint count = 0) where T : unmanaged
        {
            _components.Add(new ComponentConfiguration(typeof(T), type, count == 0 ? _maxEntities : count));
            return this;
        }
        public WorldBuilder WithSystem(EntitySystem system)
        {
            _systems.Add(system);
            return this;
        }
        internal WorldConfiguration Build() => new(_maxEntities, _components.ToArray(), _systems.ToArray());
    }
}
