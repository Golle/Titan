using System.Diagnostics;
using Titan.Assets;
using Titan.Audio.Playback;
using Titan.Core;
using Titan.Core.Logging;

namespace Titan.Audio;

public readonly struct AudioManager
{
    private readonly ObjectHandle<AudioCommandQueue> _queue;
    internal AudioManager(ObjectHandle<AudioCommandQueue> queue)
    {
        _queue = queue;
    }

    public void Play(Handle<Audio> audio)
        => _queue.Value.Enqueue(new AudioCommand { Audio = audio, Command = AudioCommands.Play });

    public void PlayOnce(in Handle<Asset> audioAsset)
        => PlayOnce(audioAsset, PlaybackSettings.Default);

    public void PlayOnce(in Handle<Asset> audioAsset, in PlaybackSettings settings)
        => _queue.Value.Enqueue(new AudioCommand
        {
            Asset = audioAsset,
            Command = AudioCommands.PlayOnce,
            Frequency = settings.Frequency,
            Volume = settings.Volume
        });

    public Handle<Audio> CreateAndPlay(in Handle<Asset> audioAsset)
        => CreateAndPlay(audioAsset, PlaybackSettings.Default);

    public Handle<Audio> CreateAndPlay(in Handle<Asset> audioAsset, in PlaybackSettings settings)
        => _queue.Value.CreateAndEnqueue(audioAsset, settings);

    public void SetVolume(in Handle<Audio> handle, float volume)
    {
        Debug.Assert(volume is >= 0.0f and <= 1.0f, "Volume must be between 0.0 and 1.0");
        _queue.Value.Enqueue(new AudioCommand { Command = AudioCommands.SetVolume, Audio = handle, Volume = volume });
    }

    public void SetFrequency(in Handle<Audio> handle, float frequency)
    {
        Debug.Assert(frequency > 0.0f, "Frequency must be greater than 0.0");
        _queue.Value.Enqueue(new AudioCommand { Command = AudioCommands.SetFrequency, Audio = handle, Frequency = frequency });
    }

    public void SetMasterVolume(float volume)
    {
        Debug.Assert(volume is >= 0.0f and <= 1.0f, "Volume must be between 0.0 and 1.0");
        _queue.Value.Enqueue(new AudioCommand { Command = AudioCommands.SetMasterVolume, Volume = volume });
    }

    public void Destroy(ref Handle<Audio> handle)
    {
        _queue.Value.Enqueue(new AudioCommand { Command = AudioCommands.Destroy, Audio = handle });
        handle = default;
    }

    public void Stop(in Handle<Audio> handle)
        => _queue.Value.Enqueue(new AudioCommand { Command = AudioCommands.Stop, Audio = handle });
    public void Pause(in Handle<Audio> handle)
        => _queue.Value.Enqueue(new AudioCommand { Command = AudioCommands.Pause, Audio = handle });
    public void Resume(in Handle<Audio> handle)
        => _queue.Value.Enqueue(new AudioCommand { Command = AudioCommands.Resume, Audio = handle });
}

