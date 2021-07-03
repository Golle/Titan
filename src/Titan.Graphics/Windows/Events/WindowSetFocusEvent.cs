using Titan.Core.Messaging;

namespace Titan.Graphics.Windows.Events
{
    public readonly struct WindowSetFocusEvent
    {
        public static readonly short Id = EventId<WindowSetFocusEvent>.Value;
    }
}
