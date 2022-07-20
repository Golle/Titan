using Titan.ECS.Scheduler;

namespace Titan.ECS.Systems;


//NOTE(Jens): Not sure this is a good idea, might create extra calls ? The calls might be inlined, but it needs additional research.
public interface ISystem
{
    void Init(in SystemsInitializer init);
    void Update();
    bool ShouldRun();
}
