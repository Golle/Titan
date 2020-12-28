using System.Collections.Generic;

namespace Titan.Graphics.Pipeline
{
    internal interface IRenderPassFactory
    {
        RenderPass Create(string name, IEnumerable<RenderPassCommand> commandList);
    }
}
