using Titan.Core;
using Titan.Core.Logging;
using Titan.Sound.Loaders;
using Titan.Sound.XAudio2;

namespace Titan.Sound;

public ref struct PlaySound
{
    public Handle<SoundClip> Clip;
}

public class SoundSystem : IDisposable
{
    private readonly SoundManager _manager;
    private readonly XAudio2Device _device;

    public SoundSystem(SoundManager manager)
    {
        _manager = manager;
        var config = new AudioDeviceConfiguration();
        Logger.Trace<SoundSystem>($"Creating Sound System using {nameof(XAudio2Device)} with {config.NumberOfSounds} channels for SFX.");
        _device = new XAudio2Device(config);
    }

    public PlaySoundHandle Play(PlaySound args)
    {
        ref readonly var sound = ref _manager.Access(args.Clip);
        if (!_device.TryPlaySound(sound.Data, out var handle))
        {
            Logger.Error<SoundSystem>("Failed to play the sound");
            return PlaySoundHandle.Invalid;
        }
        return handle;
    }

    public void Stop(PlaySoundHandle handle)
    {
        _device.Stop(handle);
    }

    public void Dispose()
    {
        _device?.Dispose();
    }
}
