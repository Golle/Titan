using System;
using Titan.ECS.Entities;
using Titan.ECS.Registry;

namespace Titan.ECS.World
{
    public interface IWorld : IDisposable
    {
        Entity CreateEntity(); // TODO: replace with builder, so that entities can be added lazily and in a prefab
        IComponentPool<T> GetComponentPool<T>() where T : unmanaged;
        IManagedComponentPool<T> GetManagedComponentPool<T>() where T : struct;
        IEntityManager EntityManager { get; }
        IEntityFilterManager FilterManager { get; }
        void Update();
    }
}
