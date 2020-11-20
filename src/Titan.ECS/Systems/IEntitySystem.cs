using System;

namespace Titan.ECS.Systems
{
    public interface IEntitySystem : IDisposable
    {
        void OnPreUpdate();
        void OnUpdate(in TimeStep timeStep);
        void OnPostUpdate();
    }
}
