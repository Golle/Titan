using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Titan.Core;
using Titan.Core.Logging;

namespace Titan.Graphics.Modules;

public unsafe struct WindowEventQueue : IApi
{
    public const int EventMaxSize = 32; // The max size in bytes for a single event. We pack all events in the same size so it's easy to read
    public const int MaxEvents = 1024;

    private fixed byte _buffer[MaxEvents * EventMaxSize];
    private volatile int _head;
    private volatile int _tail;

    private volatile int _eventCount;
    public readonly int EventCount => _eventCount;

    [StructLayout(LayoutKind.Sequential, Size = EventMaxSize)]
    public struct WindowEvent
    {
        public uint Id;
        internal byte Data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T As<T>() where T : unmanaged, IWindowEvent
        {
            Debug.Assert(T.Id == Id, $"Trying to cast WindowEvent ID {Id} to {T.Id}");
            fixed (byte* pByte = &Data)
            {
                return ref *(T*)pByte;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Is<T>() where T : unmanaged, IWindowEvent => Id == T.Id;
    }

    public bool Push<T>(in T @event) where T : unmanaged, IWindowEvent
    {
        // NOTE(Jens): this case is not handled well, maybe we need to do something else.
        if (_eventCount >= MaxEvents - 1)
        {
            Logger.Error<WindowEventQueue>($"Buffer overflow. event {typeof(T).Name} with ID {T.Id} will be discarded.");
            return false;
        }

        fixed (byte* pBuffer = _buffer)
        {
            while (true) // Add iteration check?
            {
                var current = _head;
                var index = Interlocked.CompareExchange(ref _head, (current + 1) % MaxEvents, current);
                // Some other thread updated the counter, do another lap
                if (index != current)
                {
                    continue;
                }
                var ptr = (WindowEvent*)pBuffer + index;
                ptr->Id = T.Id;
                *(T*)&ptr->Data = @event;
                Interlocked.Increment(ref _eventCount);
                break;
            }
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEvents() => _tail != _head;

    public bool TryPeekEvent(out WindowEvent @event)
    {
        Unsafe.SkipInit(out @event);
        if (HasEvents())
        {
            fixed (byte* pBuffer = _buffer)
            {
                @event = *((WindowEvent*)pBuffer + _tail);
                return true;
            }
        }
        return false;
    }

    public bool TryReadEvent(out WindowEvent @event)
    {
        Unsafe.SkipInit(out @event);
        while (HasEvents())
        {
            var current = _tail;
            var index = Interlocked.CompareExchange(ref _tail, (current + 1) % MaxEvents, current);
            if (index != current)
            {
                continue;
            }
            fixed (byte* pBuffer = _buffer)
            {
                @event = *((WindowEvent*)pBuffer + index);
                Interlocked.Decrement(ref _eventCount);
                return true;
            }
        }
        return false;
    }
}
