using System;

namespace Titan.Core.Memory
{
    public interface IMemoryManager : IDisposable
    {
        MemoryChunk GetMemoryChunk(string identifier);
        MemoryChunk<T> GetMemoryChunkValidated<T>(string identifier) where T : unmanaged;
    }
}
