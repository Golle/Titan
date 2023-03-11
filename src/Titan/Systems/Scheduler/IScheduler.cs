using Titan.Jobs;

namespace Titan.Systems.Scheduler;
public interface IScheduler
{
    void OnUpdate(IJobApi jobApi);
}
