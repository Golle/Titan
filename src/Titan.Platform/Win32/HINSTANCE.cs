using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32;

public struct HINSTANCE
{
    public nint Handle;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe implicit operator HINSTANCE(HMODULE handle) 
        => *(HINSTANCE*)&handle;
}
