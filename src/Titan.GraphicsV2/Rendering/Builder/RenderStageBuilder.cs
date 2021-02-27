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

namespace Titan.GraphicsV2.Rendering.Builder
{
    internal class RenderStageBuilder
    {
        private readonly RenderStages _renderStage;
        private readonly IDictionary<string, Handle<Texture>> _framebuffers;
        private readonly IDictionary<string, Handle<Sampler>> _samplers;
        private readonly IDictionary<string, Handle<Shader>> _shaders;

        private readonly List<Handle<Texture>> _outputs = new();
        private readonly List<Handle<Texture>> _pixelShaderInputs = new();
        private readonly List<Handle<Texture>> _vertexShaderInputs = new();
        private readonly List<Handle<Sampler>> _vertexShaderSamplers = new();
        private readonly List<Handle<Sampler>> _pixelShaderSamplers = new();


        private Handle<Shader>? _shader;

        
        private Color? _clearColor;

        public RenderStageBuilder(string name, RenderStages renderStage, IDictionary<string, Handle<Texture>> framebuffers, IDictionary<string, Handle<Sampler>> samplers, IDictionary<string, Handle<Shader>> shaders)
        {
            _renderStage = renderStage;
            _framebuffers = framebuffers;
            _samplers = samplers;
            _shaders = shaders;
            LOGGER.Debug("Stage: {0}", name);
        }

        internal void AddInput(string name, RenderBindingTypes type)
        {
            var framebuffer = _framebuffers[name];
            LOGGER.Debug("Input: {0}({1})  Framebuffer: {2}", name, type, framebuffer.Value);
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
            LOGGER.Debug("Output: {0} Framebuffer: {1}", renderTarget, framebuffer.Value);
            _outputs.Add(framebuffer);
        }

        internal void ClearRenderTargets(in Color color)
        {
            _clearColor = color;
        }

        internal void AddSampler(string name, RenderBindingTypes type)
        {
            var sampler = _samplers[name];
            LOGGER.Debug("Sampler: {0}({1})  Handle: {2}", name, type, sampler.Value);
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

        internal RenderStage Build(Device device)
        {
            using var builder = new CommandBufferBuilder();
            // Begin/Setup state
            BuildOutputs(device, builder);
            BuildInputs(device, builder);
            BuildSamplers(device, builder);
            BuildShader(device, builder);

            // Execute the renderer
            //builder.Write(new RenderCommand());

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
            
            

            return new RenderStage(builder.Build());
        }

        private void BuildShader(Device device, CommandBufferBuilder builder)
        {
            if (!_shader.HasValue)
            {
                return;
            }
            var shader = device.ShaderManager.Access(_shader.Value);
            unsafe
            {
                builder.Write(new SetShadersCommand(shader.VertexShader, shader.PixelShader, shader.InputLayout));
            }
        }

        private unsafe void BuildSamplers(Device device, CommandBufferBuilder builder)
        {
            if (_pixelShaderSamplers.Any())
            {
                var command = new SetPixelShaderSamplersCommand((uint) _pixelShaderSamplers.Count);
                for (var i = 0; i < _pixelShaderSamplers.Count; ++i)
                {
                    command.Samplers[i] = device.SamplerManager.Access(_pixelShaderSamplers[i]).SamplerState;
                }
                builder.Write(command);
            }
            if (_vertexShaderSamplers.Any())
            {
                var command = new SetVertexShaderSamplersCommand((uint)_vertexShaderSamplers.Count);
                for (var i = 0; i < _vertexShaderSamplers.Count; ++i)
                {
                    command.Samplers[i] = device.SamplerManager.Access(_vertexShaderSamplers[i]).SamplerState;
                }
                builder.Write(command);
            }
        }

        private unsafe void BuildInputs(Device device, CommandBufferBuilder builder)
        {
            if (_pixelShaderInputs.Any())
            {
                var command = new SetPixelShaderResourcesCommand((uint) _pixelShaderInputs.Count);
                for (var i = 0; i < command.NumberOfViews; ++i)
                {
                    command.Resources[i] = device.TextureManager.Access(_pixelShaderInputs[i]).D3DResource;
                }

                builder.Write(command);
            }

            if (_vertexShaderInputs.Any())
            {
                var command = new SetVertexShaderResourcesCommand((uint) _vertexShaderInputs.Count);
                for (var i = 0; i < command.NumberOfViews; ++i)
                {
                    command.Resources[i] = device.TextureManager.Access(_pixelShaderInputs[i]).D3DResource;
                }

                builder.Write(command);
            }
        }

        private unsafe void BuildOutputs(Device device, CommandBufferBuilder builder)
        {
            if (_outputs.Any())
            {
                var command = new SetRenderTargetsCommand((uint) _outputs.Count);
                for (var i = 0; i < command.NumberOfTargets; ++i)
                {
                    command.RenderTargets[i] = device.TextureManager.Access(_outputs[i]).D3DTarget;
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
