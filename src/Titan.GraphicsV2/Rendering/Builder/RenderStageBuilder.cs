using System;
using System.Collections.Generic;
using System.Linq;
using Titan.Core.Common;
using Titan.Core.Logging;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Textures;
using Titan.GraphicsV2.Rendering.Commands;

namespace Titan.GraphicsV2.Rendering.Builder
{
    internal class RenderStageBuilder
    {
        private readonly IDictionary<string, Handle<Texture>> _framebuffers;
        private readonly List<Handle<Texture>> _outputs = new();
        private readonly List<Handle<Texture>> _pixelShaderInputs = new();
        private readonly List<Handle<Texture>> _vertexShaderInputs = new();

        
        private Color? _clearColor;

        public RenderStageBuilder(string name, IDictionary<string, Handle<Texture>> framebuffers)
        {
            _framebuffers = framebuffers;
            LOGGER.Debug("Stage: {0}", name);
        }

        internal void AddInput(string name, RenderInputTypes type)
        {
            var framebuffer = _framebuffers[name];
            LOGGER.Debug("Input: {0}({1})  Framebuffer: {2}", name, type, framebuffer.Value);
            switch (type)
            {
                case RenderInputTypes.PixelShader: _pixelShaderInputs.Add(framebuffer);break;
                case RenderInputTypes.VertexShader: _vertexShaderInputs.Add(framebuffer);break;
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

        internal RenderStage Build(Device device)
        {
            using var builder = new CommandBufferBuilder();
            BuildOutputs(device, builder);
            BuilderInputs(device, builder);
            


            return new RenderStage(builder.Build());
        }

        private unsafe void BuilderInputs(Device device, CommandBufferBuilder builder)
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
