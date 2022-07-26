using System;
using Titan.ECS.Worlds;

namespace Titan.ECS.Scheduler;

public struct HeadlessRunner : IRunner
{
    // NOTE(Jens): just a sample, might be able to use something like this for a server.
    public static void Run(ref ECS.Scheduler.Scheduler scheduler, ref World world) => throw new NotImplementedException();
}