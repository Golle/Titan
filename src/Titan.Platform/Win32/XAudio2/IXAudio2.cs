using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.D3D12;

namespace Titan.Platform.Win32.XAudio2;

[Guid("2B02E3CF-2E0B-4ec3-BE45-1B2A3FE7210D")]
public unsafe struct IXAudio2 : INativeGuid
{
    public static Guid* Guid => IID.IID_IXAudio2;
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateSourceVoice(
        IXAudio2SourceVoice** ppSourceVoice,
        WAVEFORMATEX* pSourceFormat,
        uint Flags = 0,
        float MaxFrequencyRatio = XAudio2Constants.XAUDIO2_DEFAULT_FREQ_RATIO,
        IXAudio2VoiceCallback* pCallback = null,
        XAUDIO2_VOICE_SENDS* pSendList = null,
        XAUDIO2_EFFECT_CHAIN* pEffectChain = null
    )
        => ((delegate* unmanaged[Stdcall]<void*, IXAudio2SourceVoice**, WAVEFORMATEX*, uint, float, IXAudio2VoiceCallback*, XAUDIO2_VOICE_SENDS*, XAUDIO2_EFFECT_CHAIN*, HRESULT>)_vtbl[5])(Unsafe.AsPointer(ref this), ppSourceVoice, pSourceFormat, Flags, MaxFrequencyRatio, pCallback, pSendList, pEffectChain);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateMasteringVoice(
        IXAudio2MasteringVoice** ppMasteringVoice,
        uint inputChannels = 0,
        uint inputSampleRate = 0,
        uint flags = 0,
        char* szDeviceId = null,
        XAUDIO2_EFFECT_CHAIN* pEffectChain = null,
        AUDIO_STREAM_CATEGORY streamCategory = AUDIO_STREAM_CATEGORY.AudioCategory_GameEffects
    )
        => ((delegate* unmanaged[Stdcall]<void*, IXAudio2MasteringVoice**, uint, uint, uint, char*, XAUDIO2_EFFECT_CHAIN*, AUDIO_STREAM_CATEGORY, HRESULT>)_vtbl[7])(Unsafe.AsPointer(ref this), ppMasteringVoice, inputChannels, inputSampleRate, flags, szDeviceId, pEffectChain, streamCategory);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetDebugConfiguration(
        IXAudio2MasteringVoice** ppMasteringVoice,
        XAUDIO2_DEBUG_CONFIGURATION* pDebugConfiguration,
        void* pReserved = null
    )
        => ((delegate* unmanaged[Stdcall]<void*, IXAudio2MasteringVoice**, XAUDIO2_DEBUG_CONFIGURATION*, void*, void>)_vtbl[12])(Unsafe.AsPointer(ref this), ppMasteringVoice, pDebugConfiguration, pReserved);

}
