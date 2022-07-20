using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Sound.Loaders;

namespace Titan.Old.Components;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SoundClipComponent
{
    public Handle<SoundClip> Handle;

    internal SoundClipComponent(in Handle<SoundClip> handle) => Handle = handle;
}
