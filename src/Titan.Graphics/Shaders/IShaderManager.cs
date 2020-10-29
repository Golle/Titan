using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Shaders;

namespace Titan.Graphics.Shaders
{
    public interface IShaderManager : IDisposable
    {
        void AddShaderProgram(string name, VertexShaderDescriptor vsDescriptor, PixelShaderDescriptor psDescriptor, in InputLayoutDescriptor[] inputLayoutDescriptor);

        uint GetHandle(string name);
        ShaderProgram Get(uint handle);
    }

    public record VertexShaderDescriptor(string Filename, ShaderDefines[] Defines = default);

    public record PixelShaderDescriptor(string Filename, ShaderDefines[] Defines = default);

    internal class ShaderManager : IShaderManager
    {
        private readonly IShaderLoader _loader;

        private readonly ShaderProgram[] _shaderPrograms = new ShaderProgram[200];
        private uint _shaderProgramsCount;

        private readonly IDictionary<string, VertexShader> _vertexShaders = new Dictionary<string, VertexShader>();
        private readonly IDictionary<string, PixelShader> _pixelShaders = new Dictionary<string, PixelShader>();
        private readonly IDictionary<string, InputLayout> _inputLayouts = new Dictionary<string, InputLayout>();

        public ShaderManager(IShaderLoader loader)
        {
            _loader = loader;
        }

        public void AddShaderProgram(string name, VertexShaderDescriptor vsDescriptor, PixelShaderDescriptor psDescriptor, in InputLayoutDescriptor[] inputLayoutDescriptor)
        {
            Debug.Assert(!_shaderPrograms.Take((int) _shaderProgramsCount).Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)), $"ShaderProgram with name {name} already exists.");

            var psKey = GetKey(psDescriptor.Filename, psDescriptor.Defines);
            if (!_pixelShaders.TryGetValue(psKey, out var pixelShader))
            {
                pixelShader = _loader.CreatePixelShader(psDescriptor);
                _pixelShaders.Add(psKey, pixelShader);
            }

            var inputKey = string.Join("#", inputLayoutDescriptor.Select(i => $"{i.Name}:{i.Format}:{i.Classification}"));
            _inputLayouts.TryGetValue(inputKey, out var inputLayout);

            var vsKey = GetKey(vsDescriptor.Filename, vsDescriptor.Defines);
            if (!_vertexShaders.TryGetValue(vsKey, out var vertexShader))
            {
                if (inputLayout == null)
                {
                    (vertexShader, inputLayout) = _loader.CreateVertexShaderAndInputLayout(vsDescriptor, inputLayoutDescriptor);
                    _inputLayouts.Add(inputKey, inputLayout);
                }
                else
                {
                    vertexShader = _loader.CreateVertexShader(vsDescriptor);
                }
                _vertexShaders.Add(vsKey, vertexShader);
            }
            
            _shaderPrograms[_shaderProgramsCount++] = new ShaderProgram(name, inputLayout, vertexShader, pixelShader);
        }

        public uint GetHandle(string name)
        {
            for (var i = 0u; i < _shaderProgramsCount; ++i)
            {
                var program = _shaderPrograms[i];
                if (program.Name == name)
                {
                    return i;
                }
            }
            throw new KeyNotFoundException($"ShaderProgram with name {name} has not been loaded");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShaderProgram Get(uint handle) => _shaderPrograms[handle];

        private static string GetKey(string filename, IReadOnlyCollection<ShaderDefines> defines) => (defines == null || defines.Count == 0) ? filename : $"{filename}-{string.Join("-", defines.Select(d => $"{d.Name}:{d.Value}"))}";

        public void Dispose()
        {
            foreach (var inputLayout in _inputLayouts.Values)
            {
                inputLayout.Dispose();
            }

            foreach (var pixelShader in _pixelShaders.Values)
            {
                pixelShader.Dispose();
            }

            foreach (var vertexShader in _vertexShaders.Values)
            {
                vertexShader.Dispose();
            }
            Array.Clear(_shaderPrograms, 0, _shaderPrograms.Length);
            _vertexShaders.Clear();
            _pixelShaders.Clear();
            _inputLayouts.Clear();
        }
    }
}
