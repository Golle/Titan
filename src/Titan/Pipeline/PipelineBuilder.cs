using Titan.Assets;
using Titan.Core.Services;

namespace Titan.Pipeline;

public abstract class PipelineBuilder
{
    public abstract void LoadResources(AssetsManager assetsManager);
    public abstract bool IsReady(AssetsManager manager);
    public abstract Graphics.D3D11.Pipeline.Pipeline[] BuildPipeline(IServiceCollection services);
}
