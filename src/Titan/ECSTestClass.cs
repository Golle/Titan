using System.Numerics;
using Titan.ECS;
using Titan.ECS.Entities;
using Titan.ECS.Registry;
using Titan.EntitySystem.Components;

namespace Titan
{
    public class ECSTestClass
    {
        public void Run()
        {
            using var registry = new ComponentRegistry(10_000);
            registry.Register<Transform3D>();



            var world = new ECS.World.World(registry);

            var entites = new Entity[100];
            for (var i = 0; i < entites.Length; ++i)
            {
                entites[i] = world.CreateEntity();
                if (i % 2 == 0)
                {
                    entites[i].AddComponent<Transform3D>();
                }
                else
                {
                    entites[i].AddComponent(new Transform3D());
                }
            }

            for (var i = 0; i < entites.Length / 2; i += 2)
            {
                entites[i].RemoveComponent<Transform3D>();
            }

            //var child1 = entites[1].CreateChildEntity();

            //entites[2].Attach(entites[4]);
            //entites[4].Attach(entites[7]);

            //entites[2].Detach();
            //entites[4].Detach();
            //entites[4].Detach();
            //entites[4].Attach(entites[2]);

            //ref var transform1 = ref pool[entites[4]];
            //transform1.Position = new Vector3(20, 30, 40);
            //ref var transform2 = ref pool[entites[2]];
            //transform2.Position = new Vector3(202, 302, 402);



            //for (var i = 0; i < entites.Length; ++i)
            //{
            //    ref var entity = ref entites[i];
            //    Console.WriteLine(pool[entity].Position);

            //    entity.Destroy();
            //}
            //child1.Destroy();
        }
    }
}
