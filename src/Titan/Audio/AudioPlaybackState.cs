namespace Titan.Audio;

internal enum AudioPlaybackState
{
    Available,
    Acquired,
    Playing,
    Paused,
    Error,
    Completed,
    NotCreated
}
