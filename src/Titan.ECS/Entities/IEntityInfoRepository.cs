using System;

namespace Titan.ECS.Entities
{
    public interface IEntityInfoRepository : IDisposable
    {
        public ref readonly EntityInfo Get(in Entity entity);
        public ref readonly EntityInfo this[in Entity entity] { get; }
    }
}
