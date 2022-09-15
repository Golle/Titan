using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.Worlds;
using Titan.Memory;
using Titan.Memory.Allocators;

namespace Titan.ECS.Events;

internal unsafe struct EventsRegistry : IResource
{
    private static readonly uint MinEventStreamSize = MemoryUtils.KiloBytes(8);

    private MemoryManager* _memoryManager;

    private LinearAllocator _allocator1;
    private LinearAllocator _allocator2;

    private LinearAllocator* _current;
    private LinearAllocator* _previous;

    private TitanArray<InternalState> _internal;

    public bool Init(MemoryManager* memoryManager, uint size, uint maxEventCount)
    {
        Logger.Trace<EventsRegistry>($"Creating the event stream with initial size: {size} bytes and a max event count of {maxEventCount}");

        // Split the size in 2 since we just want to allocate the amount of memory that was set in the config.
        var halfSize = Math.Max(size / 2, MinEventStreamSize);

        _internal = memoryManager->AllocArray<InternalState>(maxEventCount);
        // Allocate 2 arenas that we'll swap between.
        var result = memoryManager->CreateLinearAllocator(LinearAllocatorArgs.Permanent(halfSize), out _allocator1) &&
                     memoryManager->CreateLinearAllocator(LinearAllocatorArgs.Permanent(halfSize), out _allocator2);

        if (!result)
        {
            Logger.Error<EventsRegistry>("Failed to init the allocator");
            return false;
        }

        _current = MemoryUtils.AsPointer(_allocator1);
        _previous = MemoryUtils.AsPointer(_allocator2);
        _memoryManager = memoryManager;
        return true;
    }


    public EventsReader<T> GetReader<T>() where T : unmanaged, IEvent
    {
        var state = _internal.GetPointer(EventId.Id<T>());
        var previous = state->GetPreviousPointer();
        return new(&previous->FirstEvent, &previous->Count);
    }

    public EventsWriter<T> GetWriter<T>() where T : unmanaged, IEvent
        => new(MemoryUtils.AsPointer(this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Send<T>(in T @event) where T : unmanaged, IEvent
        => _internal[EventId.Id<T>()].Send(_current, @event);

    internal void Swap()
    {
        var tmp = _current;
        _current = _previous;
        _previous = tmp;
        _current->Reset();

        foreach (ref var state in _internal.AsSpan())
        {
            state.Swap();
        }
    }

    internal void Release()
    {
        _allocator1.Release();
        _allocator2.Release();
        _memoryManager->Free(_internal);
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

        public void Send<T>(LinearAllocator* allocator, in T @event) where T : unmanaged
        {
            var size = sizeof(T) + sizeof(EventHeader);
            var header = (EventHeader*)allocator->Alloc(size);
            header->Next = null; // make sure the header doesn't point to something that doesn't exist.
            var mem = (T*)(header + 1);
            *mem = @event;
            _current.Push(header);
        }
        public EventState* GetPreviousPointer() => MemoryUtils.AsPointer(_previous);
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
