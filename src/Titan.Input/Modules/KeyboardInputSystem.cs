using System.Runtime.CompilerServices;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.Graphics.Modules;

namespace Titan.Input.Modules;

public struct KeyboardInputSystem : IStructSystem<KeyboardInputSystem>
{
    private const uint KeyCodeBufferSize = (uint)KeyCode.NumberOfKeys * sizeof(int);

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

        Unsafe.CopyBlock(state->Previous, state->Current, KeyCodeBufferSize);

        if (system._lostFocus.HasEvents())
        {
            Unsafe.InitBlock(state->Current, 0, KeyCodeBufferSize);
            return;
        }

        foreach (ref readonly var @event in system._keyPressed.GetEvents())
        {
            state->Current[@event.Code] = true;
        }

        foreach (ref readonly var @event in system._keyReleased.GetEvents())
        {
            state->Current[@event.Code] = false;
        }
    }

    // NOTE(Jens): this system must always run since the "previous" state of a key has to be updated each frame
    public static bool ShouldRun(in KeyboardInputSystem system) => true;
}
