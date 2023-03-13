using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.D3D;
using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D12;

[Guid("5b160d0f-ac1b-4185-8ba8-b3ae42a5a455")]
public unsafe struct ID3D12GraphicsCommandList : INativeGuid
{
    public static Guid* Guid => IID.IID_ID3D12GraphicsCommandList;
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetName(char* Name)
        => ((delegate* unmanaged[Stdcall]<void*, char*, HRESULT>)_vtbl[6])(Unsafe.AsPointer(ref this), Name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetDevice(Guid* riid, void** ppvDevice)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[7])(Unsafe.AsPointer(ref this), riid, ppvDevice);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public new D3D12_COMMAND_LIST_TYPE GetType()
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_COMMAND_LIST_TYPE>)_vtbl[8])(Unsafe.AsPointer(ref this));

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyBufferRegion(ID3D12Resource* pDstBuffer, ulong DstOffset, ID3D12Resource* pSrcBuffer, ulong SrcOffset, ulong NumBytes)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Resource*, ulong, ID3D12Resource*, ulong, ulong, void>)_vtbl[15])(Unsafe.AsPointer(ref this), pDstBuffer, DstOffset, pSrcBuffer, SrcOffset, NumBytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTextureRegion(D3D12_TEXTURE_COPY_LOCATION* pDst, uint DstX, uint DstY, uint DstZ, D3D12_TEXTURE_COPY_LOCATION* pSrc, D3D12_BOX* pSrcBox)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_TEXTURE_COPY_LOCATION*, uint, uint, uint, D3D12_TEXTURE_COPY_LOCATION*, D3D12_BOX*, void>)_vtbl[16])(Unsafe.AsPointer(ref this), pDst, DstX, DstY, DstZ, pSrc, pSrcBox);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyResource(ID3D12Resource* pDstResource, ID3D12Resource* pSrcResource)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Resource*, ID3D12Resource*, void>)_vtbl[17])(Unsafe.AsPointer(ref this), pDstResource, pSrcResource);

    //DECLSPEC_XFGVIRT(ID3D12GraphicsCommandList, CopyTiles)
    //    void (STDMETHODCALLTYPE* CopyTiles ) (
    //        ID3D12GraphicsCommandList* This,
    //        _In_ ID3D12Resource * pTiledResource,
    //        _In_  const D3D12_TILED_RESOURCE_COORDINATE* pTileRegionStartCoordinate,
    //        _In_  const D3D12_TILE_REGION_SIZE* pTileRegionSize,
    //        _In_  ID3D12Resource* pBuffer,
    //        UINT64 BufferStartOffsetInBytes,
    //        D3D12_TILE_COPY_FLAGS Flags);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResolveSubresource(ID3D12Resource* pDstResource, uint DstSubresource, ID3D12Resource* pSrcResource, uint SrcSubresource, DXGI_FORMAT Format)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Resource*, uint, ID3D12Resource*, uint, DXGI_FORMAT, void>)_vtbl[19])(Unsafe.AsPointer(ref this), pDstResource, DstSubresource, pSrcResource, SrcSubresource, Format);

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
        => ((delegate* unmanaged[Stdcall]<void*, uint, void>)_vtbl[24])(Unsafe.AsPointer(ref this), StencilRef);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPipelineState(ID3D12PipelineState* pPipelineState)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12PipelineState*, void>)_vtbl[25])(Unsafe.AsPointer(ref this), pPipelineState);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResourceBarrier(uint NumBarriers, D3D12_RESOURCE_BARRIER* pBarriers)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_RESOURCE_BARRIER*, void>)_vtbl[26])(Unsafe.AsPointer(ref this), NumBarriers, pBarriers);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExecuteBundle(ID3D12GraphicsCommandList* pCommandList)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12GraphicsCommandList*, void>)_vtbl[27])(Unsafe.AsPointer(ref this), pCommandList);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetDescriptorHeaps(uint NumDescriptorHeaps, ID3D12DescriptorHeap** ppDescriptorHeaps)
        => ((delegate* unmanaged[Stdcall]<void*, uint, ID3D12DescriptorHeap**, void>)_vtbl[28])(Unsafe.AsPointer(ref this), NumDescriptorHeaps, ppDescriptorHeaps);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComputeRootSignature(ID3D12RootSignature* pRootSignature)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12RootSignature*, void>)_vtbl[29])(Unsafe.AsPointer(ref this), pRootSignature);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRootSignature(ID3D12RootSignature* pRootSignature)
     => ((delegate* unmanaged[Stdcall]<void*, ID3D12RootSignature*, void>)_vtbl[30])(Unsafe.AsPointer(ref this), pRootSignature);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComputeRootDescriptorTable(uint RootParameterIndex, D3D12_GPU_DESCRIPTOR_HANDLE BaseDescriptor)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_DESCRIPTOR_HANDLE, void>)_vtbl[31])(Unsafe.AsPointer(ref this), RootParameterIndex, BaseDescriptor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRootDescriptorTable(uint RootParameterIndex, D3D12_GPU_DESCRIPTOR_HANDLE BaseDescriptor)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_DESCRIPTOR_HANDLE, void>)_vtbl[32])(Unsafe.AsPointer(ref this), RootParameterIndex, BaseDescriptor);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComputeRoot32BitConstant(uint RootParameterIndex, uint SrcData, uint DestOffsetIn32BitValues)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, uint, void>)_vtbl[33])(Unsafe.AsPointer(ref this), RootParameterIndex, SrcData, DestOffsetIn32BitValues);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRoot32BitConstant(uint RootParameterIndex, uint SrcData, uint DestOffsetIn32BitValues)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, uint, void>)_vtbl[34])(Unsafe.AsPointer(ref this), RootParameterIndex, SrcData, DestOffsetIn32BitValues);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComputeRoot32BitConstants(uint RootParameterIndex, uint Num32BitValuesToSet, void* pSrcData, uint DestOffsetIn32BitValues)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, void*, uint, void>)_vtbl[35])(Unsafe.AsPointer(ref this), RootParameterIndex, Num32BitValuesToSet, pSrcData, DestOffsetIn32BitValues);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRoot32BitConstants(uint RootParameterIndex, uint Num32BitValuesToSet, void* pSrcData, uint DestOffsetIn32BitValues)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, void*, uint, void>)_vtbl[36])(Unsafe.AsPointer(ref this), RootParameterIndex, Num32BitValuesToSet, pSrcData, DestOffsetIn32BitValues);

    public void SetComputeRootConstantBufferView(uint RootParameterIndex, D3D12_GPU_VIRTUAL_ADDRESS BufferLocation)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_VIRTUAL_ADDRESS, void>)_vtbl[37])(Unsafe.AsPointer(ref this), RootParameterIndex, BufferLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRootConstantBufferView(uint RootParameterIndex, D3D12_GPU_VIRTUAL_ADDRESS BufferLocation)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_VIRTUAL_ADDRESS, void>)_vtbl[38])(Unsafe.AsPointer(ref this), RootParameterIndex, BufferLocation);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComputeRootShaderResourceView(uint RootParameterIndex, D3D12_GPU_VIRTUAL_ADDRESS BufferLocation)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_VIRTUAL_ADDRESS, void>)_vtbl[39])(Unsafe.AsPointer(ref this), RootParameterIndex, BufferLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRootShaderResourceView(uint RootParameterIndex, D3D12_GPU_VIRTUAL_ADDRESS BufferLocation)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_VIRTUAL_ADDRESS, void>)_vtbl[40])(Unsafe.AsPointer(ref this), RootParameterIndex, BufferLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComputeRootUnorderedAccessView(uint RootParameterIndex, D3D12_GPU_VIRTUAL_ADDRESS BufferLocation)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_VIRTUAL_ADDRESS, void>)_vtbl[41])(Unsafe.AsPointer(ref this), RootParameterIndex, BufferLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRootUnorderedAccessView(uint RootParameterIndex, D3D12_GPU_VIRTUAL_ADDRESS BufferLocation)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_VIRTUAL_ADDRESS, void>)_vtbl[42])(Unsafe.AsPointer(ref this), RootParameterIndex, BufferLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IASetIndexBuffer(D3D12_INDEX_BUFFER_VIEW* pView)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_INDEX_BUFFER_VIEW*, void>)_vtbl[43])(Unsafe.AsPointer(ref this), pView);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IASetVertexBuffers(uint StartSlot, uint NumViews, D3D12_VERTEX_BUFFER_VIEW* pViews)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, D3D12_VERTEX_BUFFER_VIEW*, void>)_vtbl[44])(Unsafe.AsPointer(ref this), StartSlot, NumViews, pViews);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SOSetTargets(uint StartSlot, uint NumViews, D3D12_STREAM_OUTPUT_BUFFER_VIEW* pViews)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, D3D12_STREAM_OUTPUT_BUFFER_VIEW*, void>)_vtbl[45])(Unsafe.AsPointer(ref this), StartSlot, NumViews, pViews);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OMSetRenderTargets(uint NumRenderTargetDescriptors, D3D12_CPU_DESCRIPTOR_HANDLE* pRenderTargetDescriptors, int RTsSingleHandleToDescriptorRange, D3D12_CPU_DESCRIPTOR_HANDLE* pDepthStencilDescriptor)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_CPU_DESCRIPTOR_HANDLE*, int, D3D12_CPU_DESCRIPTOR_HANDLE*, void>)_vtbl[46])(Unsafe.AsPointer(ref this), NumRenderTargetDescriptors, pRenderTargetDescriptors, RTsSingleHandleToDescriptorRange, pDepthStencilDescriptor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearDepthStencilView(D3D12_CPU_DESCRIPTOR_HANDLE DepthStencilView, D3D12_CLEAR_FLAGS ClearFlags, float Depth, byte Stencil, uint NumRects, D3D12_RECT* pRects)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_CPU_DESCRIPTOR_HANDLE, D3D12_CLEAR_FLAGS, float, byte, uint, D3D12_RECT*, void>)_vtbl[47])(Unsafe.AsPointer(ref this), DepthStencilView, ClearFlags, Depth, Stencil, NumRects, pRects);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearRenderTargetView(D3D12_CPU_DESCRIPTOR_HANDLE RenderTargetView, float* ColorRGBA, uint NumRects, D3D12_RECT* pRects)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_CPU_DESCRIPTOR_HANDLE, float*, uint, D3D12_RECT*, void>)_vtbl[48])(Unsafe.AsPointer(ref this), RenderTargetView, ColorRGBA, NumRects, pRects);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearUnorderedAccessViewUint(D3D12_GPU_DESCRIPTOR_HANDLE ViewGPUHandleInCurrentHeap, D3D12_CPU_DESCRIPTOR_HANDLE ViewCPUHandle, ID3D12Resource* pResource, uint* Values, uint NumRects, D3D12_RECT* pRects)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_GPU_DESCRIPTOR_HANDLE, D3D12_CPU_DESCRIPTOR_HANDLE, ID3D12Resource*, uint*, uint, D3D12_RECT*, void>)_vtbl[49])(Unsafe.AsPointer(ref this), ViewGPUHandleInCurrentHeap, ViewCPUHandle, pResource, Values, NumRects, pRects);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearUnorderedAccessViewFloat(D3D12_GPU_DESCRIPTOR_HANDLE ViewGPUHandleInCurrentHeap, D3D12_CPU_DESCRIPTOR_HANDLE ViewCPUHandle, ID3D12Resource* pResource, float* Values, uint NumRects, D3D12_RECT* pRects)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_GPU_DESCRIPTOR_HANDLE, D3D12_CPU_DESCRIPTOR_HANDLE, ID3D12Resource*, float*, uint, D3D12_RECT*, void>)_vtbl[50])(Unsafe.AsPointer(ref this), ViewGPUHandleInCurrentHeap, ViewCPUHandle, pResource, Values, NumRects, pRects);

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BeginEvent(uint Metadata, void* pData, uint Size)
        => ((delegate* unmanaged[Stdcall]<void*, uint, void*, uint, void>)_vtbl[57])(Unsafe.AsPointer(ref this), Metadata, pData, Size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndEvent()
        => ((delegate* unmanaged[Stdcall]<void*, void>)_vtbl[58])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExecuteIndirect(ID3D12CommandSignature* pCommandSignature, uint MaxCommandCount, ID3D12Resource* pArgumentBuffer, ulong ArgumentBufferOffset, ID3D12Resource* pCountBuffer, ulong CountBufferOffset)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12CommandSignature*, uint, ID3D12Resource*, ulong, ID3D12Resource*, ulong, void>)_vtbl[59])(Unsafe.AsPointer(ref this), pCommandSignature, MaxCommandCount, pArgumentBuffer, ArgumentBufferOffset, pCountBuffer, CountBufferOffset);
}
