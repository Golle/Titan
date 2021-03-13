using System;
using Titan.GraphicsV2.D3D11;

namespace Titan.GraphicsV2.Rendering.Pipepline
{
    internal class RenderPipeline : IDisposable
    {
        private readonly RenderStage[] _stages;

        public RenderPipeline(string name, RenderStage[] stages)
        {
            _stages = stages;
        }

        public void Render(Context context)
        {
            foreach (var stage in _stages)
            {
                stage.Render(context);
            }
        }

        public void Dispose()
        {
            foreach (var stage in _stages)
            {
                stage.Dispose();
            }
        }
    }
}
