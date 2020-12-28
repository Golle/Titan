namespace Titan.Core
{
    public readonly ref struct TimeStep
    {
        public readonly float DeltaTime;
        public readonly float DeltaTimeSeconds;
        public TimeStep(in float deltaTime, in float deltaTimeSeconds)
        {
            DeltaTime = deltaTime;
            DeltaTimeSeconds = deltaTimeSeconds;
        }
    }
}
