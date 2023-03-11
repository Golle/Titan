namespace Titan.Events;

internal interface IEventsManager
{
    EventsWriter<T> CreateWriter<T>() where T : unmanaged, IEvent;
    EventsReader<T> CreateReader<T>() where T : unmanaged, IEvent;
    void Send<T>(in T @event) where T : unmanaged, IEvent;
}
