using System.Collections.Generic;
using Titan.Core.App;
using Titan.Core.Logging;
using Titan.ECS.Components;
using Titan.ECS.SystemsV2.Components;

namespace Titan.ECS.SystemsV2;

public class WorldConfig
{
    private const uint DefaultMaxResourceTypes = 100;
    private const uint DefaultMaxEntities = 10_000;

    private readonly List<SystemDescriptor> _systems = new();
    private readonly List<ComponentDescriptor> _components = new();
    internal IReadOnlyList<SystemDescriptor> SystemDescriptors => _systems;
    internal IReadOnlyList<ComponentDescriptor> ComponentDescriptors => _components;
    internal uint MaxEntities { get; private set; } = DefaultMaxEntities;
    internal uint MaxResourceTypes { get; private set; } = DefaultMaxResourceTypes;

    public WorldConfig WithMaxEntities(uint maxEntites)
    {
        MaxEntities = maxEntites;
        return this;
    }

    public WorldConfig WithMaxResourcesTypes(uint maxResourceTypes)
    {
        MaxResourceTypes = maxResourceTypes;
        return this;
    }

    public WorldConfig AddSystem<T>() where T : unmanaged, IStructSystem<T>
    {
        _systems.Add(SystemDescriptor.Create<T>());
        return this;
    }

    public WorldConfig AddSystemToStage<T>(Stage stage) where T : unmanaged, IStructSystem<T>
    {
        _systems.Add(SystemDescriptor.Create<T>(stage));
        return this;
    }

    public WorldConfig AddComponent<T>(uint maxComponents = 0u, ComponentPoolTypes type = ComponentPoolTypes.Packed) where T : unmanaged
    {
        _components.Add(ComponentDescriptor.Create<T>(type, maxComponents));
        AddSystemToStage<ComponentSystem<T>>(Stage.PreUpdate);
        return this;
    }

    public WorldConfig AddStartupSystem<T>()
    {
        Logger.Warning<WorldConfig>("Startup systems has not been implemented yet.");
        return this;
    }
}
