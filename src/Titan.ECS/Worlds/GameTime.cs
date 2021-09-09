using System;
using System.Diagnostics;

namespace Titan.ECS.Worlds
{
    internal class GameTime
    {
        private readonly Stopwatch _time;
        private double _previousFrame;
        private double _fixedTime;
        private GameTimeLoop _timeStep;
        public float FixedTimeStepFrequency { get; }
        public ref readonly GameTimeLoop Current => ref _timeStep;

        public GameTime(WorldConfiguration config)
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
}
