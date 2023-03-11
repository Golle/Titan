using Titan.Resources;

namespace Titan.Systems;
file unsafe struct UnmanagedSystemWrapper<T> where T : unmanaged, ISystem
{
    public static void Init(void* system, in SystemInitializer init) => ((T*)system)->Init(init);
    public static void Update(void* system) => ((T*)system)->Update();
    public static bool ShouldRun(void* system) => ((T*)system)->ShouldRun();
    public static void Shutdown(void* system) => ((T*)system)->Shutdown();
}
internal unsafe struct SystemDescriptor
{
    public ResourceId Id;
    public uint Size;
    public RunCriteria Criteria;
    public SystemStage Stage;
    public int Priority;
    public delegate*<void*, in SystemInitializer, void> Init;
    public delegate*<void*, void> Update;
    public delegate*<void*, bool> ShouldRun;
    public delegate*<void*, void> Shutdown;

    public static SystemDescriptor Create<T>(SystemStage stage, RunCriteria criteria, int priority) where T : unmanaged, ISystem =>
        new()
        {
            Id = ResourceId.Id<T>(),
            Size = (uint)sizeof(T),
            Criteria = criteria,
            Stage = stage,
            Priority = priority,
            Init = &UnmanagedSystemWrapper<T>.Init,
            ShouldRun = &UnmanagedSystemWrapper<T>.ShouldRun,
            Shutdown = &UnmanagedSystemWrapper<T>.Shutdown,
            Update = &UnmanagedSystemWrapper<T>.Update,
        };
}
