using Titan.Core.Messaging;

namespace Titan.Windows.Events
{
    [TitanEvent]
    public readonly struct CharacterTypedEvent
    {
        public readonly char Character;
        public CharacterTypedEvent(char character)
        {
            Character = character;
        }
    }
}
