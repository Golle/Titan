using Titan.Core;
using Titan.ECS.Worlds;

namespace Titan.ECS.Scheduler;

internal unsafe struct Runner : IApi
{
    private delegate*<ref Scheduler, ref World, void> _run;
    public static Runner Create<T>() where T : IRunner =>
        new()
        {
            _run = &T.Run
        };
    public void Run(ref Scheduler scheduler, ref World world) => _run(ref scheduler, ref world);
}
