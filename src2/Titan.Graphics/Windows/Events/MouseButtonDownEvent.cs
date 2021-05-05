using Titan.Core.Messaging;

namespace Titan.Graphics.Windows.Events
{
    public readonly struct MouseButtonEvent
    {
        public static readonly short Id = EventId<MouseButtonEvent>.Value;
        public readonly MouseButton Button;
        public readonly bool Down;
        public MouseButtonEvent(MouseButton button, bool down)
        {
            Down = down;
            Button = button;
        }
    }
}
