using System.Threading;

namespace Titan.Core.Messaging
{
    internal static class EventId
    {
        private static int _eventId;
        public static short NextId() => (short)Interlocked.Increment(ref _eventId);
    }

    public static class EventId<T> where T : unmanaged
    {
        public static short Value { get; } = EventId.NextId();
    }
}
