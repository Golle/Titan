using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Platform.Win32.XAudio2;

namespace Titan.Audio.XAudio2;

internal unsafe class XAudio2AudioSinkHandler : IAudioSinkHandler
{
    private const float MaxFrequencyRatio = 4f;
    //NOTE(Jens): This should be configured, and maybe we want different types?
    //NOTE(Jens): the audio loader/manifest tool must support it as well. but for now we stick to a single format.
    private static readonly AudioFormatArgs DefaultAudioFormat = new(2, 16, 44100, MaxFrequencyRatio);

    private static readonly int HandleOffset = Random.Shared.Next(ushort.MinValue, ushort.MaxValue);

    private TitanArray<XAudio2AudioSink> _sinks;
    private IXAudio2MasteringVoice* _masteringVoice;

    private XAudio2Device _device;
    private IMemoryManager _memoryManager;

    private float _masterVolume;

    public bool Init(IMemoryManager memoryManager, XAudio2Device device, AudioConfig config)
    {
        if (!memoryManager.TryAllocArray(out _sinks, config.Channels, true))
        {
            Logger.Error<XAudio2AudioSinkHandler>($"Failed to allocate memory for {config.Channels} {nameof(XAudio2AudioSink)}.");
            return false;
        }

        _device = device;
        _memoryManager = memoryManager;
        _masterVolume = config.MasterVolume;

        // initial device changed event
        OnDeviceChanged();
        return true;
    }

    public bool OnDeviceChanged()
    {
        // Cleanup the MasterVoice and SourceVoices
        DestroyAllVoices();

        // Create the MasteringVoice
        Logger.Trace<XAudio2AudioSinkHandler>("Creating master voice");
        _masteringVoice = _device.CreateMasteringVoice();
        if (_masteringVoice == null)
        {
            Logger.Warning<XAudio2AudioSinkHandler>("Failed to setup the voices. This is probably because no default audio device could be found.");
            return false;
        }
        _masteringVoice->SetVolume(_masterVolume);

        // Create the SourceVoices
        Logger.Trace<XAudio2AudioSinkHandler>($"Creating {_sinks.Length} {nameof(IXAudio2SourceVoice)}");


        for (var i = 0; i < _sinks.Length; ++i)
        {
            var voice = _sinks.GetPointer(i);
            var callback = IXAudio2VoiceCallback.Create(voice);
            var sourceVoice = _device.CreateSourceVoice(DefaultAudioFormat, &voice->Callback);
            if (sourceVoice == null)
            {
                Logger.Error($"Failed to create the SourceVoice. Index = {i}");
                goto Error;
            }
            voice->SourceVoice = sourceVoice;
            voice->Callback = callback;
            voice->State = AudioPlaybackState.Available;
            voice->Version = 0;
        }

        return true;
Error:
        DestroyAllVoices();
        return false;
    }

    public void SetMasterVolume(float volume)
    {
        _masterVolume = volume;
        if (_masteringVoice != null)
        {
            _masteringVoice->SetVolume(volume);
        }
    }

    public float GetMasterVolume()
    {
        var masterVolume = _masterVolume;
        if (_masteringVoice != null)
        {
            _masteringVoice->GetVolume(&masterVolume);
        }
        return masterVolume;
    }

    public void SetVolume(in Handle<AudioSink> handle, float volume)
    {
        var sink = GetSinkFromHandle(handle);
        if (sink != null)
        {
            sink->SetVolume(volume);
        }
    }

    public void Stop(in Handle<AudioSink> handle)
    {
        var sink = GetSinkFromHandle(handle);
        if (sink != null)
        {
            sink->Stop();
        }
    }
    public void Pause(in Handle<AudioSink> handle)
    {
        var sink = GetSinkFromHandle(handle);
        if (sink != null)
        {
            sink->Pause();
        }
    }

    public void Resume(in Handle<AudioSink> handle)
    {
        var sink = GetSinkFromHandle(handle);
        if (sink != null)
        {
            sink->Resume();
        }
    }


    public void SetFrequency(in Handle<AudioSink> handle, float frequency)
    {
#if DEBUG
        if (frequency > DefaultAudioFormat.MaxFrequencyRatio)
        {
            Logger.Warning<XAudio2AudioSinkHandler>($"The request frequency {frequency} is higher than the max frequency {DefaultAudioFormat.MaxFrequencyRatio}");
        }
#endif
        var sink = GetSinkFromHandle(handle);
        if (sink != null)
        {
            sink->SetFrequency(frequency);
        }
    }

    public void Update()
    {
        foreach (ref var sink in _sinks.AsSpan())
        {
            if (sink.State == AudioPlaybackState.Completed)
            {
                //Logger.Trace<XAudio2AudioSinkHandler>($"Changing state: {sink.State} => {AudioPlaybackState.Available}");
                sink.State = AudioPlaybackState.Available;
                unchecked
                {
                    sink.Version++;
                }
            }
            else if (sink.State == AudioPlaybackState.Error)
            {
                Logger.Error<XAudio2AudioSinkHandler>($"XAudio2 in error state. We might need to recreate it?. HRESULT = {sink.Error}");
                sink.State = AudioPlaybackState.Available;
                sink.Error = 0;
            }
        }
    }

    public void Shutdown()
    {
        DestroyAllVoices();
        _memoryManager?.Free(ref _sinks);
    }

    private void DestroyAllVoices()
    {
        Logger.Trace<XAudio2AudioSinkHandler>($"Destroying source voices. Count = {_sinks.Length}");
        foreach (ref var voice in _sinks.AsSpan())
        {
            voice.Destroy();
            voice = default;
        }
        Logger.Trace<XAudio2AudioSinkHandler>("Destroying master voice");
        if (_masteringVoice != null)
        {
            _masteringVoice->DestroyVoice();
            _masteringVoice = null;
        }
    }

    public bool TryGetAudioSink(out Handle<AudioSink> handle)
    {
        Unsafe.SkipInit(out handle);
        for (var i = 0; i < _sinks.Length; ++i)
        {
            var sink = _sinks.GetPointer(i);
            if (sink->State == AudioPlaybackState.Available)
            {
                sink->State = AudioPlaybackState.Acquired;
                //NOTE(Jens): We pack the version of the Sink into the handle
                handle = (i + HandleOffset) | (sink->Version << 16);
                return true;
            }
        }
        Logger.Warning<XAudio2AudioSinkHandler>("Failed to find an available audio sink.");
        return false;
    }

    public void Play(in Handle<AudioSink> handle, in PlayAudioArgs args)
    {
        var sink = GetSinkFromHandle(handle);
        if (sink == null)
        {
            //NOTE(Jens): this will happen if play is called on an old sink handle.
            return;
        }
        if (sink->SourceVoice == null)
        {
            Logger.Trace<XAudio2AudioSinkHandler>("SourceVoice is null, this is most likely because no audio device was found.");
            return;
        }
        if (sink->State == AudioPlaybackState.Playing)
        {
            sink->Stop();
            sink->State = AudioPlaybackState.Acquired;
        }
        Debug.Assert(sink->State is AudioPlaybackState.Acquired or AudioPlaybackState.Completed);
        sink->SetFrequency(args.Frequency);
        sink->SetVolume(args.Volume);
        sink->Play(args.AudioBuffer, args.Loop);

    }

    private XAudio2AudioSink* GetSinkFromHandle(in Handle<AudioSink> handle)
    {
        if (handle.IsInvalid)
        {
            return null;
        }

        var index = (int)((handle.Value & 0xffff) - HandleOffset);
        Debug.Assert(index >= 0 && index < _sinks.Length, "The audio sink handle is out of bounds.");

        var version = (ushort)(handle.Value >> 16);
        var sink = _sinks.GetPointer(index);
        //NOTE(Jens): Only return the sink if the Version matches
        return sink->Version == version ? sink : null;
    }
}
