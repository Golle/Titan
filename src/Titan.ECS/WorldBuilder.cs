using System;
using System.Collections.Generic;
using System.Linq;
using Titan.ECS.Components;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;

namespace Titan.ECS
{
    public class WorldBuilder
    {
        private uint _maxEntities;

        private readonly List<ComponentConfiguration> _components = new();
        private readonly List<EntitySystemConfiguration> _systems = new();
        private float _fixedTimestep = 1f/60f; // Default 60FPS

        public WorldBuilder(uint defaultMaxEntities) => _maxEntities = defaultMaxEntities;
      
        public WorldBuilder MaxEntities(uint maxEntities)
        {
            _maxEntities = maxEntities;
            return this;
        }

        public WorldBuilder WithFixedtimestep(float fixedUpdateTime)
        {
            _fixedTimestep = fixedUpdateTime;
            return this;
        }

        public WorldBuilder WithComponent<T>(ComponentPoolTypes type = ComponentPoolTypes.Packed, uint count = 0) where T : unmanaged
        {
            _components.Add(new ComponentConfiguration(typeof(T), type, count == 0 ? _maxEntities : count));
            return this;
        }
        
        public WorldBuilder WithSystem<T>() where T : EntitySystem, new()
        {
            _systems.Add(new(typeof(T)));
            return this;
        }

        public WorldConfiguration Build() => new()
        {
            MaxEntities = _maxEntities,
            Components = _components.ToArray(),
            Systems = _systems.ToArray(),
            FixedTimeStep = _fixedTimestep
        };
    }
}
