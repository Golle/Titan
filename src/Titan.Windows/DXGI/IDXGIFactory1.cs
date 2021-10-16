using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.DXGI;

public unsafe struct IDXGIFactory1
{
    public static readonly Guid UUID = new("770aae78-f26f-4dba-a829-253c83d1b387");
    private void** _vtbl;

    //HRESULT(STDMETHODCALLTYPE* QueryInterface)(
    //     IDXGIFactory1* This,
    //     /* [in] */ REFIID riid,
    //     /* [annotation][iid_is][out] */
    //     _COM_Outptr_  void** ppvObject);

    //ULONG(STDMETHODCALLTYPE* AddRef)(
    // IDXGIFactory1* This);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //    HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
    //     IDXGIFactory1* This,
    //     /* [annotation][in] */
    //     _In_ REFGUID Name,
    //     /* [in] */ UINT DataSize,
    //        /* [annotation][in] */ 
    //        _In_reads_bytes_(DataSize)  const void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
    // IDXGIFactory1* This,
    // /* [annotation][in] */
    // _In_ REFGUID Name,
    // /* [annotation][in] */
    // _In_opt_  const IUnknown* pUnknown);

    //HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
    // IDXGIFactory1* This,
    // /* [annotation][in] */
    // _In_ REFGUID Name,
    // /* [annotation][out][in] */
    // _Inout_  UINT* pDataSize,
    // /* [annotation][out] */
    // _Out_writes_bytes_(*pDataSize)  void* pData);

    //HRESULT(STDMETHODCALLTYPE* GetParent)(
    // IDXGIFactory1* This,
    // /* [annotation][in] */
    // _In_ REFIID riid,
    // /* [annotation][retval][out] */
    // _COM_Outptr_  void** ppParent);

    //HRESULT(STDMETHODCALLTYPE* EnumAdapters)(
    // IDXGIFactory1* This,
    // /* [in] */ UINT Adapter,
    // /* [annotation][out] */
    // _COM_Outptr_ IDXGIAdapter ** ppAdapter);

    //HRESULT(STDMETHODCALLTYPE* MakeWindowAssociation)(
    // IDXGIFactory1* This,
    // HWND WindowHandle,
    // UINT Flags);

    //    HRESULT(STDMETHODCALLTYPE* GetWindowAssociation)(
    //     IDXGIFactory1* This,
    //     /* [annotation][out] */
    //     _Out_ HWND * pWindowHandle);

    //HRESULT(STDMETHODCALLTYPE* CreateSwapChain)(
    // IDXGIFactory1* This,
    // /* [annotation][in] */
    // _In_ IUnknown * pDevice,
    // /* [annotation][in] */
    // _In_  DXGI_SWAP_CHAIN_DESC* pDesc,
    // /* [annotation][out] */
    // _COM_Outptr_  IDXGISwapChain** ppSwapChain);

    //HRESULT(STDMETHODCALLTYPE* CreateSoftwareAdapter)(
    // IDXGIFactory1* This,
    // /* [in] */ HMODULE Module,
    // /* [annotation][out] */
    // _COM_Outptr_ IDXGIAdapter ** ppAdapter);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT EnumAdapters1(uint adapter, IDXGIAdapter1 ** ppAdapter) =>
        ((delegate* unmanaged[Stdcall]<void*, uint, IDXGIAdapter1**, HRESULT>)_vtbl[12])(Unsafe.AsPointer(ref this), adapter, ppAdapter);

    //BOOL(STDMETHODCALLTYPE* IsCurrent)(
    // IDXGIFactory1* This);


}

public static unsafe class DXGICommon
{
    [DllImport("dxgi", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT CreateDXGIFactory1(in Guid riid, void** ppFactory);
}
