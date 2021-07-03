using Titan.Core.Messaging;

namespace Titan.Graphics.Windows.Events
{
    public readonly struct MouseMovedEvent
    {
        public static readonly short Id = EventId<MouseMovedEvent>.Value;
        public readonly int X;
        public readonly int Y;

        public MouseMovedEvent(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
