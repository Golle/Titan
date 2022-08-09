using System;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;

namespace Titan.Graphics.D3D12Take2.Systems;

internal struct D3D12TearDownSystem : IStructSystem<D3D12TearDownSystem>
{
    private MutableResource<D3D12Core> Core;

    public static void Init(ref D3D12TearDownSystem system, in SystemsInitializer init) => system.Core = init.GetMutableResource<D3D12Core>();

    public static void Update(ref D3D12TearDownSystem system)
    {
        ref var core = ref system.Core.Get();

        core.Surface.Shutdown();
        core.Command.Shutdown();
        core.Queue.Shutdown();
        core.Device.Shutdown();
        core.Adapter.Shutdown();

        core = default;
    }

    public static bool ShouldRun(in D3D12TearDownSystem system) => throw new NotImplementedException("Should be marked as always");
}
