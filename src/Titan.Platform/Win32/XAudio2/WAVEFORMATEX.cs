namespace Titan.Platform.Win32.XAudio2;

public struct WAVEFORMATEX
{
    public ushort wFormatTag;         /* format type */
    public ushort nChannels;          /* number of channels (i.e. mono, stereo...) */
    public uint nSamplesPerSec;     /* sample rate */
    public uint nAvgBytesPerSec;    /* for buffer estimation */
    public ushort nBlockAlign;        /* block size of data */
    public ushort wBitsPerSample;     /* number of bits per sample of mono data */
    public ushort cbSize;             /* the count in bytes of the size of */
    /* extra information (after cbSize) */
}
