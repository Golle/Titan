using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Core.Memory;


[DebuggerDisplay("Length: {Length}")]
public readonly unsafe struct TitanBuffer
{
    private readonly byte* _mem;
    private readonly int _length;
    public bool HasData() => _mem != null && _length > 0;
    public TitanBuffer(void* mem, uint length)
    {
        Debug.Assert(length <= int.MaxValue);
        _mem = (byte*)mem;
        _length = (int)length;
    }

    public TitanBuffer(void* mem, int length)
    {
        Debug.Assert(length >= 0);
        _mem = (byte*)mem;
        _length = length;
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    public ref byte this[uint index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _mem[index];
    }

    public ref byte this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _mem[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> AsSpan() => new(_mem, _length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlySpan() => new(_mem, _length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte* AsPointer() => _mem;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator byte*(in TitanBuffer buffer) => buffer._mem;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator void*(in TitanBuffer buffer) => buffer._mem;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<byte>(in TitanBuffer buffer) => new(buffer._mem, buffer._length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<byte>(in TitanBuffer buffer) => new(buffer._mem, buffer._length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TitanBuffer Slice(uint length) => Slice(0, length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TitanBuffer Slice(uint start, uint length)
        => new(_mem + start, length);
}
