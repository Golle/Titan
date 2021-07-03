using Titan.Core.Messaging;

namespace Titan.Graphics.Windows.Events
{
    public readonly struct WindowLostFocusEvent
    {
        public static readonly short Id = EventId<WindowLostFocusEvent>.Value;
    }
}
