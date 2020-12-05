using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Titan.Core.Messaging
{
    public class EventQueueInternal<T> where T : struct
    {
        private static readonly int MaxEvents;

        private static readonly T[] Messages;

        private static int _count;

        private static bool _high;
        
        private static int _start;
        private static int _length;

        static EventQueueInternal()
        {
            MaxEvents = 10000;
            Messages = new T[MaxEvents * 2];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Push(in T @event) => Messages[Interlocked.Increment(ref _count) - 1] = @event;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<T> GetEvents() =>  _length > 0 ? new ReadOnlySpan<T>(Messages, _start, _length) : default;

        // ReSharper disable once UnusedMember.Local
        private static void Swap()
        {
            if (_high)
            {
                _start = MaxEvents;
                _length = _count - MaxEvents;
                _count = 0;
            }
            else
            {
                _start = 0;
                _length = _count;
                _count = MaxEvents;
            }
            _high = !_high;
        }
    }
}
