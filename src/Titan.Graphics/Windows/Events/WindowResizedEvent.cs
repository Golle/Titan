using Titan.Core.Messaging;

namespace Titan.Graphics.Windows.Events
{
    public readonly struct WindowResizedEvent
    {
        public static readonly short Id = EventId<WindowResizedEvent>.Value;
        public readonly uint Width;
        public readonly uint Height;
        public WindowResizedEvent(uint width, uint height)
        {
            Width = width;
            Height = height;
        }
    }
}
