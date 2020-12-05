namespace Titan.ECS.Systems
{
    public readonly ref struct TimeStep
    {
        public readonly float ElapsedTime;
        public TimeStep(float elapsedTime)
        {
            ElapsedTime = elapsedTime;
        }
    }
}
