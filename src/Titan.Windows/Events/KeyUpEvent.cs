using Titan.Core.Messaging;
using Titan.Windows.Win32;

namespace Titan.Windows.Events
{
    [TitanEvent]
    public readonly struct KeyUpEvent
    {
        public readonly KeyCode Code;

        public KeyUpEvent(KeyCode code)
        {
            Code = code;
        }
    }
}
