using System;
using System.Collections.Generic;
using System.Diagnostics;
using Titan.Core;
using Titan.Core.Messaging;
using Titan.Graphics.Pipeline;
using Titan.Input;

namespace Titan.GameLoop
{
    internal interface IGameLoop
    {
        void Initialize();
        void Start();
    }
    internal class FixedStepSingleThreadedGameLoop : IGameLoop
    {
        private readonly IEventQueue _eventQueue;
        private readonly IInputHandler _inputHandler;
        private readonly IGraphicsPipeline _graphicsPipeline;
        private readonly float _fixedTimeStep;
        private object _systems;

        public FixedStepSingleThreadedGameLoop(TitanConfiguration configuration, IEventQueue eventQueue, IInputHandler inputHandler, IGraphicsPipeline graphicsPipeline)
        {
            _eventQueue = eventQueue;
            _inputHandler = inputHandler;
            _graphicsPipeline = graphicsPipeline;
            _fixedTimeStep = configuration.FixedTimeStep;
        }



        public void Start()
        {
            var timer = Stopwatch.StartNew();

            _eventQueue.Update();
            _inputHandler.Update();
            
            // do the stuffs
            
            
            _graphicsPipeline.Execute();

        }



        // Mock
        private void UpdateSystems(in TimeStep timeStep)
        {

        }

        public void Initialize()
        {
            //_systems = _systemsProvider.GetAll();
        }

    }

}
