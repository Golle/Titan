using System.Runtime.InteropServices.ComTypes;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.Graphics.D3D12Take2.Stats;

namespace Titan.Graphics.D3D12Take2.Systems;

internal struct SwapChainPresentSystem : IStructSystem<SwapChainPresentSystem>
{
    private MutableResource<D3D12Core> Resources;
    private MutableResource<RenderData> RenderData;

    public static void Init(ref SwapChainPresentSystem system, in SystemsInitializer init)
    {
        system.Resources = init.GetMutableResource<D3D12Core>(false);
        system.RenderData = init.GetMutableResource<RenderData>(false);
    }

    public static void Update(ref SwapChainPresentSystem system)
    {
        ref var surface = ref system.Resources.Get().Surface;
        ref var command = ref system.Resources.Get().Command;
        ref var renderData = ref system.RenderData.Get();

        command.EndFrame();
        surface.Present();
        renderData.FrameIndex = surface.FrameIndex;
        renderData.FrameCount++;
    }
    public static bool ShouldRun(in SwapChainPresentSystem system) => throw new System.NotImplementedException("This should be marked as always");
}
