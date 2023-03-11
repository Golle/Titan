using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Core.Memory;

public static unsafe class MemoryUtils
{
    private const uint OneKiloByte = 1024u;

    public static void InitArray<T>(in TitanArray<T> array, byte value = 0) where T : unmanaged
        => Init(array.GetPointer(), array.Length * sizeof(T), value);

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
    public static void Copy<T>(Span<T> dst, T* src, uint length) where T : unmanaged
    {
        Debug.Assert(length <= dst.Length);
        fixed (T* pDst = dst)
        {
            Copy(pDst, src, (uint)sizeof(T) * length);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy(void* dst, in ReadOnlySpan<byte> src, uint length)
    {
        Debug.Assert(length <= src.Length);
        fixed (byte* pSrc = src)
        {
            Copy(dst, pSrc, length);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(void* dst, in ReadOnlySpan<T> src) where T : unmanaged
    {
        fixed (T* pSrc = src)
        {
            Copy(dst, pSrc, src.Length * sizeof(T));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy(void* dst, in ReadOnlySpan<byte> src)
    {
        fixed (byte* pSrc = src)
        {
            Copy(dst, pSrc, src.Length);
        }
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
    public static uint Align(uint size, uint alignment)
        => (uint)Align((nuint)size, alignment);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint AlignNotPowerOf2(uint size, uint alignment)
        => (size + alignment - 1) / alignment * alignment;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Align(nuint size, uint alignment)
    {
        Debug.Assert(nuint.IsPow2(alignment));
        var mask = alignment - 1u;
        var alignedMemory = size & ~mask;
        return alignedMemory;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint AlignToUpper(uint size, uint alignment)
        => (uint)AlignToUpper((nuint)size, alignment);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint AlignToUpper(nuint size, uint alignment)
    {
        var alignedMemory = Align(size, alignment);
        return alignedMemory < size ? alignedMemory + alignment : alignedMemory;
    }


    ///// <summary>
    ///// Engine internal methods to allocate global memory (mainly used by internal functions/systems like Thread context)
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="count"></param>
    ///// <returns></returns>
    //internal static T* GlobalAlloc<T>(uint count = 1) where T : unmanaged 
    //    => (T*)NativeMemory.Alloc((nuint)(sizeof(T) * count));

    //internal static void* GlobalAlloc(nuint size) 
    //    => NativeMemory.Alloc(size);
}
