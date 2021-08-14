using System.Runtime.CompilerServices;
using Titan.Windows.Win32;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.D3D11
{
    public unsafe struct IDXGISurface1
    {
        private void** _vtbl;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

        //HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
        // IDXGISurface1* This,
        // /* [annotation][in] */
        // _In_ REFGUID Name,
        // /* [in] */ UINT DataSize,
        //    /* [annotation][in] */ 
        //    _In_reads_bytes_(DataSize)  const void* pData);

        //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
        // IDXGISurface1* This,
        // /* [annotation][in] */
        // _In_ REFGUID Name,
        // /* [annotation][in] */
        // _In_opt_  const IUnknown* pUnknown);

        //HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
        // IDXGISurface1* This,
        // /* [annotation][in] */
        // _In_ REFGUID Name,
        // /* [annotation][out][in] */
        // _Inout_  UINT* pDataSize,
        // /* [annotation][out] */
        // _Out_writes_bytes_(*pDataSize)  void* pData);

        //HRESULT(STDMETHODCALLTYPE* GetParent)(
        // IDXGISurface1* This,
        // /* [annotation][in] */
        // _In_ REFIID riid,
        // /* [annotation][retval][out] */
        // _COM_Outptr_  void** ppParent);

        //HRESULT(STDMETHODCALLTYPE* GetDevice)(
        // IDXGISurface1* This,
        // /* [annotation][in] */
        // _In_ REFIID riid,
        // /* [annotation][retval][out] */
        // _COM_Outptr_  void** ppDevice);

        //HRESULT(STDMETHODCALLTYPE* GetDesc)(
        // IDXGISurface1* This,
        // /* [annotation][out] */
        // _Out_ DXGI_SURFACE_DESC * pDesc);

        //HRESULT(STDMETHODCALLTYPE* Map)(
        // IDXGISurface1* This,
        // /* [annotation][out] */
        // _Out_ DXGI_MAPPED_RECT * pLockedRect,
        // /* [in] */ UINT MapFlags);
        
        //HRESULT(STDMETHODCALLTYPE* Unmap)(
        // IDXGISurface1* This);
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT GetDC(int discard, HDC* pHdc) => ((delegate* unmanaged[Stdcall]<void*, int, HDC*, HRESULT>)_vtbl[11])(Unsafe.AsPointer(ref this), discard, pHdc);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT ReleaseDC(RECT* pDirtyRect) => ((delegate* unmanaged[Stdcall]<void*, RECT*, HRESULT>)_vtbl[12])(Unsafe.AsPointer(ref this), pDirtyRect);
    }

    public unsafe struct HDC
    {
        private void* _ptr;
    }
}
