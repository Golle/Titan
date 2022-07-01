using Titan.Core.App;

namespace Titan.ECS.SystemsV2;

internal readonly unsafe struct SystemDescriptor
{
    public readonly delegate*<void*, ISystemsInitializer, void> Init;
    public readonly delegate*<void*, void> Update;
    public readonly uint Size;
    public readonly Stage Stage;
    public static SystemDescriptor Create<T>(Stage stage = Stage.Update) where T : unmanaged, IStructSystem<T>
        => new((uint)sizeof(T), stage, &FunctionWrapper<T>.Init, &FunctionWrapper<T>.Update);

    private SystemDescriptor(uint size, Stage stage, delegate*<void*, ISystemsInitializer, void> init, delegate*<void*, void> update)
    {
        Size = size;
        Stage = stage;
        Init = init;
        Update = update;
    }

    /// <summary>
    /// Helper class to wrap the system functions into function pointers that accept a void*
    /// </summary>
    private struct FunctionWrapper<T> where T : unmanaged, IStructSystem<T>
    {
        public static void Init(void* system, ISystemsInitializer init) => T.Init(ref *(T*)system, init);
        public static void Update(void* system) => T.Update(*(T*)system);
    }
}
