using Titan.Core.Messaging;

namespace Titan.Windows.Events
{
    [TitanEvent]
    public readonly struct MouseButtonDownEvent
    {
        public readonly MouseButton Button;
        public MouseButtonDownEvent(MouseButton button)
        {
            Button = button;
        }
    }
}
