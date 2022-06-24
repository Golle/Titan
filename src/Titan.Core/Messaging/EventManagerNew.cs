using System;

namespace Titan.Core.Messaging;

public interface IEventManager
{
    void Push<T>() where T : unmanaged;
    ReadOnlySpan<Event> GetEvents();
}

internal class EventManagerNew : IEventManager
{
    public void Push<T>() where T : unmanaged
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<Event> GetEvents()
    {
        throw new NotImplementedException();
    }
}
