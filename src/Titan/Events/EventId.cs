using System.Runtime.CompilerServices;
using Titan.Core;

namespace Titan.Events;

public readonly struct EventId
{
    private readonly uint _id;
    public EventId(uint id) => _id = id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EventId Id<T>() where T : unmanaged, IEvent => EventIdInternal<T>.Id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in EventId l, in EventId r) => l._id == r._id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in EventId l, in EventId r) => l._id != r._id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator uint(in EventId eventId) => eventId._id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator int(in EventId eventId) => unchecked((int)eventId._id);
    public override bool Equals(object obj) => throw new NotSupportedException("Use == to avoid boxing");
    public override int GetHashCode() => (int)_id;

#if DEBUG
    private static readonly System.Collections.Generic.Dictionary<uint, string> _eventNames = new();
    public override string ToString() => _eventNames.TryGetValue(_id, out var id) ? id : _id.ToString();
#else
    public override string ToString() => _id.ToString();
#endif

    public static uint Count => IdGenerator<EventId>.Count;
    private readonly struct EventIdInternal<T> where T : unmanaged, IEvent
    {
        public static readonly EventId Id = CreateNew();
        private static EventId CreateNew()
        {
            var id = new EventId(IdGenerator<EventId>.Next());
#if DEBUG
            _eventNames[id] = typeof(T).FormattedName();
#endif
            return id;
        }
    }
}
