using Titan.Core.Messaging;

namespace Titan.Windows.Events
{
    [TitanEvent]
    public readonly struct WindowLostFocusEvent
    {
        public static readonly short Id = EventId<WindowLostFocusEvent>.Value;
    }
}
