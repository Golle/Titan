using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Titan.Core.Logging;
using Titan.ECS.World;

namespace Titan.ECS.Events
{
    // TODO: move this to Titan.Core and replace the existing event queue
    internal unsafe class EventQueue : IDisposable
    {
        private readonly int _maxEvents;
        private byte* _events;
        private int _count;


        private bool _high;
        private byte* _evensLastFrame;
        private int _numberOfEventsLastFrame;

        
        private const int EventTypeSize = sizeof(short);
        private static readonly int EventSize = sizeof(QueuedEvent);

        static EventQueue() => ValidateSize();
        [Conditional("DEBUG")]
        private static void ValidateSize()
        {
            var biggestEventSize = typeof(EventQueue)
                .Assembly
                .GetTypes()
                .Where(t => t.IsValueType && t.GetCustomAttribute(typeof(ECSEventAttribute)) != null)
                .Max(Marshal.SizeOf);

            var eventSize = sizeof(QueuedEvent) - sizeof(short);
            Debug.Assert(biggestEventSize <= eventSize, $"The event data of {nameof(QueuedEvent)} is smaller than the biggest event size: {eventSize} < {biggestEventSize}");

            if (biggestEventSize < eventSize)
            {
                LOGGER.Warning("The biggest event has the size {0} but the fixed size of {1} have the size {2} (event data: {3})", biggestEventSize, nameof(QueuedEvent), sizeof(QueuedEvent), eventSize);
            }
        }

        public EventQueue(WorldConfiguration configuration)
        {
            _maxEvents = (int) configuration.EventsConfiguration.MaxEventQueueSize;
            
            _events = (byte*) Marshal.AllocHGlobal(_maxEvents * EventSize * 2);
        }
        
        public void Push<T>(in T @event) where T : unmanaged
        {
            Debug.Assert(!_high || (_count - _maxEvents) < _maxEvents, $"Event limit reached. max events {_maxEvents}, current: {_count - _maxEvents}");
            Debug.Assert(_high || _count < _maxEvents, $"Event limit reached. max events {_maxEvents}, current: {_count}");
            var offset = Interlocked.Increment(ref _count) - 1;
            var pEvent = _events + (offset * EventSize);
            var eventId = EventId<T>.Value;
            Unsafe.CopyBlock(pEvent, &eventId, EventTypeSize);
            fixed (T* ptr = &@event)
            {
                Unsafe.CopyBlock(pEvent + EventTypeSize, ptr, (uint)sizeof(T));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<QueuedEvent> GetEvents() => new(_evensLastFrame, _numberOfEventsLastFrame);

        public void Update()
        {
            _high = !_high;
            if (_high)
            {
                _numberOfEventsLastFrame = _count;
                _evensLastFrame = _events;
                _count = _maxEvents;
            }
            else
            {
                _numberOfEventsLastFrame = _count - _maxEvents;
                _evensLastFrame = _events + (_maxEvents * EventSize);
                _count = 0;
            }
        }
        public void Dispose()
        {
            if (_events != null)
            {
                Marshal.FreeHGlobal((nint)_events);
                _events = null;
            }
        }
    }
}
