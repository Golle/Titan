using Titan.Core.Messaging;

namespace Titan.Windows.Events
{
    [TitanEvent]
    internal readonly struct WindowClosedEvent
    {
        public static readonly short Id = EventId<WindowClosedEvent>.Value;
    }
}
