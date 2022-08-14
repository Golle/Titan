using Titan.ECS.Scheduler;
using Titan.ECS.Systems;

namespace Titan.Graphics.D3D12Take2.Systems;

internal struct BeginFrameSystem : IStructSystem<BeginFrameSystem>
{
    private MutableResource<D3D12Core> Core;

    public static void Init(ref BeginFrameSystem system, in SystemsInitializer init)
    {
        system.Core = init.GetMutableResource<D3D12Core>();
    }

    public static void Update(ref BeginFrameSystem system)
    {
        system.Core.Get().Command.BeginFrame();
    }

    public static bool ShouldRun(in BeginFrameSystem system) => throw new System.NotImplementedException("This should be marked as always");
}