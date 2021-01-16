using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.ECS.World;

// ReSharper disable InconsistentNaming
namespace Titan.ECS.Messaging
{

    // TODO: re-use the Core.Messagin.EventQueue at some point. This is a copy-paste from that with some replaced names. Not proud of this solution but the IOC doesn't support multiple instances of the same class.
    public interface IEventManager
    {
        ReadOnlySpan<ECSEvent> GetEvents();
        void Update();
        void Push<T>(in T @event) where T : unmanaged;
    }
    
    internal unsafe class EventManager : IEventManager, IDisposable
    {
        private readonly int _maxEvents;
        private byte* _events;
        private volatile int _count; // TODO: do we need it to be volatile? Multiple threads will write to this at the same time. 

        private bool _high;
        private byte* _eventsLastFrame;
        private int _numberOfEventsLastFrame;

        private const int EventTypeSize = sizeof(short);
        private static readonly int EventSize = sizeof(ECSEvent);

        static EventManager() => ValidateSize();

        [Conditional("DEBUG")]
        private static void ValidateSize()
        {
            var biggestEventSize = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.IsValueType && t.GetCustomAttribute(typeof(ECSEventAttribute)) != null))
                .Max(Marshal.SizeOf);

            var eventSize = sizeof(ECSEvent) - sizeof(short);
            Debug.Assert(biggestEventSize <= eventSize, $"The event data of {nameof(ECSEvent)} is smaller than the biggest event size: {eventSize} < {biggestEventSize}");

            if (biggestEventSize < eventSize)
            {
                LOGGER.Warning("The biggest event has the size {0} but the fixed size of {1} have the size {2} (event data: {3})", biggestEventSize, nameof(ECSEvent), sizeof(ECSEvent), eventSize);
            }
        }

        public EventManager(WorldConfiguration configuration)
        {
            _maxEvents = (int)configuration.MaxEvents;
            _events = (byte*)Marshal.AllocHGlobal(_maxEvents * EventSize * 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<ECSEvent> GetEvents() => new(_eventsLastFrame, _numberOfEventsLastFrame);

        public void Update()
        {
            _high = !_high;
            if (_high)
            {
                _numberOfEventsLastFrame = _count;
                _eventsLastFrame = _events;
                _count = _maxEvents;
            }
            else
            {
                _numberOfEventsLastFrame = _count - _maxEvents;
                _eventsLastFrame = _events + (_maxEvents * EventSize);
                _count = 0;
            }
        }

        public void Push<T>(in T @event) where T : unmanaged
        {
            Debug.Assert(_events != null, $"{nameof(EventManager)} has not been initialized");
            Debug.Assert(!_high || (_count - _maxEvents) < _maxEvents, $"Event limit reached. max events {_maxEvents}, current: {_count - _maxEvents}");
            Debug.Assert(_high || _count < _maxEvents, $"Event limit reached. max events {_maxEvents}, current: {_count}");

            var offset = Interlocked.Increment(ref _count) - 1;
            var pEvent = _events + (offset * EventSize);
            var eventId = EventId<T>.Value;
            // TODO: is this faster than copy block? *(short*) pEvent = eventId;
            Unsafe.CopyBlock(pEvent, &eventId, EventTypeSize);
            fixed (T* ptr = &@event)
            {
                Unsafe.CopyBlock(pEvent + EventTypeSize, ptr, (uint)sizeof(T));
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
