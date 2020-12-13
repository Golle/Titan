using System.Collections.Generic;
using System.Linq;
using Titan.Graphics.Resources;

namespace Titan.Graphics.Pipeline
{
    internal class RenderPassFactory : IRenderPassFactory
    {
        private readonly IGraphicsDevice _device;
        private readonly IShaderResourceViewManager _shaderResourceViewManager;
        private readonly IRenderTargetViewManager _renderTargetViewManager;

        public RenderPassFactory(IGraphicsDevice device, IShaderResourceViewManager shaderResourceViewManager, IRenderTargetViewManager renderTargetViewManager)
        {
            _device = device;
            _shaderResourceViewManager = shaderResourceViewManager;
            _renderTargetViewManager = renderTargetViewManager;
        }
        
        public RenderPass Create(string name, IEnumerable<RenderPassCommand> commandList) 
            => new(name, commandList.ToArray(), _device, _shaderResourceViewManager, _renderTargetViewManager);
    }
}
