using System;
using Titan.ECS.Entities;

namespace Titan.ECS.Registry
{
    public interface IComponentPool : IDisposable
    {
        void Destroy(in Entity entity);
    }
}
