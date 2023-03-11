using Titan.Core;
using Titan.Core.Logging;
using Titan.Events;
using Titan.Systems;
using Titan.Windows.Events;

namespace Titan.Audio;

internal struct AudioDeviceSystem : ISystem
{
    private EventsReader<AudioDeviceRemovedEvent> _removed;
    private EventsReader<AudioDeviceArrivedEvent> _arrived;
    private ObjectHandle<IAudioSinkHandler> _audioSink;

    public void Init(in SystemInitializer init)
    {
        _removed = init.GetEventsReader<AudioDeviceRemovedEvent>();
        _arrived = init.GetEventsReader<AudioDeviceArrivedEvent>();
        _audioSink = init.GetManagedApi<IAudioSinkHandler>();
    }

    public void Update()
    {
        //NOTE(Jens): we don't care if a device was added or removed, we just trigger a recreation when it happens.
        Logger.Info<AudioDeviceSystem>("Audio devices changed, recreating master voice.");

        if (!_audioSink.Value.OnDeviceChanged())
        {
            Logger.Error<AudioDeviceSystem>("Audio device recreation failed. No sound will be played.");
        }
    }

    public bool ShouldRun() => _arrived.HasEvents() || _removed.HasEvents();
}
