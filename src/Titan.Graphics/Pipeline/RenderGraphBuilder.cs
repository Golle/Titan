using System;
using System.Collections.Generic;
using System.Linq;
using Titan.Graphics.D3D11;
using Titan.Graphics.Pipeline.Configuration;
using Titan.Graphics.Pipeline.Graph;
using Titan.Graphics.Pipeline.Renderers;

namespace Titan.Graphics.Pipeline
{
    internal class RenderGraphBuilder
    {
        private readonly BackBufferRenderTargetView _backBuffer;

        private readonly IDictionary<string, IRenderer> _renderers = new Dictionary<string, IRenderer>();
        private readonly IDictionary<string, RenderPassConfiguration> _renderPasses = new Dictionary<string, RenderPassConfiguration>();
        private readonly IDictionary<string, uint> _shaderProgramHandles = new Dictionary<string, uint>();
        private readonly IDictionary<string, RenderBuffer> _buffers = new Dictionary<string, RenderBuffer>();
        private readonly IDictionary<string, DepthStencil> _depthStencils = new Dictionary<string, DepthStencil>();

        public RenderGraphBuilder(BackBufferRenderTargetView backBuffer)
        {
            _backBuffer = backBuffer;
        }

        public void AddPass(RenderPassConfiguration config)
        {
            if (_renderPasses.ContainsKey(config.Name))
            {
                throw new InvalidOperationException($"Render pass with name {config.Name} has already been added to the Render Graph");
            }
            
            _renderPasses.Add(config.Name, config);
        }

        public void AddRenderer(string name, IRenderer renderer)
        {
            if (_renderers.ContainsKey(name))
            {
                throw new InvalidOperationException($"Renderer with name {name} has already been added to the Render Graph");
            }
            _renderers.Add(name, renderer);
        }

        public void AddShaderProgram(string name, uint handle)
        {
            if (_shaderProgramHandles.ContainsKey(name))
            {
                throw new InvalidOperationException($"Shader Program with name {name} has already been added to the Render Graph");
            }
            _shaderProgramHandles.Add(name, handle);
        }

        public void AddBuffer(string name, RenderBuffer buffer)
        {
            if (_buffers.ContainsKey(name))
            {
                throw new InvalidOperationException($"Buffer with name {name} has already been added to the Render Graph");
            }
            _buffers.Add(name, buffer);
        }

        public void AddDepthStencil(string name, DepthStencil depthStencil)
        {
            if (_depthStencils.ContainsKey(name))
            {
                throw new InvalidOperationException($"Depth stencil with name {name} has already been added to the Render Graph");
            }
            _depthStencils.Add(name, depthStencil);
        }

        public RenderGraph Compile()
        {
            var renderPasses = new List<RenderPass>();
            
            foreach (var pass in _renderPasses.Values)
            {
                var commandList = new List<RenderPassCommand>();
                
                commandList.AddRange(CreateRenderTargetCommand(pass.RenderTargets).ToArray());

                if (pass.DepthStencil != null)
                {
                    commandList.Add(new RenderPassCommand{Type = CommandType.ClearDepthStencil, DepthStencil = _depthStencils[pass.DepthStencil.Name].View});
                }

                foreach (var (name, type) in pass.Resources ?? Enumerable.Empty<RenderPassResourceConfiguration>())
                {
                    var resourceView = _buffers[name].ShaderResourceView;
                    commandList.Add(type switch
                    {
                        RenderPassResourceTypes.VertexShader => new RenderPassCommand { Type = CommandType.SetVertexShaderResource, ShaderResourceView = resourceView },
                        RenderPassResourceTypes.PixelShader => new RenderPassCommand { Type = CommandType.SetPixelShaderResource, ShaderResourceView = resourceView },
                        _ => throw new NotSupportedException("The resource type is not supported")
                    });
                }

                commandList.Add(new RenderPassCommand {Type = CommandType.Render, Renderer = _renderers[pass.Renderer]});

                renderPasses.Add(new RenderPass(pass.Name, commandList.ToArray()));
            }
            return new RenderGraph(renderPasses.ToArray())
            {

            };
        }

        private IEnumerable<RenderPassCommand> CreateRenderTargetCommand(RenderTargetConfiguration[] renderTargets)
        {
            if (renderTargets.Length == 1)
            {
                var renderTarget = renderTargets[0];
                var renderTargetView = renderTarget.IsGlobal() ? _backBuffer : _buffers[renderTarget.Name].RenderTargetView;
                yield return new RenderPassCommand {Type = CommandType.SetRenderTarget, RenderTarget = renderTargetView};
                if (renderTarget.Clear)
                {
                    yield return new RenderPassCommand {Type = CommandType.ClearRenderTarget, ClearRenderTarget = new ClearRenderTargetCommand {RenderTarget = renderTargetView, Color = Color.Parse(renderTarget.Color)}};
                }
            }
            else
            {
                var command = new SetMultipleRenderTargetViewCommand { Count = (uint)renderTargets.Length };
                for (var i = 0; i < renderTargets.Length; ++i)
                {
                    var renderTarget = renderTargets[i];
                    var renderTargetView = renderTarget.IsGlobal() ? _backBuffer : _buffers[renderTarget.Name].RenderTargetView;
                    command.Set(i, renderTargetView);
                    
                    if (renderTarget.Clear)
                    {
                        yield return new RenderPassCommand { Type = CommandType.ClearRenderTarget, ClearRenderTarget = new ClearRenderTargetCommand { RenderTarget = renderTargetView, Color = Color.Parse(renderTarget.Color) } };
                    }
                }
                yield return new RenderPassCommand { Type = CommandType.SetMultipleRenderTarget, MultipleRenderTargets = command };
            }
        }
    }
}
