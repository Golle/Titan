using System.Runtime.CompilerServices;
using Titan.Jobs;

namespace Titan.Systems.Scheduler.Executors;

internal struct ReversedSequentialExecutor : ISystemsExecutor
{
    [SkipLocalsInit]
    public static unsafe void Execute(IJobApi jobApi, SystemNode* nodes, int count)
    {
        if (count == 0)
        {
            return;
        }

        for (var i = count - 1; i >= 0; --i)
        {
            ref readonly var node = ref nodes[i];
            var shouldRun = node.Criteria switch
            {
                RunCriteria.Always or RunCriteria.AlwaysInline => true,
                RunCriteria.Check or RunCriteria.CheckInline => node.ShouldRun(),
                RunCriteria.Once or _ => throw new NotImplementedException($"{nameof(RunCriteria)}.{nameof(node.Criteria)} has not been implemented yet. Use Check or Always.")
            };
            if (shouldRun)
            {
                node.Update();
            }
        }
    }
}
