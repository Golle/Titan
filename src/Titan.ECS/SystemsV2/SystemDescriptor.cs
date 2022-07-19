using Titan.Core.Logging;
using Titan.ECS.TheNew;

namespace Titan.ECS.SystemsV2;

internal readonly unsafe struct SystemDescriptor
{
    public readonly delegate*<void*, SystemsInitializer, void> Init;
    public readonly delegate*<void*, void> Update;
    public readonly delegate*<void*, bool> ShouldRun;

    public readonly uint Size;
    public readonly Stage Stage;
    public readonly ResourceId Id;
    public readonly RunCriteria Criteria;
    public readonly int Priority;

    public static SystemDescriptor Create<T>(Stage stage = Stage.Update, RunCriteria criteria = RunCriteria.Always, int priority = 0) where T : unmanaged, IStructSystem<T>
        => new(ResourceId.Id<T>(), criteria, (uint)sizeof(T), stage, &FunctionWrapper<T>.Init, &FunctionWrapper<T>.Update, &FunctionWrapper<T>.ShouldRun, priority);

    private SystemDescriptor(ResourceId id, RunCriteria criteria, uint size, Stage stage, delegate*<void*, SystemsInitializer, void> init, delegate*<void*, void> update, delegate*<void*, bool> shouldRun, int priority)
    {
        Id = id;
        Size = size;
        Stage = stage;
        Criteria = criteria;
        Init = init;
        Update = update;
        ShouldRun = shouldRun;
        Priority = priority;
    }

    /// <summary>
    /// Helper class to wrap the system functions into function pointers that accept a void*
    /// </summary>
    private struct FunctionWrapper<T> where T : unmanaged, IStructSystem<T>
    {
        // NOTE(Jens): this is a sample of how we can wrap the function pointers with things that record stats like execution time etc.
#if DEBUG
        public static void Init(void* system, SystemsInitializer init)
        {
            //Logger.Trace<T>("Init System");
            T.Init(ref *(T*)system, init);
        }
#else
        public static void Init(void* system, SystemsInitializer init) => T.Init(ref *(T*)system, init);
#endif
        public static void Update(void* system) => T.Update(ref *(T*)system);
        public static bool ShouldRun(void* system) => T.ShouldRun(*(T*)system);
    }
}
