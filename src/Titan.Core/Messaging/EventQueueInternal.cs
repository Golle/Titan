using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Titan.Core.Messaging
{
    public class EventQueueInternal<T> where T : struct
    {
        private static readonly int _maxEvents;

        private static readonly T[] _messages;

        private static int _count;

        private static bool _high;
        
        private static int _start;
        private static int _length;

        static EventQueueInternal()
        {
            _maxEvents = 100;
            _messages = new T[_maxEvents * 2];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Push(in T @event) => _messages[Interlocked.Increment(ref _count) - 1] = @event;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<T> GetEvents() =>  _length > 0 ? new ReadOnlySpan<T>(_messages, _start, _length) : default;

        // ReSharper disable once UnusedMember.Local
        private static void Swap()
        {
            if (_high)
            {
                _start = _maxEvents;
                _length = _count - _maxEvents;
                _count = 0;
            }
            else
            {
                _start = 0;
                _length = _count;
                _count = _maxEvents;
            }
            _high = !_high;
        }
    }
}
