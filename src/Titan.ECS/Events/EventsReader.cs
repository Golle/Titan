using System.Runtime.CompilerServices;
using Titan.Core;

namespace Titan.ECS.Events;
public readonly unsafe struct EventsReader<T> where T : unmanaged, IEvent
{
    private readonly EventHeader** _header;
    private readonly uint* _count;
    internal EventsReader(EventHeader** header, uint* count)
    {
        _header = header;
        _count = count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEvents() => *_count > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(_header);
    public ref struct Enumerator
    {
        private readonly EventHeader** _header;
        private EventHeader* _next;
        internal Enumerator(EventHeader** header)
        {
            _header = header;
            _next = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (_next == null)
            {
                _next = *_header;
                return _next != null;
            }
            _next = _next->Next;
            return _next != null;
        }

        public readonly ref readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *(T*)(_next + 1);
        }
    }
}
