using System;

namespace Titan.Core.Messaging
{
    public interface IEventQueue : IDisposable
    {
        void Initialize(uint maxEventQueueSize);
        ReadOnlySpan<QueuedEvent> GetEvents();
        void Update();
        void Push<T>(in T @event) where T : unmanaged;
    }
}
