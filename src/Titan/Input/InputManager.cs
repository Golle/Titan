using System.Numerics;
using System.Runtime.CompilerServices;

namespace Titan.Input;

public readonly unsafe struct InputManager
{
    private readonly InputState* _state;
    internal InputManager(InputState* state) => _state = state;

    public ref readonly Vector2 MousePosition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _state->Current.Position;
    }

    public ref readonly Vector2 PreviousMousePosition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _state->Previous.Position;
    }

    public ref readonly Vector2 DeltaMousePosition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _state->DeltaMousePosition;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasCharacters() => _state->CharacterCount > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> GetCharacters() => new(_state->Characters, (int)_state->CharacterCount);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsKeyDown(KeyCode code) => _state->KeyState[(int)code] != 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsKeyUp(KeyCode code) => _state->KeyState[(int)code] == 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsKeyPressed(KeyCode code) => _state->KeyState[(int)code] != 0 && _state->PreviousKeyState[(int)code] == 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsKeyReleased(KeyCode code) => _state->KeyState[(int)code] == 0 && _state->PreviousKeyState[(int)code] != 0;
}
