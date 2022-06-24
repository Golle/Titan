using System;
using System.Collections.Generic;
using Titan.ECS.Components;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;

namespace Titan.ECS;

public class WorldBuilderOld
{
    private uint _maxEntities;

    private readonly List<ComponentConfiguration> _components = new();
    private readonly List<EntitySystemConfiguration> _systems = new();
    private float _fixedTimestep = 1f/60f; // Default 60FPS
    private Action<World> _setup = _ => {};
    private Action<World> _teardown = _ => { };

    public WorldBuilderOld(uint defaultMaxEntities) => _maxEntities = defaultMaxEntities;
      
    public WorldBuilderOld MaxEntities(uint maxEntities)
    {
        _maxEntities = maxEntities;
        return this;
    }

    public WorldBuilderOld WithFixedtimestep(float fixedUpdateTime)
    {
        _fixedTimestep = fixedUpdateTime;
        return this;
    }

    public WorldBuilderOld WithComponent<T>(ComponentPoolTypes type = ComponentPoolTypes.Packed, uint count = 0) where T : unmanaged
    {
        _components.Add(new ComponentConfiguration(typeof(T), type, count == 0 ? _maxEntities : count));
        return this;
    }
        
    public WorldBuilderOld WithSystem<T>() where T : EntitySystem, new()
    {
        _systems.Add(new(typeof(T)));
        return this;
    }

    public WorldBuilderOld WithSetup(Action<World> setup)
    {
        _setup = setup;
        return this;
    }

    public WorldBuilderOld WithTeardown(Action<World> teardown)
    {
        _teardown = teardown;
        return this;
    }

    public WorldConfigurationOld Build(string name) => new()
    {
        Name = name,
        MaxEntities = _maxEntities,
        Components = _components.ToArray(),
        Systems = _systems.ToArray(),
        FixedTimeStep = _fixedTimestep,
        Setup = _setup,
        Teardown = _teardown
    };
}
