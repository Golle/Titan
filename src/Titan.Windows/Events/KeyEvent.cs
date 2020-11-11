using Titan.Core.Messaging;

namespace Titan.Windows.Events
{
    [TitanEvent]
    public readonly struct KeyEvent
    {
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
