using System.Runtime.CompilerServices;
using Titan.Core;

namespace Titan.Input.Modules;

public unsafe struct KeyboardState : IResource
{
    internal fixed bool Current[(int)KeyCode.NumberOfKeys];
    internal fixed bool Previous[(int)KeyCode.NumberOfKeys];

    /// <summary>
    /// Check the current state is Down
    /// </summary>
    /// <param name="code">The keycode</param>
    /// <returns>Bool state of the key</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsKeyDown(KeyCode code) => Current[(int)code];

    /// <summary>
    /// Check the current state is Up
    /// </summary>
    /// <param name="code">The keycode</param>
    /// <returns>Bool state of the key</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsKeyUp(KeyCode code) => !Current[(int)code];

    /// <summary>
    /// The same frame as the key release happens
    /// </summary>
    /// <param name="code">The keycode</param>
    /// <returns>Bool state of the key</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsKeyReleased(KeyCode code) => Previous[(int)code] && !Current[(int)code];

    /// <summary>
    /// The same frame as the key press happens
    /// </summary>
    /// <param name="code">The keycode</param>
    /// <returns>Bool state of the key</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsKeyPressed(KeyCode code) => Current[(int)code] && !Previous[(int)code];
}
