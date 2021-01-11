using System;
using System.Numerics;
using System.Threading;
using Titan.Core.Common;
using Titan.Core.Logging;
using Titan.Core.Threading;

namespace Titan.ECS.Systems
{
    public class MultiThreadedSystemsDispatcher : IDisposable
    {
        private readonly WorkerPool _workerPool;

        private readonly JobProgress _progress = new(0); // use a single instance of the JobProgress class

        private readonly JobDescription Desc = new(() =>
        {


        });
        public MultiThreadedSystemsDispatcher(WorkerPool workerPool)
        {
            _workerPool = workerPool;

        }

        public void Initialize()
        {
            // Locate all systems
            // Check dependencies (Read/Write), for example a system that writes to a component must be executed before something that reads from that component
            // Compile a Tree structure/Graph with different "stages"
            // Example:
            // Stage 1: Transform2D, Transform3D, TransformRect systems (these might be last ones to be updated)
            // Stage 2: NinePatchSprite (Uses TransformRect)
            // Stage 3: Render systems (Sprite, UI, World) -> Pushes things to the render queues
            // Stage 4: All renderers, async using Deferred context to draw lights, post processing (This is not a system and should not be a part of this class)
            
            // Stage Unknown: ScriptSystem (execute application defined scripts, can both read and write to components), maybe it should be the first? :O
            _resourceSample = _workerPool.Enqueue(new JobDescription(() => Thread.Sleep(TimeSpan.FromSeconds(1)), () => { Console.WriteLine("Resource loaded!");}, autoReset: false));
        }
        
        private Handle<WorkerPool> _resourceSample;
        public void Update()
        {
            if (_resourceSample.IsValid() && _workerPool.IsCompleted(_resourceSample))
            {
                LOGGER.Debug("Resource loaded successfully!");
                _workerPool.Reset(ref _resourceSample);
            }

            // Queue the systems in the worker pool, based on which can be executed async.
            for (var stage = 0; stage < 2; stage++)
            {
                const int jobCountInStage = 8;
                _progress.Reset(jobCountInStage);
                for (var jobCount = 0; jobCount < jobCountInStage; ++jobCount)
                {
                    _workerPool.Enqueue(Desc, _progress);
                }
                _progress.Wait(); // each stage must wait until all jobs have been finished (might tweak this later)
            }



            
        }

        public void Dispose()
        {
            _workerPool?.Dispose();
        }
    }
}
