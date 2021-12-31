using System.Runtime.CompilerServices;

namespace Titan.Sound.XAudio2;

public readonly struct PlaySoundHandle
{
    internal readonly int VoiceIndex;
    internal readonly int VoiceCounter;

    internal PlaySoundHandle(int index, int counter)
    {
        VoiceIndex = index;
        VoiceCounter = counter;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid() => VoiceIndex != -1;

    public static readonly PlaySoundHandle Invalid = new(-1, 0);
}
