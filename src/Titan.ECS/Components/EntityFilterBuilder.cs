namespace Titan.ECS.Components
{
    public class EntityFilterConfiguration
    {
        public ref readonly ComponentMask ComponentMask => ref _mask;
        private ComponentMask _mask;
        public EntityFilterConfiguration With<T>() where T : unmanaged
        {
            _mask = _mask.Add(ComponentId<T>.Id);
            return this;
        }
    }

}
