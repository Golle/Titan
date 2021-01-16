namespace Titan.ECS.Components
{
    public class EntityFilterConfiguration
    {
        public ref readonly ComponentId Components => ref _components;
        private ComponentId _components;
        public EntityFilterConfiguration With<T>() where T : unmanaged
        {
            _components = _components += ComponentId<T>.Id;
            return this;
        }
    }

}
