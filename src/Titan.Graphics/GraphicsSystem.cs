using System;
using System.Runtime.CompilerServices;
using Titan.Graphics.Pipeline;
using Titan.Graphics.Resources;

namespace Titan.Graphics
{
    internal class GraphicsSystem : IDisposable
    {
        private readonly IGraphicsDevice _device;
        private readonly IGraphicsPipeline _graphicsPipeline;
        private readonly ITextureManager _textureManager;
        private readonly IShaderResourceViewManager _shaderResourceViewManager;
        private readonly IVertexBufferManager _vertexBufferManager;
        private readonly IIndexBufferManager _indexBufferManager;
        private readonly IConstantBufferManager _constantBufferManager;
        private readonly IRenderTargetViewManager _renderTargetViewManager;

        public GraphicsSystem(IGraphicsDevice device, IGraphicsPipeline graphicsPipeline, ITextureManager textureManager, IShaderResourceViewManager shaderResourceViewManager, IVertexBufferManager vertexBufferManager, IIndexBufferManager indexBufferManager, IConstantBufferManager constantBufferManager, IRenderTargetViewManager renderTargetViewManager)
        {
            _device = device;
            _graphicsPipeline = graphicsPipeline;
            _textureManager = textureManager;
            _shaderResourceViewManager = shaderResourceViewManager;
            _vertexBufferManager = vertexBufferManager;
            _indexBufferManager = indexBufferManager;
            _constantBufferManager = constantBufferManager;
            _renderTargetViewManager = renderTargetViewManager;
        }

        public void Initialize(string assetsDirectory, uint refreshRate, PipelineConfiguration pipelineConfiguration, bool debug)
        {
            _device.Initialize(refreshRate, debug);
            
            _textureManager.Initialize(_device);
            _shaderResourceViewManager.Initialize(_device);
            _vertexBufferManager.Initialize(_device);
            _indexBufferManager.Initialize(_device);
            _constantBufferManager.Initialize(_device);
            _renderTargetViewManager.Initialize(_device);
            
            _graphicsPipeline.Initialize(pipelineConfiguration);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RenderFrame() => _graphicsPipeline.Execute();
        public void Dispose()
        {
            _textureManager.Dispose();
            _shaderResourceViewManager.Dispose();
            _device.Dispose();
        }
    }
}
