using Titan.Assets.Creators;
using Titan.Core.Logging;
using Titan.Graphics.D3D12;
using Titan.Graphics.Rendering;
using Titan.Graphics.Resources;
using Titan.Graphics.Vulkan;
using Titan.Modules;
using Titan.Setup;
using Titan.Systems;
using Titan.Windows;

namespace Titan.Graphics;

public struct GraphicsModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddModule<WindowModule>()
            .AddResource(new RenderInfo());

        if (GlobalConfiguration.OperatingSystem == OperatingSystem.Windows)
        {
            Logger.Trace<GraphicsModule>($"Using {nameof(D3D12GraphicsModule)} for rendering.");
            builder
                .AddModule<D3D12GraphicsModule>()
                .AddModule<D3D12RenderingModule>()
                
                .AddSystemToStage<SwapchainSystem>(SystemStage.Last, RunCriteria.Always)
                .AddSystemToStage<RenderInfoSystem>(SystemStage.First) // Collect all render info after the frame has been presented.
                .AddResourceCreator<Texture, TextureCreator>()
                .AddResourceCreator<Shader, ShaderCreator>()
                ;
        }
        else if (GlobalConfiguration.OperatingSystem == OperatingSystem.Linux)
        {
            Logger.Trace<GraphicsModule>($"Using {nameof(VulkanGraphicsModule)} for rendering.");
            builder
                .AddModule<VulkanGraphicsModule>();
        }
        else
        {
            Logger.Error<GraphicsModule>($"Platform {GlobalConfiguration.OperatingSystem} does not support rendering.");
            return false;
        }

        return true;
    }

    public static bool Init(IApp app)
    {
        var config = app.GetConfig<GraphicsConfig>();
        ref var renderInfo = ref app.GetResource<RenderInfo>();
        renderInfo.ClearColor = config.ClearColor;

        Logger.Trace<GraphicsModule>($"Set default clear color to R:{config.ClearColor.R} G:{config.ClearColor.G} B:{config.ClearColor.B} A:{config.ClearColor.A}");

        return true;
    }

    public static bool Shutdown(IApp app)
    {
        return true;
    }
}
