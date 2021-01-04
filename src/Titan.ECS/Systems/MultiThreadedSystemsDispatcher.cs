using System;
using Titan.Core.Threading;

namespace Titan.ECS.Systems
{
    internal class MultiThreadedSystemsDispatcher : IDisposable
    {
        private readonly WorkerPool _workerPool;

        private readonly JobProgress _progress = new(0); // use a single instance of the JobProgress class
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
            // Stage 1: Transform2D, Transform3D, TransformRect systems
            // Stage 2: NinePatchSprite (Uses TransformRect)
            // Stage 3: Render systems (Sprite, UI, World) -> Pushes things to the render queues
            // Stage 4: All renderers, async using Deferred context to draw lights, post processing (This is not a system and should not be a part of this class)
            
            // Stage Unknown: ScriptSystem (execute application defined scripts, can both read and write to components), maybe it should be the first? :O
        }


        public void Update()
        {
            // Queue the systems in the worker pool, based on which can be executed async.

            
            for (var stage = 2; stage <= 10; stage += 3)
            {
                const int jobCountInStage = 4;
                _progress.Reset(jobCountInStage);
                for (var jobCount = 0; jobCount < jobCountInStage; ++jobCount)
                {
                    var count = jobCount;
                    var stage1 = stage;
                    _workerPool.Enqueue(new JobDescription(() => Console.WriteLine($"[{stage1},{count}] Doing some amazing work")), _progress);
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
