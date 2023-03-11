using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Memory.Allocators;
using Titan.Core.Memory.Resources;

namespace Titan.Audio.Data;
internal struct AudioData
{
    public TitanBuffer Data;
}

internal record struct CreateAudioDataArgs(TitanBuffer Data, ushort Channels);

internal class AudioDataManager
{
    private IResourcePool<AudioData> _audio;
    private IGeneralAllocator _allocator;

    public bool Init(IMemoryManager memoryManager, AudioConfig config)
    {
        if (!FixedSizeResourcePool<AudioData>.Create(memoryManager, config.MaxAudioResources, out _audio))
        {
            Logger.Error<AudioDataManager>($"Failed to allocate a pool allocator for {config.MaxAudioResources} audio.");
            return false;
        }

        if (!memoryManager.TryCreateGeneralAllocator(config.MaxAudioMemory, out _allocator))
        {
            Logger.Error<AudioDataManager>($"Failed to create a general allocator of size {config.MaxAudioMemory} bytes for Audio Data");
            return false;
        }
        
        return true;
    }

    public Handle<AudioData> Create(in CreateAudioDataArgs args)
    {
        var audioData = args.Data;
        if (!audioData.HasData())
        {
            Logger.Error<AudioDataManager>($"Trying to create a {nameof(AudioData)} with an empty buffer.");
            return Handle<AudioData>.Invalid;
        }

        var handle = _audio.SafeAlloc();
        if (handle.IsInvalid)
        {
            Logger.Error<AudioDataManager>($"Failed to allocate a handle for {nameof(AudioData)}");
            return Handle<AudioData>.Invalid;
        }

        var mono = args.Channels == 1;


        var size = mono ? audioData.Length * 2u : audioData.Length;
        var buffer = _allocator.AllocateBuffer((uint)size);
        if (!buffer.HasData())
        {
            Logger.Error<AudioDataManager>($"Failed to allocate {size} bytes of memory fr the {nameof(AudioData)}");
            _audio.Free(handle);
            return Handle<AudioData>.Invalid;
        }
        unsafe
        {
            if (mono)
            {
                Interleave((ushort*)audioData.AsPointer(), (ushort*)audioData.AsPointer(), (ushort*)buffer.AsPointer(), (uint)(size / 4u));
            }
            else
            {
                MemoryUtils.Copy(buffer.AsPointer(), audioData.AsReadOnlySpan());
            }
        }

        ref var audio = ref _audio.Get(handle);
        audio.Data = buffer;

        return handle;
    }

    private static unsafe void Interleave(ushort* left, ushort* right, ushort* outBuffer, uint numberOfSamples)
    {
        for (var i = 0; i < numberOfSamples; ++i)
        {
            outBuffer[i * 2] = left[i];
            outBuffer[i * 2 + 1] = right[i];
        }
    }
    public void Destroy(in Handle<AudioData> handle)
    {
        if (handle.IsValid)
        {
            ref var audio = ref _audio.Get(handle);
            _allocator.Free(ref audio.Data);
            _audio.SafeFree(handle);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly AudioData Access(in Handle<AudioData> handle)
        => ref _audio.Get(handle);

    public void Shutdown()
    {
        _audio?.Release();
        _allocator?.Release();
    }
}
