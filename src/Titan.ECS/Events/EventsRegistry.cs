using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.TheNew;

namespace Titan.ECS.Events;

internal unsafe struct EventsRegistry : IResource
{
    private EventsInternal<byte>* _mem;
    private uint _count;

    public void Init(in MemoryPool pool, ReadOnlySpan<EventsDescriptor> descriptors)
    {
        Logger.Trace<EventsRegistry>($"Registering {descriptors.Length} events");
        _count = EventId.Count; // NOTE(Jens): not sure about this yet. Investigate if we need to do this in some other way. (Maybe source gen is better?) This might add handlers for events that has not been registered.

        _mem = pool.GetPointer<EventsInternal<byte>>(_count, initialize: true);

        foreach (ref readonly var descriptor in descriptors)
        {
            Logger.Trace<EventsRegistry>($"Initialize event {descriptor.Id}");
            _mem[descriptor.Id].Init(pool, descriptor);
        }
    }

    public void Swap()
    {
        // Start at 1 since 0 is "invalid".
        for (var i = 1; i < _count; i++)
        {
            _mem[i].Swap();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Events<T> GetEvents<T>() where T : unmanaged, IEvent => new(GetInternal<T>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private EventsInternal<T>* GetInternal<T>() where T : unmanaged, IEvent
        => (EventsInternal<T>*)(_mem + EventId.Id<T>() - 1);

    internal struct EventsInternal<T> where T : unmanaged
    {
        private void* _mem;
        private T* _low;
        private T* _high;

        private uint _maxEvents;
        private int _count;
        private int _eventsLastFrame;

        public uint Count => (uint)_count;
        public void Init(in MemoryPool pool, in EventsDescriptor descriptor)
        {
            var size = descriptor.Size * descriptor.MaxEvents * 2;
            _maxEvents = descriptor.MaxEvents;
            _mem = pool.GetPointer<byte>(size);
            _low = (T*)_mem;
            _high = _low + _maxEvents;
            _eventsLastFrame = 0;
            _count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Send(in T @event) => _high[_count++] = @event;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> GetEvents() => new(_low, _eventsLastFrame);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Swap()
        {
            var tmp = _high;
            _high = _low;
            _low = tmp;
            _eventsLastFrame = _count;
            _count = 0;
        }
    }
}
