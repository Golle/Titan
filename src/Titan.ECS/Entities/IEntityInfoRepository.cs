using System;

namespace Titan.ECS.Entities
{
    public interface IEntityInfoRepository : IDisposable
    {
        public ref EntityInfo Get(in Entity entity);
        public ref EntityInfo this[in Entity entity] { get; }
    }
}
