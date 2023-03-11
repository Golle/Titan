using Titan.Input;

namespace Titan.Windows.Events;

public readonly struct KeyReleasedEvent : IWindowEvent
{
    public static uint Id => WindowEventID.KeyReleased;
    public readonly KeyCode Code;
    public KeyReleasedEvent(KeyCode code)
    {
        Code = code;
    }
}
