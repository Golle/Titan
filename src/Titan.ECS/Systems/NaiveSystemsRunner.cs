using Titan.ECS.World;

namespace Titan.ECS.Systems
{
    internal class NaiveSystemsRunner : ISystemsRunner
    {
        private readonly IEntityFilterManager _entityFilterManager;
        private IEntitySystem[] _systems = new IEntitySystem[100];

        private int _numberOfSystems;


        public NaiveSystemsRunner(IEntityFilterManager entityFilterManager)
        {
            _entityFilterManager = entityFilterManager;
        }

        public void Update(in TimeStep timestep)
        {
            // Update the filters, listen for component messages etc.
            //_entityFilterManager.Update();

            // TODO: these steps should be run in different threads
            for (var i = 0; i < _numberOfSystems; ++i)
            {
                _systems[i].OnPreUpdate();
            }
            for (var i = 0; i < _systems.Length; ++i)
            {
                _systems[i].OnUpdate(timestep);
            }
            for (var i = 0; i < _systems.Length; ++i)
            {
                _systems[i].OnPostUpdate();
            }
        }
    }
}
