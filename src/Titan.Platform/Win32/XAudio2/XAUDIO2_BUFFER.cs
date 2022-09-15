namespace Titan.Platform.Win32.XAudio2;

// REF https://docs.microsoft.com/en-us/windows/win32/api/xaudio2/ns-xaudio2-xaudio2_buffer
public unsafe struct XAUDIO2_BUFFER
{
    public uint Flags;                       // Either 0 or XAUDIO2_END_OF_STREAM.
    public uint AudioBytes;                  // Size of the audio data buffer in bytes.
    public byte* pAudioData;             // Pointer to the audio data buffer.
    public uint PlayBegin;                   // First sample in this buffer to be played.
    public uint PlayLength;                  // Length of the region to be played in samples,
    //  or 0 to play the whole buffer.
    public uint LoopBegin;                   // First sample of the region to be looped.
    public uint LoopLength;                  // Length of the desired loop region in samples,
    //  or 0 to loop the entire buffer.
    public uint LoopCount;                   // Number of times to repeat the loop region,
    //  or XAUDIO2_LOOP_INFINITE to loop forever.
    public void* pContext;                     // Context value to be passed back in callbacks.
}
