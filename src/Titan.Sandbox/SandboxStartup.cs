using Titan.ECS.Assets;
using Titan.ECS.World;
using Titan.EntitySystem.Components;
using Titan.Graphics.Textures;

namespace Titan.Sandbox
{
    internal class SandboxStartup : IStartup
    {
        public WorldBuilder ConfigureWorld(WorldBuilder builder) =>
            builder
                .WithMaxEntities(10_000)
                .WithMaxEvents(100_000)
                
                .WithComponent<SandboxComponent>(3000)
            
                .WithSystem<SandboxSystem>()
                .WithSystem<AnotherSandboxSystem>()
                .WithSystem<ThirdSandboxSystem>()
            ;

        public void OnStart(IWorld world)
        {

            var str = new []{ @"F:\Git\Titan\resources\textures\background.png", @"F:\Git\Titan\resources\textures\chain_texture.png", @"F:\Git\Titan\resources\textures\lion.png", };


            //for (var i = 0; i < ; ++i)
            {
                var entity = world.CreateEntity();
                entity.AddComponent<Transform3D>();
                //entity.AddComponent<SandboxComponent>();
                //entity.AddComponent<Transform2D>();
                entity.AddManagedComponent(new Asset<Texture>(str[0]));
            }


            //entity.AddManagedComponent(new AssetTEMP<TextureComponent>("teh texture.png"));
        }

        public void OnStop(IWorld world)
        {
        
        }
    }
}
