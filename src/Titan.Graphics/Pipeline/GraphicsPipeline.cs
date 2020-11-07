using System;
using System.Collections.Generic;
using System.Linq;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.State;
using Titan.Graphics.Pipeline.Configuration;
using Titan.Graphics.Pipeline.Graph;
using Titan.Graphics.Pipeline.Renderers;
using Titan.Graphics.Shaders;
using Titan.IOC;
using Titan.Windows;
using static Titan.Windows.Win32.D3D11.D3D11_BIND_FLAG;
using static Titan.Windows.Win32.D3D11.DXGI_FORMAT;

namespace Titan.Graphics.Pipeline
{
    internal class GraphicsPipeline : IGraphicsPipeline
    {
        private readonly TitanConfiguration _configuration;
        private readonly IPipelineConfigurationLoader _loader;
        private readonly IShaderManager _shaderManager;
        private readonly IWindow _window;
        private readonly IContainer _container;
        private readonly IGraphicsDevice _device;
        private readonly IBufferManager _bufferManager;

        private readonly IList<IRenderer> _renderers = new List<IRenderer>();
        
        private RenderGraph _renderGraph;
        private SamplerState _samplerState;

        public GraphicsPipeline(TitanConfiguration configuration, IPipelineConfigurationLoader loader, IShaderManager shaderManager, IContainer container,  IWindow window, IGraphicsDevice device, IBufferManager bufferManager)
        {
            _configuration = configuration;
            _loader = loader;
            _shaderManager = shaderManager;
            _window = window;
            _container = container;
            _device = device;
            _bufferManager = bufferManager;
            _samplerState = new SamplerState(_device); // TODO: this is a temp solution until we have a sampler state manager where samplers can be shared.
        }

        public unsafe void Initialize(string filename)
        {
            if (_renderGraph != null)
            {
                throw new InvalidOperationException("Graphics Pipeline has already been initialized.");
            }
            var builder = new RenderPassBuilder(_device);
            
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
                    if (IsResource(renderPasses, renderTarget))
                    {
                        var textureHandle = _device.TextureManager.CreateTexture((uint) _window.Width, (uint) _window.Height, renderTarget.Format, D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE);
                        var resource = _device.TextureManager[textureHandle].Resource;
                        var resourceHandle = _device.ShaderResourceViewManager.Create(resource, renderTarget.Format);
                        var renderTargetHandle = _device.RenderTargetViewManager.Create(resource, renderTarget.Format);
                        builder.AddRenderTarget(renderTarget.Name, renderTargetHandle);
                        builder.AddShaderResource(renderTarget.Name, resourceHandle);
                    }
                    else
                    {
                        var textureHandle = _device.TextureManager.CreateTexture((uint)_window.Width, (uint)_window.Height, renderTarget.Format, D3D11_BIND_SHADER_RESOURCE);
                        var resourceHandle = _device.ShaderResourceViewManager.Create(_device.TextureManager[textureHandle].Resource, renderTarget.Format);
                        builder.AddShaderResource(renderTarget.Name, resourceHandle);
                    }
                }

                if(renderPass.DepthStencil is not null)
                {
                    LOGGER.Debug("Creating DepthStencil {0}", renderPass.DepthStencil.Name);
                    // temp impl
                    var stencil = _bufferManager.GetDepthStencil(bindFlag: D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_DEPTH_STENCIL, shaderResourceFormat: DXGI_FORMAT_R24_UNORM_X8_TYPELESS);
                    builder.AddDepthStencil(renderPass.DepthStencil.Name, stencil);
                }

                if (renderPass.Samplers != null)
                {
                    foreach (var sampler in renderPass.Samplers)
                    {
                        // TODO: replace this with some kind of a sampler definition in the pipeline. Default should be a global sample
                        LOGGER.Debug("Creating SamplerState {0}", sampler);
                        builder.AddSampler(sampler.Name, _samplerState);
                    }
                }
                
                builder.AddPass(renderPass);
            }

            _renderGraph = new RenderGraph(builder.Compile(), _device);
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

        public void Execute()
        {
            // temp
            _renderGraph.Execute();
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
}
