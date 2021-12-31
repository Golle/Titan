using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Windows;
using Titan.Windows.XAudio2;
using static Titan.Windows.Common;

namespace Titan.Sound.XAudio2;

public record AudioDeviceConfiguration(uint NumberOfSounds = 32);

internal unsafe class XAudio2Device : IDisposable
{
    private ComPtr<IXAudio2> _xAudio2;
    private IXAudio2MasteringVoice* _masteringVoice;
    private readonly SourceVoice[] _sourceVoices;

    private volatile int _nextVoice;
    private readonly int _maxVoices;

    public XAudio2Device(AudioDeviceConfiguration config)
    {
        // TODO: what flags and processor should we use?
        CheckAndThrow(XAudio2Common.XAudio2Create(_xAudio2, 0U, XAUDIO2_PROCESSOR.XAUDIO2_USE_DEFAULT_PROCESSOR), nameof(XAudio2Common.XAudio2Create)); 

        // TODO: check the flags and arguments
        fixed (IXAudio2MasteringVoice** pMasteringVoice = &_masteringVoice)
        {
            CheckAndThrow(_xAudio2.Get()->CreateMasteringVoice(pMasteringVoice), nameof(IXAudio2.CreateMasteringVoice));
        }

        _sourceVoices = new SourceVoice[config.NumberOfSounds];
        _maxVoices = (int)config.NumberOfSounds;

        // TODO: how should this be configured? 
        var format = new WAVEFORMATEX
        {
            
            //nBlockAlign = 4,
            nBlockAlign = 1,

            wFormatTag = 1,
            //wBitsPerSample = 16,
            wBitsPerSample = 8,
            nSamplesPerSec = 44100,
            //nChannels = 2,
            nChannels = 1,
            cbSize = (WORD)sizeof(WAVEFORMATEX)
        };

        format.nAvgBytesPerSec = ((uint)format.wBitsPerSample * format.nChannels * format.nSamplesPerSec) / 8;
        
        for (var i = 0; i < config.NumberOfSounds; ++i)
        {
            _sourceVoices[i] = new SourceVoice(_xAudio2, format);
        }
    }

    public bool TryPlaySound(MemoryChunk<byte> soundData, out PlaySoundHandle handle)
    {
        Unsafe.SkipInit(out handle);
        var index = GetNextVoiceIndex();
        if (index == -1)
        {
            Logger.Warning<XAudio2Device>("Can't play sound, all voices are being used.");
        }
        else
        {
            Logger.Trace<XAudio2Device>($"Playing on source voice: {index}");
            var counter = _sourceVoices[index].Play(soundData);
            handle = new PlaySoundHandle(index, counter);
            return true;
        }
        return false;
    }

    public void Stop(PlaySoundHandle handle)
    {
        if (handle.VoiceIndex >= 0 && handle.VoiceIndex < _sourceVoices.Length)
        {
            _sourceVoices[handle.VoiceIndex].StopVerified(handle.VoiceCounter);
        }
    }


    // Note(jens): this is not thread safe, should not be called from a system
    public void StopAll()
    {

        foreach (var sourceVoice in _sourceVoices)
        {
            if (sourceVoice.State == SourceVoiceState.Playing)
            {
                sourceVoice.Stop();
            }
        }
    }

    private int GetNextVoiceIndex()
    {
        var maxIterations = _maxVoices;
        while (maxIterations-- > 0)
        {
            var current = _nextVoice;
            var index = Interlocked.CompareExchange(ref _nextVoice, (current + 1) % _maxVoices, current);
            // Some other thread updated the counter, do another lap
            if (index != current)
            {
                continue;
            }

            var previousStatus = Interlocked.CompareExchange(ref _sourceVoices[index].State, SourceVoiceState.Playing, SourceVoiceState.Available);
            // If the voice is busy, loop again and try to find a new spot
            if (previousStatus != SourceVoiceState.Available)
            {
                continue;
            }
            return index;
        }
        return -1;
    }

    public void Update()
    {
        // TODO: add checking for error state in voices, log the error and reset the voice.
    }

    public void Dispose()
    {
        for (var i = 0; i < _sourceVoices.Length; ++i)
        {
            _sourceVoices[i].Dispose();
        }
        _masteringVoice->DestroyVoice();
        _xAudio2.Dispose();
    }
}
