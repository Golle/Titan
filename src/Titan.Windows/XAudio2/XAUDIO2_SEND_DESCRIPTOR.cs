namespace Titan.Windows.XAudio2;

public struct XAUDIO2_SEND_DESCRIPTOR
{
    public uint Flags;                       // Either 0 or XAUDIO2_SEND_USEFILTER.
    public unsafe IXAudio2Voice* pOutputVoice;        // This send's destination voice.
}
