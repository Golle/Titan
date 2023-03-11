using System.Runtime.InteropServices;
using Titan.Core.Memory.Platform;

namespace Titan.Core.Memory;

public static class PlatformAllocatorHelper
{
    /// <summary>
    /// This will create a Heap Allocated PlatformAllocator. This is the only place the NativeMemory class will be used within the engine.
    /// </summary>
    /// <returns>Pointer to the allocator</returns>
    public static unsafe PlatformAllocator* GetPlatformAllocator() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) switch
        {
            true => Win32PlatformAllocator.Instance,
            false or _ => PosixPlatformAllocator.Instance
        };
}
