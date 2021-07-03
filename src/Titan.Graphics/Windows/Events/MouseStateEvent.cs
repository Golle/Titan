using Titan.Core.Messaging;

namespace Titan.Graphics.Windows.Events
{
    public readonly struct MouseStateEvent
    {
        public static readonly short Id = EventId<MouseStateEvent>.Value;

        public readonly bool Visible;
        public MouseStateEvent(bool visible)
        {
            Visible = visible;
        }
    }
}
