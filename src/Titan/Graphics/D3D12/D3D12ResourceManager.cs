using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Memory.Allocators;
using Titan.Core.Memory.Resources;
using Titan.Graphics.D3D12.Memory;
using Titan.Graphics.D3D12.Upload;
using Titan.Graphics.D3D12.Utils;
using Titan.Graphics.Resources;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D12;
using Titan.Platform.Win32.DXGI;
using static Titan.Platform.Win32.D3D12.D3D12_BUFFER_SRV_FLAGS;
using static Titan.Platform.Win32.D3D12.D3D12_SRV_DIMENSION;
using static Titan.Platform.Win32.DXGI.DXGI_FORMAT;

namespace Titan.Graphics.D3D12;

internal struct D3D12RootSignature
{
    public RootSignature RootSignature;
    public ComPtr<ID3D12RootSignature> Resource;

    public void Destroy() => Resource.Dispose();
}

internal unsafe class D3D12ResourceManager : IResourceManager
{
    private IResourcePool<D3D12Texture> _textures;
    private IResourcePool<D3D12Buffer> _buffers;
    private IResourcePool<D3D12Shader> _shaders;
    private IResourcePool<D3D12PipelineState> _pipelineState;
    private IResourcePool<D3D12RootSignature> _rootSignatures;
    private D3D12GraphicsDevice _device;
    private D3D12UploadQueue _uploadQueue;
    private D3D12Allocator _d3d12Allocator;
    private IGeneralAllocator _allocator;

    public bool Init(in GraphicsResourcesConfig config, IMemoryManager memoryManager, D3D12GraphicsDevice device, D3D12UploadQueue uploadQueue, D3D12Allocator d3D12Allocator)
    {
        Debug.Assert(_device == null && _uploadQueue == null);

        if (config.MaxBuffers == 0)
        {
            Logger.Error<D3D12ResourceManager>($"{nameof(config.MaxBuffers)} is 0");
            return false;
        }
        if (config.MaxTextures == 0)
        {
            Logger.Error<D3D12ResourceManager>($"{nameof(config.MaxTextures)} is 0");
            return false;
        }

        if (config.MaxShaders == 0)
        {
            Logger.Error<D3D12ResourceManager>($"{nameof(config.MaxShaders)} is 0");
            return false;
        }
        if (config.MaxPipelineStates == 0)
        {
            Logger.Error<D3D12ResourceManager>($"{nameof(config.MaxPipelineStates)} is 0");
            return false;
        }


        if (!FixedSizeResourcePool<D3D12Texture>.Create(memoryManager, config.MaxTextures, out _textures))
        {
            Logger.Error<D3D12ResourceManager>($"Failed to create the {nameof(IResourcePool<D3D12Texture>)} for {nameof(D3D12Texture)} ({config.MaxTextures})");
            goto Error;
        }

        if (!FixedSizeResourcePool<D3D12Buffer>.Create(memoryManager, config.MaxBuffers, out _buffers))
        {
            Logger.Error<D3D12ResourceManager>($"Failed to create the {nameof(IResourcePool<D3D12Buffer>)} for {nameof(D3D12Buffer)} ({config.MaxBuffers})");
            goto Error;
        }

        if (!FixedSizeResourcePool<D3D12PipelineState>.Create(memoryManager, config.MaxPipelineStates, out _pipelineState))
        {
            Logger.Error<D3D12ResourceManager>($"Failed to create the {nameof(IResourcePool<D3D12PipelineState>)} for {nameof(D3D12PipelineState)} ({config.MaxPipelineStates})");
            goto Error;
        }

        if (!FixedSizeResourcePool<D3D12Shader>.Create(memoryManager, config.MaxShaders, out _shaders))
        {
            Logger.Error<D3D12ResourceManager>($"Failed to create the {nameof(IResourcePool<D3D12Shader>)} for {nameof(D3D12Shader)}. ({config.MaxShaders})");
            goto Error;
        }

        if (!FixedSizeResourcePool<D3D12RootSignature>.Create(memoryManager, config.MaxRootSignatures, out _rootSignatures))
        {
            Logger.Error<D3D12ResourceManager>($"Failed to create the {nameof(IResourcePool<D3D12RootSignature>)} for {nameof(ID3D12RootSignature)}. ({config.MaxRootSignatures})");
            goto Error;
        }

        if (!memoryManager.TryCreateGeneralAllocator(config.MaxResourceBuffersSize, out _allocator))
        {
            Logger.Error<D3D12ResourceManager>($"Failed to create a {nameof(GeneralAllocator)} with size {config.MaxResourceBuffersSize} bytes.");
            goto Error;
        }



        _device = device;
        _uploadQueue = uploadQueue;
        _d3d12Allocator = d3D12Allocator;

        return true;

Error:
        Logger.Error<D3D12ResourceManager>("Error occurred, releasing resources.");
        _buffers.Release();
        _pipelineState.Release();
        _textures.Release();
        _shaders.Release();
        _rootSignatures.Release();
        _allocator?.Release();

        return false;
    }

    public Handle<Texture> CreateTexture(in CreateTextureArgs args)
    {
        var handle = _textures.SafeAlloc();
        if (handle.IsInvalid)
        {
            Logger.Error<D3D12ResourceManager>("Failed to create a Texture handle.");
            return 0;
        }

        ref var texture = ref _textures.Get(handle);
        var resource = _device.CreateTexture(args.Width, args.Height, args.Format.ToDXGIFormat());
        if (resource == null)
        {
            Logger.Error<D3D12ResourceManager>("Failed to create the texture resource.");
            _textures.SafeFree(handle);
            return 0;
        }

        // we use new here to initialize all values to 0.
        texture = new D3D12Texture
        {
            Resource = resource,
            Texture =
            {
                Format = args.Format,
                Width = args.Width,
                Height = args.Height
            }
        };

        if (args.ShaderVisible)
        {
            texture.SRV = _d3d12Allocator.Allocate(DescriptorHeapType.ShaderResourceView);
            if (!texture.SRV.IsValid)
            {
                Logger.Error<D3D12ResourceManager>($"Failed to allocate a {nameof(DescriptorHandle)} with type {DescriptorHeapType.ShaderResourceView}");
                texture.Resource.Dispose();
                _textures.SafeFree(handle);
                return 0;
            }
            _device.CreateShaderResourceView(texture.Resource, (DXGI_FORMAT)args.Format, texture.SRV);
        }

        if (args.InitialData.HasData())
        {
            Logger.Trace<D3D12ResourceManager>($"Uploading {args.InitialData.Length} bytes of texture data.");
            var uploadResult = _uploadQueue.Upload(texture.Resource, args.InitialData);
            if (!uploadResult)
            {
                Logger.Error<D3D12ResourceManager>($"Failed to upload {args.InitialData.Length} bytes of data.");
                texture.Destroy();
                _textures.Free(handle);
                return 0;
            }
        }

        return handle.Value;
    }

    public bool UploadTexture(in Handle<Texture> handle, TitanBuffer buffer)
    {
        ref var texture = ref _textures.Get(handle.Value);
        Logger.Trace<D3D12ResourceManager>($"Uploading {buffer.Length} bytes of texture data.");
        var uploadResult = _uploadQueue.Upload(texture.Resource, buffer);
        if (!uploadResult)
        {
            Logger.Error<D3D12ResourceManager>($"Failed to upload {buffer.Length} bytes of data.");
            return false;
        }
        return true;
    }

    public void Destroy(Handle<Texture> handle)
    {
        ref var texture = ref _textures.Get(handle.Value);
        if (texture.SRV.IsValid)
        {
            _d3d12Allocator.Free(texture.SRV);
        }
        if (texture.RTV.IsValid)
        {
            _d3d12Allocator.Free(texture.RTV);
        }
        texture.Destroy();
        _textures.Free(handle.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Texture* AccessTexture(Handle<Texture> handle) => (Texture*)_textures.GetPointer(handle.Value);
    public Handle<GPUBuffer> CreateBuffer(in CreateBufferArgs args)
    {
        var handle = _buffers.Alloc();
        if (handle.IsInvalid)
        {
            Logger.Error<D3D12ResourceManager>("Failed to create a buffer handle.");
            return 0;
        }

        var resourcestate = args.Type switch
        {
            BufferType.ConstantBuffer => D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_VERTEX_AND_CONSTANT_BUFFER,
            BufferType.IndexBuffer => D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_INDEX_BUFFER,
            BufferType.Common or _ => D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COMMON
        };

        var format = DXGI_FORMAT_UNKNOWN;
        if (args.Type == BufferType.IndexBuffer)
        {
            if (args.StrideInBytes == sizeof(ushort))
            {
                format = DXGI_FORMAT_R16_UINT;
            }
            else if (args.StrideInBytes == sizeof(uint))
            {
                format = DXGI_FORMAT_R32_UINT;
            }
            else
            {
                Logger.Error<D3D12ResourceManager>($"Incompatible stride size({args.StrideInBytes} bytes) for an index buffer. Size can be either {sizeof(ushort)} or {sizeof(uint)}");
                _buffers.Free(handle);
                return 0;
            }
        }

        var size = args.Count * args.StrideInBytes;
        var resource = _device.CreateBuffer(size, args.CpuVisible, resourcestate);
        if (resource == null)
        {
            Logger.Error<D3D12ResourceManager>($"Failed to create the {nameof(D3D12Buffer)}");
            _buffers.Free(handle);
            return 0;
        }

        if (args.InitialData.HasData())
        {
            Debug.Assert(args.InitialData.Length <= size, $"The initial data provided is bigger than the size of the buffer. Buffer: {size} bytes Data: {args.InitialData.Length} bytes");
            if (args.CpuVisible)
            {
                void* pData;
                resource->Map(0, null, &pData);
                MemoryUtils.Copy(pData, args.InitialData);
                resource->Unmap(0, null);
            }
            else
            {
                Debug.Fail("This is currently not supported. Must add support for this in the UploadQueue.");
            }
        }

        //NOTE(Jens): Some examples creates multiple SRVs based on the number of render frames. Need to find some documentation about that and how to do it properly.

        var desc = new D3D12_SHADER_RESOURCE_VIEW_DESC
        {
            Format = DXGI_FORMAT_UNKNOWN, // not sure what  to set here?
            ViewDimension = D3D12_SRV_DIMENSION_BUFFER,
            Shader4ComponentMapping = D3D12Constants.D3D12_DEFAULT_SHADER_4_COMPONENT_MAPPING,
            Buffer = new()
            {
                FirstElement = 0,
                Flags = D3D12_BUFFER_SRV_FLAG_NONE,
                NumElements = args.Count,
                StructureByteStride = args.StrideInBytes
            }
        };

        DescriptorHandle srvHandle = default;
        if (args.ShaderVisible)
        {
            srvHandle = _d3d12Allocator.Allocate(DescriptorHeapType.ShaderResourceView);
            _device.Device->CreateShaderResourceView(resource, &desc, srvHandle);
        }

        ref var buffer = ref _buffers.Get(handle);
        buffer = new D3D12Buffer
        {
            Buffer = new GPUBuffer(args.Type),
            Resource = resource,
            SRV = srvHandle,
            GPUAddress = resource->GetGPUVirtualAddress(),
            Format = format,
            Size = size
        };
        return handle.Value;
    }

    public void DestroyBuffer(Handle<GPUBuffer> handle)
    {
        ref var buffer = ref _buffers.Get(handle.Value);
        if (buffer.SRV.IsValid)
        {
            _d3d12Allocator.Free(buffer.SRV);
        }
        buffer.Destroy();
        _buffers.Free(handle.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GPUBuffer* AccessBuffer(Handle<GPUBuffer> handle) => (GPUBuffer*)_buffers.GetPointer(handle.Value);

    public Handle<Shader> CreateShader(in CreateShaderArgs args)
    {
        var handle = _shaders.Alloc();
        if (handle.IsInvalid)
        {
            Logger.Error<D3D12ResourceManager>("Failed to create a Shader handle.");
            return 0;
        }
        ref var shader = ref _shaders.Get(handle);
        var buffer = _allocator.Allocate((uint)args.Data.Length, false);
        if (buffer == null)
        {
            Logger.Error<D3D12ResourceManager>($"Failed to allocate {args.Data.Length} bytes for the shader byte code.");
            return 0;
        }
        MemoryUtils.Copy(buffer, args.Data);

        shader.ByteCode = new D3D12_SHADER_BYTECODE
        {
            BytecodeLength = (nuint)args.Data.Length,
            pShaderBytecode = buffer
        };

        return handle.Value;
    }

    public void DestroyShader(Handle<Shader> handle)
    {
        if (handle.IsValid)
        {
            ref var shader = ref _shaders.Get(handle.Value);
            _allocator.Free(shader.ByteCode.pShaderBytecode);
            _shaders.Free(handle.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Shader* AccessShader(Handle<Shader> handle) => (Shader*)_shaders.GetPointer(handle.Value);

    public Handle<PipelineState> CreatePipelineState(in CreatePipelineStateArgs args)
    {
        var handle = _pipelineState.Alloc();
        if (handle.IsInvalid)
        {
            Logger.Error<D3D12ResourceManager>("Failed to create a PSO handle.");
            return 0;
        }

        //NOTE(Jens): We probably want to make it possible to omit these values, and make it more dynamic (for example compute shaders). But keep it simple for now.
        var vertexShader = _shaders.GetPointer(args.VertexShader.Value);
        var pixelShader = _shaders.GetPointer(args.PixelShader.Value);
        var rootSignature = _rootSignatures.GetPointer(args.RootSignature.Value);

        var topology = (D3D12_PRIMITIVE_TOPOLOGY_TYPE)args.PrimitiveTopology;

        Span<DXGI_FORMAT> renderTargetFormats = stackalloc DXGI_FORMAT[args.RenderTargetFormats.Length];
        for (var i = 0; i < args.RenderTargetFormats.Length; ++i)
        {
            renderTargetFormats[i] = args.RenderTargetFormats[i].ToDXGIFormat();
        }

        const int BufferSize = 1024;
        var buffer = stackalloc byte[BufferSize];
        var streamDesc = new D3D12PipelineSubobjectStream(buffer, BufferSize)
            .VS(vertexShader->ByteCode)
            .PS(pixelShader->ByteCode)
            .RootSignature(rootSignature->Resource)
            .Topology(topology)
            .RenderTargetFormat(new D3D12_RT_FORMAT_ARRAY(renderTargetFormats))

            // Need to add options for these
            .Razterizer(D3D12_RASTERIZER_DESC.Default())
            .Blend(D3D12Helpers.GetBlendState(args.BlendState))
            .DepthStencil(new D3D12_DEPTH_STENCIL_DESC { DepthEnable = 0, StencilEnable = 0 })
            .SampleMask(uint.MaxValue)
            .Sample(new DXGI_SAMPLE_DESC { Count = 1, Quality = 0 })
            .ToStreamDesc();

        var pipelineState = _device.CreatePipelineState(streamDesc);
        if (pipelineState == null)
        {
            Logger.Error<D3D12ResourceManager>("Failed to create the PipelineState");
            _pipelineState.Free(handle);
            return 0;
        }
        ref var state = ref _pipelineState.Get(handle);
        state.PipelineState = pipelineState;
        return handle.Value;

    }

    public void DestroyPipelineState(Handle<PipelineState> handle)
    {
        if (handle.IsValid)
        {
            ref var state = ref _pipelineState.Get(handle.Value);
            state.PipelineState.Dispose();
            _pipelineState.Free(handle.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PipelineState* AccessPipelineState(Handle<PipelineState> handle) => (PipelineState*)_pipelineState.GetPointer(handle.Value);

    public Handle<RootSignature> CreateRootSignature(in CreateRootSignatureArgs args)
    {
        var handle = _rootSignatures.Alloc();
        if (handle.IsInvalid)
        {
            Logger.Error<D3D12ResourceManager>("Failed to create a RootSignature handle.");
            return 0;
        }
        var signature = _device.CreateRootSignature(args.Flags, args.Parameters, args.StaticSamplers);

        if (signature == null)
        {
            Logger.Error<D3D12ResourceManager>("Failed to create the RootSignature");
            _rootSignatures.Free(handle);
            return 0;
        }
        ref var rootSignature = ref _rootSignatures.Get(handle);
        rootSignature.Resource = signature;
        rootSignature.RootSignature = default;
        return handle.Value;
    }

    public void DestroyRootSignature(Handle<RootSignature> handle)
    {
        if (handle.IsValid)
        {
            ref var signature = ref _rootSignatures.Get(handle.Value);
            signature.Destroy();
            _rootSignatures.Free(handle.Value);
        }
    }

    public RootSignature* AccessRootSignature(Handle<RootSignature> handle) => (RootSignature*)_rootSignatures.GetPointer(handle.Value);

    public void Shutdown()
    {
        // Loop through all allocated values?
        _buffers.Release();
        _textures.Release();
        _pipelineState.Release();
        _shaders.Release();
        _rootSignatures.Release();
        _allocator.Release();
    }
}

