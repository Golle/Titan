//using Titan.Core.Logging;
//using Titan.Core.Memory;

//namespace Titan.Memory.Allocators;

////TODO(Jens): implement this when we need it
//public unsafe struct DynamicPoolAllocator<T> where T : unmanaged
//{
//    private VirtualMemory _memory;
//    public static bool Create(PlatformAllocator* allocator, uint maxCount, bool alignedPerObject, out DynamicPoolAllocator<T> poolAllocator)
//    {
//        poolAllocator = default;
//        var size = alignedPerObject ? MemoryUtils.AlignToUpper((uint)sizeof(T)) : (uint)sizeof(T);
//        if (!VirtualMemory.Create(allocator, maxCount * size, out var memory))
//        {
//            Logger.Error<DynamicPoolAllocator<T>>($"Failed to create the {nameof(VirtualMemory)} with size: {maxCount * size} bytes");
//            return false;
//        }
//        poolAllocator = new()
//        {
//            _memory = memory
//        };

//        return true;
//    }

//    public void Release()
//    {
//        _memory.Release();
//        _memory = default;
//    }
//}
