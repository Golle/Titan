using System.Runtime.InteropServices;
using Titan.Core;
using Titan.ECS.App;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.Windows.D3D11;

namespace Titan.Graphics;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct RenderDevice : IResource
{
    [FieldOffset(0)]
    public RenderAPI API;
    [FieldOffset(sizeof(RenderAPI))]
    public ID3D11Device* D3DDevice;

    // NOTE(Jens): not implemented, just for show :)
    [FieldOffset(sizeof(RenderAPI))]
    public uint* VulkanDevice;
}

public enum RenderAPI
{
    D3D11,
    D3D12,
    Vulkan
}
public struct RenderModule : IModule
{
    public static unsafe void Build(AppBuilder builder)
    {
        var devicePtr = (ID3D11Device*)NativeMemory.Alloc(1000);
        builder.AddResource(new RenderDevice
        {
            API = RenderAPI.D3D11,
            D3DDevice = devicePtr
        });
        builder.AddSystemToStage<RenderDeviceTeardown>(Stage.Shutdown);
    }

    private struct RenderDeviceTeardown : IStructSystem<RenderDeviceTeardown>
    {
        private MutableResource<RenderDevice> _renderDevice;
        public static void Init(ref RenderDeviceTeardown system, in SystemsInitializer init) => system._renderDevice = init.GetMutableResource<RenderDevice>();
        public static unsafe void Update(ref RenderDeviceTeardown system) => NativeMemory.Free(system._renderDevice.Get().D3DDevice);
        public static bool ShouldRun(in RenderDeviceTeardown system) => true;
    }
}