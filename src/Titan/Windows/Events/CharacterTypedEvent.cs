namespace Titan.Windows.Events;

public readonly struct CharacterTypedEvent : IWindowEvent
{
    public static uint Id => WindowEventID.CharacterTyped;
    public readonly char Character;

    public CharacterTypedEvent(char character)
    {
        Character = character;
    }
}
