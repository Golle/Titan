using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Windows;
using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11.Buffers;

public unsafe class BufferManager : IDisposable
{
    private readonly ID3D11Device* _device;
    private ResourcePool<ResourceBuffer> _resourcePool;
    private const uint MaxBuffers = 1000u;
    internal BufferManager(ID3D11Device* device)
    {
        Logger.Trace<BufferManager>($"Init with {MaxBuffers} slots");
        _resourcePool.Init(MaxBuffers);
        _device = device;
    }

    public Handle<ResourceBuffer> Create(BufferCreation args)
    {
        Logger.Trace<BufferManager>($"Create {args.Type} with stride {args.Stride} usage {args.Usage}. InitialData: {args.InitialData.HasValue()}");
        var handle = _resourcePool.CreateResource();
        if (!handle.IsValid())
        {
            throw new InvalidOperationException("Failed to Create a Buffer Handle");
        }

        var bindFlag = args.Type switch
        {
            BufferTypes.ConstantBuffer => D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
            BufferTypes.IndexBuffer => D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER,
            BufferTypes.VertexBuffer => D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER,
            BufferTypes.UnorderedAccess => D3D11_BIND_FLAG.D3D11_BIND_UNORDERED_ACCESS | D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE,
            _ => throw new ArgumentOutOfRangeException()
        };

        var desc = new D3D11_BUFFER_DESC
        {
            BindFlags = bindFlag,
            Usage = args.Usage,
            CpuAccessFlags = args.CpuAccessFlags,
            MiscFlags = args.MiscFlags,
            ByteWidth = args.Count * args.Stride,
            StructureByteStride = args.Stride
        };

        var buffer = _resourcePool.GetResourcePointer(handle);
        buffer->Handle = handle;
        buffer->BindFlag = bindFlag;
        buffer->Count = args.Count;
        buffer->CpuAccessFlag = args.CpuAccessFlags;
        buffer->MiscFlag = args.MiscFlags;
        buffer->Stride = args.Stride;
        buffer->Usage = args.Usage;

        if (args.InitialData.HasValue())
        {
            var subResource = new D3D11_SUBRESOURCE_DATA
            {
                pSysMem = args.InitialData
            };
            Common.CheckAndThrow(_device->CreateBuffer(&desc, &subResource, &buffer->Resource), nameof(ID3D11Device.CreateBuffer));
        }
        else
        {
            Common.CheckAndThrow(_device->CreateBuffer(&desc, null, &buffer->Resource), nameof(ID3D11Device.CreateBuffer));
        }
        return handle;
    }

    public void Release(in Handle<ResourceBuffer> handle)
    {
        Logger.Trace<BufferManager>($"Releasing buffer with handle {handle}");
        ReleaseInternal(handle);
        _resourcePool.ReleaseResource(handle);
    }

    private void ReleaseInternal(Handle<ResourceBuffer> handle)
    {
        var buffer = _resourcePool.GetResourcePointer(handle);
        if (buffer->Resource != null)
        {
            buffer->Resource->Release();
        }
        *buffer = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref readonly ResourceBuffer Access(in Handle<ResourceBuffer> handle) => ref _resourcePool.GetResourceReference(handle);

    public void Dispose()
    {
        foreach (var handle in _resourcePool.EnumerateUsedResources())
        {
            Logger.Warning<BufferManager>($"Releasing an unreleased Buffer with handle {handle.Value}");
            ReleaseInternal(handle);
        }
        Logger.Trace<BufferManager>("Terminate resource pool");
        _resourcePool.Terminate();
    }
}
