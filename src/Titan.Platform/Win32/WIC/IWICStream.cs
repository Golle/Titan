using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32.WIC;

public unsafe struct IStream
{
    private void** _vtbl;
}
public unsafe struct IWICStream
{
    private void** _vtbl;

//        HRESULT(STDMETHODCALLTYPE* QueryInterface)(
//        __RPC__in IWICStream * This,
//        /* [in] */ __RPC__in REFIID riid,
//        /* [annotation][iid_is][out] */
//        _COM_Outptr_  void** ppvObject);

//        ULONG(STDMETHODCALLTYPE* AddRef)(
//         __RPC__in IWICStream * This);

//        ULONG(STDMETHODCALLTYPE* Release)(
//         __RPC__in IWICStream * This);

//        /* [local] */
//        HRESULT(STDMETHODCALLTYPE* Read)(
//IWICStream* This,
    ///* [annotation] */
//_Out_writes_bytes_to_(cb, * pcbRead) void* pv,
    ///* [annotation][in] */
//_In_  ULONG cb,
    ///* [annotation] */
//_Out_opt_  ULONG* pcbRead);

//        /* [local] */
//        HRESULT(STDMETHODCALLTYPE* Write)(
//IWICStream* This,
    ///* [annotation] */
//_In_reads_bytes_(cb) const void* pv,
    ///* [annotation][in] */
//_In_  ULONG cb,
    ///* [annotation] */
//_Out_opt_  ULONG* pcbWritten);

//        /* [local] */
//        HRESULT(STDMETHODCALLTYPE* Seek)(
//IWICStream* This,
    ///* [in] */ LARGE_INTEGER dlibMove,
    ///* [in] */ DWORD dwOrigin,
    ///* [annotation] */
//_Out_opt_ ULARGE_INTEGER * plibNewPosition);

//        HRESULT(STDMETHODCALLTYPE* SetSize)(
//         __RPC__in IWICStream * This,
//         /* [in] */ ULARGE_INTEGER libNewSize);

//        /* [local] */ HRESULT(STDMETHODCALLTYPE* CopyTo)(
//         IWICStream* This,
//         /* [annotation][unique][in] */
//         _In_ IStream * pstm,
//         /* [in] */ ULARGE_INTEGER cb,
//            /* [annotation] */ 
//            _Out_opt_ ULARGE_INTEGER *pcbRead,
//            /* [annotation] */ 
//            _Out_opt_ ULARGE_INTEGER *pcbWritten);

//        HRESULT(STDMETHODCALLTYPE* Commit)(
//         __RPC__in IWICStream * This,
//         /* [in] */ DWORD grfCommitFlags);

//        HRESULT(STDMETHODCALLTYPE* Revert)(
//         __RPC__in IWICStream * This);

//        HRESULT(STDMETHODCALLTYPE* LockRegion)(
//         __RPC__in IWICStream * This,
//         /* [in] */ ULARGE_INTEGER libOffset,
//            /* [in] */ ULARGE_INTEGER cb,
//            /* [in] */ DWORD dwLockType);

//        HRESULT(STDMETHODCALLTYPE* UnlockRegion)(
//         __RPC__in IWICStream * This,
//         /* [in] */ ULARGE_INTEGER libOffset,
//            /* [in] */ ULARGE_INTEGER cb,
//            /* [in] */ DWORD dwLockType);

//        HRESULT(STDMETHODCALLTYPE* Stat)(
//         __RPC__in IWICStream * This,
//         /* [out] */ __RPC__out STATSTG* pstatstg,
//         /* [in] */ DWORD grfStatFlag);

//        HRESULT(STDMETHODCALLTYPE* Clone)(
//         __RPC__in IWICStream * This,
//         /* [out] */ __RPC__deref_out_opt IStream** ppstm);

//        HRESULT(STDMETHODCALLTYPE* InitializeFromIStream)(
//         __RPC__in IWICStream * This,
//         /* [in] */ __RPC__in_opt IStream* pIStream);

//        HRESULT(STDMETHODCALLTYPE* InitializeFromFilename)(
//         __RPC__in IWICStream * This,
//         /* [in] */ __RPC__in LPCWSTR wzFileName,
//         /* [in] */ DWORD dwDesiredAccess);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT InitializeFromMemory(byte* pbBuffer, uint cbBufferSize) => ((delegate* unmanaged[Stdcall]<void*, byte*, uint, HRESULT>) _vtbl[16])(Unsafe.AsPointer(ref this), pbBuffer, cbBufferSize);

    //        HRESULT(STDMETHODCALLTYPE* InitializeFromIStreamRegion)(
    //         __RPC__in IWICStream * This,
    //         /* [in] */ __RPC__in_opt IStream* pIStream,
    //         /* [in] */ ULARGE_INTEGER ulOffset,
    //            /* [in] */ ULARGE_INTEGER ulMaxSize);
}
