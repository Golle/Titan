using Titan.Core.Logging;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2;

namespace Titan.Graphics.Modules;

internal struct WindowMessageSystem : IStructSystem<WindowMessageSystem>
{
    private ApiResource<WindowEventQueue> EventQueue;
    private EventsWriter<WindowLostFocus> LostFocus;
    private EventsWriter<WindowGainedFocus> GainedFocus;

    public static void Init(ref WindowMessageSystem system, in SystemsInitializer init)
    {
        system.EventQueue = init.GetApi<WindowEventQueue>();
        system.LostFocus = init.GetEventsWriter<WindowLostFocus>();
        system.GainedFocus = init.GetEventsWriter<WindowGainedFocus>();
    }

    public static void Update(ref WindowMessageSystem system)
    {
        ref var eventQueue = ref system.EventQueue.Get();
        var eventCount = eventQueue.EventCount;
        for (var i = 0; i < eventCount; ++i)
        {
            if (!eventQueue.TryReadEvent(out var @event))
            {
                break;
            }

            if (@event.Is<WindowLostFocus>())
            {
                system.LostFocus.Send(@event.As<WindowLostFocus>());
            }
            else if (@event.Is<WindowGainedFocus>())
            {
                system.GainedFocus.Send(@event.As<WindowGainedFocus>());
            }
            else
            {
                Logger.Warning<WindowMessageSystem>($"Unhandled WindowEvent with ID {@event.Id}. Discarded.");
            }
        }
    }

    public static bool ShouldRun(in WindowMessageSystem system) 
        => system.EventQueue.Get().HasEvents();
}
