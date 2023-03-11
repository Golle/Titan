using Titan.Setup;

namespace Titan.Audio;

public struct PlaybackSettings : IDefault<PlaybackSettings>
{
    public float Volume;
    public float Frequency;
    public bool Loop;
    public static PlaybackSettings Default => new()
    {
        Frequency = 1.0f,
        Volume = 1.0f,
        Loop = false
    };
}
