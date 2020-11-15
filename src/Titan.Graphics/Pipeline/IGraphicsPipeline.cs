using System;

namespace Titan.Graphics.Pipeline
{
    public interface IGraphicsPipeline : IDisposable
    {
        void Initialize(string filename);
        void Execute();
    }
}
