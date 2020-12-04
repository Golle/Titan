using System;
using Titan.ECS.Entities;
using Titan.ECS.Registry;

namespace Titan.ECS.World
{
    public interface IWorld : IDisposable
    {
        Entity CreateEntity();
        IComponentPool<T> GetComponentPool<T>() where T : unmanaged;
        void Update();
        IEntityManager EntityManager { get; }
        IEntityFilterManager FilterManager { get; }
    }
}
