using System.Runtime.CompilerServices;
// ReSharper disable InconsistentNaming

namespace Titan.D3D11
{
    public unsafe struct ID3D11DeviceContext
    {
        private void** _vtbl;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetDevice(ID3D11Device** ppDevice) => ((delegate* unmanaged[Stdcall]<void*, ID3D11Device**, void>)_vtbl[3])(Unsafe.AsPointer(ref this), ppDevice);

        //HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
        // ID3D11DeviceContext* This,
        // /* [annotation] */
        // _In_ REFGUID guid,
        // /* [annotation] */
        // _Inout_  UINT* pDataSize,
        // /* [annotation] */
        // _Out_writes_bytes_opt_( *pDataSize )  void* pData);

        //HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
        // ID3D11DeviceContext* This,
        // /* [annotation] */
        // _In_ REFGUID guid,
        // /* [annotation] */
        // _In_  UINT DataSize,
        // /* [annotation] */
        // _In_reads_bytes_opt_( DataSize )  const void* pData);

        //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
        // ID3D11DeviceContext* This,
        // /* [annotation] */
        // _In_ REFGUID guid,
        // /* [annotation] */
        // _In_opt_  const IUnknown* pData);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void VSSetConstantBuffers(uint startSlot, uint numBuffers, ID3D11Buffer** ppConstantBuffers) => ((delegate* unmanaged[Stdcall]<void*, uint, uint, ID3D11Buffer**, void>)_vtbl[7])(Unsafe.AsPointer(ref this), startSlot, numBuffers, ppConstantBuffers);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PSSetShaderResources(uint startSlot, uint numViews, ID3D11ShaderResourceView** ppShaderResourceViews) => ((delegate* unmanaged[Stdcall]<void*, uint, uint, ID3D11ShaderResourceView**, void>)_vtbl[8])(Unsafe.AsPointer(ref this), startSlot, numViews, ppShaderResourceViews);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PSSetShader(ID3D11PixelShader * pPixelShader, ID3D11ClassInstance** ppClassInstances, uint numClassInstances) => ((delegate* unmanaged[Stdcall]<void*, ID3D11PixelShader*, ID3D11ClassInstance**, uint, void>) _vtbl[9])(Unsafe.AsPointer(ref this), pPixelShader, ppClassInstances, numClassInstances);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PSSetSamplers(uint startSlot, uint numSamplers, ID3D11SamplerState** ppSamplers) => ((delegate* unmanaged[Stdcall]<void*, uint, uint, ID3D11SamplerState**, void>) _vtbl[10])(Unsafe.AsPointer(ref this), startSlot, numSamplers, ppSamplers);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void VSSetShader(ID3D11VertexShader * pVertexShader, ID3D11ClassInstance** ppClassInstances, uint numClassInstances) => ((delegate* unmanaged[Stdcall]<void*, ID3D11VertexShader*, ID3D11ClassInstance**, uint, void>) _vtbl[11])(Unsafe.AsPointer(ref this), pVertexShader, ppClassInstances, numClassInstances);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawIndexed(uint indexCount, uint startIndexLocation, int baseVertexLocation) => ((delegate* unmanaged[Stdcall]<void*, uint, uint, int, void>)_vtbl[12])(Unsafe.AsPointer(ref this), indexCount, startIndexLocation, baseVertexLocation);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(uint vertexCount, uint startVertexLocation) => ((delegate* unmanaged[Stdcall]<void*, uint, uint, void>)_vtbl[13])(Unsafe.AsPointer(ref this), vertexCount, startVertexLocation);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT Map(ID3D11Resource * pResource, uint subresource, D3D11_MAP mapType, uint mapFlags, D3D11_MAPPED_SUBRESOURCE* pMappedResource) => ((delegate* unmanaged[Stdcall]<void*, ID3D11Resource*, uint, D3D11_MAP, uint, D3D11_MAPPED_SUBRESOURCE*, HRESULT>)_vtbl[14])(Unsafe.AsPointer(ref this), pResource, subresource, mapType, mapFlags, pMappedResource);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unmap(ID3D11Resource * pResource, uint subresource) => ((delegate* unmanaged[Stdcall]<void*, ID3D11Resource*, uint, void>)_vtbl[15])(Unsafe.AsPointer(ref this), pResource, subresource);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PSSetConstantBuffers(uint startSlot, uint numBuffers, ID3D11Buffer** ppConstantBuffers) => ((delegate* unmanaged[Stdcall]<void*, uint, uint, ID3D11Buffer**, void>)_vtbl[16])(Unsafe.AsPointer(ref this), startSlot, numBuffers, ppConstantBuffers);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IASetInputLayout(ID3D11InputLayout * pInputLayout) => ((delegate* unmanaged[Stdcall]<void*, ID3D11InputLayout*, void>)_vtbl[17])(Unsafe.AsPointer(ref this), pInputLayout);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IASetVertexBuffers(uint startSlot, uint numBuffers, ID3D11Buffer** ppVertexBuffers, uint* pStrides, uint* pOffsets) => ((delegate* unmanaged[Stdcall]<void*, uint, uint, ID3D11Buffer**, uint*, uint*, void>)_vtbl[18])(Unsafe.AsPointer(ref this), startSlot, numBuffers, ppVertexBuffers, pStrides, pOffsets);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IASetIndexBuffer(ID3D11Buffer * pIndexBuffer, DXGI_FORMAT format, uint offset) => ((delegate* unmanaged[Stdcall]<void*, ID3D11Buffer*, DXGI_FORMAT, uint, void>)_vtbl[19])(Unsafe.AsPointer(ref this), pIndexBuffer, format, offset);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawIndexedInstanced(uint indexCountPerInstance, uint instanceCount, uint startIndexLocation, int baseVertexLocation, uint startInstanceLocation) => ((delegate* unmanaged[Stdcall]<void*, uint, uint, uint, int, uint, void>)_vtbl[20])(Unsafe.AsPointer(ref this), indexCountPerInstance, instanceCount, startIndexLocation, baseVertexLocation, startInstanceLocation);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawInstanced(uint vertexCountPerInstance, uint instanceCount,uint startVertexLocation,uint startInstanceLocation) => ((delegate* unmanaged[Stdcall]<void*, uint, uint, uint, uint, void>)_vtbl[21])(Unsafe.AsPointer(ref this), vertexCountPerInstance, instanceCount, startVertexLocation, startInstanceLocation);

        //void (STDMETHODCALLTYPE* GSSetConstantBuffers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - StartSlot )  UINT NumBuffers,
        //    /* [annotation] */
        //    _In_reads_opt_(NumBuffers)  ID3D11Buffer* const * ppConstantBuffers);

        //void (STDMETHODCALLTYPE* GSSetShader ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_opt_ ID3D11GeometryShader * pShader,
        //    /* [annotation] */
        //    _In_reads_opt_(NumClassInstances)  ID3D11ClassInstance* const * ppClassInstances,
        //    UINT NumClassInstances);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY topology) => ((delegate* unmanaged[Stdcall]<void*, D3D_PRIMITIVE_TOPOLOGY, void>)_vtbl[24])(Unsafe.AsPointer(ref this), topology);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void VSSetShaderResources(uint startSlot, uint numViews, ID3D11ShaderResourceView** ppShaderResourceViews) => ((delegate* unmanaged[Stdcall]<void*, uint, uint, ID3D11ShaderResourceView **, void>)_vtbl[25])(Unsafe.AsPointer(ref this), startSlot, numViews, ppShaderResourceViews);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void VSSetSamplers(uint startSlot, uint numSamplers, ID3D11SamplerState** ppSamplers) => ((delegate* unmanaged[Stdcall]<void*, uint, uint, ID3D11SamplerState **, void>)_vtbl[26])(Unsafe.AsPointer(ref this), startSlot, numSamplers, ppSamplers);

        //void (STDMETHODCALLTYPE* Begin ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11Asynchronous * pAsync);

        //void (STDMETHODCALLTYPE* End ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11Asynchronous * pAsync);

        //HRESULT(STDMETHODCALLTYPE* GetData)(
        // ID3D11DeviceContext* This,
        // /* [annotation] */
        // _In_ ID3D11Asynchronous * pAsync,
        // /* [annotation] */
        // _Out_writes_bytes_opt_( DataSize )  void* pData,
        // /* [annotation] */
        // _In_  UINT DataSize,
        // /* [annotation] */
        // _In_  UINT GetDataFlags);

        //void (STDMETHODCALLTYPE* SetPredication ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_opt_ ID3D11Predicate * pPredicate,
        //    /* [annotation] */
        //    _In_  BOOL PredicateValue);

        //void (STDMETHODCALLTYPE* GSSetShaderResources ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - StartSlot )  UINT NumViews,
        //    /* [annotation] */
        //    _In_reads_opt_(NumViews)  ID3D11ShaderResourceView* const * ppShaderResourceViews);

        //void (STDMETHODCALLTYPE* GSSetSamplers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - StartSlot )  UINT NumSamplers,
        //    /* [annotation] */
        //    _In_reads_opt_(NumSamplers)  ID3D11SamplerState* const * ppSamplers);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OMSetRenderTargets(uint numViews, ID3D11RenderTargetView** ppRenderTargetViews, ID3D11DepthStencilView* pDepthStencilView) => ((delegate* unmanaged[Stdcall]<void*, uint, ID3D11RenderTargetView **, ID3D11DepthStencilView *, void>)_vtbl[33])(Unsafe.AsPointer(ref this), numViews, ppRenderTargetViews, pDepthStencilView);

        //void (STDMETHODCALLTYPE* OMSetRenderTargetsAndUnorderedAccessViews ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ UINT NumRTVs,
        //    /* [annotation] */
        //    _In_reads_opt_(NumRTVs)  ID3D11RenderTargetView* const * ppRenderTargetViews,
        //    /* [annotation] */
        //    _In_opt_  ID3D11DepthStencilView* pDepthStencilView,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_1_UAV_SLOT_COUNT - 1 )  UINT UAVStartSlot,
        //    /* [annotation] */
        //    _In_  UINT NumUAVs,
        //    /* [annotation] */
        //    _In_reads_opt_(NumUAVs)  ID3D11UnorderedAccessView* const * ppUnorderedAccessViews,
        //    /* [annotation] */
        //    _In_reads_opt_(NumUAVs)  const UINT* pUAVInitialCounts);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OMSetBlendState(ID3D11BlendState * pBlendState, float* blendFactor, uint sampleMask) => ((delegate* unmanaged[Stdcall]<void*, ID3D11BlendState*, float*, uint, void>)_vtbl[35])(Unsafe.AsPointer(ref this), pBlendState, blendFactor, sampleMask);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OMSetDepthStencilState(ID3D11DepthStencilState * pDepthStencilState, uint stencilRef) => ((delegate* unmanaged[Stdcall]<void*, ID3D11DepthStencilState*, uint, void>)_vtbl[36])(Unsafe.AsPointer(ref this), pDepthStencilState, stencilRef);

        //void (STDMETHODCALLTYPE* SOSetTargets ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_SO_BUFFER_SLOT_COUNT)  UINT NumBuffers,
        //    /* [annotation] */
        //    _In_reads_opt_(NumBuffers)  ID3D11Buffer* const * ppSOTargets,
        //    /* [annotation] */
        //    _In_reads_opt_(NumBuffers)  const UINT* pOffsets);

        //void (STDMETHODCALLTYPE* DrawAuto ) (
        //    ID3D11DeviceContext* This);

        //void (STDMETHODCALLTYPE* DrawIndexedInstancedIndirect ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11Buffer * pBufferForArgs,
        //    /* [annotation] */
        //    _In_  UINT AlignedByteOffsetForArgs);

        //void (STDMETHODCALLTYPE* DrawInstancedIndirect ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11Buffer * pBufferForArgs,
        //    /* [annotation] */
        //    _In_  UINT AlignedByteOffsetForArgs);

        //void (STDMETHODCALLTYPE* Dispatch ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ UINT ThreadGroupCountX,
        //    /* [annotation] */
        //    _In_  UINT ThreadGroupCountY,
        //    /* [annotation] */
        //    _In_  UINT ThreadGroupCountZ);

        //void (STDMETHODCALLTYPE* DispatchIndirect ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11Buffer * pBufferForArgs,
        //    /* [annotation] */
        //    _In_  UINT AlignedByteOffsetForArgs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RSSetState(ID3D11RasterizerState* pRasterizerState) => ((delegate* unmanaged[Stdcall]<void*, ID3D11RasterizerState*, void>)_vtbl[43])(Unsafe.AsPointer(ref this), pRasterizerState);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RSSetViewports(uint numViewports, D3D11_VIEWPORT* pViewports) => ((delegate* unmanaged[Stdcall]<void*, uint, D3D11_VIEWPORT*, void>)_vtbl[44])(Unsafe.AsPointer(ref this), numViewports, pViewports);

        //void (STDMETHODCALLTYPE* RSSetScissorRects ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_(0, D3D11_VIEWPORT_AND_SCISSORRECT_OBJECT_COUNT_PER_PIPELINE)  UINT NumRects,
        //    /* [annotation] */
        //    _In_reads_opt_(NumRects)  const D3D11_RECT* pRects);

        //void (STDMETHODCALLTYPE* CopySubresourceRegion ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11Resource * pDstResource,
        //    /* [annotation] */
        //    _In_  UINT DstSubresource,
        //    /* [annotation] */
        //    _In_  UINT DstX,
        //    /* [annotation] */
        //    _In_  UINT DstY,
        //    /* [annotation] */
        //    _In_  UINT DstZ,
        //    /* [annotation] */
        //    _In_  ID3D11Resource* pSrcResource,
        //    /* [annotation] */
        //    _In_  UINT SrcSubresource,
        //    /* [annotation] */
        //    _In_opt_  const D3D11_BOX* pSrcBox);

        //void (STDMETHODCALLTYPE* CopyResource ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11Resource * pDstResource,
        //    /* [annotation] */
        //    _In_  ID3D11Resource* pSrcResource);

        //void (STDMETHODCALLTYPE* UpdateSubresource ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11Resource * pDstResource,
        //    /* [annotation] */
        //    _In_  UINT DstSubresource,
        //    /* [annotation] */
        //    _In_opt_  const D3D11_BOX* pDstBox,
        //    /* [annotation] */
        //    _In_  const void* pSrcData,
        //    /* [annotation] */
        //    _In_  UINT SrcRowPitch,
        //    /* [annotation] */
        //    _In_  UINT SrcDepthPitch);

        //void (STDMETHODCALLTYPE* CopyStructureCount ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11Buffer * pDstBuffer,
        //    /* [annotation] */
        //    _In_  UINT DstAlignedByteOffset,
        //    /* [annotation] */
        //    _In_  ID3D11UnorderedAccessView* pSrcView);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTargetView(ID3D11RenderTargetView * pRenderTargetView, float* colorRGBA) => ((delegate* unmanaged[Stdcall]<void*, ID3D11RenderTargetView*, float*, void>)_vtbl[50])(Unsafe.AsPointer(ref this), pRenderTargetView, colorRGBA);

        //void (STDMETHODCALLTYPE* ClearUnorderedAccessViewUint ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11UnorderedAccessView * pUnorderedAccessView,
        //    /* [annotation] */
        //    _In_  const UINT Values[4]);

        //void (STDMETHODCALLTYPE* ClearUnorderedAccessViewFloat ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11UnorderedAccessView * pUnorderedAccessView,
        //    /* [annotation] */
        //    _In_  const FLOAT Values[4]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearDepthStencilView(ID3D11DepthStencilView* pDepthStencilView, uint clearFlags, float depth, byte stencil) => ((delegate* unmanaged[Stdcall]<void*, ID3D11DepthStencilView*, uint, float, byte, void>)_vtbl[53])(Unsafe.AsPointer(ref this), pDepthStencilView, clearFlags, depth, stencil);

        //void (STDMETHODCALLTYPE* GenerateMips ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11ShaderResourceView * pShaderResourceView);

        //void (STDMETHODCALLTYPE* SetResourceMinLOD ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11Resource * pResource,
        //    FLOAT MinLOD);

        //FLOAT(STDMETHODCALLTYPE* GetResourceMinLOD)(
        // ID3D11DeviceContext* This,
        // /* [annotation] */
        // _In_ ID3D11Resource * pResource);

        //void (STDMETHODCALLTYPE* ResolveSubresource ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11Resource * pDstResource,
        //    /* [annotation] */
        //    _In_  UINT DstSubresource,
        //    /* [annotation] */
        //    _In_  ID3D11Resource* pSrcResource,
        //    /* [annotation] */
        //    _In_  UINT SrcSubresource,
        //    /* [annotation] */
        //    _In_  DXGI_FORMAT Format);

        //void (STDMETHODCALLTYPE* ExecuteCommandList ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_ ID3D11CommandList * pCommandList,
        //    BOOL RestoreContextState);

        //void (STDMETHODCALLTYPE* HSSetShaderResources ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - StartSlot )  UINT NumViews,
        //    /* [annotation] */
        //    _In_reads_opt_(NumViews)  ID3D11ShaderResourceView* const * ppShaderResourceViews);

        //void (STDMETHODCALLTYPE* HSSetShader ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_opt_ ID3D11HullShader * pHullShader,
        //    /* [annotation] */
        //    _In_reads_opt_(NumClassInstances)  ID3D11ClassInstance* const * ppClassInstances,
        //    UINT NumClassInstances);

        //void (STDMETHODCALLTYPE* HSSetSamplers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - StartSlot )  UINT NumSamplers,
        //    /* [annotation] */
        //    _In_reads_opt_(NumSamplers)  ID3D11SamplerState* const * ppSamplers);

        //void (STDMETHODCALLTYPE* HSSetConstantBuffers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - StartSlot )  UINT NumBuffers,
        //    /* [annotation] */
        //    _In_reads_opt_(NumBuffers)  ID3D11Buffer* const * ppConstantBuffers);

        //void DSSetShaderResources(uint startSlot, uint numViews, ID3D11ShaderResourceView** ppShaderResourceViews);

        //void DSSetShader(ID3D11DomainShader * pDomainShader, ID3D11ClassInstance** ppClassInstances,uint numClassInstances);

        //void (STDMETHODCALLTYPE* DSSetSamplers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - StartSlot )  UINT NumSamplers,
        //    /* [annotation] */
        //    _In_reads_opt_(NumSamplers)  ID3D11SamplerState* const * ppSamplers);

        //void (STDMETHODCALLTYPE* DSSetConstantBuffers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - StartSlot )  UINT NumBuffers,
        //    /* [annotation] */
        //    _In_reads_opt_(NumBuffers)  ID3D11Buffer* const * ppConstantBuffers);

        //void (STDMETHODCALLTYPE* CSSetShaderResources ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - StartSlot )  UINT NumViews,
        //    /* [annotation] */
        //    _In_reads_opt_(NumViews)  ID3D11ShaderResourceView* const * ppShaderResourceViews);

        //void (STDMETHODCALLTYPE* CSSetUnorderedAccessViews ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_1_UAV_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_1_UAV_SLOT_COUNT - StartSlot )  UINT NumUAVs,
        //    /* [annotation] */
        //    _In_reads_opt_(NumUAVs)  ID3D11UnorderedAccessView* const * ppUnorderedAccessViews,
        //    /* [annotation] */
        //    _In_reads_opt_(NumUAVs)  const UINT* pUAVInitialCounts);

        //void (STDMETHODCALLTYPE* CSSetShader ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_opt_ ID3D11ComputeShader * pComputeShader,
        //    /* [annotation] */
        //    _In_reads_opt_(NumClassInstances)  ID3D11ClassInstance* const * ppClassInstances,
        //    UINT NumClassInstances);

        //void (STDMETHODCALLTYPE* CSSetSamplers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - StartSlot )  UINT NumSamplers,
        //    /* [annotation] */
        //    _In_reads_opt_(NumSamplers)  ID3D11SamplerState* const * ppSamplers);

        //void (STDMETHODCALLTYPE* CSSetConstantBuffers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - StartSlot )  UINT NumBuffers,
        //    /* [annotation] */
        //    _In_reads_opt_(NumBuffers)  ID3D11Buffer* const * ppConstantBuffers);

        //void (STDMETHODCALLTYPE* VSGetConstantBuffers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - StartSlot )  UINT NumBuffers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumBuffers)  ID3D11Buffer** ppConstantBuffers);

        //void (STDMETHODCALLTYPE* PSGetShaderResources ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - StartSlot )  UINT NumViews,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumViews)  ID3D11ShaderResourceView** ppShaderResourceViews);

        //void (STDMETHODCALLTYPE* PSGetShader ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Outptr_result_maybenull_ ID3D11PixelShader ** ppPixelShader,
        //    /* [annotation] */
        //    _Out_writes_opt_(*pNumClassInstances)  ID3D11ClassInstance** ppClassInstances,
        //    /* [annotation] */
        //    _Inout_opt_  UINT* pNumClassInstances);

        //void (STDMETHODCALLTYPE* PSGetSamplers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - StartSlot )  UINT NumSamplers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumSamplers)  ID3D11SamplerState** ppSamplers);

        //void (STDMETHODCALLTYPE* VSGetShader ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Outptr_result_maybenull_ ID3D11VertexShader ** ppVertexShader,
        //    /* [annotation] */
        //    _Out_writes_opt_(*pNumClassInstances)  ID3D11ClassInstance** ppClassInstances,
        //    /* [annotation] */
        //    _Inout_opt_  UINT* pNumClassInstances);

        //void (STDMETHODCALLTYPE* PSGetConstantBuffers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - StartSlot )  UINT NumBuffers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumBuffers)  ID3D11Buffer** ppConstantBuffers);

        //void (STDMETHODCALLTYPE* IAGetInputLayout ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Outptr_result_maybenull_ ID3D11InputLayout ** ppInputLayout);

        //void (STDMETHODCALLTYPE* IAGetVertexBuffers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_IA_VERTEX_INPUT_RESOURCE_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_IA_VERTEX_INPUT_RESOURCE_SLOT_COUNT - StartSlot )  UINT NumBuffers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumBuffers)  ID3D11Buffer** ppVertexBuffers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumBuffers)  UINT* pStrides,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumBuffers)  UINT* pOffsets);

        //void (STDMETHODCALLTYPE* IAGetIndexBuffer ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Outptr_opt_result_maybenull_ ID3D11Buffer ** pIndexBuffer,
        //    /* [annotation] */
        //    _Out_opt_  DXGI_FORMAT* Format,
        //    /* [annotation] */
        //    _Out_opt_  UINT* Offset);

        //void (STDMETHODCALLTYPE* GSGetConstantBuffers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - StartSlot )  UINT NumBuffers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumBuffers)  ID3D11Buffer** ppConstantBuffers);

        //void (STDMETHODCALLTYPE* GSGetShader ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Outptr_result_maybenull_ ID3D11GeometryShader ** ppGeometryShader,
        //    /* [annotation] */
        //    _Out_writes_opt_(*pNumClassInstances)  ID3D11ClassInstance** ppClassInstances,
        //    /* [annotation] */
        //    _Inout_opt_  UINT* pNumClassInstances);

        //void (STDMETHODCALLTYPE* IAGetPrimitiveTopology ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Out_ D3D11_PRIMITIVE_TOPOLOGY * pTopology);

        //void (STDMETHODCALLTYPE* VSGetShaderResources ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - StartSlot )  UINT NumViews,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumViews)  ID3D11ShaderResourceView** ppShaderResourceViews);

        //void (STDMETHODCALLTYPE* VSGetSamplers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - StartSlot )  UINT NumSamplers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumSamplers)  ID3D11SamplerState** ppSamplers);

        //void (STDMETHODCALLTYPE* GetPredication ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Outptr_opt_result_maybenull_ ID3D11Predicate ** ppPredicate,
        //    /* [annotation] */
        //    _Out_opt_  BOOL* pPredicateValue);

        //void (STDMETHODCALLTYPE* GSGetShaderResources ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - StartSlot )  UINT NumViews,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumViews)  ID3D11ShaderResourceView** ppShaderResourceViews);

        //void (STDMETHODCALLTYPE* GSGetSamplers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - StartSlot )  UINT NumSamplers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumSamplers)  ID3D11SamplerState** ppSamplers);

        //void (STDMETHODCALLTYPE* OMGetRenderTargets ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT )  UINT NumViews,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumViews)  ID3D11RenderTargetView** ppRenderTargetViews,
        //    /* [annotation] */
        //    _Outptr_opt_result_maybenull_  ID3D11DepthStencilView** ppDepthStencilView);

        //void (STDMETHODCALLTYPE* OMGetRenderTargetsAndUnorderedAccessViews ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT )  UINT NumRTVs,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumRTVs)  ID3D11RenderTargetView** ppRenderTargetViews,
        //    /* [annotation] */
        //    _Outptr_opt_result_maybenull_  ID3D11DepthStencilView** ppDepthStencilView,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_PS_CS_UAV_REGISTER_COUNT - 1 )  UINT UAVStartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_PS_CS_UAV_REGISTER_COUNT - UAVStartSlot )  UINT NumUAVs,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumUAVs)  ID3D11UnorderedAccessView** ppUnorderedAccessViews);

        //void (STDMETHODCALLTYPE* OMGetBlendState ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Outptr_opt_result_maybenull_ ID3D11BlendState ** ppBlendState,
        //    /* [annotation] */
        //    _Out_opt_  FLOAT BlendFactor[4],
        //    /* [annotation] */
        //    _Out_opt_  UINT* pSampleMask);

        //void (STDMETHODCALLTYPE* OMGetDepthStencilState ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Outptr_opt_result_maybenull_ ID3D11DepthStencilState ** ppDepthStencilState,
        //    /* [annotation] */
        //    _Out_opt_  UINT* pStencilRef);

        //void (STDMETHODCALLTYPE* SOGetTargets ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_SO_BUFFER_SLOT_COUNT )  UINT NumBuffers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumBuffers)  ID3D11Buffer** ppSOTargets);

        //void (STDMETHODCALLTYPE* RSGetState ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Outptr_result_maybenull_ ID3D11RasterizerState ** ppRasterizerState);

        //void (STDMETHODCALLTYPE* RSGetViewports ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Inout_ /*_range(0, D3D11_VIEWPORT_AND_SCISSORRECT_OBJECT_COUNT_PER_PIPELINE )*/   UINT * pNumViewports,
        //    /* [annotation] */
        //    _Out_writes_opt_(*pNumViewports)  D3D11_VIEWPORT* pViewports);

        //void (STDMETHODCALLTYPE* RSGetScissorRects ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Inout_ /*_range(0, D3D11_VIEWPORT_AND_SCISSORRECT_OBJECT_COUNT_PER_PIPELINE )*/   UINT * pNumRects,
        //    /* [annotation] */
        //    _Out_writes_opt_(*pNumRects)  D3D11_RECT* pRects);

        //void (STDMETHODCALLTYPE* HSGetShaderResources ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - StartSlot )  UINT NumViews,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumViews)  ID3D11ShaderResourceView** ppShaderResourceViews);

        //void (STDMETHODCALLTYPE* HSGetShader ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Outptr_result_maybenull_ ID3D11HullShader ** ppHullShader,
        //    /* [annotation] */
        //    _Out_writes_opt_(*pNumClassInstances)  ID3D11ClassInstance** ppClassInstances,
        //    /* [annotation] */
        //    _Inout_opt_  UINT* pNumClassInstances);

        //void (STDMETHODCALLTYPE* HSGetSamplers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - StartSlot )  UINT NumSamplers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumSamplers)  ID3D11SamplerState** ppSamplers);

        //void (STDMETHODCALLTYPE* HSGetConstantBuffers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - StartSlot )  UINT NumBuffers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumBuffers)  ID3D11Buffer** ppConstantBuffers);

        //void (STDMETHODCALLTYPE* DSGetShaderResources ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - StartSlot )  UINT NumViews,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumViews)  ID3D11ShaderResourceView** ppShaderResourceViews);

        //void (STDMETHODCALLTYPE* DSGetShader ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Outptr_result_maybenull_ ID3D11DomainShader ** ppDomainShader,
        //    /* [annotation] */
        //    _Out_writes_opt_(*pNumClassInstances)  ID3D11ClassInstance** ppClassInstances,
        //    /* [annotation] */
        //    _Inout_opt_  UINT* pNumClassInstances);

        //void (STDMETHODCALLTYPE* DSGetSamplers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - StartSlot )  UINT NumSamplers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumSamplers)  ID3D11SamplerState** ppSamplers);

        //void (STDMETHODCALLTYPE* DSGetConstantBuffers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - StartSlot )  UINT NumBuffers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumBuffers)  ID3D11Buffer** ppConstantBuffers);

        //void (STDMETHODCALLTYPE* CSGetShaderResources ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT - StartSlot )  UINT NumViews,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumViews)  ID3D11ShaderResourceView** ppShaderResourceViews);

        //void (STDMETHODCALLTYPE* CSGetUnorderedAccessViews ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_1_UAV_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_1_UAV_SLOT_COUNT - StartSlot )  UINT NumUAVs,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumUAVs)  ID3D11UnorderedAccessView** ppUnorderedAccessViews);

        //void (STDMETHODCALLTYPE* CSGetShader ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _Outptr_result_maybenull_ ID3D11ComputeShader ** ppComputeShader,
        //    /* [annotation] */
        //    _Out_writes_opt_(*pNumClassInstances)  ID3D11ClassInstance** ppClassInstances,
        //    /* [annotation] */
        //    _Inout_opt_  UINT* pNumClassInstances);

        //void (STDMETHODCALLTYPE* CSGetSamplers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT - StartSlot )  UINT NumSamplers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumSamplers)  ID3D11SamplerState** ppSamplers);

        //void (STDMETHODCALLTYPE* CSGetConstantBuffers ) (
        //    ID3D11DeviceContext* This,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - 1 )  UINT StartSlot,
        //    /* [annotation] */
        //    _In_range_( 0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT - StartSlot )  UINT NumBuffers,
        //    /* [annotation] */
        //    _Out_writes_opt_(NumBuffers)  ID3D11Buffer** ppConstantBuffers);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearState() => ((delegate* unmanaged[Stdcall]<void*, void>)_vtbl[110])(Unsafe.AsPointer(ref this));

        //void (STDMETHODCALLTYPE* Flush ) (
        //    ID3D11DeviceContext* This);

        //D3D11_DEVICE_CONTEXT_TYPE(STDMETHODCALLTYPE* GetType)(
        // ID3D11DeviceContext* This);

        //UINT(STDMETHODCALLTYPE* GetContextFlags)(
        // ID3D11DeviceContext* This);

        //HRESULT(STDMETHODCALLTYPE* FinishCommandList)(
        // ID3D11DeviceContext* This,
        // BOOL RestoreDeferredContextState,
        // /* [annotation] */
        // _COM_Outptr_opt_ ID3D11CommandList ** ppCommandList);


    }
}
