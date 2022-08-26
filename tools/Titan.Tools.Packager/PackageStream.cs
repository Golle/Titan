using System.Runtime.InteropServices;
using Titan.Core.Memory;

namespace Titan.Tools.Packager;

public unsafe class PackageStream : IDisposable
{
    private readonly byte* _memory;
    private readonly ulong _maxSize;
    private ulong _size;
    public ulong Offset => _size;

    public void Write<T>(in T obj) where T : unmanaged
    {
        var objSize = (ulong)sizeof(T);

        if (_size + objSize >= _maxSize)
        {
            throw new Exception("Max size reached, please increase the buffer");
        }
        fixed (T* pObj = &obj)
        {
            MemoryUtils.Copy(_memory, pObj, (uint)objSize);
        }
        _size += objSize;
    }

    public void Write(ReadOnlySpan<byte> bytes)
    {
        if (_size + (ulong)bytes.Length >= _maxSize)
        {
            throw new Exception("Max size reached, please increase the buffer");
        }
        fixed (byte* pBytes = bytes)
        {
            MemoryUtils.Copy(_memory, pBytes, bytes.Length);
        }

        _size += (ulong)bytes.Length;
    }

    public void Export(Stream outputStream)
    {
        if (_size > int.MaxValue)
        {
            throw new NotSupportedException($"File size greater than {int.MaxValue} is not supported (limitation in Stream API). If this is required, implemented a paginated write.");
        }
        outputStream.Write(new ReadOnlySpan<byte>(_memory, (int)_size));
    }

    public PackageStream(nuint sizeInBytes)
    {
        _memory = (byte*)NativeMemory.Alloc(sizeInBytes);
        _maxSize = sizeInBytes;
    }
    public void Dispose()
    {
        NativeMemory.Free(_memory);
    }
}

