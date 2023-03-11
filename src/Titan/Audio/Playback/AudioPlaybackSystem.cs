using System.Diagnostics;
using Titan.Assets;
using Titan.Audio.Data;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Systems;

namespace Titan.Audio.Playback;

internal struct AudioPlaybackSystem : ISystem
{
    private AssetsManager _assetsManager;
    private ObjectHandle<AudioCommandQueue> _queue;
    private ObjectHandle<IAudioSinkHandler> _sink;
    private ObjectHandle<AudioDataManager> _audioDataManager;
    private ObjectHandle<AudioRegistry> _registry;

    public void Init(in SystemInitializer init)
    {
        _queue = init.GetManagedApi<AudioCommandQueue>();
        _sink = init.GetManagedApi<IAudioSinkHandler>();
        _assetsManager = init.GetAssetsManager();
        _audioDataManager = init.GetManagedApi<AudioDataManager>();
        _registry = init.GetManagedApi<AudioRegistry>();
    }

    public void Update()
    {
        var sink = _sink.Value;
        var queue = _queue.Value;
        var registry = _registry.Value;

        // update all the audio players
        sink.Update();

        // if there are no queued commands, just return.
        if (queue.Count == 0)
        {
            return;
        }

        foreach (ref readonly var command in queue.PopCommandsAndSwap())
        {
            switch (command.Command)
            {
                case AudioCommands.PlayOnce:
                    if (_assetsManager.IsLoaded(command.Asset))
                    {
                        Play(command.Asset, new PlaybackSettings { Frequency = command.Frequency, Volume = command.Volume });
                    }
                    else
                    {
                        // Asset is not loaded yet, requeue the command.
                        queue.Enqueue(command);
                    }
                    break;

                case AudioCommands.Play:
                    {
                        Debug.Assert(command.Audio.IsValid);
                        ref var audio = ref registry.Access(command.Audio);
                        if (_assetsManager.IsLoaded(audio.AudioAsset))
                        {
                            sink.Stop(audio.AudioSink);
                            audio.AudioSink = Play(audio.AudioAsset, audio.Settings);
                        }
                        else
                        {
                            // Asset is not loaded yet, requeue the command.
                            queue.Enqueue(command);
                        }
                        break;
                    }

                case AudioCommands.Destroy:
                    {
                        Debug.Assert(command.Audio.IsValid);
                        ref readonly var audio = ref registry.Access(command.Audio);
                        sink.Stop(audio.AudioSink);
                        registry.Destroy(command.Audio);
                        break;
                    }

                case AudioCommands.SetMasterVolume:
                    sink.SetMasterVolume(command.Volume);
                    break;

                case AudioCommands.SetVolume:
                    {
                        Debug.Assert(command.Audio.IsValid);
                        ref var audio = ref registry.Access(command.Audio);
                        audio.Settings.Volume = command.Volume;
                        if (audio.AudioSink.IsValid)
                        {
                            sink.SetVolume(audio.AudioSink, command.Volume);
                        }
                    }

                    break;
                case AudioCommands.Stop:
                    {
                        Debug.Assert(command.Audio.IsValid);
                        ref var audio = ref registry.Access(command.Audio);
                        sink.Stop(audio.AudioSink);
                        audio.AudioSink = default;
                    }
                    break;
                case AudioCommands.Pause:
                    {
                        Debug.Assert(command.Audio.IsValid);
                        ref var audio = ref registry.Access(command.Audio);
                        sink.Pause(audio.AudioSink);
                    }
                    break;
                case AudioCommands.Resume:
                    {
                        Debug.Assert(command.Audio.IsValid);
                        ref var audio = ref registry.Access(command.Audio);
                        sink.Resume(audio.AudioSink);
                    }
                    break;
                default:
                    Logger.Error<AudioPlaybackSystem>($"Command  {command.Command} has not been implemented.");
                    break;
            }
        }
    }

    public Handle<AudioSink> Play(in Handle<Asset> assetHandle, in PlaybackSettings settings)
    {
        var sink = _sink.Value;
        var data = GetDataFromHandle(assetHandle);
        if (sink.TryGetAudioSink(out var handle))
        {
            sink.Play(handle, new PlayAudioArgs(data, settings.Volume, settings.Frequency, settings.Loop));
            return handle;
        }
        Logger.Error<AudioPlaybackSystem>("Failed to find an available AudioSink. Sound will be discarded.");
        return 0;
    }
    public TitanBuffer GetDataFromHandle(in Handle<Asset> handle)
    {
        var dataHandle = _assetsManager.GetAssetHandle<AudioData>(handle);
        return _audioDataManager.Value.Access(dataHandle).Data;
    }
}

