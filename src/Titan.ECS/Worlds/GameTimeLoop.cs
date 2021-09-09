namespace Titan.ECS.Worlds
{
    internal readonly struct GameTimeLoop
    {
        public readonly Timestep DeltaTime;
        public readonly Timestep FixedUpdateDeltaTime;
        public readonly int FixedUpdateCalls;
        public GameTimeLoop(in Timestep deltaTime, in Timestep fixedUpdateDeltaTime, int fixedUpdateCalls)
        {
            DeltaTime = deltaTime;
            FixedUpdateDeltaTime = fixedUpdateDeltaTime;
            FixedUpdateCalls = fixedUpdateCalls;
        }
    }
}
