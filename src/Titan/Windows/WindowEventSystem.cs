using Titan.Core;
using Titan.Core.Logging;
using Titan.Events;
using Titan.Systems;
using Titan.Windows.Events;
using Titan.Windows.Win32;

namespace Titan.Windows;

internal struct WindowEventSystem : ISystem
{
    private MutableResource<WindowEventQueue> _eventQueue;
    private EventsWriter<KeyPressedEvent> _keyPressed;
    private EventsWriter<KeyReleasedEvent> _keyRelesed;
    private EventsWriter<WindowLostFocusEvent> _lostFocus;
    private EventsWriter<WindowGainedFocusEvent> _gainedFocus;
    private EventsWriter<WindowResizeEvent> _windowResize;
    private EventsWriter<WindowSizeEvent> _windowSize;
    private EventsWriter<CharacterTypedEvent> _characterTyped;
    private EventsWriter<AudioDeviceArrivedEvent> _audioArrived;
    private EventsWriter<AudioDeviceRemovedEvent> _audioRemoved;
    private ObjectHandle<IWindow> _window;

    public void Init(in SystemInitializer init)
    {
        _eventQueue = init.GetMutableResource<WindowEventQueue>(false);
        _keyPressed = init.GetEventsWriter<KeyPressedEvent>();
        _keyRelesed = init.GetEventsWriter<KeyReleasedEvent>();
        _lostFocus = init.GetEventsWriter<WindowLostFocusEvent>();
        _gainedFocus = init.GetEventsWriter<WindowGainedFocusEvent>();
        _windowSize = init.GetEventsWriter<WindowSizeEvent>();
        _windowResize = init.GetEventsWriter<WindowResizeEvent>();
        _characterTyped = init.GetEventsWriter<CharacterTypedEvent>();
        _audioArrived = init.GetEventsWriter<AudioDeviceArrivedEvent>();
        _audioRemoved = init.GetEventsWriter<AudioDeviceRemovedEvent>();
        _window = init.GetManagedApi<IWindow>();
    }

    public void Update()
    {
        ref var queue = ref _eventQueue.Get();
        var count = queue.EventCount;
        for (var i = 0; i < count; ++i)
        {
            if (!queue.TryReadEvent(out var @event))
            {
                break;
            }

            if (@event.Is<WindowLostFocusEvent>())
            {
                _lostFocus.Send(@event.As<WindowLostFocusEvent>());
            }
            else if (@event.Is<WindowGainedFocusEvent>())
            {
                _gainedFocus.Send(@event.As<WindowGainedFocusEvent>());
            }
            else if (@event.Is<KeyPressedEvent>())
            {
                _keyPressed.Send(@event.As<KeyPressedEvent>());
            }
            else if (@event.Is<KeyReleasedEvent>())
            {
                _keyRelesed.Send(@event.As<KeyReleasedEvent>());
            }
            else if (@event.Is<WindowSizeEvent>())
            {
                ref readonly var windowSize = ref @event.As<WindowSizeEvent>();
                //NOTE(Jens): This is not a very good solution since its hard coupled to a win32 window. But it works for now.
                var window = (Win32Window)_window.Value;
                window.Width = windowSize.Width;
                window.Height = windowSize.Height;
                _windowSize.Send(windowSize);
            }
            else if (@event.Is<WindowResizeEvent>())
            {
                _windowResize.Send(@event.As<WindowResizeEvent>());
            }
            else if (@event.Is<CharacterTypedEvent>())
            {
                _characterTyped.Send(@event.As<CharacterTypedEvent>());
            }
            else if (@event.Is<AudioDeviceArrivedEvent>())
            {
                _audioArrived.Send(@event.As<AudioDeviceArrivedEvent>());
            }
            else if (@event.Is<AudioDeviceRemovedEvent>())
            {
                _audioRemoved.Send(@event.As<AudioDeviceRemovedEvent>());
            }
            else
            {
                Logger.Warning<WindowEventSystem>($"Unhandled WindowEvent with ID {@event.Id}. Discarded.");
            }
        }
    }
}
