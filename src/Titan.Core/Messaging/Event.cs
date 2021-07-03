using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Core.Messaging
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 32)]
    public unsafe struct Event
    {
        public readonly short Type;
        private readonly EventData Data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T As<T>() where T : unmanaged
        {
            fixed (EventData* pEvent = &Data)
            {
                return ref *(T*)pEvent;
            }
        }

        internal struct EventData { }

        public override string ToString()
        {
#if DEBUG
            return EventId.EventTypes[Type];
#else
            return string.Empty;
#endif
        }
    }
}
