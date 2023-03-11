namespace Titan.Windows.Events;

public readonly struct WindowResizeEvent : IWindowEvent
{
    public static uint Id => WindowEventID.WindowExitSizeMove;
}
