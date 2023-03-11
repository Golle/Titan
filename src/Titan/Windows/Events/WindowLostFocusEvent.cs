namespace Titan.Windows.Events;

public struct WindowLostFocusEvent : IWindowEvent
{
    public static uint Id => WindowEventID.WindowLostFoucs;
}
