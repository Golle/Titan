using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS;
using Titan.ECS.App;
using Titan.ECS.SystemsV2;

namespace Titan.Modules;

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
