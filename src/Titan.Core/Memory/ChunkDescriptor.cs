namespace Titan.Core.Memory
{
    public readonly struct ChunkDescriptor
    {
        public readonly string Identifier;
        public readonly uint Size;
        public readonly uint Count;
        public ChunkDescriptor(string identifier, uint size, uint count)
        {
            Identifier = identifier;
            Size = size;
            Count = count;
        }
    }
}
