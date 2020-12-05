namespace Titan.ECS.Entities
{
    public interface IEntityFactory
    {
        Entity Create();
        void Update();
    }
}
