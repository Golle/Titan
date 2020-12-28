using System;
using Titan.ECS.Entities;

namespace Titan.ECS.Registry
{
    public interface IComponentPool<T> : IComponentPool where T : unmanaged
    {
        ref T Create(in Entity entity, in T initialValue);
        ref T Create(in Entity entity);
        ref T this[in Entity entity] { get; }
        bool Contains(in Entity entity);
    }

    public interface IManagedComponentPool<T> : IComponentPool where T : struct
    {
        ref T Create(in Entity entity, in T initialValue);
        ref T Create(in Entity entity);
        ref T this[in Entity entity] { get; }
        bool Contains(in Entity entity);
    }
    
    public interface IComponentPool : IDisposable
    {
        void Destroy(in Entity entity);
    }
}