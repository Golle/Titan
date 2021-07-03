using Titan.Core.Messaging;

namespace Titan.Graphics.Windows.Events
{
    public readonly struct KeyEvent
    {
        public static readonly short Id = EventId<KeyEvent>.Value;
        public readonly int Code;
        public readonly bool Repeat;
        public readonly bool Down;

        public KeyEvent(int code, bool down, bool repeat)
        {
            Code = code;
            Down = down;
            Repeat = repeat;
        }
    }
}
