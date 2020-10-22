using System;

namespace Titan.Core.Messaging
{
    public interface IEventQueue
    {
        void Push<T>(in T @event) where T : struct;
        ReadOnlySpan<T> GetEvents<T>() where T : struct;
        void Initialize(IEventTypeProvider eventTypeProvider);
        void Update();
    }
}
