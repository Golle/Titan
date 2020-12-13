using System;

namespace Titan.Graphics.Pipeline
{
    public interface IGraphicsPipeline : IDisposable
    {
        void Initialize(PipelineConfiguration configuration);
        void Execute();
    }
}
