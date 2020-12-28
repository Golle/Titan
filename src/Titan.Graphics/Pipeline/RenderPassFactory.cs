using System.Collections.Generic;
using System.Linq;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;
using Titan.Graphics.States;

namespace Titan.Graphics.Pipeline
{
    internal class RenderPassFactory : IRenderPassFactory
    {
        private readonly IShaderResourceViewManager _shaderResourceViewManager;
        private readonly IRenderTargetViewManager _renderTargetViewManager;
        private readonly IDepthStencilViewManager _depthStencilViewManager;
        private readonly ISamplerStateManager _samplerStateManager;
        private readonly IShaderManager _shaderManager;

        public RenderPassFactory(IShaderResourceViewManager shaderResourceViewManager, IRenderTargetViewManager renderTargetViewManager, IDepthStencilViewManager depthStencilViewManager, ISamplerStateManager samplerStateManager, IShaderManager shaderManager)
        {
            _shaderResourceViewManager = shaderResourceViewManager;
            _renderTargetViewManager = renderTargetViewManager;
            _depthStencilViewManager = depthStencilViewManager;
            _samplerStateManager = samplerStateManager;
            _shaderManager = shaderManager;
        }

        public RenderPass Create(string name, IEnumerable<RenderPassCommand> commandList)
            => new(name, commandList.ToArray(), _shaderResourceViewManager, _renderTargetViewManager, _depthStencilViewManager, _samplerStateManager, _shaderManager);
    }
}
