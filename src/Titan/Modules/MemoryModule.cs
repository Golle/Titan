using System.Reflection.Metadata.Ecma335;
using Titan.ECS.App;
using Titan.ECS.Memory;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.Memory;
using Titan.Memory.Arenas;

namespace Titan.Modules;

public unsafe struct MemoryModule : IModule
{
    // NOTE(Jens): This will only add support for a transient memory allocation (resets every frame). We want a permanent memory allocator and a "buffer" where we can borrow/return memory blocks.
    public static bool Build(AppBuilder builder)
    {
        var config = builder.GetResourceOrDefault<MemoryConfiguration>();

        if (config.TransientMemory > 0)
        {
            ref var allocator = ref builder.GetResource<PlatformAllocator>();
            var arena = FixedSizeLinearArena.Create(allocator.Allocate(config.TransientMemory), config.TransientMemory);

            builder
                .AddResource(new TransientMemory(arena))
                .AddSystemToStage<TransientMemorySystem>(Stage.First, RunCriteria.Always);
        }
        return true;
    }


    private struct TransientMemorySystem : IStructSystem<TransientMemorySystem>
    {
        private ApiResource<TransientMemory> _memory;
        public static void Init(ref TransientMemorySystem system, in SystemsInitializer init) => system._memory = init.GetApi<TransientMemory>();
        public static void Update(ref TransientMemorySystem system) => system._memory.Get().Reset();
        public static bool ShouldRun(in TransientMemorySystem system) => true;
    }
}
