using Titan.Graphics.Resources;
using Titan.Modules;
using Titan.Setup;

namespace Titan.Graphics.Vulkan;

internal struct VulkanGraphicsModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddManagedResource<IResourceManager>(new VulkanResourceManager());

        return true;
    }

    public static bool Init(IApp app)
    {
        return true;
    }

    public static bool Shutdown(IApp app)
    {
        return true;
    }
}
