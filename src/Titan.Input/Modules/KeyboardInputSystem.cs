using System.Runtime.CompilerServices;
using Titan.Core.Memory;
using Titan.ECS.Events;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.Graphics.Modules;

namespace Titan.Input.Modules;

public struct KeyboardInputSystem : IStructSystem<KeyboardInputSystem>
{
    private const uint KeyCodeBufferSize = (uint)KeyCode.NumberOfKeys * sizeof(bool);

    private EventsReader<KeyReleased> _keyReleased;
    private EventsReader<KeyPressed> _keyPressed;
    private MutableResource<KeyboardState> _state;
    private EventsReader<WindowLostFocus> _lostFocus;

    public static void Init(ref KeyboardInputSystem system, in SystemsInitializer init)
    {
        system._state = init.GetMutableResource<KeyboardState>();
        system._keyPressed = init.GetEventsReader<KeyPressed>();
        system._keyReleased = init.GetEventsReader<KeyReleased>();
        system._lostFocus = init.GetEventsReader<WindowLostFocus>();
    }

    public static unsafe void Update(ref KeyboardInputSystem system)
    {
        var state = system._state.AsPointer();
        MemoryUtils.Copy(state->Previous, state->Current, KeyCodeBufferSize);

        if (system._lostFocus.HasEvents())
        {
            MemoryUtils.Init(state->Current, KeyCodeBufferSize);
            return;
        }

        if (system._keyPressed.HasEvents())
        {
            foreach (ref readonly var @event in system._keyPressed)
            {
                state->Current[@event.Code] = true;
            }
        }
        if (system._keyReleased.HasEvents())
        {
            foreach (ref readonly var @event in system._keyReleased)
            {
                state->Current[@event.Code] = false;
            }
        }
    }

    // NOTE(Jens): this system must always run since the "previous" state of a key has to be updated each frame
    public static bool ShouldRun(in KeyboardInputSystem system) => true;
}
