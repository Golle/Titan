using System.Runtime.CompilerServices;

namespace Titan.ECS.Worlds;

public readonly struct Timestep
{
    public readonly float Seconds;
    public readonly float MilliSeconds;
    public Timestep(float seconds)
    {
        Seconds = seconds;
        MilliSeconds = seconds / 1000f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Timestep(float time) => new(time);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Timestep(double time) => new((float)time);
}