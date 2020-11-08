namespace Titan.Core.Memory
{
    public readonly unsafe struct MemoryChunk
    {
        public readonly void* Pointer;
        public readonly uint Stride;
        public readonly uint Count;
        public MemoryChunk(void* pointer, uint stride, uint count)
        {
            Stride = stride;
            Pointer = pointer;
            Count = count;
        }
    }

    public readonly unsafe struct MemoryChunk<T> where T : unmanaged
    {
        public readonly T* Pointer;
        public readonly uint Stride;
        public readonly uint Count;
        public MemoryChunk(in MemoryChunk memory)
        {
            Stride = memory.Stride;
            Pointer = (T*) memory.Pointer;
            Count = memory.Count;
        }
    }
}
