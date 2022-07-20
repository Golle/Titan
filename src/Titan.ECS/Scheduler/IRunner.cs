namespace Titan.ECS.Scheduler;

public interface IRunner
{
    static abstract void Run(ref ECS.Scheduler.Scheduler scheduler, ref World.World world);
}