using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Common;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Shaders;

namespace Titan.GraphicsV2.Resources
{
    public record PixelShaderDescriptor(string Identifier, string EntryPoint = "main", string Version = "ps_5_0");
    public record VertexShaderDescriptor(string Identifier, string EntryPoint = "main", string Version = "vs_5_0");
    public record ShaderProgramDescriptor(string Name, VertexShaderDescriptor VertexShader, PixelShaderDescriptor PixelShader, InputLayoutDescriptor[] InputLayout);

    public struct ShaderProgram
    {
        internal PixelShader PixelShader;
        internal VertexShader VertexShader;
        internal InputLayout InputLayout;
    }

    public class ShaderProgramHandler : IDisposable
    {
        private readonly ShaderCompiler _compiler;
        private readonly ShaderFactory _shaderFactory;
        private readonly InputLayoutFactory _inputLayoutFactory;


        private readonly ShaderProgram[] _shaderPrograms = new ShaderProgram[100];
        private volatile int _count;

        private readonly Dictionary<string, VertexShader> _vertexShaders = new();
        private readonly Dictionary<string, PixelShader> _pixelShaders = new();
        private readonly Dictionary<string, InputLayout> _inputLayouts = new();
        private readonly Dictionary<string, int> _shaderProgramLookup = new();

        internal ShaderProgramHandler(ShaderCompiler compiler, ShaderFactory shaderFactory, InputLayoutFactory inputLayoutFactory)
        {
            _compiler = compiler;
            _shaderFactory = shaderFactory;
            _inputLayoutFactory = inputLayoutFactory;
        }

        public Handle<ShaderProgram> Load(ShaderProgramDescriptor descriptor)
        {
            if (_shaderProgramLookup.TryGetValue(descriptor.Name, out var handle))
            {
                return handle;
            }

            var inputLayoutKey = CreateInputLayoutKey(descriptor.InputLayout);
            var hasInputLayout = _inputLayouts.TryGetValue(inputLayoutKey, out var inputLayout);

            if (!_vertexShaders.TryGetValue(descriptor.VertexShader.Identifier, out var vertexShader))
            {
                var shader = _compiler.CompileFromFile(descriptor.VertexShader.Identifier, descriptor.VertexShader.EntryPoint, descriptor.VertexShader.Version);
                vertexShader = _shaderFactory.CreateVertexShader(shader);
                _vertexShaders.Add(descriptor.VertexShader.Identifier, vertexShader);
                if (!hasInputLayout)
                {
                    inputLayout = _inputLayoutFactory.Create(shader, descriptor.InputLayout);
                    _inputLayouts.Add(inputLayoutKey, inputLayout);
                }
            }

            if (!_pixelShaders.TryGetValue(descriptor.PixelShader.Identifier, out var pixelShader))
            {
                var shader = _compiler.CompileFromFile(descriptor.PixelShader.Identifier, descriptor.PixelShader.EntryPoint, descriptor.PixelShader.Version);
                pixelShader = _shaderFactory.CreatePixelShader(shader);
                _pixelShaders.Add(descriptor.PixelShader.Identifier, pixelShader);
            }
            
            handle = Interlocked.Increment(ref _count) - 1;
            _shaderProgramLookup[descriptor.Name] = handle;

            _shaderPrograms[handle] = new ShaderProgram
            {
                VertexShader = vertexShader,
                PixelShader = pixelShader,
                InputLayout = inputLayout
            };
            return handle;
        }

        private string CreateInputLayoutKey(InputLayoutDescriptor[] descriptors) => string.Join('&',descriptors.Select(d => $"{d.Name}:{(int) d.Format}:{(int) d.Classification}:{d.Slot}"));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref readonly ShaderProgram Get(in Handle<ShaderProgram> handle) => ref _shaderPrograms[handle.Value];

        
        public void Dispose()
        {
            _count = 0;
            foreach (var shader in _vertexShaders.Values)
            {
                shader.Release();
            }
            foreach (var shader in _pixelShaders.Values)
            {
                shader.Release();
            }
            foreach (var layout in _inputLayouts.Values)
            {
                layout.Release();
            }
        }
    }
}
