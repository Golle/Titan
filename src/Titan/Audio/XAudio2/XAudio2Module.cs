using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Modules;
using Titan.Setup;

namespace Titan.Audio.XAudio2;

internal unsafe struct XAudio2Module : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        var sinkHandler = new XAudio2AudioSinkHandler();
        builder

            .AddManagedResource(new XAudio2Device())
            // Add the sink handler as the Class and interface.
            .AddManagedResource(sinkHandler)
            .AddManagedResource<IAudioSinkHandler>(sinkHandler)
            ;

        return true;
    }

    public static bool Init(IApp app)
    {
        var device = app.GetManagedResource<XAudio2Device>();
        var xaudio = app.GetManagedResource<XAudio2AudioSinkHandler>();
        
        var config = app.GetConfigOrDefault<AudioConfig>();
        var memoryManager = app.GetManagedResource<IMemoryManager>();

        if (!device.Init())
        {
            Logger.Error<XAudio2Module>($"Failed to init the {nameof(XAudio2Device)}");
            return false;
        }

        if (!xaudio.Init(memoryManager, device, config))
        {
            Logger.Error<XAudio2Module>($"Failed to init the {nameof(XAudio2AudioSinkHandler)}");
            return false;
        }

        return true;
    }

    public static bool Shutdown(IApp app)
    {
        app.GetManagedResource<XAudio2AudioSinkHandler>()
            .Shutdown();

        app.GetManagedResource<XAudio2Device>()
            .Shutdown();

        return true;
    }
}
