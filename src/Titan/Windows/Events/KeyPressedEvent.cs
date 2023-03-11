using Titan.Input;

namespace Titan.Windows.Events;

public readonly struct KeyPressedEvent : IWindowEvent
{
    public static uint Id => WindowEventID.KeyPressed;
    public readonly KeyCode Code;
    public readonly bool Repeat;
    public KeyPressedEvent(KeyCode code, bool repeat)
    {
        Code = code;
        Repeat = repeat;
    }
}
