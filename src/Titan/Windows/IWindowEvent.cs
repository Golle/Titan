using Titan.Events;

namespace Titan.Windows;

public interface IWindowEvent : IEvent
{
    static abstract uint Id { get; }
}
