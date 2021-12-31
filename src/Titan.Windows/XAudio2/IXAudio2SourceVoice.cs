using System.Runtime.CompilerServices;

namespace Titan.Windows.XAudio2;

public unsafe struct IXAudio2SourceVoice
{
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SubmitSourceBuffer(
        XAUDIO2_BUFFER* pBuffer,
        XAUDIO2_BUFFER_WMA* pBufferWMA
    )
        => ((delegate* unmanaged[Stdcall]<void*, XAUDIO2_BUFFER*, XAUDIO2_BUFFER_WMA*, HRESULT>)_vtbl[21])(Unsafe.AsPointer(ref this), pBuffer, pBufferWMA);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetFrequencyRatio(float Ratio, UINT32 OperationSet)
        => ((delegate* unmanaged[Stdcall]<void*, float, UINT32, HRESULT>)_vtbl[26])(Unsafe.AsPointer(ref this), Ratio, OperationSet);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetFrequencyRatio(float* pRatio)
        => ((delegate* unmanaged[Stdcall]<void*, float*, void>)_vtbl[27])(Unsafe.AsPointer(ref this), pRatio);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetSourceSampleRate(UINT32 NewSourceSampleRate)
        => ((delegate* unmanaged[Stdcall]<void*, UINT32, HRESULT>)_vtbl[28])(Unsafe.AsPointer(ref this), NewSourceSampleRate);

    // IXAudio2Voice
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetVolume(
            float Volume,
            UINT32 OperationSet = default //XAUDIO2_COMMIT_NOW
        )
        => ((delegate* unmanaged[Stdcall]<void*, float, UINT32, HRESULT>)_vtbl[12])(Unsafe.AsPointer(ref this), Volume, OperationSet);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetVolume(float* pVolume)
        => ((delegate* unmanaged[Stdcall]<void*, float*, void>)_vtbl[13])(Unsafe.AsPointer(ref this), pVolume);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DestroyVoice()
        => ((delegate* unmanaged[Stdcall]<void*, void>)_vtbl[18])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT Start(
        UINT32 Flags = default,
        UINT32 OperationSet = default //XAUDIO2_COMMIT_NOW
    )
        => ((delegate* unmanaged[Stdcall]<void*, UINT32, UINT32, HRESULT>)_vtbl[19])(Unsafe.AsPointer(ref this), Flags, OperationSet);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT Stop(
        UINT32 Flags = default,
        UINT32 OperationSet = default //XAUDIO2_COMMIT_NOW
    )
        => ((delegate* unmanaged[Stdcall]<void*, UINT32, UINT32, HRESULT>)_vtbl[20])(Unsafe.AsPointer(ref this), Flags, OperationSet);
}
