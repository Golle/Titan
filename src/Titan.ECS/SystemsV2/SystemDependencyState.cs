using Titan.ECS.Components;

namespace Titan.ECS.SystemsV2;

internal struct SystemDependencyState
{
    public ComponentId MutableComponents;
    public ComponentId ReadOnlyComponents;

    public SystemDependency ReadOnlyResources;
    public SystemDependency MutableResources;
    public SystemDependency ReadOnlyGlobalResources;
    public SystemDependency MutableGlobalResources;

    public bool DependsOn(in SystemDependencyState state)
    {
        return false;
        // do the stuff
        return true;
    }
}
