using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32.XAudio2;

public unsafe struct IXAudio2
{
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
}
