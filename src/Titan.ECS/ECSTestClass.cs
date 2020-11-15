namespace Titan.ECS
{
    public class ECSTestClass
    {
        public void Run()
        {
            var world = new World.World();

            var entites = new Entity[10];
            for (var i = 0; i < entites.Length; ++i)
            {
                entites[i] = world.CreateEntity();
            }

            for (var i = 0; i < entites.Length; ++i)
            {
                ref var entity = ref entites[i];

                entity.Destroy();
            }
        }
    }
}
