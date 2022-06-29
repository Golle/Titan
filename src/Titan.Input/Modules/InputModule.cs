using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.App;
using Titan.Core.Events;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2;
using Titan.Graphics.Modules;

namespace Titan.Input.Modules;

public readonly struct InputModule : IModule
{
    public static void Build(IApp app)
    {
        if (!app.HasResource<Window>())
        {
            throw new InvalidOperationException($"{nameof(InputModule)} requires the {nameof(Window)} resource to be added.");
        }

        app
            .AddModule<KeyboardInputModule>()
            .AddModule<MouseInputModule>()


            ;
    }
}

public readonly struct MouseInputModule : IModule
{
    public static void Build(IApp app)
    {
        if (!app.HasResource<Window>())
        {
            throw new InvalidOperationException($"{nameof(InputModule)} requires the {nameof(Window)} resource to be added.");
        }

        app.AddSystem<MouseInputSystem>();
    }
}
public readonly struct KeyboardInputModule : IModule
{
    public static void Build(IApp app)
    {
        throw new NotImplementedException();
    }
}


public unsafe struct KeyboardState
{
    public fixed bool Current[(int)KeyCode.NumberOfKeys];
    public fixed bool Previous[(int)KeyCode.NumberOfKeys];
}
public class KeyboardInputSystem : ResourceSystem
{
    private const uint KeyCodeBufferSize  = (uint) KeyCode.NumberOfKeys * sizeof(int);
    private readonly MutableResource<KeyboardState> _keyboardState;
    private readonly MutableResource<WindowEventQueue> _windowEvents;

    public KeyboardInputSystem()
    {
        _keyboardState = GetMutableResource<KeyboardState>();
        _windowEvents = GetMutableResource<WindowEventQueue>();
    }
    public override unsafe void OnUpdate()
    {
        var state = _keyboardState.AsPointer();
        Unsafe.CopyBlock(state->Previous, state->Current, KeyCodeBufferSize);
        ref var events = ref _windowEvents.Get();
        var eventCount = events.EventCount;
        // crap. this wont work

        //NOTE(Jens): only try to read the number of events at the start of the loop, leave any new events for next frame
        for (var i = 0; i < eventCount; ++i)
        {
            if (!events.TryReadEvent(out var windowEvent))
            {
                break;
            }

            if (windowEvent.Id == KeyReleased.Id)
            {
                state->Current[1] = false;
            }
            else if (windowEvent.Id == KeyPressed.Id)
            {
                state->Current[1] = true;
            }
            else if(windowEvent.Id == WindowLostFocus.Id)
            {
                // If the focus is lost, just clear the entire buffer.
                Unsafe.InitBlock(state->Current, 0, KeyCodeBufferSize);
            }
        }
    }
}

public class MouseInputSystem : ResourceSystem
{
    private readonly ReadOnlyResource<Window> _window;

    public MouseInputSystem()
    {
        _window = GetReadOnlyResource<Window>();
    }
    
    public override void OnUpdate()
    {
        ref readonly var window = ref _window.Get();
        if(window.GetRelativeCursorPosition(out var position))
        {
            // do the things
        }
    }
}
