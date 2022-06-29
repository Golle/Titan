using Titan.Core.Logging;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2;

namespace Titan.Graphics.Modules;

public class WindowMessageSystem : ResourceSystem
{
    private readonly MutableResource<WindowEventQueue> _eventQueue;
    
    private readonly EventsWriter<WindowLostFocus> _lostFocusEvents;
    private readonly EventsWriter<WindowGainedFocus> _gainedFocusEvents;

    public WindowMessageSystem()
    {
        _eventQueue = GetMutableResource<WindowEventQueue>();
        _lostFocusEvents = GetEventsWriter<WindowLostFocus>();
        _gainedFocusEvents = GetEventsWriter<WindowGainedFocus>();
    }

    public override void OnUpdate()
    {
        ref var eventQueue = ref _eventQueue.Get();
        var eventCount = eventQueue.EventCount;
        for (var i = 0; i < eventCount; ++i)
        {
            if (!eventQueue.TryReadEvent(out var @event))
            {
                break;
            }

            if (@event.Is<WindowLostFocus>())
            {
                _lostFocusEvents.Send(@event.As<WindowLostFocus>());
            }
            else if (@event.Is<WindowGainedFocus>())
            {
                _gainedFocusEvents.Send(@event.As<WindowGainedFocus>());
            }
            else
            {
                Logger.Warning<WindowMessageSystem>($"Unhandled WindowEvent with ID {@event.Id}. Discarded.");
            }
        }
    }
}
