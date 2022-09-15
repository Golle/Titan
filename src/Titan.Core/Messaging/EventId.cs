namespace Titan.Core.Messaging
{
    [Obsolete("Dont use this")]
    internal static class EventId
    {
        private static int _eventId;
        
#if DEBUG
        public static short NextId(Type type)
        {
            var id = (short) Interlocked.Increment(ref _eventId);
            EventTypes[id] = type.Name;
            return id;
        }

        public static readonly Dictionary<short, string> EventTypes = new();
#else
        public static short NextId() => (short)Interlocked.Increment(ref _eventId);
#endif
    }

    public static class EventId<T> where T : unmanaged
    {
#if DEBUG
        public static short Value { get; } = EventId.NextId(typeof(T));
#else
        public static short Value { get; } = EventId.NextId();
#endif
    }
}
