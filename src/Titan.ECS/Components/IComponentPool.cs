using System;
using Titan.ECS.Entities;

namespace Titan.ECS.Components
{
    public interface IComponentPool : IDisposable
    {
        void Destroy(in Entity entity);
        void Destroy(uint entityId);
        bool Contains(in Entity entity);
        internal void OnEntityDestroyed(uint entityId);
    }
    public interface IComponentPool<T> : IComponentPool where T : unmanaged
    {
        ref T Create(in Entity entity, in T initialValue);
        ref T Create(in Entity entity);
        ref T Get(in Entity entity);
        ref T this[in Entity entity] { get; }
    }
}
