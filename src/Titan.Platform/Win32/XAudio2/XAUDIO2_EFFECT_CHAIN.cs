namespace Titan.Platform.Win32.XAudio2;

public struct XAUDIO2_EFFECT_CHAIN
{
    public uint EffectCount;                 // Number of effects in this voice's effect chain.
    public unsafe XAUDIO2_EFFECT_DESCRIPTOR* pEffectDescriptors; // Array of effect descriptors.
}
