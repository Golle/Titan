using System;
using System.Collections.Generic;
using System.Linq;
using Titan.Graphics.D3D11;
using Titan.Graphics.Pipeline.Renderers;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;
using Titan.Graphics.States;

namespace Titan.Graphics.Pipeline
{
    internal class RenderPassBuilder
    {
        private readonly IDictionary<string, IRenderer> _renderers = new Dictionary<string, IRenderer>();
        private readonly IDictionary<string, RenderPassConfiguration> _renderPasses = new Dictionary<string, RenderPassConfiguration>();
        private readonly IDictionary<string, ShaderProgram> _shaderProgramHandles = new Dictionary<string, ShaderProgram>();
        private readonly IDictionary<string, RenderTargetViewHandle> _renderTargets = new Dictionary<string, RenderTargetViewHandle>();
        private readonly IDictionary<string, ShaderResourceViewHandle> _shaderResources = new Dictionary<string, ShaderResourceViewHandle>();

        private readonly IDictionary<string, DepthStencilViewHandle> _depthStencils = new Dictionary<string, DepthStencilViewHandle>();
        private readonly IDictionary<string, SamplerStateHandle> _samplers = new Dictionary<string, SamplerStateHandle>();
        
        private readonly IGraphicsDevice _device;

        internal RenderPassBuilder(IGraphicsDevice device)
        {
            _device = device;
            _renderTargets["$Backbuffer"] = 0; // The backbuffer will always be at handle 0
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

        public void AddShaderProgram(string name, in ShaderProgram shaderProgram)
        {
            if (_shaderProgramHandles.ContainsKey(name))
            {
                throw new InvalidOperationException($"Shader Program with name {name} has already been added to the Render Graph");
            }
            _shaderProgramHandles.Add(name, shaderProgram);
        }

        public void AddRenderTarget(string name, in RenderTargetViewHandle handle)
        {
            if (_renderTargets.ContainsKey(name))
            {
                throw new InvalidOperationException($"RenderTarget with name {name} has already been added");
            }
            _renderTargets.Add(name, handle);
        }

        public void AddShaderResource(string name, in ShaderResourceViewHandle handle)
        {
            if (_shaderResources.ContainsKey(name))
            {
                throw new InvalidOperationException($"ShaderResource with name {name} has already been added");
            }
            _shaderResources.Add(name, handle);
        }

        public void AddDepthStencil(string name, in DepthStencilViewHandle handle)
        {
            if (_depthStencils.ContainsKey(name))
            {
                throw new InvalidOperationException($"Depth stencil with name {name} has already been added to the Render Graph");
            }
            _depthStencils.Add(name, handle);
        }

        public void AddSampler(string name, in SamplerStateHandle handle)
        {
            if (_samplers.ContainsKey(name))
            {
                throw new InvalidOperationException($"Sampler with name {name} has already been added to the Render Graph");
            }
            _samplers.Add(name, handle);
        }

        public RenderPass[] Compile(IRenderPassFactory renderPassFactory)
        {
            var renderPasses = new List<RenderPass>();
            
            foreach (var pass in _renderPasses.Values)
            {
                var commandList = new List<RenderPassCommand>();
                if (pass.DepthStencil != null)
                {
                    commandList.Add(new RenderPassCommand{Type = CommandType.ClearDepthStencil, DepthStencil = _depthStencils[pass.DepthStencil.Name]});
                }

                commandList.AddRange(CreateResourcesCommands(pass.Resources));
                commandList.AddRange(CreateSamplerCommands(pass.Samplers).ToArray());

                commandList.AddRange(CreateRenderTargetCommand(pass.RenderTargets, pass.DepthStencil).ToArray());
                commandList.Add(new RenderPassCommand {Type = CommandType.Render, Renderer = _renderers[pass.Renderer]});

                
                if (pass.UnbindRenderTargets)
                {
                    commandList.Add(new RenderPassCommand{Type = CommandType.UnbindRenderTargets });
                }

                if (pass.UnbindResources)
                {
                    commandList.AddRange(CreateUnbindResourcesCommands(pass.Resources));
                }

                renderPasses.Add(renderPassFactory.Create(pass.Name, commandList));
            }
            
            return renderPasses.ToArray();
        }

        private static IEnumerable<RenderPassCommand> CreateUnbindResourcesCommands(RenderPassResourceConfiguration[] resources)
        {
            var pixelShaderResourceCount = resources.Count(r => r.Type == RenderPassResourceTypes.PixelShader);
            var vertexShaderResourceCount = resources.Count(r => r.Type == RenderPassResourceTypes.VertexShader);

            if (pixelShaderResourceCount > 0)
            {
                yield return new RenderPassCommand {Type = CommandType.UnbindPixelShaderResources, Count = (uint) pixelShaderResourceCount};
            }

            if (vertexShaderResourceCount > 0)
            {
                yield return new RenderPassCommand {Type = CommandType.UnbindVertexShaderResources, Count = (uint) vertexShaderResourceCount};
            }
        }

        private IEnumerable<RenderPassCommand> CreateSamplerCommands(RenderPassSamplerConfiguration[] samplers)
        {
            if (samplers == null)
            {
                yield break;
            }

            for (var i = 0u; i < samplers.Length; ++i)
            {
                var (name, type) = samplers[i];
                var resourceType = type == SamplerType.PixelShader ? CommandType.SetPixelShaderSampler : CommandType.SetVertexShaderSampler;
                yield return new RenderPassCommand {Type = resourceType, SamplerState = new SetSamplerStateCommand {Sampler = _samplers[name], Slot = i}};
            }
        }

        private IEnumerable<RenderPassCommand> CreateResourcesCommands(RenderPassResourceConfiguration[] resources)
        {
            if (resources == null)
            {
                yield break;
            }
            for (var i = 0u; i < resources.Length; ++i)
            {
                var (name, type) = resources[i];
                var resourceType = type == RenderPassResourceTypes.PixelShader ? CommandType.SetPixelShaderResource : CommandType.SetVertexShaderResource;
                yield return new RenderPassCommand {Type = resourceType, ShaderResource = new SetShaderResourceCommand {Handle = _shaderResources[name], Slot = i}};
            }
        }

        private IEnumerable<RenderPassCommand> CreateRenderTargetCommand(RenderTargetConfiguration[] renderTargets, DepthStencilConfiguration depthStencil)
        {
            if (renderTargets.Length == 1)
            {
                var renderTarget = renderTargets[0];
                var renderTargetView = _renderTargets[renderTarget.Name];

                if (depthStencil != null)
                {
                    yield return new RenderPassCommand { Type = CommandType.SetRenderTargetAndDepthStencil, RenderTarget = renderTargetView, DepthStencil = _depthStencils[depthStencil.Name]};
                }
                else
                {
                    yield return new RenderPassCommand { Type = CommandType.SetRenderTarget, RenderTarget = renderTargetView };
                }
                
                if (renderTarget.Clear)
                {
                    yield return new RenderPassCommand {Type = CommandType.ClearRenderTarget, ClearRenderTarget = new ClearRenderTargetCommand {RenderTarget = renderTargetView, Color = Color.Parse(renderTarget.Color)}};
                }
            }
            else
            {
                var commandType = CommandType.SetMultipleRenderTargets;
                var command = new SetMultipleRenderTargetViewCommand { Count = (uint)renderTargets.Length };

                if (depthStencil != null)
                {
                    commandType = CommandType.SetMultipleRenderTargetAndDepthStencil;
                    command.DepthStencilView = _depthStencils[depthStencil.Name];
                }
                for (var i = 0; i < renderTargets.Length; ++i)
                {
                    var renderTarget = renderTargets[i];
                    var renderTargetView = _renderTargets[renderTarget.Name];
                    command.Set(i, renderTargetView);
                    if (renderTarget.Clear)
                    {
                        yield return new RenderPassCommand { Type = CommandType.ClearRenderTarget, ClearRenderTarget = new ClearRenderTargetCommand { RenderTarget = renderTargetView, Color = Color.Parse(renderTarget.Color) } };
                    }
                }
                yield return new RenderPassCommand { Type = commandType, MultipleRenderTargets = command };
            }
        }
    }
}
