using Titan.ECS.World;
using Titan.EntitySystem.Components;

namespace Titan.Sandbox
{
    internal class SandboxStartup : IStartup
    {
        public WorldBuilder ConfigureWorld(WorldBuilder builder) =>
            builder
                .WithMaxEntities(10_000)
                .WithComponent<Transform3D>(3000)
                .WithComponent<SandboxComponent>(3000)
                .WithComponent<AssetTEMP<TextureComponent>>(200)
                .WithComponent<AssetTEMP<int>>(200)
                .WithComponent<AssetTEMP<float>>(100)

                .WithSystem<SandboxSystem>()
                .WithSystem<AnotherSandboxSystem>()
            ;

        public void OnStart(IWorld world)
        {
            var entity = world.CreateEntity();
            entity.AddComponent<Transform3D>();
            entity.AddComponent<SandboxComponent>();
            entity.AddManagedComponent(new AssetTEMP<TextureComponent>("teh texture.png"));
        }

        public void OnStop(IWorld world)
        {
        
        }
    }
}
