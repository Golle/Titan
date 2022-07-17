using System;

namespace Titan.ECS.Scheduler;

public struct HeadlessRunner : IRunner
{
    // NOTE(Jens): just a sample, might be able to use something like this for a server.
    public static void Run(ref ECS.Scheduler.Scheduler scheduler, ref World.World world) => throw new NotImplementedException();
}