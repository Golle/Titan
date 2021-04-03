using System.Numerics;
using Titan.ECS.Assets;
using Titan.ECS.World;
using Titan.EntitySystem;
using Titan.EntitySystem.Components;

namespace Titan.Sandbox
{
    internal class SandboxStartup : IStartup
    {
        public WorldBuilder ConfigureWorld(WorldBuilder builder) =>
            builder
                .WithMaxEntities(10_000)
                .WithMaxEvents(1_000)
                
                .WithComponent<SandboxComponent>(3000)
                .WithSystem<FirstPersonCameraSystem>()
            
                //.WithSystem<SandboxSystem>()
                //.WithSystem<AnotherSandboxSystem>()
                //.WithSystem<ThirdSandboxSystem>()
            ;

        public void OnStart(IWorld world)
        {

            //var str = new []{ @"F:\Git\Titan\resources\textures\background.png", @"F:\Git\Titan\resources\textures\chain_texture.png", @"F:\Git\Titan\resources\textures\lion.png", };
            //var models = new []{ @"F:\Git\Titan\resources\models\sphere.dat", @"F:\Git\Titan\resources\models\sponza.dat" };


            //for (var i = 0; i < 2; ++i)
            //{
            //    var entity = world.CreateEntity();
            //    entity.AddComponent<Transform3D>();
            //    //entity.AddComponent<SandboxComponent>();
            //    //entity.AddComponent<Transform2D>();
            //    //entity.AddManagedComponent(new Asset<Texture>(str[0]));
            //    entity.AddManagedComponent(new Asset<MeshComponent>(models[i]));
            //}

     
            var entity = world.CreateEntity();
            entity.AddComponent<Transform3D>();
            entity.AddComponent(CameraComponent.CreatePerspective(2560, 1440, 0.5f, 10000f));

            var meshEntity = world.CreateEntity();
            meshEntity.AddComponent(new Transform3D { Scale = Vector3.One, Rotation = Quaternion.Identity, Position = new Vector3(0, 0, 10) });
            meshEntity.AddManagedComponent(new Asset<MeshComponent>("clock"));
            meshEntity.AddComponent(new UnmanagedAsset<MeshComponent>("clock"));

            var modelEntity = world.CreateEntity();
            modelEntity.AddComponent(new Transform3D {Scale = Vector3.One, Rotation = Quaternion.Identity, Position = new Vector3(0, 0, 10)});
            modelEntity.AddComponent<TEMPModel3D>();

            //entity.AddManagedComponent(new AssetTEMP<TextureComponent>("teh texture.png"));
        }

        public void OnStop(IWorld world)
        {
        
        }
    }
}
