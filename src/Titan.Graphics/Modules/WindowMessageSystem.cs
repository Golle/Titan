using Titan.Core.Logging;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2;

namespace Titan.Graphics.Modules;

internal struct WindowMessageSystem : IStructSystem<WindowMessageSystem>
{
    private ApiResource<WindowEventQueue> _eventQueue;
    private EventsWriter<WindowLostFocus> _lostFocus;
    private EventsWriter<WindowGainedFocus> _gainedFocus;
    private EventsWriter<KeyPressed> _keyPressed;
    private EventsWriter<KeyReleased> _keyReleased;
    private EventsWriter<WindowSize> _windowSize;
    private EventsWriter<WindowResizeComplete> _windowResize;
    
    private MutableResource<Window> _window;

    public static void Init(ref WindowMessageSystem system, in SystemsInitializer init)
    {
        system._eventQueue = init.GetApi<WindowEventQueue>();
        system._lostFocus = init.GetEventsWriter<WindowLostFocus>();
        system._gainedFocus = init.GetEventsWriter<WindowGainedFocus>();
        system._keyPressed = init.GetEventsWriter<KeyPressed>();
        system._keyReleased = init.GetEventsWriter<KeyReleased>();
        system._windowSize = init.GetEventsWriter<WindowSize>();
        system._windowResize = init.GetEventsWriter<WindowResizeComplete>();

        system._window = init.GetMutableGlobalResource<Window>();
    }

    public static void Update(ref WindowMessageSystem system)
    {
        ref var eventQueue = ref system._eventQueue.Get();
        var eventCount = eventQueue.EventCount;
        for (var i = 0; i < eventCount; ++i)
        {
            if (!eventQueue.TryReadEvent(out var @event))
            {
                break;
            }

            if (@event.Is<WindowLostFocus>())
            {
                system._lostFocus.Send(@event.As<WindowLostFocus>());
            }
            else if (@event.Is<WindowGainedFocus>())
            {
                system._gainedFocus.Send(@event.As<WindowGainedFocus>());
            }
            else if (@event.Is<KeyPressed>())
            {
                system._keyPressed.Send(@event.As<KeyPressed>());
            }
            else if (@event.Is<KeyReleased>())
            {
                system._keyReleased.Send(@event.As<KeyReleased>());
            }
            else if (@event.Is<WindowSize>())
            {
                ref readonly var windowSize = ref @event.As<WindowSize>();
                system._windowSize.Send(windowSize);
                ref var window = ref system._window.Get();
                window.Height = windowSize.Height;
                window.Width = windowSize.Width;
            }
            else if (@event.Is<WindowResizeComplete>())
            {
                system._windowResize.Send(@event.As<WindowResizeComplete>());
            }
            else
            {
                Logger.Warning<WindowMessageSystem>($"Unhandled WindowEvent with ID {@event.Id}. Discarded.");
            }
        }
    }

    public static bool ShouldRun(in WindowMessageSystem system) 
        => system._eventQueue.Get().HasEvents();
}
