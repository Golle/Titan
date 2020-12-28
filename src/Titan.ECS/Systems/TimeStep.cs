using System.Runtime.CompilerServices;

namespace Titan.ECS.Systems
{
    public readonly ref struct TimeStep
    {
        public readonly float ElapsedTime;
        public TimeStep(float elapsedTime)
        {
            ElapsedTime = elapsedTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float(in TimeStep timeStep) => timeStep.ElapsedTime;
    }
}
