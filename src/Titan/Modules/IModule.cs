using Titan.Setup;

namespace Titan.Modules;
public record struct ModuleLifetime(Type Type, Func<IApp, bool> Init, Func<IApp, bool> Shutdown);
public interface IModule
{
    static abstract bool Build(IAppBuilder builder);
    static abstract bool Init(IApp app);
    static abstract bool Shutdown(IApp app);
}

