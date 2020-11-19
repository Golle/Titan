using System;
using Titan.ECS.Entities;

namespace Titan.ECS.World
{
    public interface IWorld : IDisposable
    {
        Entity CreateEntity();
    }
}
