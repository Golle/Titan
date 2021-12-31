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
        var dataLength = (uint)args.Data.Length;
        soundclip->Data = MemoryUtils.AllocateBlock<byte>(dataLength);
        fixed (byte* pBytes = args.Data)
        {
            Unsafe.CopyBlockUnaligned(soundclip->Data.AsPointer(), pBytes, dataLength);
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
