using System;
using System.Collections.Generic;
using System.Linq;
using Titan.Core.Common;
using Titan.Core.Logging;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Samplers;
using Titan.GraphicsV2.D3D11.Shaders;
using Titan.GraphicsV2.D3D11.Textures;
using Titan.GraphicsV2.Rendering.Commands;
using Titan.GraphicsV2.Rendering.Pipepline;
using Titan.GraphicsV2.Rendering.Renderers;

namespace Titan.GraphicsV2.Rendering.Builder
{
    internal class RenderStageBuilder
    {
        private readonly IDictionary<string, Texture> _framebuffers;
        private readonly IDictionary<string, Sampler> _samplers;
        private readonly IDictionary<string, Shader> _shaders;

        private readonly List<Texture> _outputs = new();
        private readonly List<Texture> _pixelShaderInputs = new();
        private readonly List<Texture> _vertexShaderInputs = new();
        private readonly List<Sampler> _vertexShaderSamplers = new();
        private readonly List<Sampler> _pixelShaderSamplers = new();
        
        private readonly IRenderer _renderer;


        private Shader? _shader;


        private Color? _clearColor;

        public RenderStageBuilder(string name, IRenderer renderer, IDictionary<string, Texture> framebuffers, IDictionary<string, Sampler> samplers, IDictionary<string, Shader> shaders)
        {
            _renderer = renderer;
            _framebuffers = framebuffers;
            _samplers = samplers;
            _shaders = shaders;
            LOGGER.Debug("Stage: {0}", name);
        }

        internal void AddInput(string name, RenderBindingTypes type)
        {
            var framebuffer = _framebuffers[name];
            LOGGER.Debug("Input: {0}({1})  Framebuffer: {2}", name, type, framebuffer.Handle);
            switch (type)
            {
                case RenderBindingTypes.PixelShader: _pixelShaderInputs.Add(framebuffer);break;
                case RenderBindingTypes.VertexShader: _vertexShaderInputs.Add(framebuffer);break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        internal void AddOutput(string renderTarget)
        {
            var framebuffer = _framebuffers[renderTarget];
            LOGGER.Debug("Output: {0} Framebuffer: {1}", renderTarget, framebuffer.Handle);
            _outputs.Add(framebuffer);
        }

        internal void ClearRenderTargets(in Color color)
        {
            _clearColor = color;
        }

        internal void AddSampler(string name, RenderBindingTypes type)
        {
            var sampler = _samplers[name];
            LOGGER.Debug("Sampler: {0}({1})  Handle: {2}", name, type, sampler.Handle);
            switch (type)
            {
                case RenderBindingTypes.PixelShader: _pixelShaderSamplers.Add(sampler); break;
                case RenderBindingTypes.VertexShader: _vertexShaderSamplers.Add(sampler); break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void UseShader(string name)
        {
            if (_shader.HasValue)
            {
                throw new InvalidOperationException("Multiple shader programs set for a single pass");
            }
            _shader = _shaders[name];
            LOGGER.Debug("Shader: {0}  Handle: {1}", name, _shader.Value);
        }

        internal RenderStage Build()
        {
            // Init the renderer
            _renderer.Init();

            using var builder = new CommandBufferBuilder();
            // Begin/Setup state
            BuildOutputs(builder);
            BuildInputs(builder);
            BuildSamplers(builder);
            BuildShader(builder);

            // Execute the renderer
            builder.Write(RenderCommand.Default);

            // Cleanup/unbind resources
            if (_outputs.Any())
            {
                builder.Write(new UnbindRenderTargetsCommand(_outputs.Count));
            }

            if (_vertexShaderInputs.Any())
            {
                builder.Write(new UnbindVertexShaderResourcesCommand(_vertexShaderInputs.Count));
            }
            if (_pixelShaderInputs.Any())
            {
                builder.Write(new UnbindPixelShaderResourcesCommand(_pixelShaderInputs.Count));
            }
            
            // TODO: add support for other renderers
            return new RenderStage(builder.Build(), _renderer);
        }

        private void BuildShader(CommandBufferBuilder builder)
        {
            if (!_shader.HasValue)
            {
                return;
            }
            var shader = _shader.Value;
            unsafe
            {
                builder.Write(new SetShadersCommand(shader.VertexShader, shader.PixelShader, shader.InputLayout));
            }
        }

        private unsafe void BuildSamplers(CommandBufferBuilder builder)
        {
            if (_pixelShaderSamplers.Any())
            {
                var command = new SetPixelShaderSamplersCommand((uint) _pixelShaderSamplers.Count);
                for (var i = 0; i < _pixelShaderSamplers.Count; ++i)
                {
                    command.Samplers[i] = _pixelShaderSamplers[i].SamplerState;
                }
                builder.Write(command);
            }
            if (_vertexShaderSamplers.Any())
            {
                var command = new SetVertexShaderSamplersCommand((uint)_vertexShaderSamplers.Count);
                for (var i = 0; i < _vertexShaderSamplers.Count; ++i)
                {
                    command.Samplers[i] = _vertexShaderSamplers[i].SamplerState;
                }
                builder.Write(command);
            }
        }

        private unsafe void BuildInputs(CommandBufferBuilder builder)
        {
            if (_pixelShaderInputs.Any())
            {
                var command = new SetPixelShaderResourcesCommand((uint) _pixelShaderInputs.Count);
                for (var i = 0; i < command.NumberOfViews; ++i)
                {
                    command.Resources[i] = _pixelShaderInputs[i].D3DResource;
                }

                builder.Write(command);
            }

            if (_vertexShaderInputs.Any())
            {
                var command = new SetVertexShaderResourcesCommand((uint) _vertexShaderInputs.Count);
                for (var i = 0; i < command.NumberOfViews; ++i)
                {
                    command.Resources[i] = _pixelShaderInputs[i].D3DResource;
                }

                builder.Write(command);
            }
        }

        private unsafe void BuildOutputs(CommandBufferBuilder builder)
        {
            if (_outputs.Any())
            {
                var command = new SetRenderTargetsCommand((uint) _outputs.Count);
                for (var i = 0; i < command.NumberOfTargets; ++i)
                {
                    command.RenderTargets[i] = _outputs[i].D3DTarget;
                }

                if (_clearColor.HasValue)
                {
                    for (var i = 0; i < command.NumberOfTargets; ++i)
                    {
                        builder.Write(new ClearRenderTargetCommand(command.RenderTargets[i], _clearColor.Value));
                    }
                }
                builder.Write(command);
            }
        }
    }
}
