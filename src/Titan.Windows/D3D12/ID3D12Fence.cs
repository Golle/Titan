using System;
using System.Runtime.CompilerServices;

namespace Titan.Windows.D3D12;

public unsafe struct ID3D12Fence
{
    public static readonly Guid UUID = new("0a753dcf-c4d8-4b91-adf6-be5a60d95a76");
    private void** _vtbl;

    //HRESULT(STDMETHODCALLTYPE* QueryInterface)(
    //ID3D12Fence* This,
    //REFIID riid,
    //_COM_Outptr_  void** ppvObject);

    //ULONG(STDMETHODCALLTYPE* AddRef)(
    //    ID3D12Fence* This);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
    //ID3D12Fence* This,
    //_In_ REFGUID guid,
    //_Inout_  UINT* pDataSize,
    //_Out_writes_bytes_opt_( *pDataSize )  void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
    //ID3D12Fence* This,
    //_In_ REFGUID guid,
    //_In_  UINT DataSize,
    //_In_reads_bytes_opt_( DataSize )  const void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
    //ID3D12Fence* This,
    //_In_ REFGUID guid,
    //_In_opt_  const IUnknown* pData);

    //HRESULT(STDMETHODCALLTYPE* SetName)(
    //ID3D12Fence* This,
    //_In_z_ LPCWSTR Name);

    //HRESULT(STDMETHODCALLTYPE* GetDevice)(
    //ID3D12Fence* This,
    //REFIID riid,
    //_COM_Outptr_opt_  void** ppvDevice);

    //UINT64(STDMETHODCALLTYPE* GetCompletedValue)(
    //    ID3D12Fence* This);

    //HRESULT(STDMETHODCALLTYPE* SetEventOnCompletion)(
    //    ID3D12Fence* This,
    //    UINT64 Value,
    //    HANDLE hEvent);

    //HRESULT(STDMETHODCALLTYPE* Signal)(
    //    ID3D12Fence* This,
    //    UINT64 Value);

}
