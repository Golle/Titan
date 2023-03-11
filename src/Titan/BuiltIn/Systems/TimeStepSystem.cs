using Titan.BuiltIn.Resources;
using Titan.Systems;

namespace Titan.BuiltIn.Systems;

internal struct TimeStepSystem : ISystem
{
    private MutableResource<TimeStep> _timeStep;
    private DateTime _lastFrame;
    public void Init(in SystemInitializer init)
    {
        _timeStep = init.GetMutableResource<TimeStep>();
        _lastFrame = DateTime.MinValue;
    }

    public void Update()
    {
        var current = DateTime.Now;
        ref var timeStep = ref _timeStep.Get();

        if (_lastFrame == DateTime.MinValue)
        {
            //NOTE(Jens): first iteration will set delta to 0. which is fine.
            _lastFrame = current;
        }
        var delta = current - _lastFrame;
        timeStep.DeltaTimeSeconds = delta.TotalSeconds;
        timeStep.DeltaTimeSecondsF = (float)delta.TotalSeconds;
        timeStep.DeltaTimeMS = delta.TotalMilliseconds;
        timeStep.DeltaTimeMSF = (float)delta.TotalMilliseconds;
        timeStep.FrameCount++;
        
        _lastFrame = current;
    }

    public bool ShouldRun()
    {
        //NOTE(Jens): add a check if the game is not in focus?
        return true;
    }
}