namespace Titan.Platform.Win32.XAudio2;

public static class XAudio2Constants
{

    /**************************************************************************
     *
     * XAudio2 constants, flags and error codes.
     *
     **************************************************************************/

    // Numeric boundary values
    public const uint XAUDIO2_MAX_BUFFER_BYTES = 0x80000000; // Maximum bytes allowed in a source buffer
    public const uint XAUDIO2_MAX_QUEUED_BUFFERS = 64; // Maximum buffers allowed in a voice queue
    public const uint XAUDIO2_MAX_BUFFERS_SYSTEM = 2; // Maximum buffers allowed for system threads (Xbox 360 only)
    public const uint XAUDIO2_MAX_AUDIO_CHANNELS = 64; // Maximum channels in an audio stream
    public const uint XAUDIO2_MIN_SAMPLE_RATE = 1000; // Minimum audio sample rate supported
    public const uint XAUDIO2_MAX_SAMPLE_RATE = 200000; // Maximum audio sample rate supported
    public const float XAUDIO2_MAX_VOLUME_LEVEL = 16777216.0f; // Maximum acceptable volume level (2^24)
    public const float XAUDIO2_MIN_FREQ_RATIO = (1 / 1024.0f); // Minimum SetFrequencyRatio argument
    public const float XAUDIO2_MAX_FREQ_RATIO = 1024.0f; // Maximum MaxFrequencyRatio argument
    public const float XAUDIO2_DEFAULT_FREQ_RATIO = 2.0f; // Default MaxFrequencyRatio argument
    public const float XAUDIO2_MAX_FILTER_ONEOVERQ = 1.5f; // Maximum XAUDIO2_FILTER_PARAMETERS.OneOverQ
    public const float XAUDIO2_MAX_FILTER_FREQUENCY = 1.0f; // Maximum XAUDIO2_FILTER_PARAMETERS.Frequency
    public const uint XAUDIO2_MAX_LOOP_COUNT = 254; // Maximum non-infinite XAUDIO2_BUFFER.LoopCount
    public const uint XAUDIO2_MAX_INSTANCES = 8; // Maximum simultaneous XAudio2 objects on Xbox 360

    // For XMA voices on Xbox 360 there is an additional restriction on the MaxFrequencyRatio
    // argument and the voice's sample rate: the product of these numbers cannot exceed 600000
    // for one-channel voices or 300000 for voices with more than one channel.
    public const uint XAUDIO2_MAX_RATIO_TIMES_RATE_XMA_MONO = 600000;
    public const uint XAUDIO2_MAX_RATIO_TIMES_RATE_XMA_MULTICHANNEL = 300000;

    // Numeric values with special meanings
    public const uint XAUDIO2_COMMIT_NOW = 0; // Used as an OperationSet argument
    public const uint XAUDIO2_COMMIT_ALL = 0; // Used in IXAudio2::CommitChanges
    public const uint XAUDIO2_INVALID_OPSET = unchecked((uint)(-1)); // Not allowed for OperationSet arguments
    public const uint XAUDIO2_NO_LOOP_REGION = 0; // Used in XAUDIO2_BUFFER.LoopCount
    public const uint XAUDIO2_LOOP_INFINITE = 255; // Used in XAUDIO2_BUFFER.LoopCount
    public const uint XAUDIO2_DEFAULT_CHANNELS = 0; // Used in CreateMasteringVoice
    public const uint XAUDIO2_DEFAULT_SAMPLERATE = 0; // Used in CreateMasteringVoice

    // Flags
    public const uint XAUDIO2_DEBUG_ENGINE = 0x0001; // Used in XAudio2Create
    public const uint XAUDIO2_VOICE_NOPITCH = 0x0002; // Used in IXAudio2::CreateSourceVoice
    public const uint XAUDIO2_VOICE_NOSRC = 0x0004; // Used in IXAudio2::CreateSourceVoice
    public const uint XAUDIO2_VOICE_USEFILTER = 0x0008; // Used in IXAudio2::CreateSource/SubmixVoice
    public const uint XAUDIO2_PLAY_TAILS = 0x0020; // Used in IXAudio2SourceVoice::Stop
    public const uint XAUDIO2_END_OF_STREAM = 0x0040; // Used in XAUDIO2_BUFFER.Flags
    public const uint XAUDIO2_SEND_USEFILTER = 0x0080; // Used in XAUDIO2_SEND_DESCRIPTOR.Flags
    public const uint XAUDIO2_VOICE_NOSAMPLESPLAYED = 0x0100; // Used in IXAudio2SourceVoice::GetState
    public const uint XAUDIO2_STOP_ENGINE_WHEN_IDLE = 0x2000; // Used in XAudio2Create to force the engine to Stop when no source voices are Started, and Start when a voice is Started
    public const uint XAUDIO2_1024_QUANTUM = 0x8000; // Used in XAudio2Create to specify nondefault processing quantum of 21.33 ms (1024 samples at 48KHz)
    public const uint XAUDIO2_NO_VIRTUAL_AUDIO_CLIENT = 0x10000; // Used in CreateMasteringVoice to create a virtual audio client

    // Default parameters for the built-in filter
//#define XAUDIO2_DEFAULT_FILTER_TYPE     LowPassFilter
//#define XAUDIO2_DEFAULT_FILTER_FREQUENCY XAUDIO2_MAX_FILTER_FREQUENCY
    public const float XAUDIO2_DEFAULT_FILTER_ONEOVERQ = 1.0f;

    // Internal XAudio2 constants
    // The audio frame quantum can be calculated by reducing the fraction:
    //     SamplesPerAudioFrame / SamplesPerSecond
    public const uint XAUDIO2_QUANTUM_NUMERATOR = 1; // On Windows, XAudio2 processes audio
    public const uint XAUDIO2_QUANTUM_DENOMINATOR = 100; //  in 10ms chunks (= 1/100 seconds)
    public const float XAUDIO2_QUANTUM_MS = (1000.0f * XAUDIO2_QUANTUM_NUMERATOR / XAUDIO2_QUANTUM_DENOMINATOR);

// XAudio2 error codes
    public const uint FACILITY_XAUDIO2 = 0x896;
    public static readonly HRESULT XAUDIO2_E_INVALID_CALL = 0x88960001; // An API call or one of its arguments was illegal
    public static readonly HRESULT XAUDIO2_E_XMA_DECODER_ERROR = 0x88960002; // The XMA hardware suffered an unrecoverable error
    public static readonly HRESULT XAUDIO2_E_XAPO_CREATION_FAILED = 0x88960003; // XAudio2 failed to initialize an XAPO effect
    public static readonly HRESULT XAUDIO2_E_DEVICE_INVALIDATED = 0x88960004; // An audio device became unusable (unplugged, etc)


    public const ushort WAVE_FORMAT_PCM = 1;
}
