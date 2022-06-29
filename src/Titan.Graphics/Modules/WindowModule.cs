using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Titan.Core.Logging;
using Titan.Core;

namespace Titan.Graphics.Modules;

public record struct WindowCreated;
public record struct WindowClosed;

public struct WindowModule : IModule
{
    public static unsafe void Build(IApp app)
    {
        // Register the resources and events
        var eventQueue = app.GetMutableResourcePointer<WindowEventQueue>();
        // NOTE(Jens): Windows functions, replace with any other platform when we support that.
        var windowFunctions = WindowFunctions.Create<Win32WindowFunctions>();
        
        // Create the Window struct with functions and the event queue
        var window = new Window(windowFunctions, eventQueue);

        app
            .AddResource(window)
            .AddEvent<WindowCreated>()
            .AddEvent<WindowClosed>()
            ;

        // Get the window descriptor and create the window
        if (!app.HasResource<WindowDescriptor>())
        {
            app.AddResource(WindowDescriptor.Default());
        }

        var descriptor = app.GetResource<WindowDescriptor>();
        Logger.Trace<WindowModule>($"Window descriptor: {descriptor.Title} - Size: {descriptor.Width}x{descriptor.Height}. Can resize: {descriptor.Resizable}");

        var createWindowResult = app
            .GetMutableResource<Window>()
            .CreateWindow(descriptor);

        if (!createWindowResult)
        {
            Logger.Error<WindowModule>("Failed to create the Window.");
            throw new Exception($"{nameof(WindowModule)} failed to initialize the window.");
        }
    }
}


public unsafe struct WindowEventQueue
{
    public const int EventMaxSize = 32; // The max size in bytes for a single event. We pack all events in the same size so it's easy to read
    public const int MaxEvents = 1024;

    private fixed byte _buffer[MaxEvents * EventMaxSize];
    private volatile int _head;
    private volatile int _tail;

    private volatile int _eventCount;

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

public interface IWindowEvent
{
    static abstract uint Id { get; }
}

