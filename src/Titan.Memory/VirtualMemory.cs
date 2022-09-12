using System.Diagnostics;
using Titan.Core.Memory;

namespace Titan.Memory;

internal unsafe struct VirtualMemory
{
    private readonly PlatformAllocator* _allocator;
    private void* _mem;
    private uint _pages;
    private readonly uint _maxPages;

    public void* Mem => _mem;
    public uint Size => _pages * _allocator->PageSize;
    public uint MaxSize => _maxPages * _allocator->PageSize;
    public uint PageSize => _allocator->PageSize;

    public VirtualMemory(PlatformAllocator* allocator, void* mem, uint maxPages)
    {
        _allocator = allocator;
        _mem = mem;
        _maxPages = maxPages;
        _pages = 0;
    }

    public static bool Create(PlatformAllocator* platformAllocator, nuint minReserveSize, out VirtualMemory memory)
    {
        memory = default;
        var pages = GetPageCount(minReserveSize, platformAllocator->PageSize);
        var mem = platformAllocator->Reserve(null, pages);
        if (mem == null)
        {
            return false;
        }
        memory = new(platformAllocator, mem, pages);

        return true;
    }

    public void Release()
    {
        if (_mem != null && _allocator != null)
        {
            _allocator->Release(_mem, _pages);
        }
    }

    public void Resize(nuint newSize)
    {
        Debug.Assert(_mem != null);
        var pages = GetPageCount(newSize, _allocator->PageSize);
        var pageDiff = (int)pages - _pages;

        Console.WriteLine($"Resize {newSize} bytes ({pages}). Page diff {pageDiff}");
        if (pageDiff > 0)
        {
            var startAddress = (byte*)_mem + _pages * _allocator->PageSize;
            _allocator->Commit(startAddress, (uint)pageDiff);
        }
        else if (pageDiff < 0)
        {
            var startAddress = (byte*)_mem + pages * _allocator->PageSize;
            _allocator->Decommit(startAddress, (uint)-pageDiff);
        }
        _pages = pages;

        Console.WriteLine($"Updated size: {Size} bytes (Max: {MaxSize} bytes) Pages: {_pages}");
        // if pageDiff is 0 , do nothing
    }

    private static uint GetPageCount(nuint size, uint pageSize) => (uint)(MemoryUtils.AlignToUpper(size, pageSize) / pageSize);
}
