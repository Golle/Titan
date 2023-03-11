namespace Titan.Platform.Win32.XAudio2;

public unsafe struct XAUDIO2_EFFECT_CHAIN
{
    public uint EffectCount;                 // Number of effects in this voice's effect chain.
    public XAUDIO2_EFFECT_DESCRIPTOR* pEffectDescriptors; // Array of effect descriptors.
}
