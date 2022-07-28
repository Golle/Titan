using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.ECS.Events;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.Graphics.Modules;

namespace Titan.Graphics.D3D12;

public struct D3D12RenderModule : IModule
{
    public static void Build(AppBuilder builder)
    {
        ref readonly var window = ref builder.GetResource<Window>();
        if (D3D12Device.CreateAndInit(window.Handle, window.Width, window.Height, true, out var device))
        {
            device.LoadAssets();

            Logger.Info<D3D12RenderModule>($"Created the {nameof(D3D12Device)} with feature level {device.FeatureLevel}!");
            builder
                .AddShutdownSystem<D3D12DeviceTearDown>(RunCriteria.Always)
                .AddResource(new GraphicsDevice { Device = device })
                .AddSystemToStage<D3D12RenderSystem>(Stage.PostUpdate, RunCriteria.Always)
                .AddSystemToStage<D3D12ResizeSystem>(Stage.PreUpdate);
        }
        else
        {
            Logger.Error<D3D12RenderModule>($"Failed to create {nameof(D3D12Device)} :(");
        }
    }
    private struct D3D12RenderSystem : IStructSystem<D3D12RenderSystem>
    {
        private ApiResource<GraphicsDevice> RenderDevice;
        public static void Init(ref D3D12RenderSystem system, in SystemsInitializer init) => system.RenderDevice = init.GetApi<GraphicsDevice>();

        public static void Update(ref D3D12RenderSystem system)
        {
            system.RenderDevice.Get().Device.Render();
        }

        public static bool ShouldRun(in D3D12RenderSystem system)
        {
            throw new System.NotImplementedException();
        }
    }
    private struct D3D12DeviceTearDown : IStructSystem<D3D12DeviceTearDown>
    {
        private ApiResource<GraphicsDevice> Device;
        public static void Init(ref D3D12DeviceTearDown system, in SystemsInitializer init) => system.Device = init.GetApi<GraphicsDevice>();
        public static void Update(ref D3D12DeviceTearDown system)
        {
            Logger.Trace<D3D12DeviceTearDown>("Release D3D12Device");
            system.Device.Get().Device.Release();
        }
        public static bool ShouldRun(in D3D12DeviceTearDown system) => throw new System.NotImplementedException();
    }

    private struct D3D12ResizeSystem : IStructSystem<D3D12ResizeSystem>
    {
        private EventsReader<WindowResizeComplete> WindowResize;
        private ApiResource<GraphicsDevice> Device;
        private ReadOnlyResource<Window> Window;

        public static void Init(ref D3D12ResizeSystem system, in SystemsInitializer init)
        {
            system.WindowResize = init.GetEventsReader<WindowResizeComplete>();
            system.Device = init.GetApi<GraphicsDevice>();
            system.Window = init.GetReadOnlyResource<Window>();
        }

        public static void Update(ref D3D12ResizeSystem system)
        {
            ref readonly var window = ref system.Window.Get();
            var height = window.Height;
            var width = window.Width;
            foreach (ref readonly var @event in system.WindowResize)
            {
                system.Device.Get().Device.Resize(width, height);
                Logger.Error<D3D12ResizeSystem>($"Resize device: {width}x{height}");
            }
        }

        public static bool ShouldRun(in D3D12ResizeSystem system)
            => system.WindowResize.HasEvents();
    }
}
