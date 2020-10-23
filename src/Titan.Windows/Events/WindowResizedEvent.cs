using Titan.Core.Messaging;

namespace Titan.Windows.Events
{
    [TitanEvent]
    public readonly struct WindowResizedEvent
    {
        public readonly int Width;
        public readonly int Height;
        public WindowResizedEvent(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
