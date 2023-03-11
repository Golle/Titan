namespace Titan.ECS.Components;

/// <summary>
/// Marker interface for shared resources that can be used within systems through GetMutableResource/GetReadOnlyResource.
/// </summary>
public interface IResource
{
}

/// <summary>
/// Marker interface for Components that can be used within systems through GetMutableStorage/GetReadOnlyStorage.
/// </summary>
public interface IComponent
{
}
