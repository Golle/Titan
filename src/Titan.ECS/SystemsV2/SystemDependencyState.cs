using Titan.ECS.Components;

namespace Titan.ECS.SystemsV2;


internal enum DependencyType
{
    None,
    OneWay,
    TwoWay
}
internal struct SystemDependencyState
{
    public ComponentId MutableComponents;
    public ComponentId ReadOnlyComponents;

    public SystemDependency ReadOnlyResources;
    public SystemDependency MutableResources;
    
    public SystemDependency RunAfter;

    public readonly DependencyType DependsOn(in SystemDependencyState state)
    {
        // If the system has read only access to components that the input system has Mutable access to
        if (ReadOnlyComponents.MatchesAny(state.MutableComponents))
        {
            return DependencyType.OneWay;
        }

        // Both systems have mutable access to the same resource
        if (MutableComponents.MatchesAny(state.MutableComponents))
        {
            return DependencyType.TwoWay;
        }

        // Current system has read only access to a resource that the other system mutates
        if (ReadOnlyResources.ContainsAny(state.MutableResources))
        {
            return DependencyType.OneWay;
        }

        // Both systems mutate the same system
        if (MutableResources.ContainsAny(state.MutableResources))
        {
            return DependencyType.TwoWay;
        }
        
        // No dependencies
        return DependencyType.None;
    }
}
