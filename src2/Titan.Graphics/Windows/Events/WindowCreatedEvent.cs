using Titan.Core.Messaging;

namespace Titan.Graphics.Windows.Events
{
    internal readonly struct WindowCreatedEvent
    {
        public static readonly short Id = EventId<WindowCreatedEvent>.Value;
    }
}
