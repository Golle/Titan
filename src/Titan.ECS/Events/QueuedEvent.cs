using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.ECS.Events
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 32)]
    internal readonly unsafe struct QueuedEvent
    {
        public readonly short Type;
        private readonly EventData Data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T As<T>() where T : unmanaged
        {
            fixed (EventData* pEvent = &Data)
            {
                return ref *((T*) pEvent);
            }
        }
        internal struct EventData {}
    }
}
