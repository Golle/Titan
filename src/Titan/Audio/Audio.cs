using Titan.Assets;
using Titan.Core;

namespace Titan.Audio;

public struct Audio
{
    internal Handle<Asset> AudioAsset;
    internal PlaybackSettings Settings;
    internal Handle<AudioSink> AudioSink;
}
