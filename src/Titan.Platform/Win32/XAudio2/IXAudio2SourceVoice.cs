using System.Runtime.CompilerServices;
using static Titan.Platform.Win32.XAudio2.XAudio2Constants;

namespace Titan.Platform.Win32.XAudio2;

public unsafe struct IXAudio2SourceVoice
{
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SubmitSourceBuffer(XAUDIO2_BUFFER* pBuffer, XAUDIO2_BUFFER_WMA* pBufferWMA)
        => ((delegate* unmanaged[Stdcall]<void*, XAUDIO2_BUFFER*, XAUDIO2_BUFFER_WMA*, HRESULT>)_vtbl[21])(Unsafe.AsPointer(ref this), pBuffer, pBufferWMA);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT FlushSourceBuffer()
        => ((delegate* unmanaged[Stdcall]<void*, HRESULT>)_vtbl[22])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetFrequencyRatio(float Ratio, uint OperationSet = XAUDIO2_COMMIT_NOW)
        => ((delegate* unmanaged[Stdcall]<void*, float, uint, HRESULT>)_vtbl[26])(Unsafe.AsPointer(ref this), Ratio, OperationSet);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetFrequencyRatio(float* pRatio)
        => ((delegate* unmanaged[Stdcall]<void*, float*, void>)_vtbl[27])(Unsafe.AsPointer(ref this), pRatio);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetSourceSampleRate(uint NewSourceSampleRate)
        => ((delegate* unmanaged[Stdcall]<void*, uint, HRESULT>)_vtbl[28])(Unsafe.AsPointer(ref this), NewSourceSampleRate);



    // IXAudio2Voice
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetVolume(float Volume, uint OperationSet = XAUDIO2_COMMIT_NOW)
        => ((delegate* unmanaged[Stdcall]<void*, float, uint, HRESULT>)_vtbl[12])(Unsafe.AsPointer(ref this), Volume, OperationSet);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetVolume(float* pVolume)
        => ((delegate* unmanaged[Stdcall]<void*, float*, void>)_vtbl[13])(Unsafe.AsPointer(ref this), pVolume);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DestroyVoice()
        => ((delegate* unmanaged[Stdcall]<void*, void>)_vtbl[18])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT Start(uint Flags = 0, uint OperationSet = XAUDIO2_COMMIT_NOW)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, HRESULT>)_vtbl[19])(Unsafe.AsPointer(ref this), Flags, OperationSet);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT Stop(uint Flags = 0, uint OperationSet = XAUDIO2_COMMIT_NOW)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, HRESULT>)_vtbl[20])(Unsafe.AsPointer(ref this), Flags, OperationSet);
}
