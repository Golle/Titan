using Titan.Audio.Data;
using Titan.Audio.Playback;
using Titan.Audio.XAudio2;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Modules;
using Titan.Setup;
using Titan.Systems;

namespace Titan.Audio;

public struct AudioModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddModule<XAudio2Module>() // we can change this to something else when needed. for example in Linux/mac (if we ever support that)

            .AddManagedResource(new AudioRegistry())
            .AddManagedResource(new AudioCommandQueue())
            .AddManagedResource(new AudioDataManager())
            .AddResourceCreator<AudioData, AudioDataCreator>()
            .AddSystemToStage<AudioPlaybackSystem>(SystemStage.PostUpdate, RunCriteria.Always)
            .AddSystemToStage<AudioDeviceSystem>(SystemStage.PreUpdate);

        return true;
    }

    public static bool Init(IApp app)
    {
        var queue = app.GetManagedResource<AudioCommandQueue>();
        var registry = app.GetManagedResource<AudioRegistry>();
        var manager = app.GetManagedResource<AudioDataManager>();
        var memoryManager = app.GetManagedResource<IMemoryManager>();
        var config = app.GetConfigOrDefault<AudioConfig>();

        if (!manager.Init(memoryManager, config))
        {
            Logger.Error<AudioModule>($"Failed to init the {nameof(AudioDataManager)}");
            return false;
        }

        if (!registry.Init(memoryManager, config))
        {
            Logger.Error<AudioModule>($"Failed to init the {nameof(AudioRegistry)}");
            return false;
        }

        if (!queue.Init(memoryManager, registry, config))
        {
            Logger.Error<AudioModule>($"Failed to init the {nameof(AudioCommandQueue)}");
            return false;
        }

        return true;
    }

    public static bool Shutdown(IApp app)
    {
        app.GetManagedResource<AudioCommandQueue>()
            .Shutdown();
        app.GetManagedResource<AudioRegistry>()
            .Shutdown();
        app.GetManagedResource<AudioDataManager>()
            .Shutdown();

        return true;
    }
}
