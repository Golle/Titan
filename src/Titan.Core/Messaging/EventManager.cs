using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Core.Messaging;

public record EventManagerConfiguration(uint MaxEvents);

public unsafe class EventManager
{
    private static MemoryChunk<Event> _events;
        
    private static int _count;
    private static bool _high;
        
    private static int _numberOfEventsLastFrame;
    private static int _maxEvents;

    private const int EventTypeSize = sizeof(short);
    public static void Init(EventManagerConfiguration config)
    {
        Debug.Assert(config.MaxEvents > 0u);

        Logger.Trace<EventManager>($"Initialize {nameof(EventManager)} with max events {config.MaxEvents}");
        _maxEvents = (int)config.MaxEvents;
        _events = MemoryUtilsOld.AllocateBlock<Event>(config.MaxEvents * 2);
    }

    public static void Terminate()
    {
        Logger.Trace<EventManager>($"Freeing memory used by {nameof(EventManager)}");
        _events.Free();
    }

    public static void Push<T>(in T @event) where T : unmanaged
    {
        Debug.Assert(_events.Size != 0, $"{nameof(EventManager)} has not been initialized");
        Debug.Assert(!_high || (_count - _maxEvents) < _maxEvents, $"Event limit reached. max events {_maxEvents}, current: {_count - _maxEvents}");
        Debug.Assert(_high || _count < _maxEvents, $"Event limit reached. max events {_maxEvents}, current: {_count}");

        var offset = Interlocked.Increment(ref _count) - 1;
        var pEvent = (byte*)_events.GetPointer(offset);
        var eventId = EventId<T>.Value;
        // TODO: is this faster than copy block? *(short*) pEvent = eventId;
        Unsafe.CopyBlock(pEvent, &eventId, EventTypeSize);
        fixed (T* ptr = &@event)
        {
            Unsafe.CopyBlock(pEvent + EventTypeSize, ptr, (uint)sizeof(T));
        }
    }


    public static void Update()
    {
        _high = !_high;
        if (_high)
        {
            _numberOfEventsLastFrame = _count;
            _count = _maxEvents;
        }
        else
        {
            _numberOfEventsLastFrame = _count - _maxEvents;
            _count = 0;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<Event> GetEvents() => new (_events.GetPointer(_high ? 0 : _maxEvents), _numberOfEventsLastFrame);
}
