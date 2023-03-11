namespace Titan.Platform.Win32.XAudio2;

public unsafe struct XAUDIO2_BUFFER_WMA
{
    public uint* pDecodedPacketCumulativeBytes; // Decoded packet's cumulative size array.
    //  Each element is the number of bytes accumulated
    //  when the corresponding XWMA packet is decoded in
    //  order.  The array must have PacketCount elements.
    public uint PacketCount;                          // Number of XWMA packets submitted. Must be >= 1 and
    //  divide evenly into XAUDIO2_BUFFER.AudioBytes.
}
