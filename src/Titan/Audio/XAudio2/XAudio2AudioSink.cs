using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Platform.Win32;
using Titan.Platform.Win32.XAudio2;

namespace Titan.Audio.XAudio2;

internal unsafe struct XAudio2AudioSink : IXAudio2VoiceCallbackFunctions
{
    public IXAudio2VoiceCallback Callback;
    public IXAudio2SourceVoice* SourceVoice;
    public AudioPlaybackState State;
    public HRESULT Error;
    public ushort Version;

    public void OnBufferEnd(void* pBufferContext)
    {
        State = AudioPlaybackState.Completed;
    }

    public void Destroy()
    {
        if (IsValid())
        {
            //NOTE(Jens): In some cases this seems to crash. Not sure which method   
            if (State == AudioPlaybackState.Playing)
            {
                SourceVoice->Stop();
            }
            SourceVoice->DestroyVoice();
            SourceVoice = null;
        }
        State = AudioPlaybackState.NotCreated;
    }

    public void OnVoiceError(void* pBufferContext, HRESULT error)
    {
        Error = error;
        State = AudioPlaybackState.Error;
    }

    public void OnLoopEnd(void* pBufferContext)
    {
        //NOTE(Jens): do we want to do something about this one?
    }

    public void Play(in TitanBuffer audioData, bool loop)
    {
        if (!IsValid())
        {
            return;
        }

        var buffer = new XAUDIO2_BUFFER
        {
            AudioBytes = (uint)audioData.Length,
            pAudioData = audioData.AsPointer(),
            pContext = null, // we can use this if we need more data.
            LoopCount = loop ? XAudio2Constants.XAUDIO2_LOOP_INFINITE : 0u
        };

        var hr = SourceVoice->SubmitSourceBuffer(&buffer, null);
        Debug.Assert(Win32Common.SUCCEEDED(hr), $"SubmitSourceBuffer failed. HRESULT = {hr}");
        hr = SourceVoice->Start();
        Debug.Assert(Win32Common.SUCCEEDED(hr), $"Start failed. HRESULT = {hr}");
        State = AudioPlaybackState.Playing;
    }

    public void Stop()
    {
        if (IsValid())
        {
#if DEBUG
            var hr = SourceVoice->Stop();
            if (Win32Common.FAILED(hr))
            {
                Logger.Error<XAudio2AudioSink>($"{nameof(IXAudio2SourceVoice.Stop)} failed. HRESULT = {hr}");
            }
            hr = SourceVoice->FlushSourceBuffer();
            if (Win32Common.FAILED(hr))
            {
                Logger.Error<XAudio2AudioSink>($"{nameof(IXAudio2SourceVoice.FlushSourceBuffer)} failed. HRESULT = {hr}");
            }
#else
            SourceVoice->Stop();
            SourceVoice->FlushSourceBuffer();
#endif
        }
    }

    public void Resume()
    {
        if (IsValid() && State == AudioPlaybackState.Paused)
        {
            SourceVoice->Start();
        }
    }
    public void Pause()
    {
        if (IsValid())
        {
#if DEBUG
            var hr = SourceVoice->Stop();
            if (Win32Common.FAILED(hr))
            {
                Logger.Error<XAudio2AudioSink>($"{nameof(IXAudio2SourceVoice.Stop)} failed. HRESULT = {hr}");
            }
#else
            SourceVoice->Stop();
#endif
            State = AudioPlaybackState.Paused;
        }
    }

    public void SetVolume(float volume)
    {
        if (IsValid())
        {
            SourceVoice->SetVolume(volume);
        }
    }

    public void SetFrequency(float frequency)
    {
        if (IsValid())
        {
            SourceVoice->SetFrequencyRatio(frequency);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsValid() => SourceVoice != null;
}
