using Titan.Core;
using Titan.Core.App;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.Modules;
using Titan.ECS.SystemsV2;

namespace Titan.ECS.AnotherTry;

public struct CoreModule : IModule2
{
    public static void Build(AppBuilder app) =>
        app
            .AddModule<MemoryModule>()
            .AddModule<LoggingModule>()
            .AddModule<ThreadingModule>()
            .AddModule<ECSModule>()
            .AddModule<SchedulerModule>();
}

public struct EntityConfiguration : IDefault<EntityConfiguration>
{
    private const uint DefaultMaxEntities = 10_000;
    public uint MaxEntities;

    public static EntityConfiguration Default
        => new()
        {
            MaxEntities = DefaultMaxEntities
        };
}

public struct ECSModule : IModule2
{
    public static void Build(AppBuilder builder)
    {
        ref readonly var config = ref builder.GetResourceOrDefault<EntityConfiguration>();

        Logger.Trace<ECSModule>($"MaxEntities: {config.MaxEntities}");
        // Create entity manager?
    }
}


public struct MemoryConfiguration : IDefault<MemoryConfiguration>
{
    private const uint DefaultTransientMemoryPool = 128 * 1024 * 1024;
    public uint TransientMemory;
    public static MemoryConfiguration Default =>
        new()
        {
            TransientMemory = DefaultTransientMemoryPool
        };
}

public unsafe struct MemoryModule : IModule2
{
    // NOTE(Jens): This will only add support for a transient memory allocation (resets every frame). We want a permanent memory allocator and a "buffer" where we can borrow/return memory blocks.
    public static void Build(AppBuilder builder)
    {
        var config = builder.GetResourceOrDefault<MemoryConfiguration>();
        if (config.TransientMemory > 0)
        {
            Logger.Trace<MemoryModule>($"Creating transient {nameof(MemoryAllocator)} with size {config.TransientMemory} bytes.");
            var pool = builder.GetResourcePointer<MemoryPool>();
            var allocator = pool->CreateAllocator<MemoryAllocator>(config.TransientMemory);

            builder
                .AddResource(allocator)
                .AddSystemToStage<TransientMemorySystem>(Stage.PreUpdate);
        }
    }


    private struct TransientMemorySystem : IStructSystem<TransientMemorySystem>
    {
        private ApiResource<MemoryAllocator> _memory;
        public static void Init(ref TransientMemorySystem system, in SystemsInitializer init) => system._memory = init.GetApi<MemoryAllocator>();
        public static void Update(ref TransientMemorySystem system) => system._memory.Get().Reset();
        public static bool ShouldRun(in TransientMemorySystem system) => true;
    }
}
