using System;
using Titan.ECS.Entities;

namespace Titan.ECS.Components
{
    public interface IEntityFilter
    {
        ref readonly ComponentId Components { get; }
        ReadOnlySpan<Entity> GetEntities();
    }
}
