using Titan.Core.Messaging;
using Titan.Windows.Win32;

namespace Titan.Windows.Events
{
    [TitanEvent]
    public readonly struct KeyDownEvent
    {
        public readonly KeyCode Code;
        public readonly bool Repeat;

        public KeyDownEvent(KeyCode code, bool repeat)
        {
            Code = code;
            Repeat = repeat;
        }
    }
}
