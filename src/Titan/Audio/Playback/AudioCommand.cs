using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Assets;
using Titan.Core;

namespace Titan.Audio.Playback;

[StructLayout(LayoutKind.Explicit, Pack = 4)]
[SkipLocalsInit]
internal struct AudioCommand
{
    [FieldOffset(0)]
    public AudioCommands Command;
    [FieldOffset(4)]
    public Handle<Asset> Asset;
    [FieldOffset(4)]
    public Handle<Audio> Audio;
    [FieldOffset(8)]
    public float Volume;
    [FieldOffset(12)]
    public float Frequency;
}
