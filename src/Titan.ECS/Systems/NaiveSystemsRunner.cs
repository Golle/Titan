using System.Collections.Generic;

namespace Titan.ECS.Systems
{
    internal class NaiveSystemsRunner : ISystemsRunner
    {
        private readonly IList<IEntitySystem> _systems = new List<IEntitySystem>();

        public NaiveSystemsRunner()
        {
            
        }
        public void Update(in TimeStep timestep)
        {
            // Update the filters, listen for component messages etc.
            //_entityFilterManager.Update();

            // TODO: these steps should be run in different threads
            foreach (var entitySystem in _systems)
            {
                entitySystem.OnPreUpdate();
                entitySystem.OnUpdate(timestep);
                entitySystem.OnPostUpdate();
            }
        }
    }
}
