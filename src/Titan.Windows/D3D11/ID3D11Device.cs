using System;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.D3D11
{
    public unsafe struct ID3D11Device
    {
        private void** _vtbl;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT QueryInterface(Guid* riid, void** ppvObject) => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT CreateBuffer(D3D11_BUFFER_DESC* pDesc, D3D11_SUBRESOURCE_DATA* pInitialData, ID3D11Buffer** ppBuffer) => ((delegate* unmanaged[Stdcall]<void*, D3D11_BUFFER_DESC*, D3D11_SUBRESOURCE_DATA*, ID3D11Buffer**, HRESULT>)_vtbl[3])(Unsafe.AsPointer(ref this), pDesc, pInitialData, ppBuffer);

        //HRESULT(STDMETHODCALLTYPE* CreateTexture1D)(
        // ID3D11Device* This,
        // /* [annotation] */
        // _In_  const D3D11_TEXTURE1D_DESC* pDesc,
        // /* [annotation] */
        // _In_reads_opt_(_Inexpressible_(pDesc->MipLevels * pDesc->ArraySize))  const D3D11_SUBRESOURCE_DATA* pInitialData,
        // /* [annotation] */
        // _COM_Outptr_opt_  ID3D11Texture1D** ppTexture1D);

        public HRESULT CreateTexture2D(D3D11_TEXTURE2D_DESC* pDesc, D3D11_SUBRESOURCE_DATA* pInitialData, ID3D11Texture2D** ppTexture2D) => ((delegate* unmanaged[Stdcall]<void*, D3D11_TEXTURE2D_DESC*, D3D11_SUBRESOURCE_DATA*, ID3D11Texture2D**, HRESULT>)_vtbl[5])(Unsafe.AsPointer(ref this), pDesc, pInitialData, ppTexture2D);

        //HRESULT(STDMETHODCALLTYPE* CreateTexture3D)(
        // ID3D11Device* This,
        // /* [annotation] */
        // _In_  const D3D11_TEXTURE3D_DESC* pDesc,
        // /* [annotation] */
        // _In_reads_opt_(_Inexpressible_(pDesc->MipLevels))  const D3D11_SUBRESOURCE_DATA* pInitialData,
        // /* [annotation] */
        // _COM_Outptr_opt_  ID3D11Texture3D** ppTexture3D);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT CreateShaderResourceView(ID3D11Resource* pResource, D3D11_SHADER_RESOURCE_VIEW_DESC* pDesc, ID3D11ShaderResourceView** ppSRView) => ((delegate* unmanaged[Stdcall]<void*, ID3D11Resource*, D3D11_SHADER_RESOURCE_VIEW_DESC*, ID3D11ShaderResourceView**, HRESULT>)_vtbl[7])(Unsafe.AsPointer(ref this), pResource, pDesc, ppSRView);

        //HRESULT(STDMETHODCALLTYPE* CreateUnorderedAccessView)(
        // ID3D11Device* This,
        // /* [annotation] */
        // _In_ ID3D11Resource * pResource,
        // /* [annotation] */
        // _In_opt_  const D3D11_UNORDERED_ACCESS_VIEW_DESC* pDesc,
        // /* [annotation] */
        // _COM_Outptr_opt_  ID3D11UnorderedAccessView** ppUAView);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT CreateRenderTargetView(ID3D11Resource* pResource, D3D11_RENDER_TARGET_VIEW_DESC* pDesc, ID3D11RenderTargetView** ppRTView) => ((delegate* unmanaged[Stdcall]<void*, ID3D11Resource*, D3D11_RENDER_TARGET_VIEW_DESC*, ID3D11RenderTargetView**, HRESULT>)_vtbl[9])(Unsafe.AsPointer(ref this), pResource, pDesc, ppRTView);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT CreateDepthStencilView(ID3D11Resource* pResource, D3D11_DEPTH_STENCIL_VIEW_DESC* pDesc, ID3D11DepthStencilView** ppDepthStencilView) => ((delegate* unmanaged[Stdcall]<void*, ID3D11Resource*, D3D11_DEPTH_STENCIL_VIEW_DESC*, ID3D11DepthStencilView**, HRESULT>)_vtbl[10])(Unsafe.AsPointer(ref this), pResource, pDesc, ppDepthStencilView);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT CreateInputLayout(D3D11_INPUT_ELEMENT_DESC* pInputElementDescs, uint numElements, void* pShaderBytecodeWithInputSignature, nuint bytecodeLength, ID3D11InputLayout** ppInputLayout)
        => ((delegate* unmanaged[Stdcall]<void*, D3D11_INPUT_ELEMENT_DESC*, uint, void*, nuint, ID3D11InputLayout**, HRESULT>)_vtbl[11])(Unsafe.AsPointer(ref this), pInputElementDescs, numElements, pShaderBytecodeWithInputSignature, bytecodeLength, ppInputLayout);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT CreateVertexShader(void* pShaderBytecode, nuint bytecodeLength, ID3D11ClassLinkage* pClassLinkage, ID3D11VertexShader** ppVertexShader) 
        => ((delegate* unmanaged[Stdcall]<void*, void*, nuint, ID3D11ClassLinkage*, ID3D11VertexShader**, HRESULT>)_vtbl[12])(Unsafe.AsPointer(ref this), pShaderBytecode, bytecodeLength, pClassLinkage, ppVertexShader);

        ////HRESULT(STDMETHODCALLTYPE* CreateGeometryShader)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_reads_(BytecodeLength) const void* pShaderBytecode,
        /////* [annotation] */
        ////_In_  SIZE_T BytecodeLength,
        /////* [annotation] */
        ////_In_opt_  ID3D11ClassLinkage* pClassLinkage,
        /////* [annotation] */
        ////_COM_Outptr_opt_  ID3D11GeometryShader** ppGeometryShader);

        ////HRESULT(STDMETHODCALLTYPE* CreateGeometryShaderWithStreamOutput)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_reads_(BytecodeLength) const void* pShaderBytecode,
        /////* [annotation] */
        ////_In_  SIZE_T BytecodeLength,
        /////* [annotation] */
        ////_In_reads_opt_(NumEntries)  const D3D11_SO_DECLARATION_ENTRY* pSODeclaration,
        /////* [annotation] */
        ////_In_range_( 0, D3D11_SO_STREAM_COUNT * D3D11_SO_OUTPUT_COMPONENT_COUNT )  UINT NumEntries,
        /////* [annotation] */
        ////_In_reads_opt_(NumStrides)  const UINT* pBufferStrides,
        /////* [annotation] */
        ////_In_range_( 0, D3D11_SO_BUFFER_SLOT_COUNT )  UINT NumStrides,
        /////* [annotation] */
        ////_In_  UINT RasterizedStream,
        /////* [annotation] */
        ////_In_opt_  ID3D11ClassLinkage* pClassLinkage,
        /////* [annotation] */
        ////_COM_Outptr_opt_  ID3D11GeometryShader** ppGeometryShader);

        public HRESULT CreatePixelShader(void* pShaderBytecode, nuint bytecodeLength, ID3D11ClassLinkage* pClassLinkage, ID3D11PixelShader** ppPixelShader)
            => ((delegate* unmanaged[Stdcall]<void*, void*, nuint, ID3D11ClassLinkage*, ID3D11PixelShader**, HRESULT>)_vtbl[15])(Unsafe.AsPointer(ref this), pShaderBytecode, bytecodeLength, pClassLinkage, ppPixelShader);

        ////HRESULT(STDMETHODCALLTYPE* CreateHullShader)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_reads_(BytecodeLength) const void* pShaderBytecode,
        /////* [annotation] */
        ////_In_  SIZE_T BytecodeLength,
        /////* [annotation] */
        ////_In_opt_  ID3D11ClassLinkage* pClassLinkage,
        /////* [annotation] */
        ////_COM_Outptr_opt_  ID3D11HullShader** ppHullShader);

        ////HRESULT(STDMETHODCALLTYPE* CreateDomainShader)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_reads_(BytecodeLength) const void* pShaderBytecode,
        /////* [annotation] */
        ////_In_  SIZE_T BytecodeLength,
        /////* [annotation] */
        ////_In_opt_  ID3D11ClassLinkage* pClassLinkage,
        /////* [annotation] */
        ////_COM_Outptr_opt_  ID3D11DomainShader** ppDomainShader);

        ////HRESULT(STDMETHODCALLTYPE* CreateComputeShader)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_reads_(BytecodeLength) const void* pShaderBytecode,
        /////* [annotation] */
        ////_In_  SIZE_T BytecodeLength,
        /////* [annotation] */
        ////_In_opt_  ID3D11ClassLinkage* pClassLinkage,
        /////* [annotation] */
        ////_COM_Outptr_opt_  ID3D11ComputeShader** ppComputeShader);

        ////HRESULT(STDMETHODCALLTYPE* CreateClassLinkage)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _COM_Outptr_ ID3D11ClassLinkage ** ppLinkage);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT CreateBlendState(D3D11_BLEND_DESC* pBlendStateDesc, ID3D11BlendState** ppBlendState) => ((delegate* unmanaged[Stdcall]<void*, D3D11_BLEND_DESC*, ID3D11BlendState**, HRESULT>)_vtbl[20])(Unsafe.AsPointer(ref this), pBlendStateDesc, ppBlendState);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT CreateDepthStencilState(D3D11_DEPTH_STENCIL_DESC* pDepthStencilDesc, ID3D11DepthStencilState** ppDepthStencilState) => ((delegate* unmanaged[Stdcall]<void*, D3D11_DEPTH_STENCIL_DESC*, ID3D11DepthStencilState**, HRESULT>)_vtbl[21])(Unsafe.AsPointer(ref this), pDepthStencilDesc, ppDepthStencilState);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT CreateRasterizerState(D3D11_RASTERIZER_DESC* pRasterizerDesc, ID3D11RasterizerState** ppRasterizerState) => ((delegate* unmanaged[Stdcall]<void*, D3D11_RASTERIZER_DESC*, ID3D11RasterizerState**, HRESULT>)_vtbl[22])(Unsafe.AsPointer(ref this), pRasterizerDesc, ppRasterizerState);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT CreateSamplerState(D3D11_SAMPLER_DESC* pSamplerDesc, ID3D11SamplerState** ppSamplerState) => ((delegate* unmanaged[Stdcall]<void*, D3D11_SAMPLER_DESC*, ID3D11SamplerState**, HRESULT>)_vtbl[23])(Unsafe.AsPointer(ref this), pSamplerDesc, ppSamplerState);

        ////HRESULT(STDMETHODCALLTYPE* CreateQuery)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_  const D3D11_QUERY_DESC* pQueryDesc,
        //// /* [annotation] */
        //// _COM_Outptr_opt_  ID3D11Query** ppQuery);

        ////HRESULT(STDMETHODCALLTYPE* CreatePredicate)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_  const D3D11_QUERY_DESC* pPredicateDesc,
        //// /* [annotation] */
        //// _COM_Outptr_opt_  ID3D11Predicate** ppPredicate);

        ////HRESULT(STDMETHODCALLTYPE* CreateCounter)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_  const D3D11_COUNTER_DESC* pCounterDesc,
        //// /* [annotation] */
        //// _COM_Outptr_opt_  ID3D11Counter** ppCounter);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT CreateDeferredContext(uint contextFlags, ID3D11DeviceContext ** ppDeferredContext) =>  ((delegate* unmanaged[Stdcall]<void*, uint, ID3D11DeviceContext**, HRESULT>)_vtbl[27])(Unsafe.AsPointer(ref this), contextFlags, ppDeferredContext);

        ////HRESULT(STDMETHODCALLTYPE* OpenSharedResource)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_ HANDLE hResource,
        //// /* [annotation] */
        //// _In_  REFIID ReturnedInterface,
        //// /* [annotation] */
        //// _COM_Outptr_opt_  void** ppResource);

        ////HRESULT(STDMETHODCALLTYPE* CheckFormatSupport)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_ DXGI_FORMAT Format,
        //// /* [annotation] */
        //// _Out_  UINT* pFormatSupport);

        ////HRESULT(STDMETHODCALLTYPE* CheckMultisampleQualityLevels)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_ DXGI_FORMAT Format,
        //// /* [annotation] */
        //// _In_  UINT SampleCount,
        //// /* [annotation] */
        //// _Out_  UINT* pNumQualityLevels);

        ////void (STDMETHODCALLTYPE* CheckCounterInfo ) (
        ////    ID3D11Device* This,
        ////    /* [annotation] */
        ////    _Out_ D3D11_COUNTER_INFO * pCounterInfo);

        ////HRESULT(STDMETHODCALLTYPE* CheckCounter)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_  const D3D11_COUNTER_DESC* pDesc,
        //// /* [annotation] */
        //// _Out_  D3D11_COUNTER_TYPE* pType,
        //// /* [annotation] */
        //// _Out_  UINT* pActiveCounters,
        //// /* [annotation] */
        //// _Out_writes_opt_(*pNameLength)  LPSTR szName,
        //// /* [annotation] */
        //// _Inout_opt_  UINT* pNameLength,
        //// /* [annotation] */
        //// _Out_writes_opt_(*pUnitsLength)  LPSTR szUnits,
        //// /* [annotation] */
        //// _Inout_opt_  UINT* pUnitsLength,
        //// /* [annotation] */
        //// _Out_writes_opt_(*pDescriptionLength)  LPSTR szDescription,
        //// /* [annotation] */
        //// _Inout_opt_  UINT* pDescriptionLength);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT CheckFeatureSupport(D3D11_FEATURE feature, void* pFeatureSupportData, uint featureSupportDataSize) =>
            ((delegate* unmanaged[Stdcall]<void*, D3D11_FEATURE, void*, uint, HRESULT>)_vtbl[33])(Unsafe.AsPointer(ref this), feature, pFeatureSupportData, featureSupportDataSize);

        ////HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_ REFGUID guid,
        //// /* [annotation] */
        //// _Inout_  UINT* pDataSize,
        //// /* [annotation] */
        //// _Out_writes_bytes_opt_(*pDataSize)  void* pData);

        ////HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_ REFGUID guid,
        //// /* [annotation] */
        //// _In_  UINT DataSize,
        //// /* [annotation] */
        //// _In_reads_bytes_opt_(DataSize)  const void* pData);

        ////HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
        //// ID3D11Device* This,
        //// /* [annotation] */
        //// _In_ REFGUID guid,
        //// /* [annotation] */
        //// _In_opt_  const IUnknown* pData);

        //D3D_FEATURE_LEVEL(STDMETHODCALLTYPE* GetFeatureLevel)(
        // ID3D11Device* This);

        ////UINT(STDMETHODCALLTYPE* GetCreationFlags)(
        //// ID3D11Device* This);

        ////HRESULT(STDMETHODCALLTYPE* GetDeviceRemovedReason)(
        //// ID3D11Device* This);

        ////void (STDMETHODCALLTYPE* GetImmediateContext ) (
        ////    ID3D11Device* This,
        ////    /* [annotation] */
        ////    _Outptr_ ID3D11DeviceContext ** ppImmediateContext);

        ////HRESULT(STDMETHODCALLTYPE* SetExceptionMode)(
        //// ID3D11Device* This,
        //// UINT RaiseFlags);

        ////UINT(STDMETHODCALLTYPE* GetExceptionMode)(
        //// ID3D11Device* This);
    }
}
