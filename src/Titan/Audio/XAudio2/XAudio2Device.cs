using Titan.Core.Logging;
using Titan.Platform.Win32;
using Titan.Platform.Win32.XAudio2;
using static Titan.Platform.Win32.Win32Common;

namespace Titan.Audio.XAudio2;


internal record struct AudioFormatArgs(uint Channels, uint BitsPerSample, uint SamplesPerSec, float MaxFrequencyRatio);
internal unsafe class XAudio2Device
{
    private ComPtr<IXAudio2> _xAudio2;
    public bool Init()
    {
#if DEBUG
        var hr = XAudio2Common.XAudio2Create(_xAudio2.GetAddressOf(), XAudio2Constants.XAUDIO2_DEBUG_ENGINE);
#else
        var hr = XAudio2Common.XAudio2Create(_xAudio2.GetAddressOf()); 
#endif

        if (FAILED(hr))
        {
            Logger.Error<XAudio2Device>($"Failed to create the {nameof(IXAudio2)}. HRESULT = {hr}");
            return false;
        }

        return true;
    }

    public IXAudio2MasteringVoice* CreateMasteringVoice()
    {
        IXAudio2MasteringVoice* masteringVoice = null;
        var hr = _xAudio2.Get()->CreateMasteringVoice(&masteringVoice);
        if (SUCCEEDED(hr))
        {
            return masteringVoice;
        }

        if (hr == ERROR_NOT_FOUND)
        {
            Logger.Warning<XAudio2Device>($"No default audio device found. Can't create a {nameof(IXAudio2MasteringVoice)}.");
            return null;
        }
        Logger.Error<XAudio2Device>($"Failed to create the {nameof(IXAudio2MasteringVoice)}. HRESULT = {hr}");
        return null;
    }

    public IXAudio2SourceVoice* CreateSourceVoice(in AudioFormatArgs args, IXAudio2VoiceCallback* callback)
    {
        var blockAlign = (args.Channels * args.BitsPerSample) / 8;
        var averageBytesPerSec = (args.BitsPerSample * args.Channels * args.SamplesPerSec) / 8;
        var format = new WAVEFORMATEX
        {
            nBlockAlign = (ushort)blockAlign,
            wFormatTag = XAudio2Constants.WAVE_FORMAT_PCM,
            wBitsPerSample = (ushort)args.BitsPerSample,
            nSamplesPerSec = args.SamplesPerSec,
            nChannels = (ushort)args.Channels,
            cbSize = (ushort)sizeof(WAVEFORMATEX),
            nAvgBytesPerSec = averageBytesPerSec
        };

        IXAudio2SourceVoice* sourceVoice;
        var hr = _xAudio2.Get()->CreateSourceVoice(&sourceVoice, &format, MaxFrequencyRatio: args.MaxFrequencyRatio, pCallback: callback);
        if (FAILED(hr))
        {
            Logger.Error<XAudio2Device>($"Failed to create the {nameof(IXAudio2SourceVoice)}. HRESULT = {hr}");
            return null;
        }
        return sourceVoice;
    }

    public void Shutdown()
        => _xAudio2.Dispose();
}
