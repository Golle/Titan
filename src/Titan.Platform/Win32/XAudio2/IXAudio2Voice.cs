using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32.XAudio2;

public unsafe struct IXAudio2Voice
{
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetVolume(float Volume, uint OperationSet = XAudio2Constants.XAUDIO2_COMMIT_NOW)
        => ((delegate* unmanaged[Stdcall]<void*, float, uint, HRESULT>)_vtbl[12])(Unsafe.AsPointer(ref this), Volume, OperationSet);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetVolume(float* pVolume)
        => ((delegate* unmanaged[Stdcall]<void*, float*, void>)_vtbl[13])(Unsafe.AsPointer(ref this), pVolume);
}
