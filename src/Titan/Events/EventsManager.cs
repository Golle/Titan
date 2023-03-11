using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Memory.Allocators;

namespace Titan.Events;

/// <summary>
/// The EventManager is a struct that will be allocated in unmanaged memory, this it make it possible to use pointers to access it through the Writer and Reader.
/// </summary>
internal unsafe class EventsManager : IEventsManager
{
    private static readonly uint MinEventStreamSize = MemoryUtils.KiloBytes(80);

    private TitanArray<InternalState> _internal;
    private ILinearAllocator _current;
    private ILinearAllocator _previous;
    private IMemoryManager _memoryManager;

    private ObjectHandle<EventsManager> _eventManagerHandle;

    public bool Init(IMemoryManager memoryManager, uint size, uint maxEventCount)
    {
        Logger.Trace<EventsManager>($"Creating the event stream with initial size: {size} bytes and a max event count of {maxEventCount}");

        // Split the size in 2 since we just want to allocate the amount of memory that was set in the config.
        var halfSize = MemoryUtils.AlignToUpper(Math.Max(size / 2, MinEventStreamSize));

        _internal = memoryManager.AllocArray<InternalState>(maxEventCount, true);
        if (!_internal.IsValid)
        {
            Logger.Error<EventsManager>("Failed to allocate an array for the internal state.");
            return false;
        }

        // Create 2 allocators that we'll swap between.
        var result = memoryManager.TryCreateLinearAllocator(AllocatorStrategy.Permanent, halfSize, out _current) &&
                     memoryManager.TryCreateLinearAllocator(AllocatorStrategy.Permanent, halfSize, out _previous);
        if (!result)
        {
            Logger.Error<EventsManager>("Failed to init the allocator");
            memoryManager.Free(ref _internal);
            return false;
        }

        _memoryManager = memoryManager;
        _eventManagerHandle = new ObjectHandle<EventsManager>(this);


#if DEBUG
        for (var i = 0; i < 10; ++i)
        {
            Logger.Warning<EventsManager>("THE EVENTSMANAGER USES A STATIC LOCK TO PREVENT A CRASH IN THE ALLOCATOR. Rewrite this when possible.");
        }
#endif
        return true;
    }

    public void Swap()
    {
        (_current, _previous) = (_previous, _current);
        _current.Reset();
        foreach (ref var state in _internal.AsSpan())
        {
            state.Swap();
        }
    }

    public EventsWriter<T> CreateWriter<T>() where T : unmanaged, IEvent
        => new(_eventManagerHandle);

    public EventsReader<T> CreateReader<T>() where T : unmanaged, IEvent
    {
        var state = _internal.GetPointer(EventId.Id<T>());
        var previous = state->GetPreviousPointer();
        return new(&previous->FirstEvent, &previous->Count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Send<T>(in T @event) where T : unmanaged, IEvent
        => _internal.GetPointer(EventId.Id<T>())->Send(_current, @event);


    private struct InternalState
    {
        private EventState _current;
        private EventState _previous;

        //NOTE(Jens): This is to prevent the allocator from returning the same pointer to different types of events. WE should really rewrite this.
        private static SpinLock _lock;
        public void Swap()
        {
            _previous = _current;
            _current = default;
        }

        public void Send<T>(ILinearAllocator allocator, in T @event) where T : unmanaged
        {
            //NOTE(Jens): can we restructure this so we don't need to use a spin lock?  if we use a pool allocator with a fixed size for each event it would work. performance vs memory usage.
            var size = EventSize<T>.Size;
            var gotLock = false;
            _lock.Enter(ref gotLock);
            var header = (EventHeader*)allocator.Alloc(size);
            header->Next = null; // make sure the header doesn't point to something that doesn't exist.
            var mem = (T*)(header + 1);
            *mem = @event;
            _current.Push(header);
            _lock.Exit();
        }
        public EventState* GetPreviousPointer() => MemoryUtils.AsPointer(_previous);

        private struct EventSize<T> where T : unmanaged
        {
            public static readonly uint Size = MemoryUtils.AlignToUpper(sizeof(T) + sizeof(EventHeader));
        }
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

    public void Shutdown()
    {
        if (_memoryManager != null)
        {
            _memoryManager.Free(ref _internal);
            _current.Destroy();
            _previous.Destroy();
            _previous = _current = null;
            _memoryManager = null;
            _eventManagerHandle.Release();
        }
    }
}
