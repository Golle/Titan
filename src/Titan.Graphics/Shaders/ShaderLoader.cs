using System;
using System.Reflection.Metadata.Ecma335;
using Titan.Core;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Shaders;

namespace Titan.Graphics.Shaders
{
    internal class ShaderLoader : IShaderLoader
    {
        private readonly TitanConfiguration _configuration;
        private readonly IGraphicsDevice _device;
        private readonly IShaderCompiler _shaderCompiler;

        public ShaderLoader(TitanConfiguration configuration, IGraphicsDevice device, IShaderCompiler shaderCompiler)
        {
            _configuration = configuration;
            _device = device;
            _shaderCompiler = shaderCompiler;
        }

        public VertexShader CreateVertexShader(VertexShaderDescriptor descriptor)
        {
            using var shader = _shaderCompiler.CompileShaderFromFile(_configuration.GetPath(descriptor.Filename), descriptor.Entrypoint, descriptor.Version, descriptor.Defines ?? Array.Empty<ShaderDefines>());
            
            return new VertexShader(_device, shader);
        }

        public (VertexShader vertexshader, InputLayout inputLayout) CreateVertexShaderAndInputLayout(VertexShaderDescriptor descriptor, in InputLayoutDescriptor[] layout)
        {
            using var shader = _shaderCompiler.CompileShaderFromFile(_configuration.GetPath(descriptor.Filename), descriptor.Entrypoint, descriptor.Version, descriptor.Defines ?? Array.Empty<ShaderDefines>());
            var inputLayout = new InputLayout(_device, shader, layout);
            var vertexShader = new VertexShader(_device, shader);
            
            return (vertexShader, inputLayout);
        }

        public PixelShader CreatePixelShader(PixelShaderDescriptor descriptor)
        {
            var shader = _shaderCompiler.CompileShaderFromFile(_configuration.GetPath(descriptor.Filename), descriptor.Entrypoint, descriptor.Version, descriptor.Defines ?? Array.Empty<ShaderDefines>());
            
            return new PixelShader(_device, shader);
        }
    }
}
