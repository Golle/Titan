using Titan.Core;
using Titan.Core.Logging;
using Titan.Graphics.Pipeline.Configuration;
using Titan.Graphics.Shaders;

namespace Titan.Graphics.Pipeline
{
    internal class GraphicsPipeline : IGraphicsPipeline
    {
        private readonly TitanConfiguration _configuration;
        private readonly IPipelineConfigurationLoader _loader;
        private readonly IShaderManager _shaderManager;

        public GraphicsPipeline(TitanConfiguration configuration, IPipelineConfigurationLoader loader, IShaderManager shaderManager)
        {
            _configuration = configuration;
            _loader = loader;
            _shaderManager = shaderManager;
        }

        public void Initialize(string filename)
        {
            var path = _configuration.GetPath(filename);
            LOGGER.Debug("Loading Pipeline configuration from {0}", path);
            var configuration = _loader.Load(path);
            LOGGER.Debug("Adding ShaderPrograms {0}", configuration.ShaderPrograms.Length);

            foreach (var (name, vertexShader, pixelShader, layout) in configuration.ShaderPrograms)
            {
                LOGGER.Debug("Compiling shader program {0}", name);
                _shaderManager.AddShaderProgram(name, vertexShader, pixelShader, layout);
            }

            var builder = new RenderGraphBuilder();
            foreach (var renderPass in configuration.RenderPasses)
            {
                builder.AddPass(renderPass);
            }
        }
    }

    internal class RenderGraphBuilder
    {
        public void AddPass(RenderPassConfiguration config)
        {
            //throw new System.NotImplementedException();
        }

        public RenderGraph Compile()
        {
            return null;
        }
    }

    internal class RenderGraph
    {
        
    }


    public class FullScreenRenderPass {}
    public class SceneRenderPass{}
}
