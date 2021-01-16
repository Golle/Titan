using System;

namespace Titan.ECS.Entities
{
    public interface IEntityManager : IDisposable
    {
        Entity Create();
        void Attach(in Entity parent, in Entity entity);
        void Detach(in Entity entity);
        bool HasParent(in Entity entity);
        Entity GetParent(in Entity entity);
        void Destroy(in Entity entity);
        void Update();
        bool TryGetParent(in Entity entity, out Entity parent);
    }
}
