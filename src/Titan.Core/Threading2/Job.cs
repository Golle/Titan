namespace Titan.Core.Threading2;

internal unsafe struct Job
{
    public JobItem JobItem;
    public volatile int State; //JobState enum
    public void Execute() => JobItem.Function(JobItem.Context);
}
