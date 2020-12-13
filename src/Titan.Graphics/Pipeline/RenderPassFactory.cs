using System.Collections.Generic;
using System.Linq;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;

namespace Titan.Graphics.Pipeline
{
    internal class RenderPassFactory : IRenderPassFactory
    {
        private readonly IGraphicsDevice _device;
        private readonly IShaderResourceViewManager _shaderResourceViewManager;
        private readonly IRenderTargetViewManager _renderTargetViewManager;
        private readonly IDepthStencilViewManager _depthStencilViewManager;
        private readonly IShaderManager _shaderManager;

        public RenderPassFactory(IGraphicsDevice device, IShaderResourceViewManager shaderResourceViewManager, IRenderTargetViewManager renderTargetViewManager, IDepthStencilViewManager depthStencilViewManager, IShaderManager shaderManager)
        {
            _device = device;
            _shaderResourceViewManager = shaderResourceViewManager;
            _renderTargetViewManager = renderTargetViewManager;
            _depthStencilViewManager = depthStencilViewManager;
            _shaderManager = shaderManager;
        }
        
        public RenderPass Create(string name, IEnumerable<RenderPassCommand> commandList) 
            => new(name, commandList.ToArray(), _device, _shaderResourceViewManager, _renderTargetViewManager, _depthStencilViewManager, _shaderManager);
    }
}
