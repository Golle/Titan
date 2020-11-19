using System;
using Titan.ECS.Entities;

namespace Titan.ECS.World
{
    public interface IEntityRelationship : IDisposable
    {
        void Attach(in Entity parent, in Entity child);
        void Detach(in Entity child);
        bool HasParent(in Entity entity);
        Entity GetParent(in Entity entity);
    }
}
