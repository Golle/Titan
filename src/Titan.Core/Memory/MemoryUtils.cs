using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Core.Memory;

public static unsafe class MemoryUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Init(void* mem, int sizeInBytes, byte value = 0)
        => Init(mem, (nuint)sizeInBytes, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Init(void* mem, uint sizeInBytes, byte value = 0)
        => Init(mem, (nuint)sizeInBytes, value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Init(void* mem, nuint sizeInBytes, byte value = 0)
    {
        Debug.Assert(sizeInBytes < uint.MaxValue, $"Can't clear memory that has a size bigger size than {uint.MaxValue} bytes.");
        Unsafe.InitBlockUnaligned(mem, value, (uint)sizeInBytes);

        //NOTE(Jens): Should we have a platform layer for this? is ZeroMemory/SecureZeroMemory faster on Windows?
        //https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/aa366877(v=vs.85)
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy(void* dst, void* src, int sizeInBytes)
    {
        Debug.Assert(sizeInBytes >= 0, $"{nameof(sizeInBytes)} >= 0");
        Copy(dst, src, (nuint)sizeInBytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy(void* dst, void* src, uint sizeInBytes)
        => Copy(dst, src, (nuint)sizeInBytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy(void* dst, void* src, nuint sizeInBytes)
    {
        Debug.Assert(sizeInBytes < uint.MaxValue, $"Can't copy memory that has a size bigger size than {uint.MaxValue} bytes.");
        Unsafe.CopyBlockUnaligned(dst, src, (uint)sizeInBytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* AsPointer<T>(ref T value) where T : unmanaged
        => (T*)Unsafe.AsPointer(ref value);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T ToRef<T>(T* ptr) where T : unmanaged
    {
        Debug.Assert(ptr != null);
        return ref *ptr;
    }
}

