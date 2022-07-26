using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.ECS.Worlds;
using Titan.Memory;
using Titan.Memory.Arenas;

namespace Titan.ECS.Events;

internal unsafe struct EventsRegistry : IResource
{
    private const uint MinEventStreamSize = 1024 * 10; // 10KB

    private DynamicLinearArena _arena1;
    private DynamicLinearArena _arena2;

    private DynamicLinearArena* _current;
    private DynamicLinearArena* _previous;

    private InternalState* _internal;

    private uint _count;

    public void Init(in PlatformAllocator allocator, uint size, uint maxEventCount)
    {
        Logger.Trace<EventsRegistry>($"Creating the event stream with initial size: {size} bytes and a max event count of {maxEventCount}");

        // Split the size in 2 since we just want to allocate the amount of memory that was set in the config.
        var halfSize = Math.Max(size / 2, MinEventStreamSize);

        _count = maxEventCount;
        _internal = allocator.Allocate<InternalState>(_count, true);

        // Allocate 2 arenas that we'll swap between.
        _arena1 = DynamicLinearArena.Create(allocator, halfSize);
        _arena2 = DynamicLinearArena.Create(allocator, halfSize);

        _current = (DynamicLinearArena*)Unsafe.AsPointer(ref _arena1);
        _previous = (DynamicLinearArena*)Unsafe.AsPointer(ref _arena2);
    }


    public EventsReader<T> GetReader<T>() where T : unmanaged, IEvent
    {
        var state = _internal + EventId.Id<T>();
        var previous = state->GetPreviousPointer();
        return new(&previous->FirstEvent, &previous->Count);
    }

    public EventsWriter<T> GetWriter<T>() where T : unmanaged, IEvent 
        => new((EventsRegistry*)Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Send<T>(in T @event) where T : unmanaged, IEvent 
        => _internal[EventId.Id<T>()].Send(_current, @event);

    internal void Swap()
    {
        var tmp = _current;
        _current = _previous;
        _previous = tmp;
        _current->Reset();

        for (var i = 0; i < _count; ++i)
        {
            _internal[i].Swap();
        }
    }

    internal void Release(in PlatformAllocator allocator)
    {
        _arena1.Release();
        _arena2.Release();
        allocator.Free(_internal);
    }

    private struct InternalState
    {
        private EventState _current;
        private EventState _previous;
        public void Swap()
        {
            _previous = _current;
            _current = default;
        }

        public void Send<T>(DynamicLinearArena* arena, in T @event) where T : unmanaged
        {
            var size = sizeof(T) + sizeof(EventHeader);
            var header = (EventHeader*)arena->Allocate((nuint)size);
            var mem = (T*)(header + 1);
            *mem = @event;
            _current.Push(header);
        }
        public EventState* GetPreviousPointer() => (EventState*)Unsafe.AsPointer(ref _previous);
    }

    private struct EventState
    {
        public EventHeader* FirstEvent;
        private EventHeader* _lastEvent;
        public uint Count;
        public void Push(EventHeader* @event)
        {
            if (FirstEvent == null)
            {
                _lastEvent = FirstEvent = @event;
            }
            else
            {
                _lastEvent->Next = @event;
                _lastEvent = @event;
            }
            Count++;
        }
    }
}
