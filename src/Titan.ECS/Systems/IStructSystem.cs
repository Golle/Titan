using Titan.ECS.Scheduler;

namespace Titan.ECS.Systems;

public interface IStructSystem<T> where T : unmanaged
{
    static abstract void Init(ref T system, in SystemsInitializer init);
    static abstract void Update(ref T system);
    static abstract bool ShouldRun(in T system);

    //NOTE(Jens): We might want OnCreate, OnTearDown, OnStateSwitch ? to be able to handle lifetimes of objects. For example if you allocate temporary memory, you need to free it at some point.
}
