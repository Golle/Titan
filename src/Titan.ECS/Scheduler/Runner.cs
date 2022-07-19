using Titan.Core;

namespace Titan.ECS.Scheduler;

internal unsafe struct Runner : IApi
{
    private delegate*<ref ECS.Scheduler.Scheduler, ref World.World, void> _run;
    public static Runner Create<T>() where T : IRunner =>
        new()
        {
            _run = &T.Run
        };
    public void Run(ref ECS.Scheduler.Scheduler scheduler, ref World.World world) => _run(ref scheduler, ref world);
}