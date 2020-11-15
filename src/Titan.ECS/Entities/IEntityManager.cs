namespace Titan.ECS.Entities
{
    public interface IEntityManager
    {
        Entity Create();
        void Destroy(in Entity entity);
    }
}