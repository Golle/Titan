using System.Runtime.CompilerServices;
using Titan.ECS.Scheduler;

namespace Titan.ECS.Systems;

internal struct SystemWrapper<T> : IStructSystem<T> where T : unmanaged, ISystem
{
    public static void Init(ref T system, in SystemsInitializer init) => system.Init(init);
    public static void Update(ref T system) => system.Update();
    public static bool ShouldRun(in T system) => Unsafe.AsRef(system).ShouldRun();
}
