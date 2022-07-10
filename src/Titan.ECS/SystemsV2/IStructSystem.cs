namespace Titan.ECS.SystemsV2;

public interface IStructSystem<T> where T : unmanaged
{
    static abstract void Init(ref T system, in SystemsInitializer init);
    static abstract void Update(ref T system);
    static abstract bool ShouldRun(in T system);
}
