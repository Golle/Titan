namespace Titan.Platform.Win32.XAudio2;

public struct XAUDIO2_EFFECT_DESCRIPTOR
{
    public unsafe IUnknown* pEffect; // Pointer to the effect object's IUnknown interface.
    public int InitialState; // TRUE if the effect should begin in the enabled state.
    public uint OutputChannels; // How many output channels the effect should produce.
}
