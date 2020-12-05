using System;
using Titan.ECS.Entities;

namespace Titan.ECS.Components
{
    public interface IEntityFilter
    {
        ref readonly ComponentMask ComponentMask { get; }
        ReadOnlySpan<Entity> GetEntities();
    }
}
