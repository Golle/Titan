using System.Runtime.CompilerServices;
using System;
using System.Runtime.InteropServices;
using Titan.Windows.Win32;

namespace Titan.Windows.D3D12;

[Guid("5b160d0f-ac1b-4185-8ba8-b3ae42a5a455")]
public unsafe struct ID3D12GraphicsCommandListVtbl
{
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject) => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);
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

    //    DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, Close)
    //    HRESULT(STDMETHODCALLTYPE* Close)(
    //        ID3D12GraphicsCommandList* This);

    //    DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, Reset)
    //    HRESULT(STDMETHODCALLTYPE* Reset)(
    //        ID3D12GraphicsCommandList* This,
    //        _In_ ID3D12CommandAllocator * pAllocator,
    //        _In_opt_  ID3D12PipelineState* pInitialState);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, ClearState)
    //    void (STDMETHODCALLTYPE* ClearState ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_opt_ ID3D12PipelineState * pPipelineState);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, DrawInstanced)
    //    void (STDMETHODCALLTYPE* DrawInstanced ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT VertexCountPerInstance,
    //        _In_  UINT InstanceCount,
    //        _In_  UINT StartVertexLocation,
    //        _In_  UINT StartInstanceLocation);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, DrawIndexedInstanced)
    //    void (STDMETHODCALLTYPE* DrawIndexedInstanced ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT IndexCountPerInstance,
    //        _In_  UINT InstanceCount,
    //        _In_  UINT StartIndexLocation,
    //        _In_  INT BaseVertexLocation,
    //        _In_  UINT StartInstanceLocation);

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

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, IASetPrimitiveTopology)
    //    void (STDMETHODCALLTYPE* IASetPrimitiveTopology ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ D3D12_PRIMITIVE_TOPOLOGY PrimitiveTopology);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, RSSetViewports)
    //    void (STDMETHODCALLTYPE* RSSetViewports ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_range_(0, D3D12_VIEWPORT_AND_SCISSORRECT_OBJECT_COUNT_PER_PIPELINE)  UINT NumViewports,
    //        _In_reads_( NumViewports)  const D3D12_VIEWPORT* pViewports);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, RSSetScissorRects)
    //    void (STDMETHODCALLTYPE* RSSetScissorRects ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_range_(0, D3D12_VIEWPORT_AND_SCISSORRECT_OBJECT_COUNT_PER_PIPELINE)  UINT NumRects,
    //        _In_reads_( NumRects)  const D3D12_RECT* pRects);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, OMSetBlendFactor)
    //    void (STDMETHODCALLTYPE* OMSetBlendFactor ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_reads_opt_(4)  const FLOAT BlendFactor[4]);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, OMSetStencilRef)
    //    void (STDMETHODCALLTYPE* OMSetStencilRef ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT StencilRef);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetPipelineState)
    //    void (STDMETHODCALLTYPE* SetPipelineState ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ ID3D12PipelineState * pPipelineState);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, ResourceBarrier)
    //    void (STDMETHODCALLTYPE* ResourceBarrier ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT NumBarriers,
    //        _In_reads_(NumBarriers)  const D3D12_RESOURCE_BARRIER* pBarriers);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, ExecuteBundle)
    //    void (STDMETHODCALLTYPE* ExecuteBundle ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ ID3D12GraphicsCommandList * pCommandList);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetDescriptorHeaps)
    //    void (STDMETHODCALLTYPE* SetDescriptorHeaps ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT NumDescriptorHeaps,
    //        _In_reads_(NumDescriptorHeaps)  ID3D12DescriptorHeap* const * ppDescriptorHeaps);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetComputeRootSignature)
    //    void (STDMETHODCALLTYPE* SetComputeRootSignature ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_opt_ ID3D12RootSignature * pRootSignature);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SetGraphicsRootSignature)
    //    void (STDMETHODCALLTYPE* SetGraphicsRootSignature ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_opt_ ID3D12RootSignature * pRootSignature);

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

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, IASetIndexBuffer)
    //    void (STDMETHODCALLTYPE* IASetIndexBuffer ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_opt_  const D3D12_INDEX_BUFFER_VIEW* pView);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, IASetVertexBuffers)
    //    void (STDMETHODCALLTYPE* IASetVertexBuffers ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT StartSlot,
    //        _In_  UINT NumViews,
    //        _In_reads_opt_(NumViews)  const D3D12_VERTEX_BUFFER_VIEW* pViews);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, SOSetTargets)
    //    void (STDMETHODCALLTYPE* SOSetTargets ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT StartSlot,
    //        _In_  UINT NumViews,
    //        _In_reads_opt_(NumViews)  const D3D12_STREAM_OUTPUT_BUFFER_VIEW* pViews);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, OMSetRenderTargets)
    //    void (STDMETHODCALLTYPE* OMSetRenderTargets ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ UINT NumRenderTargetDescriptors,
    //        _In_opt_  const D3D12_CPU_DESCRIPTOR_HANDLE* pRenderTargetDescriptors,
    //        _In_  BOOL RTsSingleHandleToDescriptorRange,
    //        _In_opt_  const D3D12_CPU_DESCRIPTOR_HANDLE* pDepthStencilDescriptor);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, ClearDepthStencilView)
    //    void (STDMETHODCALLTYPE* ClearDepthStencilView ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ D3D12_CPU_DESCRIPTOR_HANDLE DepthStencilView,
    //        _In_  D3D12_CLEAR_FLAGS ClearFlags,
    //        _In_  FLOAT Depth,
    //        _In_  UINT8 Stencil,
    //        _In_  UINT NumRects,
    //        _In_reads_(NumRects)  const D3D12_RECT* pRects);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearRenderTargetView(D3D12_CPU_DESCRIPTOR_HANDLE RenderTargetView, float* ColorRGBA, uint NumRects, D3D12_RECT* pRects) 
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_CPU_DESCRIPTOR_HANDLE , float*, uint, D3D12_RECT*, void>)_vtbl[48])(Unsafe.AsPointer(ref this), RenderTargetView, ColorRGBA, NumRects, pRects);

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

    //    DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, ExecuteIndirect)
    //    void (STDMETHODCALLTYPE* ExecuteIndirect ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ ID3D12CommandSignature * pCommandSignature,
    //        _In_  UINT MaxCommandCount,
    //        _In_  ID3D12Resource* pArgumentBuffer,
    //        _In_  UINT64 ArgumentBufferOffset,
    //        _In_opt_  ID3D12Resource* pCountBuffer,
    //        _In_  UINT64 CountBufferOffset);

    //END_INTERFACE
}
