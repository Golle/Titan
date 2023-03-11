using Titan.Modules;
using Titan.Setup;
using Titan.Systems;

namespace Titan.Input;

public struct InputModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddResource(new InputState())
            .AddSystemToStage<InputSystem>(SystemStage.PreUpdate, RunCriteria.Always)
            ;

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
