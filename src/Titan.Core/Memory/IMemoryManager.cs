using System;

namespace Titan.Core.Memory
{
    public interface IMemoryManager : IDisposable
    {
        MemoryChunk GetMemoryChunk(string identifier);
    }
}
