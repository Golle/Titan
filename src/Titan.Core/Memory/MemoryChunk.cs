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
}
