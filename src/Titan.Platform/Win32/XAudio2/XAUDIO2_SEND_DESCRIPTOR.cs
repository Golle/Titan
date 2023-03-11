namespace Titan.Platform.Win32.XAudio2;

public unsafe struct XAUDIO2_SEND_DESCRIPTOR
{
    public uint Flags;                       // Either 0 or XAUDIO2_SEND_USEFILTER.
    public IXAudio2Voice* pOutputVoice;        // This send's destination voice.
}
