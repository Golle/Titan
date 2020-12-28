using System;
using Titan.ECS.World;

namespace Titan
{
    public interface IStartup // TODO: maybe need a better name?
    {
        WorldBuilder ConfigureWorld(WorldBuilder builder);
        void OnStart(IWorld world);
        void OnStop(IWorld world);
    }
}
