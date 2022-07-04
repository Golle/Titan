using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core.Memory;

namespace Titan.Core.Events;

public unsafe struct EventCollection<T> where T : unmanaged
{
    private byte* _mem;

    private T* _low;
    private T* _high;

    // NOTE(Jens): using pointers will allow "mistakes" that copies the Events<T> struct.
    private readonly int* _eventsLastFrame;
    private readonly int* _count;
    private readonly int _maxEvents;

    public int Count => *_count;
    public EventCollection(uint count, in PermanentMemory allocator)
    {
        // NOTE(Jens): this should be allocated on a common events pool
        var size = (sizeof(int) * 2 + count * 2 * sizeof(T));
        _mem = allocator.GetBlock((uint)size).AsPointer();

        _eventsLastFrame = (int*)_mem;
        _count = _eventsLastFrame + 1;
        _low = (T*)(_count + 1);
        _high = _low + count;
        _maxEvents = (int)count;
        *_eventsLastFrame = 0;
        *_count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Send(in T @event)
    {
        Debug.Assert(*_count < _maxEvents);
        ref var count = ref *_count;
        _high[count++] = @event;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> GetEvents() => new(_low, *_eventsLastFrame);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Swap()
    {
        var tmp = _high;
        _high = _low;
        _low = tmp;
        *_eventsLastFrame = *_count;
        *_count = 0;
    }

    public void Release()
    {
        if (_mem != null)
        {
            NativeMemory.Free(_mem);
            _mem = null;
        }
    }
}
