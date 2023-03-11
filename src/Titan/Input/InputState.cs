using System.Numerics;
using Titan.ECS.Components;

namespace Titan.Input;

internal unsafe struct InputState : IResource
{
    public const int MaxCharacterCount = 32;
    public MouseState Current;
    public MouseState Previous;
    public Vector2 DeltaMousePosition;
    public uint CharacterCount;
    public fixed byte KeyState[(int)KeyCode.NumberOfKeys];
    public fixed byte PreviousKeyState[(int)KeyCode.NumberOfKeys];
    public fixed char Characters[MaxCharacterCount];
}
