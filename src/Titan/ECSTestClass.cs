using Titan.ECS.Entities;

namespace Titan
{
    public class ECSTestClass
    {
        public void Run()
        {
            var world = new Titan.ECS.World.World();

            var entites = new Entity[10];
            for (var i = 0; i < entites.Length; ++i)
            {
                entites[i] = world.CreateEntity();
            }

            var child1 = entites[1].CreateChildEntity();

            //entites[2].Attach(entites[4]);
            //entites[4].Attach(entites[7]);

            //entites[2].Detach();
            //entites[4].Detach();
            //entites[4].Detach();
            //entites[4].Attach(entites[2]);

            for (var i = 0; i < entites.Length; ++i)
            {
                ref var entity = ref entites[i];

                entity.Destroy();
            }
            child1.Destroy();
        }
    }
}
