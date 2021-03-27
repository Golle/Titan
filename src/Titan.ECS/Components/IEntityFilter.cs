using System;
using Titan.ECS.Entities;

namespace Titan.ECS.Components
{
    public interface IEntityFilter
    {
        ref readonly ComponentId Components { get; }
        ref readonly ComponentId Exclude { get; }
        ReadOnlySpan<Entity> GetEntities();
    }
}
