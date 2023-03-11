using System.Diagnostics;
using System.Numerics;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Events;
using Titan.Systems;
using Titan.Windows;
using Titan.Windows.Events;

namespace Titan.Input;

internal unsafe struct InputSystem : ISystem
{
    private MutableResource<InputState> _state;
    private ObjectHandle<IWindow> _window;
    private EventsReader<KeyPressedEvent> _keyPressed;
    private EventsReader<KeyReleasedEvent> _keyReleased;
    private EventsReader<WindowLostFocusEvent> _windowLostFocus;
    private EventsReader<CharacterTypedEvent> _characterTyped;

    public void Init(in SystemInitializer init)
    {
        _state = init.GetMutableResource<InputState>(false);
        _window = init.GetManagedApi<IWindow>();
        _keyPressed = init.GetEventsReader<KeyPressedEvent>();
        _keyReleased = init.GetEventsReader<KeyReleasedEvent>();
        _characterTyped = init.GetEventsReader<CharacterTypedEvent>();
        _windowLostFocus = init.GetEventsReader<WindowLostFocusEvent>();
    }

    public void Update()
    {
        var window = _window.Value;
        var cursorPosition = window.GetRelativeCursorPosition();
        var state = _state.AsPointer();

        state->CharacterCount = 0;
        state->Previous = state->Current;
        state->Current = default;// not sure this is needed
        state->Current.Position = new Vector2(cursorPosition.X, cursorPosition.Y);
        state->DeltaMousePosition = state->Previous.Position - state->Current.Position;
        MemoryUtils.Copy(state->PreviousKeyState, state->KeyState, (int)KeyCode.NumberOfKeys);
        foreach (ref readonly var @event in _keyPressed)
        {
            state->KeyState[(int)@event.Code] = 1;
        }
        foreach (ref readonly var @event in _keyReleased)
        {
            state->KeyState[(int)@event.Code] = 0;
        }

        foreach (ref readonly var @event in _characterTyped)
        {
            Debug.Assert(state->CharacterCount < InputState.MaxCharacterCount);
            if (state->CharacterCount < InputState.MaxCharacterCount)
            {
                state->Characters[state->CharacterCount++] = @event.Character;
            }
            else
            {
                Logger.Warning<InputSystem>($"The limit for buffered Characters has been reached({InputState.MaxCharacterCount}). Character '{@event.Character}' discarded.");
            }
        }

        if (_windowLostFocus.HasEvents())
        {
            // clear all states
            //Logger.Trace<InputSystem>("Window has lost focus, clearing all keystates (This should be configured at some point)");
            //MemoryUtils.Init(state->KeyState, (int)KeyCode.NumberOfKeys*2); // can use this to clear both current and previous in a single call.

            MemoryUtils.Init(state->KeyState, (int)KeyCode.NumberOfKeys);
            MemoryUtils.Init(state->PreviousKeyState, (int)KeyCode.NumberOfKeys);
        }

        // cant read mouse button state without the message loop.
    }
}
