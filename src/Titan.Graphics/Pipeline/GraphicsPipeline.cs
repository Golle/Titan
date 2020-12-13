using System;
using System.Collections.Generic;
using System.Linq;
using Titan.Core.Logging;
using Titan.Graphics.Pipeline.Renderers;
using Titan.Graphics.Resources;
using Titan.IOC;
using Titan.Windows;
using static Titan.Windows.Win32.D3D11.D3D11_BIND_FLAG;
using static Titan.Windows.Win32.D3D11.DXGI_FORMAT;

namespace Titan.Graphics.Pipeline
{
    internal class GraphicsPipeline : IGraphicsPipeline
    {
        private readonly IWindow _window;
        private readonly IContainer _container;
        private readonly IGraphicsDevice _device;
        private readonly ITextureManager _textureManager;
        private readonly IShaderResourceViewManager _shaderResourceViewManager;
        private readonly IRenderTargetViewManager _renderTargetViewManager;
        private readonly IRenderPassFactory _renderPassFactory;

        private readonly IList<IRenderer> _renderers = new List<IRenderer>();
        
        private RenderGraph _renderGraph;

        public GraphicsPipeline(IContainer container,  IWindow window, IGraphicsDevice device, ITextureManager textureManager, IShaderResourceViewManager shaderResourceViewManager, IRenderTargetViewManager renderTargetViewManager, IRenderPassFactory renderPassFactory)
        {
            _window = window;
            _container = container;
            _device = device;
            _textureManager = textureManager;
            _shaderResourceViewManager = shaderResourceViewManager;
            _renderTargetViewManager = renderTargetViewManager;
            _renderPassFactory = renderPassFactory;
        }

        public unsafe void Initialize(PipelineConfiguration configuration)
        {
            if (_renderGraph != null)
            {
                throw new InvalidOperationException("Graphics Pipeline has already been initialized.");
            }
            var builder = _container.CreateInstance<RenderPassBuilder>();
            
            
            var (shaderPrograms, renderPasses, renderers, samplers) = configuration;

            // Shaders must be compiled first since they are used in the Renderers
            LOGGER.Debug("Adding ShaderPrograms {0}", shaderPrograms.Length);
            foreach (var (name, vertexShader, pixelShader, layout) in shaderPrograms)
            {
                LOGGER.Debug("Compiling shader program {0}", name);

                var program = _device.ShaderManager.AddShader(name, vertexShader, pixelShader, layout);
                builder.AddShaderProgram(name, program);
            }


            foreach (var (name, renderer) in InitializeRenderers(renderers, renderPasses))
            {
                LOGGER.Debug("Created renderer {0} with name {1}", renderer.GetType().Name, name);
                builder.AddRenderer(name, renderer);
            }

            foreach (var sampler in samplers)
            {
                LOGGER.Debug("Creating SamplerState {0}", sampler.Name);
                var samplerHandle = _device.SamplerStateManager.GetOrCreate(sampler.Filter, sampler.AddressU, sampler.AddressV, sampler.AddressW, sampler.ComparisonFunc);
                builder.AddSampler(sampler.Name, samplerHandle);
            }

            foreach (var renderPass in renderPasses)
            {
                LOGGER.Debug("Creating RenderPass {0}", renderPass.Name);
                foreach (var renderTarget in renderPass.RenderTargets.Where(r => !r.Name.StartsWith("$")))
                {
                    LOGGER.Debug("Creating RenderTarget {0}", renderTarget.Name);
                    if (IsResource(renderPasses, renderTarget))
                    {
                        var textureHandle = _textureManager.CreateTexture((uint) _window.Width, (uint) _window.Height, renderTarget.Format, D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE);
                        var resource = _textureManager[textureHandle].Resource;
                        var resourceHandle = _shaderResourceViewManager.Create(resource, renderTarget.Format);
                        var renderTargetHandle = _renderTargetViewManager.Create(resource, renderTarget.Format);
                        builder.AddRenderTarget(renderTarget.Name, renderTargetHandle);
                        builder.AddShaderResource(renderTarget.Name, resourceHandle);
                    }
                    else
                    {
                        var textureHandle = _textureManager.CreateTexture((uint)_window.Width, (uint)_window.Height, renderTarget.Format, D3D11_BIND_RENDER_TARGET);
                        var resource = _textureManager[textureHandle].Resource;
                        var renderTargetHandle = _renderTargetViewManager.Create(resource, renderTarget.Format);
                        builder.AddRenderTarget(renderTarget.Name, renderTargetHandle);
                        //var resourceHandle = _device.ShaderResourceViewManager.Create(_device.TextureManager[textureHandle].Resource, renderTarget.Format);
                        //builder.AddShaderResource(renderTarget.Name, resourceHandle);
                    }
                }

                if(renderPass.DepthStencil is not null)
                {
                    LOGGER.Debug("Creating DepthStencil {0}", renderPass.DepthStencil.Name);
                    // temp impl
                    var stencilTextureHandle = _textureManager.CreateTexture((uint) _window.Width, (uint) _window.Height, DXGI_FORMAT_R24G8_TYPELESS, D3D11_BIND_DEPTH_STENCIL | D3D11_BIND_SHADER_RESOURCE);
                    var stencilHandle = _device.DepthStencilViewManager.Create(_textureManager[stencilTextureHandle].Resource);
                    builder.AddDepthStencil(renderPass.DepthStencil.Name, stencilHandle);
                }
                
                builder.AddPass(renderPass);
            }

            var passes = builder.Compile(_renderPassFactory);
            _renderGraph = new RenderGraph(passes, _device);
            
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
            _renderGraph.Dispose();
        }
    }
}
