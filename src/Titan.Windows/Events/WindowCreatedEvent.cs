using Titan.Core.Messaging;

namespace Titan.Windows.Events
{
    [TitanEvent]
    internal readonly struct WindowCreatedEvent
    {
        public static readonly short Id = EventId<WindowCreatedEvent>.Value;
    }
}
