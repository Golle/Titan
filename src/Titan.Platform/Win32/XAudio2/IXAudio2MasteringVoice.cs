using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32.XAudio2;

//NOTE(Jens): a lot fo methods missing :O
public unsafe struct IXAudio2MasteringVoice
{
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetChannelMask(uint* pChannelmask)
        => ((delegate* unmanaged[Stdcall]<void*, uint*, HRESULT>)_vtbl[19])(Unsafe.AsPointer(ref this), pChannelmask);

    // IXAudio2Voice
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetVolume(float Volume, uint OperationSet = XAudio2Constants.XAUDIO2_COMMIT_NOW) 
        => ((delegate* unmanaged[Stdcall]<void*, float, uint, HRESULT>)_vtbl[12])(Unsafe.AsPointer(ref this), Volume, OperationSet);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetVolume(float* pVolume)
        => ((delegate* unmanaged[Stdcall]<void*, float*, void>)_vtbl[13])(Unsafe.AsPointer(ref this), pVolume);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DestroyVoice()
        => ((delegate* unmanaged[Stdcall]<void*, void>)_vtbl[18])(Unsafe.AsPointer(ref this));
}
