using System;
using System.Diagnostics;
using Titan.Core.Logging;

namespace Titan.ECS.Worlds;

internal class GameTime
{
    private const float MaxDeltaTime = 1.0f/60f; // 16.7ms // TODO: what should we do with this? if the framerate is less than 60fps the game will run at a slower speed since it will clamp it.
    private readonly Stopwatch _time;
    private double _previousFrame;
    private double _fixedTime;
    private GameTimeLoop _timeStep;
    public float FixedTimeStepFrequency { get; }
    public ref readonly GameTimeLoop Current => ref _timeStep;

    public GameTime(WorldConfigurationOld config)
    {
        if (config.FixedTimeStep is <= 0f or > 10f)
        {
            throw new InvalidOperationException("The fixed timestep can't be less than 0 or greater than 10.");
        }

        FixedTimeStepFrequency = config.FixedTimeStep;
        _time = Stopwatch.StartNew();
        _previousFrame = _time.Elapsed.TotalSeconds;
    }

    internal void Update()
    {
        var current = _time.Elapsed.TotalSeconds;
        var delta = current - _previousFrame;
        if (delta > MaxDeltaTime)
        {
            //Logger.Warning<GameTime>($"Delta time is greater than {MaxDeltaTime}, clamping the value to {MaxDeltaTime}.");
            delta = MaxDeltaTime;
        }
        _fixedTime += delta;
        var fixedUpdateCount = 0;
        while (_fixedTime > FixedTimeStepFrequency)
        {
            _fixedTime -= FixedTimeStepFrequency;
            fixedUpdateCount += 1;
        }

        _timeStep = new GameTimeLoop(delta, FixedTimeStepFrequency, fixedUpdateCount);
        _previousFrame = current;
    }
}
