using Titan.Core.App;
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

    public static SystemDescriptor Create<T>(Stage stage = Stage.Update) where T : unmanaged, IStructSystem<T>
        => new(ResourceId.Id<T>(), (uint)sizeof(T), stage, &FunctionWrapper<T>.Init, &FunctionWrapper<T>.Update, &FunctionWrapper<T>.ShouldRun);

    private SystemDescriptor(ResourceId id, uint size, Stage stage, delegate*<void*, SystemsInitializer, void> init, delegate*<void*, void> update, delegate*<void*, bool> shouldRun)
    {
        Id = id;
        Size = size;
        Stage = stage;
        Init = init;
        Update = update;
        ShouldRun = shouldRun;
    }

    /// <summary>
    /// Helper class to wrap the system functions into function pointers that accept a void*
    /// </summary>
    private struct FunctionWrapper<T> where T : unmanaged, IStructSystem<T>
    {
        public static void Init(void* system, SystemsInitializer init) => T.Init(ref *(T*)system, init);
        public static void Update(void* system) => T.Update(ref *(T*)system);
        public static bool ShouldRun(void* system) => T.ShouldRun(*(T*)system);
    }
}
