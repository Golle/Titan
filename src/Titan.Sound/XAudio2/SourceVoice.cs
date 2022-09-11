using System;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Platform.Win32;
using Titan.Platform.Win32.XAudio2;

namespace Titan.Sound.XAudio2;

internal unsafe class SourceVoice : IXAudio2VoiceCallback.Interface, IDisposable
{
    private IXAudio2SourceVoice* _voice;
    private IXAudio2VoiceCallback* _callback;
    private HRESULT Error;
    internal volatile int State;
    internal int _playCount;
    
    public SourceVoice(ComPtr<IXAudio2> xAudio2, WAVEFORMATEX format)
    {
        _callback = IXAudio2VoiceCallback.Create(this);
        fixed (IXAudio2SourceVoice** ppVoice = &_voice)
        {
            Common.CheckAndThrow(xAudio2.Get()->CreateSourceVoice(ppVoice, &format, pCallback: _callback), nameof(IXAudio2.CreateSourceVoice));
        }

        _voice->SetVolume(0.1f);
    }

    public int Play(in MemoryChunk<byte> soundData, in WAVEFORMATEX format)
    {
        var buffer = new XAUDIO2_BUFFER
        {
            AudioBytes = soundData.Size,
            pAudioData = soundData.AsPointer()
        };
        _voice->SubmitSourceBuffer(&buffer, null);
        _voice->Start();
        return unchecked(++_playCount);
    }

    public void Stop() => _voice->Stop();
    public void StopVerified(int playCount)
    {
        if (_playCount == playCount)
        {
            _voice->Stop();
        }
    }
    public void OnVoiceProcessingPassStart(uint bytesRequired) {}
    public void OnVoiceProcessingPassEnd() {}
    public void OnStreamEnd() {}
    public void OnBufferStart(void* pBufferContext) {}
    public void OnLoopEnd(void* pBufferContext) {}
    public void OnBufferEnd(void* pBufferContext)
    {
        State = SourceVoiceState.Available;
    }

    public void OnVoiceError(void* pBufferContext, HRESULT error)
    {
        State = SourceVoiceState.Error;
        Error = error;
        Logger.Error<SourceVoice>($"Error occured in SourceVoice. HRESULT {error}");
    }

    public void Dispose()
    {
        if (_voice != null)
        {
            _voice->DestroyVoice();
            _voice = null;
        }
        IXAudio2VoiceCallback.Free(_callback);
        _callback = null;
    }
}
