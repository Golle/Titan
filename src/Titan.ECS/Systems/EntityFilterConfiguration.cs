using Titan.ECS.Components;

namespace Titan.ECS.Systems;

public class EntityFilterConfiguration
{
    public ref readonly ComponentId Components => ref _includeComponents;
    public ref readonly ComponentId ExcludeComponents => ref _excludeComponents;

    private ComponentId _includeComponents;
    private ComponentId _excludeComponents;
    public EntityFilterConfiguration With<T>() where T : unmanaged
    {
        _includeComponents += ComponentId<T>.Id;
        return this;
    }

    public EntityFilterConfiguration Not<T>() where T : unmanaged
    {
        _excludeComponents += ComponentId<T>.Id;
        return this;
    }
}
