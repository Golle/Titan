using System;

namespace Titan.ECS.Events
{
    internal interface IEventQueue
    {
        ReadOnlySpan<QueuedEvent> GetEvents();
        void Update();
        void Push<T>(in T @event) where T : unmanaged;
    }
}
