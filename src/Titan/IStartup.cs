using Titan.ECS.Systems;
using Titan.ECS.World;

namespace Titan
{
    public interface IStartup // TODO: maybe need a better name?
    {
        WorldBuilder ConfigureWorld(WorldBuilder builder);

        SystemsBuilder ConfigureSystems(SystemsBuilder builder);
        void OnStart(IWorld world);
        void OnStop(IWorld world);
    }
}
