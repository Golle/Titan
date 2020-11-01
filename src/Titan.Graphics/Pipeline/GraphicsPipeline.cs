using System;
using System.Collections.Generic;
using System.Linq;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Graphics.D3D11;
using Titan.Graphics.Pipeline.Configuration;
using Titan.Graphics.Pipeline.Graph;
using Titan.Graphics.Pipeline.Renderers;
using Titan.Graphics.Shaders;
using Titan.IOC;
using static Titan.Windows.Win32.D3D11.D3D11_BIND_FLAG;
using static Titan.Windows.Win32.D3D11.DXGI_FORMAT;

namespace Titan.Graphics.Pipeline
{
    internal class GraphicsPipeline : IGraphicsPipeline
    {
        private readonly TitanConfiguration _configuration;
        private readonly IPipelineConfigurationLoader _loader;
        private readonly IShaderManager _shaderManager;
        private readonly IBufferManager _bufferManager;
        private readonly IContainer _container;
        private readonly IGraphicsDevice _device;

        private readonly IList<IRenderer> _renderers = new List<IRenderer>();
        public GraphicsPipeline(TitanConfiguration configuration, IPipelineConfigurationLoader loader, IShaderManager shaderManager, IBufferManager bufferManager,  IContainer container, IGraphicsDevice device)
        {
            _configuration = configuration;
            _loader = loader;
            _shaderManager = shaderManager;
            _bufferManager = bufferManager;
            _container = container;
            _device = device;
        }

        public void Initialize(string filename)
        {
            var builder = new RenderGraphBuilder(new BackBufferRenderTargetView(_device));
            
            var path = _configuration.GetPath(filename);
            LOGGER.Debug("Loading Pipeline configuration from {0}", path);
            
            var (shaderPrograms, renderPasses, renderers) = _loader.Load(path);
            foreach (var (name, renderer) in InitializeRenderers(renderers, renderPasses))
            {
                LOGGER.Debug("Created renderer {0} with name {1}", renderer.GetType().Name, name);
                builder.AddRenderer(name, renderer);
            }
            
            LOGGER.Debug("Adding ShaderPrograms {0}", shaderPrograms.Length);
            foreach (var (name, vertexShader, pixelShader, layout) in shaderPrograms)
            {
                LOGGER.Debug("Compiling shader program {0}", name);
                var handle = _shaderManager.AddShaderProgram(name, vertexShader, pixelShader, layout);
                builder.AddShaderProgram(name, handle);
            }
            
            foreach (var renderPass in renderPasses)
            {
                LOGGER.Debug("Creating RenderPass {0}", renderPass.Name);
                foreach (var renderTarget in renderPass.RenderTargets.Where(r => !r.IsGlobal()))
                {
                    LOGGER.Debug("Creating RenderTarget {0}", renderTarget.Name);
                    var flags = IsResource(renderPasses, renderTarget) ? D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE : D3D11_BIND_RENDER_TARGET;
                    var buffer = _bufferManager.GetBuffer(renderTarget.Format, flags); // TODO: Buffers should be fully managed by the manager (cleanup etc)
                    builder.AddBuffer(renderTarget.Name, buffer);
                }

                if(renderPass.DepthStencil is not null)
                {
                    LOGGER.Debug("Creating DepthStencil {0}", renderPass.DepthStencil.Name);
                    // temp impl
                    var stencil = _bufferManager.GetDepthStencil(bindFlag: D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_DEPTH_STENCIL, shaderResourceFormat: DXGI_FORMAT_R24_UNORM_X8_TYPELESS);
                    builder.AddDepthStencil(renderPass.DepthStencil.Name, stencil);
                }
                builder.AddPass(renderPass);
            }

            var apa = builder.Compile();
            
        }

        private static bool IsResource(RenderPassConfiguration[] passes, RenderTargetConfiguration target) => passes.Any(p => p.Resources?.Any(r => r.Name == target.Name) ?? false);

        private IEnumerable<(string name, IRenderer renderer)> InitializeRenderers(RendererConfiguration[] renderers, RenderPassConfiguration[] renderPasses)
        {
            LOGGER.Debug("Adding Renderers {0}", renderers.Length);
            foreach (var (name, type) in renderers)
            {
                if (renderPasses.All(p => p.Renderer != name))
                {
                    // If the renderer is not used in the render pass we ignore it to avoid allocating resources.
                    LOGGER.Debug("Renderer {0} of type {1} is not used in the Render Passes configuration. Ignoring.", name, type);
                    continue;
                }

                var rendererType = Type.GetType(type);
                if (rendererType == null)
                {
                    throw new InvalidOperationException($"Type {type} could not be found");
                }

                if (_container.CreateInstance(rendererType) is IRenderer renderer)
                {
                    _renderers.Add(renderer); // Save the reference to the renderer in the pipeline for cleanup.
                    yield return (name, renderer);
                }
                else
                {
                    throw new NotSupportedException($"Only renderers that implement {typeof(IRenderer)} are supported.");
                }
            }
        }

        public void Dispose()
        {
            // TODO: move this to a renderer manager so the clenaup can be done there
            foreach (var renderer in _renderers)
            {
                renderer.Dispose();
            }
        }
    }


    internal class RenderGraph
    {
        private readonly RenderPass[] _renderPasses;
        public RenderGraph(RenderPass[] renderPasses)
        {
            _renderPasses = renderPasses;
        }
    }

}
