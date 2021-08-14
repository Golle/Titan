using Titan.Core.Messaging;

namespace Titan.Graphics.Windows.Events
{
    public readonly struct WindowCreatedEvent
    {
        public static readonly short Id = EventId<WindowCreatedEvent>.Value;
        public readonly uint Width;
        public readonly uint Height;
        public WindowCreatedEvent(uint width, uint height)
        {
            Width = width;
            Height = height;
        }
    }
}
