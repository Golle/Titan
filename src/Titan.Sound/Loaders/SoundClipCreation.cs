using System;
using Titan.Platform.Win32.XAudio2;

namespace Titan.Sound.Loaders;

public ref struct SoundClipCreation
{
    public WAVEFORMATEX Format;
    public ReadOnlySpan<byte> Data;

}
