namespace Titan.Systems;

public interface ISystem
{
    void Init(in SystemInitializer init);
    void Update();
    bool ShouldRun() => true;
    void Shutdown() { }
}

public enum RunCriteria
{
    Check = 0,
    Always = 1,
    Once = 2,


    AlwaysInline = 3,   // AlwaysInline will skip the scheduling step and run the update inline. This should only be used if you know what you're doing.
    CheckInline = 4     // CheckInline will skip the scheduling step and run the update inline. This should only be used if you know what you're doing.
}

