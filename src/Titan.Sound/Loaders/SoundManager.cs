using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Memory;

namespace Titan.Sound.Loaders;

public unsafe class SoundManager : IDisposable
{
    private ResourcePool<SoundClip> _resources;
    public SoundManager(uint numberOfSoundClips = 100)
    {
        _resources.Init(numberOfSoundClips);
    }

    public Handle<SoundClip> Create(in SoundClipCreation args)
    {
        var handle = _resources.CreateResource();
        if (!handle.IsValid())
        { 
            throw new InvalidOperationException("Failed to Create SoundClip Handle");
        }

        var soundclip = _resources.GetResourcePointer(handle);
        soundclip->Format = args.Format;

        var size = (uint)args.Data.Length;

        
        //TODO: MONO, convert to stereo (add support for more formats)
        //NOTE(jens): might not be the best solution, but we want to keep the sound playback simple for now, so convert all sounds at runtime.
        //NOTE(jens): 16 samples, 44.1kz, 2 channels.
        if (args.Format.nChannels == 1 && args.Format.wBitsPerSample == 8)
        {
            
            // change from uint8 to int16 (byte to short)
            size *= 4;
            soundclip->Data = MemoryUtilsOld.AllocateBlock<byte>(size, true);
            var mem = (short*)soundclip->Data;

            for (var i = 0; i < args.Data.Length; ++i)
            {
                var value = args.Data[i] * 256 - short.MaxValue;
                mem[i * 2 + 1] = mem[i * 2] = (short)value;
            }
        }
        else
        {
            var dataLength = (uint)args.Data.Length;
            soundclip->Data = MemoryUtilsOld.AllocateBlock<byte>(dataLength);
            fixed (byte* pBytes = args.Data)
            {
                MemoryUtils.Copy(soundclip->Data.AsPointer(), pBytes, dataLength);
            }
        }
        
        
        return handle;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref SoundClip Access(in Handle<SoundClip> handle) => ref _resources.GetResourceReference(handle);

    public void Release(in Handle<SoundClip> handle)
    {
        var soundClip = _resources.GetResourcePointer(handle);
        soundClip->Data.Free();
        _resources.ReleaseResource(handle);
    }

    public void Dispose()
    {
        foreach (var handle in _resources.EnumerateUsedResources())
        {
            _resources.GetResourcePointer(handle)->Data.Free();
        }
        _resources.Terminate();
    }
}
