using System.Diagnostics;
using Titan.Assets;
using Titan.Assets.Creators;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Audio.Data;

internal struct AudioDataCreator : IResourceCreator<AudioData>
{
    private ObjectHandle<AudioDataManager> _manager;
    public static AssetDescriptorType Type => AssetDescriptorType.Audio;
    public bool Init(in ResourceCreatorInitializer initializer)
    {
        _manager = initializer.GetManagedResource<AudioDataManager>();
        return true;
    }

    public Handle<AudioData> Create(in AssetDescriptor descriptor, TitanBuffer data)
    {
        Debug.Assert(descriptor.Type == AssetDescriptorType.Audio);
        Debug.Assert(descriptor.Audio.BitsPerSample == 16, $"Currently only support {nameof(AudioAssetDescriptor.BitsPerSample)} = 16 for Audio");
        Debug.Assert(descriptor.Audio.SamplesPerSecond == 44100, $"Currently only support {nameof(AudioAssetDescriptor.SamplesPerSecond)} = 44100 for Audio");

        var handle = _manager.Value.Create(new CreateAudioDataArgs(data, descriptor.Audio.Channels));
        if (handle.IsInvalid)
        {
            Logger.Error<AudioDataCreator>("Failed to create the AudioData");
        }
        return handle;

    }

    public bool Recreate(in Handle<AudioData> handle, in AssetDescriptor descriptor, TitanBuffer data)
    {
        throw new NotImplementedException();
    }

    public void Destroy(Handle<AudioData> handle)
    {
        Logger.Trace<AudioDataCreator>($"Destroying {nameof(AudioData)} with handle={handle.Value}");
        _manager.Value.Destroy(handle);
    }

    public void Release()
    {
        Logger.Warning<AudioDataCreator>("Release has not been implemented, need to figure out the lifetime of objects.");
    }
}
