//using Titan.Core.Logging;

//namespace Titan.Memory.Arenas;

//public unsafe struct BestFitFixedArena
//{
//    public PlatformAllocator _allocator;
//    public uint _maxSize;

//    public static bool Create(uint minBlockSize, uint maxSizeInBytes, out BestFitFixedArena arena)
//    {
//        Logger.Warning<BestFitFixedArena>($"The {nameof(BestFitFixedArena)} has not been implemented yet, it uses VirtualAlloc/NativeLibrary.");

//        arena = new BestFitFixedArena
//        {
//            _allocator = PlatformAllocator.Create(),
//            _maxSize = maxSizeInBytes
//        };

//        return true;
//    }

//    public void* Allocate(nuint size, bool initialize = false) 
//        => _allocator.Allocate(size, initialize);


//    public void Free(void* ptr)
//        => _allocator.Free(ptr);

//    public void Release()
//    {
//        Logger.Warning<BestFitFixedArena>($"Not implemented yet");
//    }
//}
