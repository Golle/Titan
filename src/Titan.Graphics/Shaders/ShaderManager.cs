using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Shaders
{
    internal unsafe class ShaderManager : IShaderManager
    {
        private ComPtr<ID3D11Device> _device;

        private readonly IMemoryManager _memoryManager;
        private readonly IShaderCompiler _shaderCompiler;

        private readonly IDictionary<PixelShaderDescriptor, PixelShaderHandle> _pixelShaderCache = new Dictionary<PixelShaderDescriptor, PixelShaderHandle>();
        private readonly IDictionary<VertexShaderDescriptor, VertexShaderHandle> _vertexShaderCache = new Dictionary<VertexShaderDescriptor, VertexShaderHandle>();
        private readonly IDictionary<string, InputLayoutHandle> _inputLayoutCache = new Dictionary<string, InputLayoutHandle>();

        private readonly IDictionary<string, ShaderProgram> _cachedPrograms = new Dictionary<string, ShaderProgram>();

        private void* _memory;
        private uint _maxHandles;

        private int _handle;
        private string _assetsPath;


        public ShaderManager(IMemoryManager memoryManager, IShaderCompiler shaderCompiler)
        {
            _memoryManager = memoryManager;
            _shaderCompiler = shaderCompiler;
        }

        public void Initialize(IGraphicsDevice graphicsDevice, string assetsPath)
        {
            Debug.Assert(sizeof(VertexShader) == sizeof(PixelShader) && sizeof(PixelShader) == sizeof(InputLayout), "VertexShader, PixelShader and InputLayout must be of the same size.");

            if (_memory != null)
            {
                throw new InvalidOperationException($"{nameof(ShaderManager)} has already been initialized.");
            }
            _device = graphicsDevice is D3D11GraphicsDevice device ? new ComPtr<ID3D11Device>(device.Ptr) : throw new ArgumentException($"Trying to initialize a D3D11 {nameof(ShaderManager)} with the wrong device.", nameof(graphicsDevice));
            var memory = _memoryManager.GetMemoryChunk("Shaders");
            _memory = memory.Pointer;
            _maxHandles = memory.Count;
            _assetsPath = assetsPath;
        }

        public ShaderProgram GetByName(string name) => _cachedPrograms.TryGetValue(name, out var shader) ? shader : throw new KeyNotFoundException($"Shader with name {name} does not exist");
        public ShaderProgram AddShader(string name, in VertexShaderDescriptor vertexShaderDescriptor, in PixelShaderDescriptor pixelShaderDescriptor, in InputLayoutDescriptor[] layout)
        {
            if (_cachedPrograms.ContainsKey(name))
            {
                throw new InvalidOperationException($"Shader with name {name} has already been registed");
            }

            var inputLayoutKey = CreateInputLayoutKey(layout);
            var hasCachedInputHandle = _inputLayoutCache.TryGetValue(inputLayoutKey, out var inputHandle);

            if (!_vertexShaderCache.TryGetValue(vertexShaderDescriptor, out var vertexShaderHandle))
            {
                vertexShaderHandle = NextHandle();
                var filename = Path.Combine(_assetsPath, vertexShaderDescriptor.Filename);
                using var shader = _shaderCompiler.CompileShaderFromFile(filename, vertexShaderDescriptor.Entrypoint, vertexShaderDescriptor.Version, vertexShaderDescriptor.Defines);
                Common.CheckAndThrow(_device.Get()->CreateVertexShader(shader.Buffer, shader.BufferSize, null, &((VertexShader*)_memory)[vertexShaderHandle].Pointer), "CreateVertexShader");
                _vertexShaderCache.Add(vertexShaderDescriptor, vertexShaderHandle);
                if (!hasCachedInputHandle)
                {
                    inputHandle = CreateInputLayout(shader, layout);
                    _inputLayoutCache.Add(inputLayoutKey, inputHandle);
                }
            }

            var pixelShaderHandle = GetOrCreatePixelShader(pixelShaderDescriptor);
            
            return _cachedPrograms[name] = new ShaderProgram(vertexShaderHandle, pixelShaderHandle, inputHandle);
        }

        private PixelShaderHandle GetOrCreatePixelShader(PixelShaderDescriptor pixelShaderDescriptor)
        {
            if (!_pixelShaderCache.TryGetValue(pixelShaderDescriptor, out var pixelShaderHandle))
            {
                pixelShaderHandle = NextHandle();
                var filename = Path.Combine(_assetsPath, pixelShaderDescriptor.Filename);
                using var shader = _shaderCompiler.CompileShaderFromFile(filename, pixelShaderDescriptor.Entrypoint, pixelShaderDescriptor.Version, pixelShaderDescriptor.Defines);
                
                Common.CheckAndThrow(_device.Get()->CreatePixelShader(shader.Buffer, shader.BufferSize, null, &((PixelShader*) _memory)[pixelShaderHandle].Pointer), "CreatePixelShader");
                _pixelShaderCache.Add(pixelShaderDescriptor, pixelShaderHandle);
            }

            return pixelShaderHandle;
        }

        private InputLayoutHandle CreateInputLayout(CompiledShader shader, in InputLayoutDescriptor[] layout)
        {
            static uint GetSize(DXGI_FORMAT format) =>
                (uint)(format switch
                {
                    DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT => sizeof(Vector2),
                    DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT => sizeof(Vector3),
                    DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT => sizeof(Vector4),
                    _ => throw new NotSupportedException($"Format {format} is not supported.")
                });
            
            var handle = NextHandle();

            var inputDescs = stackalloc D3D11_INPUT_ELEMENT_DESC[layout.Length];
            var size = 0u;
            for (var i = 0; i < layout.Length; ++i)
            {
                var (name, format, slot, classification) = layout[i];
                
                ref var desc = ref inputDescs[i];
                fixed (byte* pName = name.AsBytes())
                {
                    desc.SemanticName = (sbyte*)pName;
                }

                //TODO: This needs some changes to support different things properly
                desc.InstanceDataStepRate = classification == D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_INSTANCE_DATA ? 1 : 0;
                //desc.InputSlot = classification == D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_INSTANCE_DATA ? 1 : 0; //TODO: How do we support instance data?
                desc.InputSlot = slot;
                desc.InputSlotClass = classification;
                desc.SemanticIndex = classification == D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_INSTANCE_DATA ? 1 : 0;
                desc.AlignedByteOffset = classification == D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_INSTANCE_DATA ? 0 : size;
                desc.Format = format;

                size += GetSize(format);
            }

            Common.CheckAndThrow(_device.Get()->CreateInputLayout(inputDescs, (uint)layout.Length, shader.Buffer, shader.BufferSize, &((InputLayout*)_memory)[handle].Pointer), "CreateInputLayout");
            return handle;
        }
        
        private int NextHandle() => Interlocked.Increment(ref _handle) - 1;
        private static string CreateInputLayoutKey(in InputLayoutDescriptor[] layout) => string.Join('#', layout.Select(l => $"{l.Name}:{l.Format}:{l.Classification}".ToLowerInvariant()));
        
        public ref readonly InputLayout this[in InputLayoutHandle handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref ((InputLayout*) _memory)[handle];
        }

        public ref readonly PixelShader this[in PixelShaderHandle handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref ((PixelShader*) _memory)[handle];
        }

        public ref readonly VertexShader this[in VertexShaderHandle handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref ((VertexShader*) _memory)[handle];
        }

        public void Dispose()
        {
            if (_memory != null)
            {
                for (var i = 0; i < _handle; ++i)
                {
                    ref var pUnknown = ref ((IUnknown**)_memory)[i];
                    pUnknown->Release();
                    pUnknown = null;
                }
                _handle = 0;
                _device.Dispose();
                _memory = null;
            }
        }
    }
}
