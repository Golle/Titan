using System;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.World
{
    public interface IEntityFilterManager
    {
        IEntityFilter Create(EntityFilterConfiguration configuration);
        void EntityChanged(in Entity entity, in ComponentMask components);
        void Update();
    }
}
