using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.ECS.Events;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.Graphics.Modules;
using Titan.Memory;

namespace Titan.Graphics.D3D12;

public struct D3D12RenderModule : IModule
{
    public static bool Build(AppBuilder builder)
    {
        ref readonly var window = ref builder.GetResource<Window>();
        if (!D3D12Device.CreateAndInit(window.Handle, window.Width, window.Height, true, out var device))
        {
            Logger.Error<D3D12RenderModule>($"Failed to create {nameof(D3D12Device)} :(");
            return false;
        }
         
        Logger.Info<D3D12RenderModule>($"Created the {nameof(D3D12Device)} with feature level {device.FeatureLevel}!");

        ref readonly var allocator = ref builder.GetResource<PlatformAllocator>();
        if (!D3D12RenderContext.CreateAndInit(allocator, device, out var context))
        {
            Logger.Error<D3D12RenderModule>($"Failed to create the {nameof(D3D12RenderContext)}. ");
            return false;
        }
        builder
            .AddShutdownSystem<D3D12DeviceTearDown>(RunCriteria.Always)
            .AddResource(new GraphicsDevice { Device = device })
            .AddResource(new RenderContext { Context = context })
            .AddSystemToStage<D3D12RenderSystem>(Stage.PostUpdate, RunCriteria.Always)
            .AddSystemToStage<D3D12ResizeSystem>(Stage.PreUpdate);
        return true;
    }
    private struct D3D12RenderSystem : IStructSystem<D3D12RenderSystem>
    {
        private ApiResource<RenderContext> RenderContext;
        private ReadOnlyResource<Window> Window;

        public static void Init(ref D3D12RenderSystem system, in SystemsInitializer init)
        {
            system.RenderContext = init.GetApi<RenderContext>();
            system.Window = init.GetReadOnlyResource<Window>();
        }

        public static void Update(ref D3D12RenderSystem system)
        {
            ref var context = ref system.RenderContext.Get().Context;
            ref readonly var window = ref system.Window.Get();
            context.BeginFrame();

            //var commandList = context.CreateCommandList();
            //unsafe
            //{
            //    var viewport = new D3D12_VIEWPORT
            //    {
            //        Width = window.Width,
            //        Height = window.Height,
            //        MinDepth = 0,
            //        MaxDepth = 1.0f,
            //        TopLeftX = 0,
            //        TopLeftY = 0
            //    };
            //    var scissorRect = new D3D12_RECT
            //    {
            //        Left = 0,
            //        Right = (int)window.Width,
            //        Top = 0,
            //        Bottom = (int)window.Height
            //    };
            //    var internalCmdList = commandList._commandList;
            //    internalCmdList->RSSetViewports(1, &viewport);
            //    internalCmdList->RSSetScissorRects(1, (D3D12_RECT*)Unsafe.AsPointer(ref scissorRect));
            //    //internalCmdList->ClearRenderTargetView();
            //}


            context.EndFrame();

        }

        public static bool ShouldRun(in D3D12RenderSystem system) => throw new System.NotImplementedException("This system should ba marked at AlwaysRun");
    }

    private struct D3D12DeviceTearDown : IStructSystem<D3D12DeviceTearDown>
    {
        private ApiResource<GraphicsDevice> Device;
        private ApiResource<RenderContext> Context;

        public static void Init(ref D3D12DeviceTearDown system, in SystemsInitializer init)
        {
            system.Context = init.GetApi<RenderContext>();
            system.Device = init.GetApi<GraphicsDevice>();
        }

        public static void Update(ref D3D12DeviceTearDown system)
        {
            Logger.Trace<D3D12DeviceTearDown>("Release D3D12RenderContext");
            system.Context.Get().Context.Release();
            Logger.Trace<D3D12DeviceTearDown>("Release D3D12Device");
            system.Device.Get().Device.Release();

        }
        public static bool ShouldRun(in D3D12DeviceTearDown system) => throw new System.NotImplementedException("This system should ba marked at AlwaysRun");
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
            foreach (ref readonly var _ in system.WindowResize)
            {
                system.Device.Get().Device.Resize(width, height);
                Logger.Error<D3D12ResizeSystem>($"Resize device: {width}x{height}");
            }
        }

        public static bool ShouldRun(in D3D12ResizeSystem system)
            => system.WindowResize.HasEvents();
    }
}
