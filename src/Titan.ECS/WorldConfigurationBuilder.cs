using System.Collections.Generic;
using System.Linq;
using Titan.ECS.Components;
using Titan.ECS.TheNew;
using Titan.ECS.Worlds;

namespace Titan.ECS;

public sealed record WorldConfiguration(string Name, uint MaxEntities, ComponentConfiguration[] Components, EntitySystemConfiguration[] Systems);

public class WorldConfigurationBuilder
{
    private readonly uint _maxEntities;
    private readonly HashSet<EntitySystemConfiguration> _systems = new ();
    private readonly List<ComponentConfiguration> _components = new();
    public WorldConfigurationBuilder(uint maxEntities)
    {
        _maxEntities = maxEntities;
    }

    public WorldConfigurationBuilder WithSystem<T>() where T : BaseSystem
    {
        _systems.Add(new EntitySystemConfiguration(typeof(T)));
        return this;
    }

    public WorldConfigurationBuilder WithComponent<T>(ComponentPoolTypes type = ComponentPoolTypes.Packed, uint count = 0) where T : unmanaged
    {
        _components.Add(new ComponentConfiguration(typeof(T), type, count == 0 ? _maxEntities : count));
        return this;
    }

    public WorldConfiguration Build(string name) =>
        new(
            name,
            _maxEntities, 
            _components.ToArray(),
            _systems.ToArray());
}
