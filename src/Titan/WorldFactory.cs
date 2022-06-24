using Titan.ECS;
using Titan.ECS.TheNew;

namespace Titan;

internal static class WorldFactory
{
    public static World_ Create(WorldConfiguration config, SystemsConfiguration systems)
    {
        var nodes = DispatchTreeFactory.Construct(config.Systems, systems);

        var dispatcher = new SystemsDispatcher_(nodes);

        return new World_(config.Name, dispatcher);
    }
}
