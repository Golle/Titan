using System;
using Titan.ECS.Entities;

namespace Titan.ECS.Components
{
    public interface IComponentPool : IDisposable
    {
        void Destroy(in Entity entity);
        bool Contains(in Entity entity);
        void Update();
    }

    public interface IComponentPool<T> : IComponentPool where T : unmanaged
    {
        ref T Create(in Entity entity, in T initialValue);
        ref T Create(in Entity entity);
        ref T Get(in Entity entity);
        ref T this[in Entity entity] { get; }
    }
}
