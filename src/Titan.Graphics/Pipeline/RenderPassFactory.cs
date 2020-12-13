using System.Collections.Generic;
using System.Linq;
using Titan.Graphics.Resources;

namespace Titan.Graphics.Pipeline
{
    internal class RenderPassFactory : IRenderPassFactory
    {
        private readonly IGraphicsDevice _device;
        private readonly IShaderResourceViewManager _shaderResourceViewManager;

        public RenderPassFactory(IGraphicsDevice device, IShaderResourceViewManager shaderResourceViewManager)
        {
            _device = device;
            _shaderResourceViewManager = shaderResourceViewManager;
        }
        
        public RenderPass Create(string name, IEnumerable<RenderPassCommand> commandList) 
            => new(name, commandList.ToArray(), _device, _shaderResourceViewManager);
    }
}
