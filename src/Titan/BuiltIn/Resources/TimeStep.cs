using Titan.ECS.Components;

namespace Titan.BuiltIn.Resources;

public struct TimeStep : IResource
{
    public float DeltaTimeSecondsF;
    public float DeltaTimeMSF;
    public double DeltaTimeSeconds;
    public double DeltaTimeMS;

    public ulong FrameCount;
}