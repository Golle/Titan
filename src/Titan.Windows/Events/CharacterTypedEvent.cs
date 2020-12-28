using Titan.Core.Messaging;

namespace Titan.Windows.Events
{
    [TitanEvent]
    public readonly struct CharacterTypedEvent
    {
        public static readonly short Id = EventId<CharacterTypedEvent>.Value;
        public readonly char Character;
        public CharacterTypedEvent(char character)
        {
            Character = character;
        }
    }
}
