using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Memory;

namespace Titan.Memory.Allocators;

/// <summary>
/// The general purpose allocator will allow any size allocations (8 byte aligns).
/// It uses best-fit (O(N)) and a free list to decide where to put the allocation, making it slower than a more restrictive allocator.
/// Uses this for temporary(or permanent) big buffers that last several frames or use this as the base for other Allocators like Linear/Stack/Pool.
/// </summary>
public unsafe struct GeneralAllocator
{
    private const uint MinBlockSize = 64;
    private static readonly uint HeaderSize = (uint)sizeof(Header);

    private VirtualMemory _memory;
    private nuint _offset;
    private Header* _allocations;
    private Header* _lastAllocation;

    public static bool Create(PlatformAllocator* platformAllocator, nuint minReserveSize, nuint initialSize, out GeneralAllocator allocator)
    {
        Debug.Assert(platformAllocator != null);
        Debug.Assert(minReserveSize != 0);
        Debug.Assert(initialSize <= minReserveSize);
        allocator = default;
        if (!VirtualMemory.Create(platformAllocator, minReserveSize, out var memory))
        {
            Console.WriteLine("Failed to create the virtual memory");
            return false;
        }

        allocator = new()
        {
            _memory = memory,
            _offset = 0,
            _allocations = null
        };

        if (initialSize != 0u)
        {
            allocator.Expand(initialSize);
        }
        return true;
    }


    public T* Allocate<T>() where T : unmanaged
        => (T*)Allocate((uint)sizeof(T));
    public void* Allocate(uint size)
    {
        Debug.Assert(_memory.MaxSize > 0);
        var totalSize = MemoryUtils.AlignToUpper(size) + HeaderSize;

        var node = GetFreeNode(totalSize);
        Debug.Assert(node != null);

        var remainingSize = node->BlockSize - totalSize;
        // if the remaining size is greater than MinBlockSize, split the node into 2. 
        if (remainingSize > MinBlockSize)
        {
            Console.WriteLine($"Split the memory block into 2 pieces: {totalSize} and {remainingSize}");
            var header = (Header*)((byte*)node + totalSize);
            header->State = AllocationState.Free;
            header->Previous = node;
            header->Next = node->Next;
            header->BlockSize = remainingSize;

            node->Next = header;
            if (header->Next != null)
            {
                header->Next->Previous = header;
            }
            node->BlockSize = totalSize;
            if (node == _lastAllocation)
            {
                _lastAllocation = header;
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Block size mismatch but less than {MinBlockSize} bytes. {remainingSize} bytes lost.");
            Console.ResetColor();
        }
        node->State = AllocationState.Allocated;
        var data = (void*)(node + 1);
        Debug.Assert((nuint)data == MemoryUtils.Align((nuint)data), "The data pointer returned should be aligned at 8 bytes. Not sure why this happened.");
        Console.WriteLine($"Allocated node with requested size: {totalSize} bytes (Data size of allocated block: {size} bytes)");
        return data;
    }

    private Header* GetFreeNode(uint totalSize)
    {
        // todo insert a count, and assert if it expires. should not have to run more than 2 laps.
        while (true)
        {
            //NOTE(Jens): this is a "first fit" implementation, should change it to a "best fit" when it works.
            var header = _allocations;
            while (header != null)
            {
                if (header->State == AllocationState.Free && header->BlockSize >= totalSize)
                {
                    Console.WriteLine($"Found a matching block: {header->BlockSize} bytes. Requested size: {totalSize}");
                    return header;
                }
                header = header->Next;
            }
            Expand(totalSize);
        }
    }

    private void Expand(nuint requestedBlockSize)
    {
        if (_allocations == null)
        {
            // first allocation
            _memory.Resize(requestedBlockSize); // set the size to the requested size (aligned with the page size)
            _allocations = (Header*)_memory.Mem;
            _allocations->Next = null;
            _allocations->Previous = null;
            _allocations->BlockSize = _memory.Size;
            _allocations->State = AllocationState.Free;
            _lastAllocation = _allocations;
            return;
        }

        var newSize = _memory.Size;
        do
        {
            newSize = Math.Min(newSize * 2, _memory.MaxSize);
            if (_offset + requestedBlockSize < newSize)
            {
                var previousSize = _memory.Size;
                _memory.Resize(newSize);
                Debug.Assert(_lastAllocation != null, "This should not happen, since this can't be reached without any allocations occuring.");
                var mem = (Header*)((byte*)_lastAllocation + _lastAllocation->BlockSize);
                mem->Next = null;
                mem->Previous = _lastAllocation;
                mem->BlockSize = _memory.Size - previousSize;
                mem->State = AllocationState.Free;
                _lastAllocation->Next = mem;
                _lastAllocation = mem;

                MergeWithNext(mem->Previous);
                return;
            }
        } while (newSize < _memory.MaxSize);

        Debug.Assert(true, "ran out of memory.");
    }

    public void Free(void* ptr)
    {
        Debug.Assert(ptr != null);
        Debug.Assert(_memory.MaxSize > 0);
        var header = (Header*)ptr - 1;
        // Check bounds
        Debug.Assert(header >= _memory.Mem);
        Debug.Assert(header < (byte*)_memory.Mem + _memory.Size);
        if (header->State != AllocationState.Allocated)
        {
            Console.WriteLine("Memory has already been freed or was not allocated with this allocator.");
            return;
        }
        Console.WriteLine($"Freeing memory with size: {header->BlockSize} bytes");
        header->State = AllocationState.Free;

        // See if we can merge blocks that are nearby
        MergeWithNext(header);
        MergeWithNext(header->Previous);
    }


    private void MergeWithNext(Header* current)
    {
        //NOTE(Jens): if both current and next are free, merge them
        if (IsFree(current) && IsFree(current->Next))
        {
            var next = current->Next;
            var nextNext = next->Next;
            current->Next = nextNext;
            Console.WriteLine($"Merge block with size: {current->BlockSize} bytes and {next->BlockSize} bytes. ({current->BlockSize + next->BlockSize} bytes)");
            current->BlockSize += next->BlockSize;
            if (nextNext != null)
            {
                nextNext->Previous = current;
            }
            else
            {
                //NOTE(Jens): this happens when the current node is merged with the last allocated block.
                _lastAllocation = current;
            }
        }

        static bool IsFree(Header* header) => header != null && header->State == AllocationState.Free;
    }
    public void Release()
    {
        _memory.Release();
        _memory = default;
    }

    //NOTE(Jens): Current header size is 24 bytes, we can reduce that by packing and doing some extra calculations. Memory/Performance trade. 
    private struct Header
    {
        //NOTE(Jens): we could pack the State into 2 bytes and the size into 6 bytes if we want to support greater sizes.
        public AllocationState State;
        public uint BlockSize;

        //NOTE(Jens): next pointer is not really required since it is the same as  (Header)((byte*)&this + BlockSize). But if we do this we need to check that it's not out of bounds. This will save 8 bytes per allocation but use more CPU per allocation.
        public Header* Next;
        public Header* Previous;
    }

    private enum AllocationState : uint
    {
        Allocated = 0xeeffffee,
        Free = 0xfffaafff
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsWithinMemoryBlock(void* ptr)
        => ptr >= _memory.Mem && ptr < (byte*)_memory.Mem + _memory.Size;

    public void PrintDebugInfo()
    {
        var header = _allocations;
        var memoryAllocated = 0u;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine();
        Console.WriteLine($"Memory: 0x{(nuint)_memory.Mem}. Committed: {_memory.Size} bytes. Reserved: {_memory.MaxSize} bytes");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{"Address",-18} | {"State",-10} | {"Next",-18} | {"Previous",-18} | Size");
        while (header != null)
        {
            Console.ForegroundColor = header->State == AllocationState.Free ? ConsoleColor.Green : ConsoleColor.Red;

            Console.WriteLine($"0x{(nuint)header,-16} | {header->State,-10} | 0x{(nuint)header->Next,-16} | 0x{(nuint)header->Previous,-16} | {header->BlockSize} bytes");
            memoryAllocated += header->BlockSize;
            header = header->Next;
        }

        var diff = (int)memoryAllocated - _memory.Size;
        if (diff != 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Total Memory: {memoryAllocated} bytes. Difference between blocks and allocated in virtual mem: {diff} bytes");
        }

        Console.ResetColor();
        Console.WriteLine();
    }
}
