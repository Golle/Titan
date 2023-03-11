using Titan.Core.Memory;
using Titan.Setup;
using Titan.Setup.Configs;

namespace Titan.Audio;

public record AudioConfig : IConfiguration, IDefault<AudioConfig>
{
    /// <summary>
    /// The number of Audios playbacks that can be queued (or created)
    /// </summary>
    public uint MaxAudioPlaybackResources { get; init; }
    /// <summary>
    /// The number of audio resources that can be loaded into memory
    /// </summary>
    public uint MaxAudioResources { get; init; }
    /// <summary>
    /// The max queued audio commands per frame. For example change volume, frequency, start/stop etc.
    /// </summary>
    public uint MaxQueuedAudioCommands { get; init; }
    /// <summary>
    /// The storage size of the audio loaded into memory. Default is 32 Megabytes.
    /// </summary>
    public uint MaxAudioMemory { get; init; }

    /// <summary>
    /// The number of channels that can be used with the Audio playback (max concurrent playing sounds)
    /// </summary>
    public uint Channels { get; init; }
    
    /// <summary>
    /// The default master volume, this can be changed by calling SetMasterVolume on the AudioManager
    /// </summary>
    public float MasterVolume { get; init; }

    public const uint DefaultMaxQueuedAudio = 128;
    public const uint DefaultMaxAudioPlaybackResources = 1024;
    public const uint DefaultMaxAudioResources = 64;
    public const uint DefaultChannels = 32;
    public const float DefaultMasterVolume = 0.1f;
    private static readonly uint DefaultMaxAudioMemory = MemoryUtils.MegaBytes(32);

    public static AudioConfig Default => new()
    {
        MaxQueuedAudioCommands = DefaultMaxQueuedAudio,
        MaxAudioPlaybackResources = DefaultMaxAudioPlaybackResources,
        MaxAudioResources = DefaultMaxAudioResources,
        MaxAudioMemory = DefaultMaxAudioMemory,
        Channels = DefaultChannels,
        MasterVolume = DefaultMasterVolume
    };
}
