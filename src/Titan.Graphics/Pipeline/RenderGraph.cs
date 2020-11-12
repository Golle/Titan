using System;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Pipeline
{
    internal class RenderGraph : IDisposable
    {
        private readonly RenderPass[] _renderPasses;
        private readonly IGraphicsDevice _device;
        private readonly Swapchain _swapchain;

        public RenderGraph(RenderPass[] renderPasses, IGraphicsDevice device)
        {
            _renderPasses = renderPasses;
            _device = device;

            _device.ImmediateContext.SetViewport(new Viewport(1920, 1080));
            _swapchain = new Swapchain(device, true, 0);
        }

        public void Execute()
        {
            foreach (var renderPass in _renderPasses)
            {
                renderPass.Render(_device.ImmediateContext);
            }
            _swapchain.Present();
        }

        public void Dispose()
        {
            _swapchain.Dispose();
        }
    }
}
