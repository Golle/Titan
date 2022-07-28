using System.Runtime.CompilerServices;
using System;
using System.Runtime.InteropServices;
using Titan.Windows.D3D;

namespace Titan.Windows.D3D12;

[Guid("5b160d0f-ac1b-4185-8ba8-b3ae42a5a455")]
public unsafe struct ID3D12GraphicsCommandList
{
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(in Guid riid, void** ppvObject) => ((delegate* unmanaged[Stdcall]<void*, in Guid, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //    DECLSPEC_XFGVIRT(ID3D12Object, GetPrivateData)
    //    HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
    //        ID3D12GraphicsCommandList* This,
    //        _In_ REFGUID guid,
    //        _Inout_  UINT* pDataSize,
    //        _Out_writes_bytes_opt_( *pDataSize )  void* pData);

    //DECLSPEC_XFGVIRT(ID3D12Object, SetPrivateData)
    //    HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
    //        ID3D12GraphicsCommandList* This,
    //        _In_ REFGUID guid,
    //        _In_  UINT DataSize,
    //        _In_reads_bytes_opt_( DataSize )  const void* pData);

    //DECLSPEC_XFGVIRT(ID3D12Object, SetPrivateDataInterface)
    //    HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
    //        ID3D12GraphicsCommandList* This,
    //        _In_ REFGUID guid,
    //        _In_opt_  const IUnknown* pData);

    //DECLSPEC_XFGVIRT(ID3D12Object, SetName)
    //    HRESULT(STDMETHODCALLTYPE* SetName)(
    //        ID3D12GraphicsCommandList* This,
    //        _In_z_ LPCWSTR Name);

    //DECLSPEC_XFGVIRT(ID3D12DeviceChild, GetDevice)
    //    HRESULT(STDMETHODCALLTYPE* GetDevice)(
    //        ID3D12GraphicsCommandList* This,
    //REFIID riid,
    //        _COM_Outptr_opt_  void** ppvDevice);

    //DECLSPEC_XFGVIRT(ID3D12CommandList, GetType)
    //    D3D12_COMMAND_LIST_TYPE(STDMETHODCALLTYPE* GetType)(
    //        ID3D12GraphicsCommandList* This);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT Close()
        => ((delegate* unmanaged[Stdcall]<void*, HRESULT>)_vtbl[9])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT Reset(ID3D12CommandAllocator* pAllocator, ID3D12PipelineState* pInitialState)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12CommandAllocator*, ID3D12PipelineState*, HRESULT>)_vtbl[10])(Unsafe.AsPointer(ref this), pAllocator, pInitialState);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearState(ID3D12PipelineState* pPipelineState)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12PipelineState*, void>)_vtbl[11])(Unsafe.AsPointer(ref this), pPipelineState);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawInstanced(uint VertexCountPerInstance, uint InstanceCount, uint StartVertexLocation, uint StartInstanceLocation)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, uint, uint, void>)_vtbl[12])(Unsafe.AsPointer(ref this), VertexCountPerInstance, InstanceCount, StartVertexLocation, StartInstanceLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawIndexedInstanced(uint IndexCountPerInstance, uint InstanceCount, uint StartIndexLocation, int BaseVertexLocation, uint StartInstanceLocation)
    => ((delegate* unmanaged[Stdcall]<void*, uint, uint, uint, int, uint, void>)_vtbl[13])(Unsafe.AsPointer(ref this), IndexCountPerInstance, InstanceCount, StartIndexLocation, BaseVertexLocation, StartInstanceLocation);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, Dispatch)
    //    void (STDMETHODCALLTYPE* Dispatch ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT ThreadGroupCountX,
    //        _In_  UINT ThreadGroupCountY,
    //        _In_  UINT ThreadGroupCountZ);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, CopyBufferRegion)
    //    void (STDMETHODCALLTYPE* CopyBufferRegion ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ ID3D12Resource * pDstBuffer,
    //        UINT64 DstOffset,
    //        _In_ ID3D12Resource *pSrcBuffer,
    //        UINT64 SrcOffset,
    //        UINT64 NumBytes);

    //    DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, CopyTextureRegion)
    //    void (STDMETHODCALLTYPE* CopyTextureRegion ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_  const D3D12_TEXTURE_COPY_LOCATION* pDst,
    //        UINT DstX,
    //        UINT DstY,
    //        UINT DstZ,
    //        _In_ const D3D12_TEXTURE_COPY_LOCATION* pSrc,
    //        _In_opt_  const D3D12_BOX* pSrcBox);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, CopyResource)
    //    void (STDMETHODCALLTYPE* CopyResource ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ ID3D12Resource * pDstResource,
    //        _In_  ID3D12Resource* pSrcResource);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, CopyTiles)
    //    void (STDMETHODCALLTYPE* CopyTiles ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ ID3D12Resource * pTiledResource,
    //        _In_  const D3D12_TILED_RESOURCE_COORDINATE* pTileRegionStartCoordinate,
    //        _In_  const D3D12_TILE_REGION_SIZE* pTileRegionSize,
    //        _In_  ID3D12Resource* pBuffer,
    //        UINT64 BufferStartOffsetInBytes,
    //        D3D12_TILE_COPY_FLAGS Flags);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, ResolveSubresource)
    //    void (STDMETHODCALLTYPE* ResolveSubresource ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ ID3D12Resource * pDstResource,
    //        _In_  UINT DstSubresource,
    //        _In_  ID3D12Resource* pSrcResource,
    //        _In_  UINT SrcSubresource,
    //        _In_  DXGI_FORMAT Format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY PrimitiveTopology)
        => ((delegate* unmanaged[Stdcall]<void*, D3D_PRIMITIVE_TOPOLOGY, void>)_vtbl[20])(Unsafe.AsPointer(ref this), PrimitiveTopology);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RSSetViewports(uint NumViewports, D3D12_VIEWPORT* pViewports)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_VIEWPORT*, void>)_vtbl[21])(Unsafe.AsPointer(ref this), NumViewports, pViewports);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RSSetScissorRects(uint NumRects, D3D12_RECT* pRects)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_RECT*, void>)_vtbl[22])(Unsafe.AsPointer(ref this), NumRects, pRects);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OMSetBlendFactor(float* BlendFactor /*FLOAT[4]*/)
        => ((delegate* unmanaged[Stdcall]<void*, float*, void>)_vtbl[23])(Unsafe.AsPointer(ref this), BlendFactor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OMSetStencilRef(uint StencilRef)
        => ((delegate* unmanaged[Stdcall]<void*, uint, void>)_vtbl[23])(Unsafe.AsPointer(ref this), StencilRef);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPipelineState(ID3D12PipelineState* pPipelineState)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12PipelineState*, void>)_vtbl[24])(Unsafe.AsPointer(ref this), pPipelineState);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResourceBarrier(uint NumBarriers, D3D12_RESOURCE_BARRIER* pBarriers)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_RESOURCE_BARRIER*, void>)_vtbl[26])(Unsafe.AsPointer(ref this), NumBarriers, pBarriers);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, ExecuteBundle)
    //    void (STDMETHODCALLTYPE* ExecuteBundle ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ ID3D12GraphicsCommandList * pCommandList);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetDescriptorHeaps(uint NumDescriptorHeaps, ID3D12DescriptorHeap** ppDescriptorHeaps)
        => ((delegate* unmanaged[Stdcall]<void*, uint, ID3D12DescriptorHeap**, void>)_vtbl[28])(Unsafe.AsPointer(ref this), NumDescriptorHeaps, ppDescriptorHeaps);


    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetComputeRootSignature)
    //    void (STDMETHODCALLTYPE* SetComputeRootSignature ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_opt_ ID3D12RootSignature * pRootSignature);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRootSignature(ID3D12RootSignature* pRootSignature)
     => ((delegate* unmanaged[Stdcall]<void*, ID3D12RootSignature*, void>)_vtbl[30])(Unsafe.AsPointer(ref this), pRootSignature);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetComputeRootDescriptorTable)
    //    void (STDMETHODCALLTYPE* SetComputeRootDescriptorTable ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT RootParameterIndex,
    //        _In_  D3D12_GPU_DESCRIPTOR_HANDLE BaseDescriptor);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetGraphicsRootDescriptorTable)
    //    void (STDMETHODCALLTYPE* SetGraphicsRootDescriptorTable ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT RootParameterIndex,
    //        _In_  D3D12_GPU_DESCRIPTOR_HANDLE BaseDescriptor);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetComputeRoot32BitConstant)
    //    void (STDMETHODCALLTYPE* SetComputeRoot32BitConstant ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT RootParameterIndex,
    //        _In_  UINT SrcData,
    //        _In_  UINT DestOffsetIn32BitValues);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetGraphicsRoot32BitConstant)
    //    void (STDMETHODCALLTYPE* SetGraphicsRoot32BitConstant ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT RootParameterIndex,
    //        _In_  UINT SrcData,
    //        _In_  UINT DestOffsetIn32BitValues);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetComputeRoot32BitConstants)
    //    void (STDMETHODCALLTYPE* SetComputeRoot32BitConstants ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT RootParameterIndex,
    //        _In_  UINT Num32BitValuesToSet,
    //        _In_reads_(Num32BitValuesToSet * sizeof(UINT))  const void* pSrcData,
    //        _In_  UINT DestOffsetIn32BitValues);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetGraphicsRoot32BitConstants)
    //    void (STDMETHODCALLTYPE* SetGraphicsRoot32BitConstants ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT RootParameterIndex,
    //        _In_  UINT Num32BitValuesToSet,
    //        _In_reads_(Num32BitValuesToSet * sizeof(UINT))  const void* pSrcData,
    //        _In_  UINT DestOffsetIn32BitValues);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetComputeRootConstantBufferView)
    //    void (STDMETHODCALLTYPE* SetComputeRootConstantBufferView ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT RootParameterIndex,
    //        _In_  D3D12_GPU_VIRTUAL_ADDRESS BufferLocation);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetGraphicsRootConstantBufferView)
    //    void (STDMETHODCALLTYPE* SetGraphicsRootConstantBufferView ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT RootParameterIndex,
    //        _In_  D3D12_GPU_VIRTUAL_ADDRESS BufferLocation);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetComputeRootShaderResourceView)
    //    void (STDMETHODCALLTYPE* SetComputeRootShaderResourceView ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT RootParameterIndex,
    //        _In_  D3D12_GPU_VIRTUAL_ADDRESS BufferLocation);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetGraphicsRootShaderResourceView)
    //    void (STDMETHODCALLTYPE* SetGraphicsRootShaderResourceView ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT RootParameterIndex,
    //        _In_  D3D12_GPU_VIRTUAL_ADDRESS BufferLocation);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetComputeRootUnorderedAccessView)
    //    void (STDMETHODCALLTYPE* SetComputeRootUnorderedAccessView ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT RootParameterIndex,
    //        _In_  D3D12_GPU_VIRTUAL_ADDRESS BufferLocation);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetGraphicsRootUnorderedAccessView)
    //    void (STDMETHODCALLTYPE* SetGraphicsRootUnorderedAccessView ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT RootParameterIndex,
    //        _In_  D3D12_GPU_VIRTUAL_ADDRESS BufferLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IASetIndexBuffer(D3D12_INDEX_BUFFER_VIEW* pView)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_INDEX_BUFFER_VIEW*, void>)_vtbl[43])(Unsafe.AsPointer(ref this), pView);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IASetVertexBuffers(uint StartSlot, uint NumViews, D3D12_VERTEX_BUFFER_VIEW* pViews)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, D3D12_VERTEX_BUFFER_VIEW*, void>)_vtbl[44])(Unsafe.AsPointer(ref this), StartSlot, NumViews, pViews);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SOSetTargets)
    //    void (STDMETHODCALLTYPE* SOSetTargets ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT StartSlot,
    //        _In_  UINT NumViews,
    //        _In_reads_opt_(NumViews)  const D3D12_STREAM_OUTPUT_BUFFER_VIEW* pViews);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OMSetRenderTargets(uint NumRenderTargetDescriptors, D3D12_CPU_DESCRIPTOR_HANDLE* pRenderTargetDescriptors, int RTsSingleHandleToDescriptorRange, D3D12_CPU_DESCRIPTOR_HANDLE* pDepthStencilDescriptor)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_CPU_DESCRIPTOR_HANDLE*, int, D3D12_CPU_DESCRIPTOR_HANDLE*, void>)_vtbl[46])(Unsafe.AsPointer(ref this), NumRenderTargetDescriptors, pRenderTargetDescriptors, RTsSingleHandleToDescriptorRange, pDepthStencilDescriptor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearDepthStencilView(D3D12_CPU_DESCRIPTOR_HANDLE DepthStencilView, D3D12_CLEAR_FLAGS ClearFlags, float Depth, byte Stencil, uint NumRects, D3D12_RECT* pRects)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_CPU_DESCRIPTOR_HANDLE, D3D12_CLEAR_FLAGS, float, byte, uint, D3D12_RECT*, void>)_vtbl[47])(Unsafe.AsPointer(ref this), DepthStencilView, ClearFlags, Depth, Stencil, NumRects, pRects);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearRenderTargetView(D3D12_CPU_DESCRIPTOR_HANDLE RenderTargetView, float* ColorRGBA, uint NumRects, D3D12_RECT* pRects)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_CPU_DESCRIPTOR_HANDLE, float*, uint, D3D12_RECT*, void>)_vtbl[48])(Unsafe.AsPointer(ref this), RenderTargetView, ColorRGBA, NumRects, pRects);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, ClearUnorderedAccessViewUint)
    //    void (STDMETHODCALLTYPE* ClearUnorderedAccessViewUint ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ D3D12_GPU_DESCRIPTOR_HANDLE ViewGPUHandleInCurrentHeap,
    //        _In_  D3D12_CPU_DESCRIPTOR_HANDLE ViewCPUHandle,
    //        _In_  ID3D12Resource* pResource,
    //        _In_  const UINT Values[4],
    //        _In_  UINT NumRects,
    //        _In_reads_(NumRects)  const D3D12_RECT* pRects);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, ClearUnorderedAccessViewFloat)
    //    void (STDMETHODCALLTYPE* ClearUnorderedAccessViewFloat ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ D3D12_GPU_DESCRIPTOR_HANDLE ViewGPUHandleInCurrentHeap,
    //        _In_  D3D12_CPU_DESCRIPTOR_HANDLE ViewCPUHandle,
    //        _In_  ID3D12Resource* pResource,
    //        _In_  const FLOAT Values[4],
    //        _In_  UINT NumRects,
    //        _In_reads_(NumRects)  const D3D12_RECT* pRects);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, DiscardResource)
    //    void (STDMETHODCALLTYPE* DiscardResource ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ ID3D12Resource * pResource,
    //        _In_opt_  const D3D12_DISCARD_REGION* pRegion);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, BeginQuery)
    //    void (STDMETHODCALLTYPE* BeginQuery ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ ID3D12QueryHeap * pQueryHeap,
    //        _In_  D3D12_QUERY_TYPE Type,
    //        _In_  UINT Index);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, EndQuery)
    //    void (STDMETHODCALLTYPE* EndQuery ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ ID3D12QueryHeap * pQueryHeap,
    //        _In_  D3D12_QUERY_TYPE Type,
    //        _In_  UINT Index);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, ResolveQueryData)
    //    void (STDMETHODCALLTYPE* ResolveQueryData ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ ID3D12QueryHeap * pQueryHeap,
    //        _In_  D3D12_QUERY_TYPE Type,
    //        _In_  UINT StartIndex,
    //        _In_  UINT NumQueries,
    //        _In_  ID3D12Resource* pDestinationBuffer,
    //        _In_  UINT64 AlignedDestinationBufferOffset);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetPredication)
    //    void (STDMETHODCALLTYPE* SetPredication ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_opt_ ID3D12Resource * pBuffer,
    //        _In_  UINT64 AlignedBufferOffset,
    //        _In_  D3D12_PREDICATION_OP Operation);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetMarker)
    //    void (STDMETHODCALLTYPE* SetMarker ) (
    //        ID3D12GraphicsCommandList* This,
    //        UINT Metadata,
    //        _In_reads_bytes_opt_(Size) const void* pData,
    //        UINT Size);

    //    DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, BeginEvent)
    //    void (STDMETHODCALLTYPE* BeginEvent ) (
    //        ID3D12GraphicsCommandList* This,
    //        UINT Metadata,
    //        _In_reads_bytes_opt_(Size) const void* pData,
    //        UINT Size);

    //    DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, EndEvent)
    //    void (STDMETHODCALLTYPE* EndEvent ) (
    //        ID3D12GraphicsCommandList* This);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExecuteIndirect(ID3D12CommandSignature* pCommandSignature, uint MaxCommandCount, ID3D12Resource* pArgumentBuffer, ulong ArgumentBufferOffset, ID3D12Resource* pCountBuffer, ulong CountBufferOffset)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12CommandSignature*, uint, ID3D12Resource*, ulong, ID3D12Resource*, ulong, void>)_vtbl[59])(Unsafe.AsPointer(ref this), pCommandSignature, MaxCommandCount, pArgumentBuffer, ArgumentBufferOffset, pCountBuffer, CountBufferOffset);
}
