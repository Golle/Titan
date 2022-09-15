using Titan.Core.Memory;
using Titan.Platform.Win32.XAudio2;

namespace Titan.Sound.Loaders;

public struct SoundClip
{
    public WAVEFORMATEX Format; //TODO: Replace this with something else?
    public MemoryChunk<byte> Data;
}
