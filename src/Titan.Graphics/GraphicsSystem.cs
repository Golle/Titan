using System;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Graphics.Pipeline;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;
using Titan.Graphics.States;

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
        private readonly IDepthStencilViewManager _depthStencilViewManager;
        private readonly ISamplerStateManager _samplerStateManager;
        private readonly IDepthStencilStateManager _depthStencilStateManager;
        private readonly IShaderManager _shaderManager;

        public GraphicsSystem(IGraphicsDevice device, IGraphicsPipeline graphicsPipeline, ITextureManager textureManager, IShaderResourceViewManager shaderResourceViewManager, IVertexBufferManager vertexBufferManager, IIndexBufferManager indexBufferManager, IConstantBufferManager constantBufferManager, IRenderTargetViewManager renderTargetViewManager, IDepthStencilViewManager depthStencilViewManager, ISamplerStateManager samplerStateManager, IDepthStencilStateManager depthStencilStateManager, IShaderManager shaderManager)
        {
            _device = device;
            _graphicsPipeline = graphicsPipeline;
            _textureManager = textureManager;
            _shaderResourceViewManager = shaderResourceViewManager;
            _vertexBufferManager = vertexBufferManager;
            _indexBufferManager = indexBufferManager;
            _constantBufferManager = constantBufferManager;
            _renderTargetViewManager = renderTargetViewManager;
            _depthStencilViewManager = depthStencilViewManager;
            _samplerStateManager = samplerStateManager;
            _depthStencilStateManager = depthStencilStateManager;
            _shaderManager = shaderManager;
        }

        public void Initialize(string assetsDirectory, uint refreshRate, PipelineConfiguration pipelineConfiguration, bool debug)
        {
            LOGGER.Debug("Initialize graphics device {0}", debug ? "(debug)" : "");
            _device.Initialize(refreshRate, debug);

            LOGGER.Debug("Initialize GraphicManagers");
            _textureManager.Initialize(_device);
            _shaderResourceViewManager.Initialize(_device);
            _vertexBufferManager.Initialize(_device);
            _indexBufferManager.Initialize(_device);
            _constantBufferManager.Initialize(_device);
            _renderTargetViewManager.Initialize(_device);
            _depthStencilViewManager.Initialize(_device);
            _samplerStateManager.Initialize(_device);
            _depthStencilStateManager.Initialize(_device);
            
            _shaderManager.Initialize(_device, assetsDirectory);
            LOGGER.Debug("Finished initializing GraphicManagers");

            LOGGER.Debug("Initialize graphics pipeline");
            _graphicsPipeline.Initialize(pipelineConfiguration);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RenderFrame() => _graphicsPipeline.Execute();
        public void Dispose()
        {
            // Dispose in reverse order of creation
            _graphicsPipeline.Dispose();

            _shaderManager.Dispose();
            
            _textureManager.Dispose();
            _shaderResourceViewManager.Dispose();
            _vertexBufferManager.Dispose();
            _indexBufferManager.Dispose();
            _constantBufferManager.Dispose();
            _renderTargetViewManager.Dispose();
            _depthStencilViewManager.Dispose();
            _samplerStateManager.Dispose();
            _depthStencilStateManager.Dispose();

            _device.Dispose();
        }
    }
}
