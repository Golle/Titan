using Titan.Core.Messaging;

namespace Titan.Windows.Events
{
    [TitanEvent]
    public readonly struct MouseButtonEvent
    {
        public readonly MouseButton Button;
        public readonly bool Down;
        public MouseButtonEvent(MouseButton button, bool down)
        {
            Down = down;
            Button = button;
        }
    }
}