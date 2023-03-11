namespace Titan.Windows.Events;

public readonly struct WindowSizeEvent : IWindowEvent
{
    public static uint Id => WindowEventID.WindowSize;
    public readonly uint Width;
    public readonly uint Height;
    public WindowSizeEvent(uint width, uint height)
    {
        Width = width;
        Height = height;
    }
}
