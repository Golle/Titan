using Titan.Core.Messaging;

namespace Titan.Windows.Events
{
    [TitanEvent]
    public readonly struct MouseButtonUpEvent
    {
        public readonly MouseButton Button;
        public MouseButtonUpEvent(MouseButton button)
        {
            Button = button;
        }
    }
}
