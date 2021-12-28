namespace Titan.Windows.XAudio2;

public struct WAVEFORMATEX
{
    public WORD wFormatTag;         /* format type */
    public WORD nChannels;          /* number of channels (i.e. mono, stereo...) */
    public DWORD nSamplesPerSec;     /* sample rate */
    public DWORD nAvgBytesPerSec;    /* for buffer estimation */
    public WORD nBlockAlign;        /* block size of data */
    public WORD wBitsPerSample;     /* number of bits per sample of mono data */
    public WORD cbSize;             /* the count in bytes of the size of */
    /* extra information (after cbSize) */
}
