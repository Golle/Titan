using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Graphics.D3D12;
using Titan.Graphics.Resources;
using Titan.Platform.Win32.D3D;
using Titan.Platform.Win32.D3D12;
using static Titan.Platform.Win32.Win32Common;

namespace Titan.Graphics.Rendering;

public readonly unsafe ref struct D3D12CommandList
{
    private readonly ID3D12GraphicsCommandList* _commandlist;
    private readonly ID3D12CommandAllocator* _allocator;
    private readonly IResourceManager _resourceManager;

    internal D3D12CommandList(ID3D12GraphicsCommandList* commandlist, ID3D12CommandAllocator* allocator, IResourceManager resourceManager)
    {
        _commandlist = commandlist;
        _allocator = allocator;
        _resourceManager = resourceManager;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetRootSignature(Handle<RootSignature> handle)
    {
        var resource = (D3D12RootSignature*)_resourceManager.AccessRootSignature(handle);
        Debug.Assert(resource != null && resource->Resource.Get() != null);
        _commandlist->SetGraphicsRootSignature(resource->Resource.Get());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPipelineState(Handle<PipelineState> handle)
    {
        var resource = (D3D12PipelineState*)_resourceManager.AccessPipelineState(handle);
        Debug.Assert(resource != null && resource->PipelineState.Get() != null);
        _commandlist->SetPipelineState(resource->PipelineState.Get());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetIndexBuffer(Handle<GPUBuffer> handle)
    {
        var resource = (D3D12Buffer*)_resourceManager.AccessBuffer(handle);
        Debug.Assert(resource->Resource.Get() != null);
        var indexBufferView = resource->IndexBufferView();
        _commandlist->IASetIndexBuffer(&indexBufferView);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawIndexInstanced(uint indexCountPerInstance, uint instanceCount, uint startIndexLocation = 0u, int baseVertexLocation = 0, uint startInstanceLocation = 0u)
        => _commandlist->DrawIndexedInstanced(indexCountPerInstance, instanceCount, startIndexLocation, baseVertexLocation, startInstanceLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPrimitiveTopology(PrimitiveTopology topology)
        => _commandlist->IASetPrimitiveTopology((D3D_PRIMITIVE_TOPOLOGY)topology);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetRenderTarget(Handle<Texture> handle)
    {
        var texture = (D3D12Texture*)_resourceManager.AccessTexture(handle);
        SetRenderTarget(*texture);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetRenderTarget(in D3D12Texture texture)
    {
        var handle = texture.RTV.CPU;
        _commandlist->OMSetRenderTargets(1, &handle, 0, null);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetScissorRect(D3D12_RECT* rect, uint count = 1)
        => _commandlist->RSSetScissorRects(count, rect);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetViewport(D3D12_VIEWPORT* viewport, uint count = 1)
        => _commandlist->RSSetViewports(count, viewport);

    public void SetDescriptorHeaps(ID3D12DescriptorHeap** heaps, uint count = 1)
        => _commandlist->SetDescriptorHeaps(count, heaps);

    public void SetGraphicsRootDescriptorTable(uint index, D3D12_GPU_DESCRIPTOR_HANDLE handle)
    {
        _commandlist->SetGraphicsRootDescriptorTable(index, handle);
    }

    public void SetGraphicsRootConstantBufferView(uint rootParameterIndex, D3D12_GPU_VIRTUAL_ADDRESS address)
    {
        _commandlist->SetGraphicsRootConstantBufferView(rootParameterIndex, address);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset()
    {
        Debug.Assert(_allocator != null);
        var allocatorHr = _allocator->Reset();
        Debug.Assert(SUCCEEDED(allocatorHr), $"Reset failed with HRESULT {allocatorHr}");
        var commandListHr = _commandlist->Reset(_allocator, null);
        Debug.Assert(SUCCEEDED(commandListHr), $"Reset failed with HRESULT {commandListHr}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset(Handle<PipelineState> stateHandle)
    {
        Debug.Assert(_allocator != null);
        Debug.Assert(stateHandle.IsValid);
        var state = (D3D12PipelineState*)_resourceManager.AccessPipelineState(stateHandle);
        Debug.Assert(state->PipelineState.Get() != null);
        var allocatorHr = _allocator->Reset();
        Debug.Assert(SUCCEEDED(allocatorHr), $"Reset failed with HRESULT {allocatorHr}");
        var commandListHr = _commandlist->Reset(_allocator, state->PipelineState.Get());
        Debug.Assert(SUCCEEDED(commandListHr), $"Reset failed with HRESULT {commandListHr}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Close()
    {
        var hr = _commandlist->Close();
        Debug.Assert(SUCCEEDED(hr), $"Close failed with HRESULT: {hr}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Transition(Handle<Texture> resource, D3D12_RESOURCE_STATES before, D3D12_RESOURCE_STATES after)
    {
        var texture = (D3D12Texture*)_resourceManager.AccessTexture(resource);
        Transition(*texture, before, after);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Transition(in D3D12Texture texture, D3D12_RESOURCE_STATES before, D3D12_RESOURCE_STATES after)
    {
        //NOTE(Jens): remove this method and use the one with Handle<Texture> when we have proper render target implementation
        Unsafe.SkipInit(out D3D12_RESOURCE_BARRIER barrier);

        barrier.Flags = D3D12_RESOURCE_BARRIER_FLAGS.D3D12_RESOURCE_BARRIER_FLAG_NONE;
        barrier.Type = D3D12_RESOURCE_BARRIER_TYPE.D3D12_RESOURCE_BARRIER_TYPE_TRANSITION;
        barrier.Transition.StateAfter = after;
        barrier.Transition.StateBefore = before;
        barrier.Transition.Subresource = 0;
        barrier.Transition.pResource = texture.Resource;
        _commandlist->ResourceBarrier(1, &barrier);
    }

    public void ClearRenderTarget(D3D12_CPU_DESCRIPTOR_HANDLE handle, float* colorRGBA)
        => _commandlist->ClearRenderTargetView(handle, colorRGBA, 0, null);
}
