using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Core.Memory;

public static unsafe class MemoryUtils
{
    private const uint OneKiloByte = 1024u;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Init(void* mem, long sizeInBytes, byte value = 0)
        => Init(mem, (nuint)sizeInBytes, value);

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
    public static T* AsPointer<T>(in T value) where T : unmanaged
        => (T*)Unsafe.AsPointer(ref Unsafe.AsRef(value));


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T ToRef<T>(T* ptr) where T : unmanaged
    {
        Debug.Assert(ptr != null);
        return ref *ptr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T** AddressOf<T>(in T* ptr) where T : unmanaged
    {
        //NOTE(Jens): any risk with doing it like this?
        fixed (T** pptr = &ptr)
        {
            return pptr;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GigaBytes(uint size) => MegaBytes(size) * OneKiloByte;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint MegaBytes(uint size) => KiloBytes(size) * OneKiloByte;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint KiloBytes(uint size) => size * OneKiloByte;




    /// <summary>
    /// Aligns to 8 bytes.
    /// </summary>
    /// <param name="size">The size of the memory block</param>
    /// <returns>The 8 bytes aligned size</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Align(nuint size)
        => size & ~(nuint)7u;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Align(uint size)
        => size & ~7u;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint AlignToUpper(int size)
    {
        Debug.Assert(size >= 0);
        return AlignToUpper((uint)size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint AlignToUpper(uint size)
    {
        var alignedSize = Align(size);
        return alignedSize < size ? alignedSize + 8u : alignedSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Align(nuint size, uint alignment)
    {
        var mask = alignment - 1u;
        var alignedMemory = size & ~mask;
        return alignedMemory;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint AlignToUpper(nuint size, uint alignment)
    {
        var alignedMemory = Align(size, alignment);
        return alignedMemory < size ? alignedMemory + alignment : alignedMemory;
    }
}

