using Titan.GraphicsV2.D3D11;

namespace Titan.GraphicsV2.Rendering
{
    internal class RenderPipeline
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
    }
}
