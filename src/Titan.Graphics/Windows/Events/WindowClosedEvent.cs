using Titan.Core.Messaging;

namespace Titan.Graphics.Windows.Events
{
    internal readonly struct WindowClosedEvent
    {
        public static readonly short Id = EventId<WindowClosedEvent>.Value;
    }
}
