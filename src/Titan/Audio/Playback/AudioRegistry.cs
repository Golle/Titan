using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Assets;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Memory.Resources;

namespace Titan.Audio.Playback;

internal class AudioRegistry
{
    private IResourcePool<Audio> _pool;

    public bool Init(IMemoryManager memoryManager, AudioConfig config)
    {
        if (!FixedSizeResourcePool<Audio>.Create(memoryManager, config.MaxAudioPlaybackResources, out _pool))
        {
            Logger.Error<AssetsRegistry>($"Failed to allocate a resource pool for {nameof(Audio)}");
            return false;
        }
        return true;
    }

    public Handle<Audio> Create(in Handle<Asset> asset, in PlaybackSettings settings)
    {
        var handle = _pool.SafeAlloc();
        if (handle.IsInvalid)
        {
            Logger.Error<AudioRegistry>($"Failed to create handle for {nameof(Audio)}.");
            return 0;
        }
        ref var audio = ref _pool.Get(handle);
        audio.AudioAsset = asset;
        audio.Settings = settings;
        audio.AudioSink = 0;
        return handle;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref Audio Access(in Handle<Audio> handle)
    {
        Debug.Assert(handle.IsValid);
        return ref _pool.Get(handle);
    }

    public void Destroy(in Handle<Audio> handle)
    {
        Debug.Assert(handle.IsValid);
        _pool.SafeFree(handle);
    }

    public void Shutdown()
    {
        _pool.Release();
    }
}
