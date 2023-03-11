using Titan.Core;
using Titan.Core.Memory;

namespace Titan.Audio;
internal record struct PlayAudioArgs(TitanBuffer AudioBuffer, float Volume, float Frequency, bool Loop);
internal interface IAudioSinkHandler
{
    bool TryGetAudioSink(out Handle<AudioSink> handle);
    void Play(in Handle<AudioSink> handle, in PlayAudioArgs args);
    void Stop(in Handle<AudioSink> handle);
    void Pause(in Handle<AudioSink> handle);
    void Resume(in Handle<AudioSink> handle);
    bool OnDeviceChanged();
    void SetMasterVolume(float volume);
    float GetMasterVolume();
    void SetVolume(in Handle<AudioSink> handle, float volume);
    void Update();
    void SetFrequency(in Handle<AudioSink> handle, float frequency);
}
