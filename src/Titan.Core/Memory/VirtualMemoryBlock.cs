using System.Diagnostics;
using Titan.Core.Logging;

namespace Titan.Core.Memory;

internal unsafe struct VirtualMemoryBlock
{
    private readonly PlatformAllocator* _allocator;
    private readonly void* _startAddress;
    private readonly uint _maxPages;
    private uint _committedPages;
    public void* Mem => _startAddress;
    public uint Size => _committedPages * _allocator->PageSize;
    public uint MaxSize => _maxPages * _allocator->PageSize;
    public uint PageSize => _allocator->PageSize;
    public VirtualMemoryBlock(PlatformAllocator* allocator, void* startAddress, uint maxPages)
    {
        _allocator = allocator;
        _startAddress = startAddress;
        _maxPages = maxPages;
        _committedPages = 0;
    }

    public bool Resize(nuint newSize)
    {
        Debug.Assert(_startAddress != null);
        var pages = GetPageCount(newSize, _allocator->PageSize);

        if (pages > _maxPages)
        {
            Logger.Error<VirtualMemoryBlock>($"Max pages has been reached, can't allocate more memory. {_maxPages}");
            return false;
        }
        var pageDiff = (int)pages - _committedPages;

        Logger.Trace<VirtualMemoryBlock>($"Resize {newSize} bytes ({pages}). Page diff {pageDiff}");
        if (pageDiff > 0)
        {
            //NOTE(Jens): Verify that the offset is used correctly
            //var startAddress = (byte*)_startAddress + _committedPages * _allocator->PageSize;
            _allocator->Commit(_startAddress, (uint)pageDiff, _committedPages);
        }
        else if (pageDiff < 0)
        {
            //NOTE(Jens): Verify that the offset is used correctly
            //var startAddress = (byte*)_startAddress + pages * _allocator->PageSize;
            _allocator->Decommit(_startAddress, (uint)-pageDiff, pages);
        }
        _committedPages = pages;
        return true;
    }

    public void Release()
    {
        //NOTE(Jens): this is not the owner of the memory, just decommit the memory and let the owner cleanup.
        if (_startAddress != null && _allocator != null && _committedPages > 0) // no decommit if no pages have been allocated
        {
            _allocator->Decommit(_startAddress, _committedPages);
        }
    }
    private static uint GetPageCount(nuint size, uint pageSize)
    {
        Debug.Assert(pageSize != 0);
        return (uint)(MemoryUtils.AlignToUpper(size, pageSize) / pageSize);
    }
}
