namespace Titan.ECS.Systems
{
    internal interface ISystemsRunner
    {
        void Update(in TimeStep timeStep);
    }
}
